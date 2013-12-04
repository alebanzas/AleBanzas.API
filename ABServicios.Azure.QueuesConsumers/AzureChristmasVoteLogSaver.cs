using System;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.QueuesConsumers
{
    public class AzureChristmasVoteLogSaver : IQueueMessageConsumer<AzureChristmasVoteLog>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(40);

	    public TimeSpan? EstimatedTimeToProcessMessageBlock { get; private set; }

        private static TableServiceContext _tableContext;
        private static TablePersister<AzureChristmasVoteLogData> _tablePersister;
        private static TablePersister<AzureChristmasVoteUserData> _tableImagePersister;

        public AzureChristmasVoteLogSaver()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasVoteLogData>(_tableContext);
            _tableImagePersister = new TablePersister<AzureChristmasVoteUserData>(_tableContext);
        }

        public void ProcessMessages(QueueMessage<AzureChristmasVoteLog> message)
        {
            //if (message.DequeueCount > 100)
            //    return;

            var messageLog = message.Data;
            _tablePersister.Add(new AzureChristmasVoteLogData(message.Id, messageLog.UserId)
            {
                Date = messageLog.Date,
                Ip = messageLog.Ip,
                Referer = messageLog.Referer,
            });

            _tableImagePersister.Add(new AzureChristmasVoteUserData(message.Id, messageLog.UserId));

            _tableContext.SaveChangesWithRetries();
        }
	}
}