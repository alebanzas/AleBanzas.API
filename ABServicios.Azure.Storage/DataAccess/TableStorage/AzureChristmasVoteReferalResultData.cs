using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasVoteReferalResultData : TableEntity
    {
        public AzureChristmasVoteReferalResultData() { }

        public AzureChristmasVoteReferalResultData(string referalId)
        {
            PartitionKey = PKey;
            ReferalId = referalId;
            RowKey = referalId;
            Puntos = 0;
        }

        //Aca podria escalar por pais
	    public static string PKey = "PK";

        public string ReferalId { get; set; }
        
        public int Puntos { get; set; }
    }
}