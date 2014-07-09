using System;
using System.Collections.Generic;

namespace ABServicios.Api.Models
{
    public class LiveTrenModel
    {
        public LiveTrenModel()
        {
            Estaciones = new List<LiveTrenModelItem>();
        }

        public IList<LiveTrenModelItem> Estaciones { get; set; }

        public DateTime Actualizacion { get; set; }

        public string ActualizacionStr
        {
            get { return string.Format("{0} {1}", Actualizacion.ToLongDateString(), Actualizacion.ToLongTimeString()); }
        }
    }

    public class LiveTrenModelItem
    {
        public string Nombre { get; set; }

        public int? Ida1 { get; set; }
        public int? Ida2 { get; set; }
        public int? Vuelta1 { get; set; }
        public int? Vuelta2 { get; set; }
    }
}