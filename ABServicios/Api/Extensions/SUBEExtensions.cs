using ABServicios.Api.Models;
using ABServicios.BLL.Entities;

namespace ABServicios.Api.Extensions
{
    public static class SUBEExtensions
    {
        public static RecargaSUBEViewModel ToRecargaSUBEViewModel(this RecargaSUBE puntoRecarga)
        {
            return new RecargaSUBEViewModel
            {
                Latitud = puntoRecarga.Ubicacion.Y,
                Longitud = puntoRecarga.Ubicacion.X,
                Nombre = puntoRecarga.Nombre.ToUpperInvariant(),
            };
        }

        public static VentaSUBEViewModel ToVentaSUBEViewModel(this VentaSUBE puntoVenta)
        {
            return new VentaSUBEViewModel
            {
                Latitud = puntoVenta.Ubicacion.Y,
                Longitud = puntoVenta.Ubicacion.X,
                Nombre = puntoVenta.Nombre.ToUpperInvariant(),
            };
        }
    }
}