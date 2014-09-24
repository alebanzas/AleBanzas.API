using System;
using System.Collections.Generic;

namespace ABServicios.Models
{
    public class BicicletasStatusModel
    {
        public BicicletasStatusModel()
        {
            Estaciones = new List<BicicletaEstacion>();
        }

        public IList<BicicletaEstacion> Estaciones { get; set; }

        public DateTime Actualizacion { get; set; }

        public string ActualizacionStr
        {
            get { return string.Format("{0} {1}", Actualizacion.ToLongDateString(), Actualizacion.ToLongTimeString()); }
        }
    }

    public class BicicletaEstacion
    {
        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public string Nombre { get; set; }

        public string Estado { get; set; }

        public string Horario { get; set; }

        public int Cantidad { get; set; }
    }
}