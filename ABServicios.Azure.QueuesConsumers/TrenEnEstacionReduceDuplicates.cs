using System;
using System.Collections.Generic;
using System.Linq;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;

namespace ABServicios.Azure.QueuesConsumers
{
    public class TrenEnEstacionReduceDuplicates : IQueueMessageBlocksConsumer<TrenEnEstacion>
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
       
        public void ProcessMessagesGroup(IQueueMessageRemover<TrenEnEstacion> messagesRemover, IEnumerable<QueueMessage<TrenEnEstacion>> messages)
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