using System;
using System.Collections.Generic;

namespace ABServicios.BLL.Entities
{
    public class TipoMoneda : ITipoMoneda
    {
        public Guid ID { get; set; }
    }

    public static class TiposMoneda
    {
        private static readonly Dictionary<Guid, ITipoMoneda> tiposMoneda;

        static TiposMoneda()
        {
            tiposMoneda = new Dictionary<Guid, ITipoMoneda>(10)
                {
                    {PesoArgentino.ID, PesoArgentino},
                    {DolarEstadounidense.ID, DolarEstadounidense},
                };
        }

        public static IEnumerable<ITipoMoneda> All
        {
            get { return tiposMoneda.Values; }
        }

        public static ITipoMoneda ByCode(Guid code)
        {
            ITipoMoneda result;
            return !tiposMoneda.TryGetValue(code, out result) ? PesoArgentino : result;
        }

        public static ITipoMoneda PesoArgentino = new TipoMoneda
            {
            };

        public static ITipoMoneda DolarEstadounidense = new TipoMoneda
            {
            };
    }
}