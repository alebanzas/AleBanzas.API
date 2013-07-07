using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class ApiAccessLogData : TableDataRow
    {
        public DateTime Date { get; set; }
		public string Type { get; set; }
        public string Host { get; set; }
        public string PathAndQuery { get; set; }

	    public ApiAccessLogData(DateTime dateTime)
	    {
            Date = dateTime.ToUniversalTime();
	    }

		protected override string CreatePartitionKey()
		{
            return Date.ToString("yyyyMMdd");
		}
        
		protected override string CreateRowKey()
		{
            return string.Format("{0}-{1}", Date.ToString("HHmmssfffffff"), Guid.NewGuid().ToString("N"));
		}
	}
}