using System.Collections.Generic;
using ABServicios.BLL.Entities;
using NetTopologySuite.Geometries;

namespace ABServicios.BLL.DataInterfaces.Queries
{
    public interface IGetTransporteCercanoQuery
	{
		IList<Transporte> GetMasCercanos(Point origen, int maxDistance);
	}
}