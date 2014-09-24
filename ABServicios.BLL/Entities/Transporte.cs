using System;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace ABServicios.BLL.Entities
{
    public class Transporte : AbstractEntity<Guid>
    {
        public Transporte()
        {
            Ubicacion = new LineString(new Coordinate[0]);
        }

        public virtual TipoTransporte Tipo { get; set; }

        public virtual string Nombre { get; set; }

        public virtual string Linea { get; set; }
        
        public virtual string Ramal { get; set; }

        public virtual LineString Ubicacion { get; set; }

        public virtual string RecorridoText { get; set; }

        public virtual bool Regreso { get; set; }
    }
}
