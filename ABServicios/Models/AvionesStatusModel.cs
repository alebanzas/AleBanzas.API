using System;
using System.Collections.Generic;

namespace ABServicios.Models
{
    public class AvionesStatusModel
    {
        public AvionesStatusModel()
        {
            Terminales = new List<AvionesTerminalStatusModel>();
        }

        public IList<AvionesTerminalStatusModel> Terminales { get; set; }

        public DateTime Actualizacion { get; set; }

        public string ActualizacionStr
        {
            get { return string.Format("{0} {1}", Actualizacion.ToLongDateString(), Actualizacion.ToLongTimeString()); }
        }
    }

    public class AvionesTerminalStatusModel
    {
        public AvionesTerminalStatusModel()
        {
            Partidas = new List<VueloPartidaModel>();
            Arribos = new List<VueloArriboModel>();
        }

        public string Nombre { get; set; }

        public string NickName { get; set; }

        public IList<VueloPartidaModel> Partidas { get; set; }

        public IList<VueloArriboModel> Arribos { get; set; }
    }

    public class VueloPartidaModel
    {
        public string Nombre { get; set; }

        public string Linea { get; set; }

        public string Destino { get; set; }

        public DateTime Hora { get; set; }

        public DateTime? Estima { get; set; }

        public DateTime? Partida { get; set; }

        public string Estado { get; set; }

        public string Terminal { get; set; }
    }

    public class VueloArriboModel
    {
        public string Nombre { get; set; }

        public string Linea { get; set; }

        public string Origen { get; set; }

        public DateTime Hora { get; set; }

        public DateTime? Estima { get; set; }

        public DateTime? Arribo { get; set; }

        public string Estado { get; set; }

        public string Terminal { get; set; }
    }
}