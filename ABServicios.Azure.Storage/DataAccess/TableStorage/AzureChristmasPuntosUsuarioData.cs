using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class AzureChristmasPuntosUsuarioData : TableDataRow
    {
        public string UserId { get; set; }
        public int Puntos { get; set; }
        
	    public AzureChristmasPuntosUsuarioData()
	    {
	    }

        public AzureChristmasPuntosUsuarioData(string userId, int points)
        {
            UserId = userId;
            Puntos = points;
	    }

		protected override string CreatePartitionKey()
		{
            return UserId;
		}
        
		protected override string CreateRowKey()
		{
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff");
		}
    }
}