using NHibernate.Spatial.Type;
using System;

namespace AB.Data
{
    [Serializable]
    public class Wgs84GeographyType : MsSql2008GeographyType
    {
        //protected override void SetDefaultSRID(IGeometry geometry)
        //{
        //    geometry.SRID = 4326;
        //}
    }
}
