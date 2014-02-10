using System;
using System.Collections.Generic;

namespace ABServicios.Api.Models
{
    public class ReservasModel
    {
        public ReservasModel()
        {
            Lista = new List<ReservasItem>();
        }

        public IList<ReservasItem> Lista { get; set; }

        public DateTime Actualizacion { get; set; }

        public string ActualizacionStr
        {
            get { return string.Format("{0} {1}", Actualizacion.ToLongDateString(), Actualizacion.ToLongTimeString()); }
        }
    }

    public class ReservasItem
    {
        public DateTime Fecha { get; set; }

        public int Monto { get; set; }
    }
}