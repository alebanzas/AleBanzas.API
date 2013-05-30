using System;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace ABServicios.BLL.Entities
{
    public class RecargaSUBE : AbstractEntity<Guid>
    {
        public RecargaSUBE()
        {
            Ubicacion = new Point(0,0);
        }

        public virtual string Nombre { get; set; }

        public virtual double Lat { get; set; }
        public virtual double Lon { get; set; }

        public virtual Point Ubicacion { get; set; }
    }
}
