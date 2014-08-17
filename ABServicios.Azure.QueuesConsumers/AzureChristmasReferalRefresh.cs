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
        private static TablePersister<AzureChristmasVoteLogData> _tablePersister;
        private static TablePersister<AzureChristmasVoteUserData> _tableImagePersister;

        public AzureChristmasReferalRefresh()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasVoteLogData>(_tableContext);
            _tableImagePersister = new TablePersister<AzureChristmasVoteUserData>(_tableContext);
        }

        public void ProcessMessagesGroup(IQueueMessageRemover<AzureChristmasRefreshReferal> messagesRemover, IEnumerable<QueueMessage<AzureChristmasRefreshReferal>> messages)
        {
            var queueMessages = messages.ToList();
            var focusingMessagge = queueMessages.FirstOrDefault();
            if (focusingMessagge == null)
            {
                return;
            }

            //TODO: implementar cuenta refresh
            //TODO: armar tabla de resultados en una tabla nueva
            //TODO: refrescar la tabla a demanda con este consumer
            //TODO: la tabla se va armando con el otro consumer
            //TODO: si no esta el userid en este consumer, ignoro el mensaje y vuelve a la cola

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

	}

}