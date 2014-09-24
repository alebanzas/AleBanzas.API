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

            var cleanList = queueMessages.Where(x => x.DequeueCount <= 1).Select(x => x.Data).Distinct(new TrenEnEstacionCompare()).ToList();

            foreach (var queueMessage in cleanList)
            {
                AzureQueue.Enqueue(new TrenEnEstacionClean
                {
                    Estacion = queueMessage.Estacion,
                    Key = queueMessage.Key,
                    SentidoDescription = queueMessage.SentidoDescription,
                    Time = queueMessage.Time,
                    Vuelta = queueMessage.Vuelta,
                });
            }

            messagesRemover.RemoveProcessedMessages(queueMessages);
        }

        class TrenEnEstacionCompare : IEqualityComparer<TrenEnEstacion>
        {
            public bool Equals(TrenEnEstacion x, TrenEnEstacion y)
            {
                var diff = (x.Time.Subtract(y.Time)).TotalSeconds;

                return x.Key.Equals(y.Key) && 
                        x.Vuelta.Equals(y.Vuelta) &&
                        x.Estacion.Equals(y.Estacion) &&
                        diff < 300; //5 mins
            }
            public int GetHashCode(TrenEnEstacion codeh)
            {
                return codeh.GetHashCode();
            }
        }
	}

}