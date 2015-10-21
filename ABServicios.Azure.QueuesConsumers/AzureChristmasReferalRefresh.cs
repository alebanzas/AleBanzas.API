using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;

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
        
        private static TablePersister<AzureChristmasVoteLogReferalData> _tablePersister;

        public readonly AzureChristmasResultQuery query;

        public AzureChristmasReferalRefresh()
        {
            query = new AzureChristmasResultQuery(AzureAccount.DefaultAccount());

            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tablePersister = new TablePersister<AzureChristmasVoteLogReferalData>(tableClient);
        }

        public void ProcessMessagesGroup(IQueueMessageRemover<AzureChristmasRefreshReferal> messagesRemover, IEnumerable<QueueMessage<AzureChristmasRefreshReferal>> messages)
        {
            var queueMessages = messages.ToList();
            var focusingMessagge = queueMessages.FirstOrDefault();
            if (focusingMessagge == null)
            {
                return;
            }
            
            var messagesToDequeue = new List<QueueMessage<AzureChristmasRefreshReferal>>(queueMessages);

            Console.WriteLine("Procesados: " + messagesToDequeue.Count);
            foreach (var filteredMessage in messagesToDequeue)
            {
                try
                {
                    if ("aquienrefiero".Equals(filteredMessage.Data.Referal) || !filteredMessage.Data.UserId.EndsWith("azurewebsites.net"))
                    {
                        continue;
                    }

                    SaveOrUpdatePuntos(filteredMessage.Data.UserId, filteredMessage.Data.Referal);
                }
                catch (Exception)
                {
                    messagesToDequeue.Remove(filteredMessage);
                    Trace.TraceWarning("Error: user={0} referal={1}", filteredMessage.Data.UserId, filteredMessage.Data.Referal);
                }
            }

            Console.WriteLine("Remove: " + queueMessages.Count);
            messagesRemover.RemoveProcessedMessages(messagesToDequeue);
        }

        private static void SaveOrUpdatePuntos(string userId, string referalId)
        {
            var i = _tablePersister.Get(AzureChristmasVoteLogReferalData.PKey, userId + "|" + referalId);

            if (i == null)
            {
                _tablePersister.Add(new AzureChristmasVoteLogReferalData(userId, referalId));
            }
        }


        //private static void SaveOrUpdatePuntos(string userId)
        //{
        //    var i = _tablePersister.Get(AzureChristmasVoteReferalResultData.PKey, userId);
        //
        //    if (i == null)
        //    {
        //        _tablePersister.Add(new AzureChristmasVoteReferalResultData(userId)
        //        {
        //            Puntos = 1,
        //        });
        //    }
        //    else
        //    {
        //        i.Puntos += 1;
        //        _tablePersister.Update(i);
        //    }
        //}

    }

}