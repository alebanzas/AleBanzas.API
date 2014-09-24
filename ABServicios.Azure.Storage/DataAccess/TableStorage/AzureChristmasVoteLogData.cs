using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
    public class AzureChristmasVoteLogData : TableEntity
    {
        public AzureChristmasVoteLogData(){}

        public AzureChristmasVoteLogData(string referal, string userId, DateTime date)
        {
            Referal = referal;
            UserId = userId;
            PartitionKey = userId;
            Date = date;
            RowKey = date.ToString("yyyyMMddHHmmssfffffff") + Guid.NewGuid();
        }
        

        public string UserId { get; set; }
        
        public DateTime Date { get; set; }

        public string Ip { get; set; }

        public string Referer { get; set; }

        /// <summary>
        /// quien refiere al usuario
        /// </summary>
	    public string Referal { get; set; }
    }
}