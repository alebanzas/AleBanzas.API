using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

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

        private static TablePersister<AzureChristmasVoteLogData> _tablePersister;
       
        public AzureChristmasVoteLogSaver()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
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

            var filteredMessages = queueMessages.Distinct(new AzureChristmasVoteLogComparer()).ToList();


            var groups =
                from c in filteredMessages
                group c by c.Data.UserId;

                try
                {
                    foreach (IGrouping<string, QueueMessage<AzureChristmasVoteLog>> @group in groups)
                    {
                        TableBatchOperation batchOperation = new TableBatchOperation();
                        foreach (QueueMessage<AzureChristmasVoteLog> queueMessage in group)
                        {
                            batchOperation.Insert(new AzureChristmasVoteLogData(queueMessage.Data.Referal, queueMessage.Data.UserId, queueMessage.Data.Date)
                            {
                                Ip = queueMessage.Data.Ip,
                                Referer = queueMessage.Data.Referer,
                                Referal = queueMessage.Data.Referal,
                            });

                            if (!string.IsNullOrWhiteSpace(queueMessage.Data.Referal) && !"aquienrefiero.cloudapp.net".Equals(queueMessage.Data.Referal))
                            {
                                AzureQueue.Enqueue(new AzureChristmasRefreshReferal
                                {
                                    UserId = queueMessage.Data.UserId,
                                    Referal = queueMessage.Data.Referal,
                                });
                            }
                        }
                        _tablePersister.AddBatch(batchOperation);
                        Console.WriteLine("Batch: " + batchOperation.Count);
                    }
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

                Console.WriteLine("Remove: " + queueMessages.Count);
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