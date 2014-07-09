using System.Collections.Generic;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using NHibernate;
using NetTopologySuite.Geometries;

namespace AB.Data.Queries
{
	public class GetTransporteCercanoQuery : IGetTransporteCercanoQuery
	{
		private readonly ISessionFactory sessionFactory;

        public GetTransporteCercanoQuery(ISessionFactory sessionFactory)
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

        public IList<Transporte> GetMasCercanos(Point origen, int maxDistance, int maxResults)
        {
            var session = sessionFactory.GetCurrentSession();
            var query = session.CreateQuery(@"select t from Transporte t 
                                        where NHSP.Distance(t.Ubicacion, :coord) <= :maxDistance 
                                        order by NHSP.Distance(t.Ubicacion, :coord)")
                                    .SetParameter("coord", origen, NHibernateUtil.Custom(typeof(Wgs84GeographyType)))
                                    .SetDouble("maxDistance", maxDistance)
                                    .SetMaxResults(maxResults);

            return query.List<Transporte>();
        }
	}
}