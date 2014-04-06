using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
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
        private static TablePersister<AzureChristmasVoteUserData> _tableImagePersister;

        public AzureChristmasVoteLogSaver()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasVoteLogData>(_tableContext);
            _tableImagePersister = new TablePersister<AzureChristmasVoteUserData>(_tableContext);
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
                    where !c.Data.UserId.Contains("luchogifts.cloudapp.net")
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
                        Ip = gcs.Key.Ip,
                        UserId = gcs.Key.UserId,
                        Referer = gcs.FirstOrDefault().Data.Referer,
                    },
                    Id = gcs.FirstOrDefault().Id,
                    Count = gcs.Count(),
                    };

            
                try
                {
                    foreach (AzureChristmasVoteLogMessage @group in groups)
                    {
                        _tablePersister.Add(new AzureChristmasVoteLogData(group.Id, group.Data.UserId)
                        {
                            Date = group.Data.Date,
                            Ip = group.Data.Ip,
                            Referer = group.Data.Referer,
                        });

                        _tableImagePersister.Add(new AzureChristmasVoteUserData(group.Id, group.Data.UserId));

                        Console.WriteLine(group.Count);
                    }

                    _tableContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
                }
                catch (Exception)
                {
                }

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

        public void ProcessMessages(QueueMessage<AzureChristmasVoteLog> message)
        {
            if (message.DequeueCount > 100)
                return;
            
            try
            {
                var messageLog = message.Data;
                _tablePersister.Add(new AzureChristmasVoteLogData(message.Id, messageLog.UserId)
                {
                    Date = messageLog.Date,
                    Ip = messageLog.Ip,
                    Referer = messageLog.Referer,
                });

                _tableImagePersister.Add(new AzureChristmasVoteUserData(message.Id, messageLog.UserId));

                _tableContext.SaveChangesWithRetries();
            }
            catch (Exception)
            {
                
            }
        }
	}

    public class AzureChristmasVoteLogMessage
    {
        public AzureChristmasVoteLog Data { get; set; }
        public string Id { get; set; }
        public int Count { get; set; }
    }
}