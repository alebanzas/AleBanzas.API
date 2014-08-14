using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasVoteLogData : TableDataRow
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        
        public DateTime Date { get; set; }

        public string Ip { get; set; }

        public string Referer { get; set; }

        /// <summary>
        /// quien refiere al usuario
        /// </summary>
	    public string Referal { get; set; }


	    public AzureChristmasVoteLogData()
	    {
	    }

        public AzureChristmasVoteLogData(string id, string referal, string userId)
	    {
            Id = id;
            Referal = referal;
            UserId = userId;
	    }

		protected override string CreatePartitionKey()
		{
            return UserId;
		}
        
		protected override string CreateRowKey()
		{
            return string.Format("{0}-{1}", Date.ToString("yyyyMMddHHmmssfffffff"), Id);
		}
    }
}