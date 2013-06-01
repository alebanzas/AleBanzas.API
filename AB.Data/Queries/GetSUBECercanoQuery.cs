using System.Collections.Generic;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using NHibernate;
using NetTopologySuite.Geometries;

namespace AB.Data.Queries
{
	public class GetSUBECercanoQuery : IGetSUBECercanoQuery
	{
		private readonly ISessionFactory sessionFactory;

        public GetSUBECercanoQuery(ISessionFactory sessionFactory)
		{
			this.sessionFactory = sessionFactory;
		}

        public IList<Transporte> GetMasCercanos(Point origen, int maxDistance)
        {
            var session = sessionFactory.GetCurrentSession();
            var query = session.CreateQuery(@"select t from Transporte t 
                                        where NHSP.Distance(t.Ubicacion, :coord) <= :maxDistance 
                                        order by NHSP.Distance(t.Ubicacion, :coord)")
                                    .SetParameter("coord", origen, NHibernateUtil.Custom(typeof(Wgs84GeographyType)))
                                    .SetDouble("maxDistance", maxDistance);

            return query.List<Transporte>();
        }

	    public IList<VentaSUBE> GetVentaMasCercanos(Point origen, int count)
	    {
            var session = sessionFactory.GetCurrentSession();
            var query = session.CreateQuery(@"select v from VentaSUBE v
                                        order by NHSP.Distance(v.Ubicacion, :coord)")
                                    .SetParameter("coord", origen, NHibernateUtil.Custom(typeof(Wgs84GeographyType)))
                                    .SetMaxResults(count);

            return query.List<VentaSUBE>();
	    }

	    public IList<RecargaSUBE> GetRecargaMasCercanos(Point origen, int count)
        {
            var session = sessionFactory.GetCurrentSession();
            var query = session.CreateQuery(@"select r from RecargaSUBE r
                                        order by NHSP.Distance(r.Ubicacion, :coord)")
                                    .SetParameter("coord", origen, NHibernateUtil.Custom(typeof(Wgs84GeographyType)))
                                    .SetMaxResults(count);

            return query.List<RecargaSUBE>();
	    }
	}
}