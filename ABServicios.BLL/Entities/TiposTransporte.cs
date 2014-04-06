using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AB.Common.Helpers;

namespace ABServicios.BLL.Entities
{
    [Serializable]
    public class TipoTransporte : AbstractEntity<Guid>
    {
        public string Nombre { get; set; }
    }

    public class TiposTransporte : ReadOnlyCollection<TipoTransporte>
    {
        private static readonly HashSet<TipoTransporte> _repository;

        public static TipoTransporte Subte = new TipoTransporte { ID = new Guid("E74E932C-BF15-4ED6-ADA6-F0CBF0688B78"), Nombre = "Subte", };
        public static TipoTransporte Tren = new TipoTransporte { ID = new Guid("440C21D3-71DE-4C94-849D-66139EADCE4C"), Nombre = "Tren", };
        public static TipoTransporte Colectivo = new TipoTransporte { ID = new Guid("8C9A672B-9103-47BF-A373-0648C0F10C5C"), Nombre = "Colectivo", };

        static TiposTransporte()
        {
            _repository = new HashSet<TipoTransporte> { Subte, Tren, Colectivo };
        }

        public TiposTransporte(IList<TipoTransporte> list) : base(list)
        {
        }

        public static IEnumerable<TipoTransporte> Repository
        {
            get { return _repository; }
        }

        public static TipoTransporte ById(Guid id)
        {
            return _repository.FirstOrDefault(x => x.ID == id);
        }

        public static TipoTransporte ByNickName(string nombre)
        {
            return _repository.FirstOrDefault(x => x.Nombre.ToUrl() == nombre);
        }
    }
}