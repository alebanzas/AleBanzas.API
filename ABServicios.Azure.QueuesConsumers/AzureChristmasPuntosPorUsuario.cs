using System;
using System.Collections.Generic;
using System.Linq;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;

namespace ABServicios.Azure.QueuesConsumers
{
    public class AzureChristmasPuntosPorUsuario : IQueueMessageConsumer<PuntosProcesados>
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
        
        private static TablePersister<AzureChristmasVoteUserResultData> _tablePersister;

        public AzureChristmasPuntosPorUsuario()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tablePersister = new TablePersister<AzureChristmasVoteUserResultData>(tableClient);
        }

        public void ProcessMessages(QueueMessage<PuntosProcesados> message)
        {
            var data = message.Data;
            SaveOrUpdatePuntos(data.UserID, data.Puntaje);
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
                        select new AzureChristmasVoteUserResultData(gcs.Key)
                        {
                            Puntos = gcs.Count()
                        };
            
            try
            {
                foreach (var votacionItem in rows)
                {
                    SaveOrUpdatePuntos(votacionItem.UserId, votacionItem.Puntos);
                }
            }
            catch (Exception)
            {
                return;
            }

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

        private static void SaveOrUpdatePuntos(string userId, int puntos)
        {
            var i = _tablePersister.Get(AzureChristmasVoteUserResultData.PKey, userId);

            if (i == null)
            {
                _tablePersister.Add(new AzureChristmasVoteUserResultData(userId)
                {
                    Puntos = puntos,
                });
            }
            else
            {
                i.Puntos += puntos;
                _tablePersister.Update(i);
            }
        }
	}

}