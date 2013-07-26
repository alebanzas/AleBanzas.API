using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AB.Common.Helpers;
using NetTopologySuite.Geometries;

namespace ABServicios.BLL.Entities
{
    [Serializable]
    public class TerminalAerea
    {
        public string Nombre { get; set; }

        public string NickName { get; set; }

        public virtual Point Ubicacion { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }
    }

    public class TerminalesAereas : ReadOnlyCollection<TerminalAerea>
    {
        private static readonly HashSet<TerminalAerea> _repository;

        public static TerminalAerea Ezeiza = new TerminalAerea
        {
            Nombre = "Aeropuerto Internacional de Ezeiza \"Ministro Pistarini\"",
            NickName = "EZE",
            Ubicacion = new Point(-34.812393945083, -58.5363578796387),
            Direccion = "Autopista Tte. Gral. Ricchieri Km 33,5",
            Telefono = "(54 11) 5480 2500",
        };

        public static TerminalAerea Aeroparque = new TerminalAerea
        {
            Nombre = "Aeroparque Internacional \"Jorge Newbery\"",
            NickName = "AEP",
            Ubicacion = new Point(-34.5584560886206, 58.4167098999023),
            Direccion = "Av. Rafael Obligado s/n°",
            Telefono = "(54 11) 5480 6111",
        };
        
        static TerminalesAereas()
        {
            _repository = new HashSet<TerminalAerea> { Ezeiza, Aeroparque };
        }

        public TerminalesAereas(IList<TerminalAerea> list)
            : base(list)
        {
        }

        public static IEnumerable<TerminalAerea> Repository
        {
            get { return _repository; }
        }

        public static TerminalAerea ByNickName(string nombre)
        {
            return _repository.FirstOrDefault(x => x.NickName.ToUrl() == nombre.ToUrl());
        }
    }
}