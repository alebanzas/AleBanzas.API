using System;
using System.Collections.Generic;

namespace ABServicios.Api.Models
{
    public class PuntoViewModel
    {
        public double X { get; set; }

        public double Y { get; set; }
    }

    public class TransporteViewModel
    {
        public Guid ID { get; set; }

        public string TipoNickName { get; set; }

        public string Nombre { get; set; }

        public string Linea { get; set; }

        public string Ramal { get; set; }

        public List<PuntoViewModel> Puntos { get; set; }

        public string RecorridoText { get; set; }
    }
}