using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasVoteUserData : TableDataRow
    {
        public string Id { get; set; }

        public string ReferalId { get; set; }
        public string UserId { get; set; }

	    public AzureChristmasVoteUserData()
	    {
	    }

        public AzureChristmasVoteUserData(string id, string referalId, string userId)
	    {
            Id = id;
            ReferalId = referalId;
            UserId = userId;
	    }

		protected override string CreatePartitionKey()
		{
            return ReferalId;
		}
        
		protected override string CreateRowKey()
		{
            return string.Format("{0}-{1}", DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff"), Id);
		}
    }
}