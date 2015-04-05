using System;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;

namespace ABServicios.Azure.QueuesConsumers
{
    public class DenunciaPreciosSaver : IQueueMessageConsumer<DenunciaPrecios>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromHours(1);

	    public TimeSpan? EstimatedTimeToProcessMessageBlock { get; private set; }
        
        private static TablePersister<DenunciaPreciosData> _tablePersister;

        public DenunciaPreciosSaver()
        {
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tablePersister = new TablePersister<DenunciaPreciosData>(tableClient);
        }

        public void ProcessMessages(QueueMessage<DenunciaPrecios> message)
        {
            if (message.DequeueCount > 100)
                return;

            var messageLog = message.Data;
            _tablePersister.Add(new DenunciaPreciosData(message.Id, messageLog.Date)
                {
                    Address = messageLog.Address,
                    AppId = messageLog.AppId,
                    AppVersion = messageLog.AppVersion,
                    Comment = messageLog.Comment,
                    InstallationId = messageLog.InstallationId,
                    Lat = messageLog.Lat,
                    Lon = messageLog.Lon,
                    MarketId = messageLog.MarketId,
                    MarketName = messageLog.MarketName,
                    ProductId = messageLog.ProductId,
                    RegionId = messageLog.RegionId,
                    TrackingId = messageLog.TrackingId,
                    Type = messageLog.Type,
                });
        }
	}
}