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
    public class AzureChristmasVoteLogSaver : IQueueMessageConsumer<AzureChristmasVoteLog>
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
                            Referal = gcs.FirstOrDefault().Data.Referal,
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

                        _tablePersister.Add(new AzureChristmasVoteLogData(group.Id, group.Data.Referal, group.Data.UserId)
                        {
                            Date = group.Data.Date,
                            Ip = group.Data.Ip,
                            Referer = group.Data.Referer,
                            Referal = group.Data.Referal,
                        });

                        //TODO: para que sirve?
                        //_tableImagePersister.Add(new AzureChristmasVoteUserData(group.Id, group.Data.Referal, group.Data.UserId));

                        if (!string.IsNullOrWhiteSpace(group.Data.Referal))
                        {
                            AzureQueue.Enqueue(new AzureChristmasRefreshReferal{ Referal = group.Data.Referal });
                        }

                        Console.WriteLine(group.Count);
                    }

                    //_tableContext.SaveChangesWithRetries(SaveChangesOptions.Batch);
                    _tableContext.SaveChangesWithRetries();
                }
                catch (Exception)
                {
                    return;
                }

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

        public void ProcessMessages(QueueMessage<AzureChristmasVoteLog> message)
        {
            //TODO: sacar comentario cuando funcione
            //if (message.DequeueCount > 100)
            //    return;
            
            try
            {
                var messageLog = message.Data;
                
                if (string.IsNullOrWhiteSpace(messageLog.UserId)) return;
                if (string.IsNullOrWhiteSpace(messageLog.Referer)) return;

                _tablePersister.Add(new AzureChristmasVoteLogData(message.Id, messageLog.Referal, messageLog.UserId)
                {
                    Date = messageLog.Date,
                    Ip = messageLog.Ip,
                    Referer = messageLog.Referer,
                    Referal = messageLog.Referal,
                });

                _tableImagePersister.Add(new AzureChristmasVoteUserData(message.Id, messageLog.Referal, messageLog.UserId));

                if (!string.IsNullOrWhiteSpace(messageLog.Referal))
                {
                    AzureQueue.Enqueue(new AzureChristmasRefreshReferal { Referal = messageLog.Referal });
                }

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