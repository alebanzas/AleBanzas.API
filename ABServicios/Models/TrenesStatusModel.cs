using System;
using System.Collections.Generic;

namespace ABServicios.Models
{
    public class TrenesStatusModel
    {
        public TrenesStatusModel()
        {
            Lineas = new List<LineaTrenModel>();
        }

        public IList<LineaTrenModel> Lineas { get; set; }

        public DateTime Actualizacion { get; set; }

        public string ActualizacionStr
        {
            get { return string.Format("{0} {1}", Actualizacion.ToLongDateString(), Actualizacion.ToLongTimeString()); }
        }
    }

    public class LineaTrenModel
    {
        public string Nombre { get; set; }

        /// <summary>
        /// TODO: el peor de los estados de sus ramales
        /// </summary>
        public string Estado { get; set; }

        public List<RamalTrenModel> Ramales { get; set; }
    }

    public class RamalTrenModel
    {
        public string Nombre { get; set; }

        public string Estado { get; set; }

        public string MasInfo { get; set; }
    }
}