using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.QueuesConsumers
{
    public class AzureChristmasVoteLogDespegar : IQueueMessageBlocksConsumer<AzureChristmasVoteLog>
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

        public AzureChristmasVoteLogDespegar()
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

            var despegarMsg = queueMessages.Where(x => x.Data.UserId.Contains("luchogifts.cloudapp.net"));

            var reEnqueue = queueMessages.Where(x => !x.Data.UserId.Contains("luchogifts.cloudapp.net"));

            Parallel.ForEach(reEnqueue, message =>
            {
                AzureQueue.Enqueue(message.Data);
            });

            Console.WriteLine(despegarMsg.Count() + "|" + reEnqueue.Count());

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }
	}
}