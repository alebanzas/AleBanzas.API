using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasVoteUserResultData : TableEntity
    {
        public AzureChristmasVoteUserResultData() { }

        public AzureChristmasVoteUserResultData(string userId)
        {
            PartitionKey = PKey;
            UserId = userId;
            RowKey = userId;
            Visitas = 0;
            VisitasReferidos = 0;
            Puntos = 0;
        }

        //Aca podria escalar por pais
	    public static string PKey = "PK";

        public string UserId { get; set; }

        public int Visitas { get; set; }
        public int VisitasReferidos { get; set; }
        public int Puntos { get; set; }
    }
}