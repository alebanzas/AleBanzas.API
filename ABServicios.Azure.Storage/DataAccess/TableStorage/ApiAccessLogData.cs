using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class ApiAccessLogData : TableEntity
    {
        public ApiAccessLogData() { }

        public ApiAccessLogData(string id, DateTime date)
        {
            PartitionKey = date.ToString("yyyyMMdd");
            RowKey = string.Format("{0}-{1}", Date.ToString("HHmmssfffffff"), Id);
            Id = id;
            Date = date.ToUniversalTime();
        }

        public DateTime Date { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string PathAndQuery { get; set; }
        
    }
}