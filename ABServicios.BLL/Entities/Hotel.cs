using System;
using NetTopologySuite.Geometries;

namespace ABServicios.BLL.Entities
{
    public class Hotel : AbstractEntity<Guid>
    {
        public Hotel()
        {
            Ubicacion = new Point(0, 0);
        }

        public virtual string Nombre { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Direccion { get; set; }
        public virtual string Barrio { get; set; }
        public virtual string Ciudad { get; set; }
        public virtual string Provincia { get; set; }
        public virtual string Sitio { get; set; }
        public virtual string Telefono { get; set; }

        public virtual string Id1 { get; set; }
        public virtual string Id2 { get; set; }
        public virtual string Id3 { get; set; }

        public virtual double Lat { get; set; }
        public virtual double Lon { get; set; }

        public virtual Point Ubicacion { get; set; }
        
        public override string ToString()
        {
            return string.Format("{0} {1} {2}, {3}, {4}, {5}, {6} | {7} | {8}", base.ID, Nombre, Direccion, Barrio, Ciudad, Provincia, Telefono, Sitio, string.Format("{0}, {1}", Ubicacion.X, Ubicacion.Y));
        }
    }
}