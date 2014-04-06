using System.Collections.Generic;
using System.Linq;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using NHibernate;

namespace AB.Data.Queries
{
	public class SampleQuery : ISampleQuery
	{
		private readonly ISessionFactory sessionFactory;

        public SampleQuery(ISessionFactory sessionFactory)
		{
			this.sessionFactory = sessionFactory;
		}

		/// <summary>
		/// Trae todas las Categorías (de una parte)
		/// </summary>
		/// <param name="tipoCategoria">1 Ficha Técnica | 2 Equipamineto</param>
		/// <param name="paisId"></param>
		/// <returns></returns>
		public IList<Hotel> GetCategorias(int tipoCategoria, int paisId)
		{
			var session = sessionFactory.GetCurrentSession();
			var query = session.CreateQuery("from Categoria as c where c.Pais.ID =:PaisID and c.TipoCategoria = :TipoCategoria order by c.Nombre")
					.SetInt32("PaisID", paisId)
					.SetInt32("TipoCategoria", tipoCategoria)
					.SetReadOnly(true)
					.SetCacheable(true)
					.SetCacheRegion("static");
            return query.List<Hotel>();
		}

        public Hotel GetCategoria(string categoria, int paisId)
		{
			var session = sessionFactory.GetCurrentSession();
			var query = session.CreateQuery("from Categoria as c where c.Pais.ID =:PaisID and c.Nombre = :Categoria")
					.SetInt32("PaisID", paisId)
					.SetString("Categoria", categoria)
					.SetReadOnly(true)
					.SetMaxResults(1);
            return query.List<Hotel>().FirstOrDefault();
		}
	}
}