using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	/// <summary>
	/// Record de Table de marcas de catalogos
	/// </summary>
	/// <remarks>
	/// Se copian directamente los valores de la tabla de marcas que pueden servir en varias ocasiones en Azure.
	/// </remarks>
	public class TableSampleData : TableEntity
	{
        public TableSampleData() { }

	    public TableSampleData(string pk, string rk)
	    {
	        PartitionKey = pk;
	        RowKey = rk;
	    }

		public string ToplevelDomain { get; set; }
		public int MarcaId { get; set; }
		public string Nickname { get; set; }
		public string Nombre { get; set; }
	}
}