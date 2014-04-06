using System;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	/// <summary>
	/// Record de Table de marcas de catalogos
	/// </summary>
	/// <remarks>
	/// Se copian directamente los valores de la tabla de marcas que pueden servir en varias ocasiones en Azure.
	/// </remarks>
	public class TableSampleData : TableDataRow
	{
		public string ToplevelDomain { get; set; }
		public int MarcaId { get; set; }
		public string Nickname { get; set; }
		public string Nombre { get; set; }

		protected override string CreatePartitionKey()
		{
			ThrowsIfNotValidKey();
			return GetPartionKeyFor(ToplevelDomain, MarcaId);
		}

		public static string GetPartionKeyFor(string toplevelDomain, int marcaId)
		{
			return string.Format("{0}", toplevelDomain);
		}

		protected override string CreateRowKey()
		{
			ThrowsIfNotValidKey();
			return GetRowKey(ToplevelDomain, MarcaId);
		}

		public static string GetRowKey(string toplevelDomain, int marcaId)
		{
			return string.Format("{0:d8}", marcaId);
		}

		private void ThrowsIfNotValidKey()
		{
			if (string.IsNullOrWhiteSpace(ToplevelDomain))
			{
				throw new InvalidOperationException("ToplevelDomain was not assigned.");
			}
			if (MarcaId <= 0)
			{
				throw new InvalidOperationException("MarcaId was not assigned.");
			}
		}
	}
}