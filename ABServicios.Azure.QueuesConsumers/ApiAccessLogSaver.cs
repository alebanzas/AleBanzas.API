using System;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.QueuesConsumers
{
    public class ApiAccessLogSaver : IQueueMessageConsumer<ApiAccessLog>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(40);

	    public TimeSpan? EstimatedTimeToProcessMessageBlock { get; private set; }

        private static TableServiceContext _tableContext;
        private static TablePersister<ApiAccessLogData> _tablePersister;

        public ApiAccessLogSaver()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<ApiAccessLogData>(_tableContext);
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
            _tableContext.SaveChangesWithRetries();
        }
	}
}