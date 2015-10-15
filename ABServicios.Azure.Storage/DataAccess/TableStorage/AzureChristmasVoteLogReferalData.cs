using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasVoteLogReferalData : TableEntity
    {
        public AzureChristmasVoteLogReferalData() { }

        public AzureChristmasVoteLogReferalData(string userId, string referalId)
        {
            PartitionKey = PKey;
            ReferalId = referalId;
            RowKey = userId + "|" + referalId;
        }

        //Aca podria escalar por pais
	    public static string PKey = "PK";

        public string ReferalId { get; set; }
    }
}