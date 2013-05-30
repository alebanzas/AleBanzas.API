using System;
using System.Collections.Generic;

namespace ABServicios.Models
{
    public class SubteStatusModel
    {
        public SubteStatusModel()
        {
            Lineas = new List<SubteStatusItem>();
        }

        public IList<SubteStatusItem> Lineas { get; set; }

        public DateTime Actualizacion { get; set; }

        public string ActualizacionStr
        {
            get { return string.Format("{0} {1}", Actualizacion.ToLongDateString(), Actualizacion.ToLongTimeString()); }
        }
    }

    public class SubteStatusItem
    {
        public string Nombre { get; set; }

        public string Detalles { get; set; }
    }
}