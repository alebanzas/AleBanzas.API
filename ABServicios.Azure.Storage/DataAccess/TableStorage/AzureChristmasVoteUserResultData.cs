namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasVoteUserResultData : TableDataRow
    {
        //Aca podria escalar por pais
	    public static string PKey = "PK";

        public string UserId { get; set; }

        public int Visitas { get; set; }
        public int VisitasReferidos { get; set; }
        public int Puntos { get; set; }

	    public AzureChristmasVoteUserResultData()
	    {
	    }

        public AzureChristmasVoteUserResultData(string userId)
	    {
            UserId = userId;
            Visitas = 0;
            VisitasReferidos = 0;
            Puntos = 0;
	    }

		protected override string CreatePartitionKey()
		{
            return PKey;
		}
        
		protected override string CreateRowKey()
		{
            return UserId;
		}
    }
}