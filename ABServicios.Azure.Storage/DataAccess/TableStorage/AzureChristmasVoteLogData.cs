﻿using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasVoteLogData : TableDataRow
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public string Ip { get; set; }

        public string Referer { get; set; }


	    public AzureChristmasVoteLogData()
	    {
	    }

	    public AzureChristmasVoteLogData(string id, string userId)
	    {
	        Id = id;
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