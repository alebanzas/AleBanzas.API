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
    public class AzureChristmasPuntosPorUsuario : IQueueMessageBlocksConsumer<PuntosProcesados>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromMinutes(1);

        public TimeSpan? EstimatedTimeToProcessMessageBlock
        {
            get { return EstimatedTime; }
        }
        
        public int BlockSize
        {
            get { return 64; }
        }
        
        private static TableServiceContext _tableContext;
        private static TablePersister<AzureChristmasPuntosUsuarioData> _tablePersister;

        public AzureChristmasPuntosPorUsuario()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasPuntosUsuarioData>(_tableContext);
        }

        public void ProcessMessagesGroup(IQueueMessageRemover<PuntosProcesados> messagesRemover, IEnumerable<QueueMessage<PuntosProcesados>> messages)
        {
            var queueMessages = messages.ToList();
            var focusingMessagge = queueMessages.FirstOrDefault();
            if (focusingMessagge == null)
            {
                return;
            }
            var rows =
                    from c in queueMessages
                    group c by c.Data.UserID
                    into gcs
                    select new AzureChristmasPuntosUsuarioData(gcs.Key, gcs.Count());
            
            try
            {
                foreach (var row in rows)
                {
                    _tablePersister.Add(row);
                }

                _tableContext.SaveChangesWithRetries();
            }
            catch (Exception)
            {
                return;
            }

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

	}

}