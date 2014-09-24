﻿using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Linq;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;
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

        public readonly AzureChristmasResultQuery query;

        public AzureChristmasReferalRefresh()
        {
            query = new AzureChristmasResultQuery(AzureAccount.DefaultAccount());

            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasVoteUserResultData>(tableClient);
        }

        public void ProcessMessagesGroup(IQueueMessageRemover<AzureChristmasRefreshReferal> messagesRemover, IEnumerable<QueueMessage<AzureChristmasRefreshReferal>> messages)
        {
            var queueMessages = messages.ToList();
            var focusingMessagge = queueMessages.FirstOrDefault();
            if (focusingMessagge == null)
            {
                return;
            }

            var filteredMessages = queueMessages.Distinct(new AzureChristmasReferalDataComparer()).ToList();
            var messagesToDequeue = new List<QueueMessage<AzureChristmasRefreshReferal>>(queueMessages);
            
            foreach (var filteredMessage in filteredMessages)
            {
                try
                {
                    if ("aquienrefiero.cloudapp.net".Equals(filteredMessage.Data.Referal))
                    {
                        continue;
                    }

                    var referal = _tablePersister.Get(AzureChristmasVoteUserResultData.PKey,
                        filteredMessage.Data.Referal);

                    if (referal != null && filteredMessage.DequeueCount < 100)
                    {
                        var users = query.GetUsersFromReferal(filteredMessage.Data.Referal);

                        var visitasReferidas = users.Select(user => _tablePersister.Get(AzureChristmasVoteUserResultData.PKey, user)).Where(u => u != null).Sum(u => u.Visitas);

                        //al referal le tengo que dar la mitad de los puntos del userid
                        referal.VisitasReferidos = visitasReferidas / 2;

                        _tablePersister.Update(referal);
                    }
                }
                catch (Exception)
                {
                    messagesToDequeue.Remove(filteredMessage);
                    Trace.TraceWarning("Error: user={0} referal={1}", filteredMessage.Data.UserId, filteredMessage.Data.Referal);
                }
            }

            _tableContext.SaveChangesWithRetries();

            messagesRemover.RemoveProcessedMessages(messagesToDequeue);
        }

	}

}