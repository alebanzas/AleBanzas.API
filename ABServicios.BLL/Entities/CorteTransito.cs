using System;
using NetTopologySuite.Geometries;

namespace ABServicios.BLL.Entities
{
    public class CorteTransito : AbstractEntity<Guid>
    {
        public CorteTransito()
        {
            Ubicacion = new Point(0, 0);
        }

        public virtual string Nombre { get; set; }
        public virtual string Descripcion { get; set; }
        
        public virtual Point Ubicacion { get; set; }
    }
}