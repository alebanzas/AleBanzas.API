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
    public class AzureChristmasReferalRefresh : IQueueMessageBlocksConsumer<AzureChristmasRefreshReferal>
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
        private static TablePersister<AzureChristmasVoteUserResultData> _tablePersister;

        public AzureChristmasReferalRefresh()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasVoteUserResultData>(_tableContext);
        }

        public void ProcessMessagesGroup(IQueueMessageRemover<AzureChristmasRefreshReferal> messagesRemover, IEnumerable<QueueMessage<AzureChristmasRefreshReferal>> messages)
        {
            var queueMessages = messages.ToList();
            var focusingMessagge = queueMessages.FirstOrDefault();
            if (focusingMessagge == null)
            {
                return;
            }

            //TODO: query que actualiza puntos referidos, si no existe la row, no hago dequeue
            //TODO: ver si encolo usuario o referido

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

	}

}