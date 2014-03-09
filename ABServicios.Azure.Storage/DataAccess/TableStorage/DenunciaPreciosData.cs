using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class DenunciaPreciosData : TableDataRow
    {
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


	    public DenunciaPreciosData()
	    {
	    }

        public DenunciaPreciosData(string id, DateTime dateTime)
	    {
	        Id = id;
            Date = dateTime.ToUniversalTime();
	    }

		protected override string CreatePartitionKey()
		{
            return GetPartionKeyFor(Date);
		}
        
		protected override string CreateRowKey()
		{
            return string.Format("{0}-{1}", Date.ToString("HHmmssfffffff"), Id);
		}

	    public static string GetPartionKeyFor(DateTime date)
	    {
            return date.ToString("yyyyMMdd");
	    }
    }
}