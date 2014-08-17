using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages
{
    public class TrenEnEstacion
    {
        public string Key { get; set; }

        public string Estacion { get; set; }

        public DateTime Time { get; set; }

        public bool Vuelta { get; set; }

        public string SentidoDescription { get; set; }
    }
    public class TrenEnEstacionClean : TrenEnEstacion { }
}