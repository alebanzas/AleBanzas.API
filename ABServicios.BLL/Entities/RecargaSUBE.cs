using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABServicios.BLL.Entities
{
    public class RecargaSUBE : AbstractEntity<Guid>
    {
        public RecargaSUBE()
        {
            Ubicacion = new Point(0, 0);
        }

        public virtual string Nombre { get; set; }

        public virtual double Lat { get; set; }
        public virtual double Lon { get; set; }

        public virtual Point Ubicacion
        {
            get { return new Point(Lat, Lon); }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                Lat = value.X;
                Lon = value.Y;
            }
        }
    }
}
