using System;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;

namespace ABServicios.Azure.QueuesConsumers
{
    public class ApiAccessLogSaver : IQueueMessageConsumer<ApiAccessLog>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(40);

	    public TimeSpan? EstimatedTimeToProcessMessageBlock { get; private set; }
        
        private static TablePersister<ApiAccessLogData> _tablePersister;

        public ApiAccessLogSaver()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tablePersister = new TablePersister<ApiAccessLogData>(tableClient);
        }

        public void ProcessMessages(QueueMessage<ApiAccessLog> message)
        {
            if (message.DequeueCount > 100)
                return;

            var messageLog = message.Data;
            _tablePersister.Add(new ApiAccessLogData(message.Id ,messageLog.DateTime)
                {
                    Host = messageLog.Host,
                    PathAndQuery = messageLog.PathAndQuery,
                    Type = messageLog.FullUrl,
                });
        }
	}
}