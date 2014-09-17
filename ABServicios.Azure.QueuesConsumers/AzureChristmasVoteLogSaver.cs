using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.QueuesConsumers
{
    public class AzureChristmasVoteLogSaver : IQueueMessageBlocksConsumer<AzureChristmasVoteLog>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(1);

        public TimeSpan? EstimatedTimeToProcessMessageBlock
        {
            get { return EstimatedTime; }
        }

        public int BlockSize
        {
            get { return 64; }
        }
        
        private static TableServiceContext _tableContext;
        private static TablePersister<AzureChristmasVoteLogData> _tablePersister;
       
        public AzureChristmasVoteLogSaver()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasVoteLogData>(tableClient);
        }

        public void ProcessMessagesGroup(IQueueMessageRemover<AzureChristmasVoteLog> messagesRemover, IEnumerable<QueueMessage<AzureChristmasVoteLog>> messages)
        {
            var queueMessages = messages.ToList();
            var focusingMessagge = queueMessages.FirstOrDefault();
            if (focusingMessagge == null)
            {
                return;
            }

            var groups =
                    from c in queueMessages
                    group c by new
                    {
                        c.Data.UserId,
                        c.Data.Ip,
                        c.Data.Date.Day,
                    } into gcs
                    select new AzureChristmasVoteLogMessage{
                        Data = new AzureChristmasVoteLog
                        {
                            Date = gcs.FirstOrDefault().Data.Date,
                            UserId = gcs.Key.UserId,
                            Referer = gcs.FirstOrDefault().Data.Referer,
                            Referal = gcs.FirstOrDefault().Data.Referal,
                            Ip = gcs.FirstOrDefault().Data.Ip,
                        },
                        Id = gcs.FirstOrDefault().Id,
                        Count = gcs.Count(),
                    };

            
                try
                {
                    foreach (AzureChristmasVoteLogMessage @group in groups)
                    {
                        if (string.IsNullOrWhiteSpace(group.Data.UserId)) continue;
                        if (string.IsNullOrWhiteSpace(group.Data.Referer)) continue;

                        _tablePersister.Add(new AzureChristmasVoteLogData(group.Data.Referal, group.Data.UserId)
                        {
                            Date = group.Data.Date,
                            Ip = group.Data.Ip,
                            Referer = group.Data.Referer,
                            Referal = group.Data.Referal,
                        });

                        if (!string.IsNullOrWhiteSpace(group.Data.Referal) && !"aquienrefiero.cloudapp.net".Equals(group.Data.Referal))
                        {
                            AzureQueue.Enqueue(new AzureChristmasRefreshReferal
                            {
                                UserId = group.Data.UserId, 
                                Referal = group.Data.Referal,
                            });
                        }

                        Console.WriteLine(group.Count);
                    }

                    //_tableContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
                    _tableContext.SaveChangesWithRetries();
                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation.HttpStatusCode != (int) HttpStatusCode.Conflict)
                    {
                        return;
                    }
                }
                catch (Exception)
                {
                    return;
                }

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

	}

    public class AzureChristmasVoteLogMessage
    {
        public AzureChristmasVoteLog Data { get; set; }
        public string Id { get; set; }
        public int Count { get; set; }
    }
}