using System.Collections.Generic;
using ABServicios.BLL.Entities;
using NetTopologySuite.Geometries;

namespace ABServicios.BLL.DataInterfaces.Queries
{
    public interface IGetSUBECercanoQuery
    {
        IList<VentaSUBE> GetVentaMasCercanos(Point origen, int count);

        IList<RecargaSUBE> GetRecargaMasCercanos(Point origen, int count);
	}
}