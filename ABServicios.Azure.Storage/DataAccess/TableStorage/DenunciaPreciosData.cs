using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class DenunciaPreciosData : TableEntity
    {
        public DenunciaPreciosData() { }

        public DenunciaPreciosData(string id, DateTime date)
        {
            PartitionKey = date.ToString("yyyyMMdd");
            RowKey = $"{date.ToString("HHmmssfffffff")}-{id}";
            Id = id;
            Date = date.ToUniversalTime();
        }

        public DateTime Date { get; set; }
        public string Id { get; set; }
        public string AppId { get; set; }
        public string AppVersion { get; set; }
        public string InstallationId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
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