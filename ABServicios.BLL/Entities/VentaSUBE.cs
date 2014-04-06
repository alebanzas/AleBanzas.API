using System;
using NetTopologySuite.Geometries;

namespace ABServicios.BLL.Entities
{
    public class VentaSUBE : AbstractEntity<Guid>
    {
        public VentaSUBE()
        {
            Ubicacion = new Point(0, 0);
        }

        public virtual string Nombre { get; set; }

        public virtual Point Ubicacion { get; set; }
    }
}
