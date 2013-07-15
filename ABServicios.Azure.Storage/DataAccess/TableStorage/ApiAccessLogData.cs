using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class ApiAccessLogData : TableDataRow
    {
        public DateTime Date { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string PathAndQuery { get; set; }

	    public ApiAccessLogData()
	    {
	    }

	    public ApiAccessLogData(string id, DateTime dateTime)
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