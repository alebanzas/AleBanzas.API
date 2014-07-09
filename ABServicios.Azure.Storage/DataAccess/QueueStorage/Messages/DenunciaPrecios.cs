using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages
{
    public class DenunciaPrecios
    {
        public string AppId { get; set; }

        public string AppVersion { get; set; }

        public string InstallationId { get; set; }

        public double Lat { get; set; }

        public double Lon { get; set; }

        public DateTime Date { get; set; }

        public Guid TrackingId { get; set; }

        public int RegionId { get; set; }

        public int MarketId { get; set; }

        public int ProductId { get; set; }

        public string Type { get; set; }

        public string Address { get; set; }

        public string MarketName { get; set; }

        public string Comment { get; set; }
    }
}
