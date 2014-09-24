using ABServicios.BLL.Entities;

namespace ABServicios.Models
{
    public class HotelViewModel
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Barrio { get; set; }
        public string Ciudad { get; set; }
        public string Provincia { get; set; }
        public string Sitio { get; set; }
        public string Telefono { get; set; }

        public string Id1 { get; set; }
        public string Id2 { get; set; }
        public string Id3 { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public static HotelViewModel ToViewModel(Hotel hotel)
        {
            return new HotelViewModel
                                    {
                                        Nombre = hotel.Nombre,
                                        Barrio = hotel.Barrio,
                                        Ciudad = hotel.Ciudad,
                                        Descripcion = hotel.Descripcion,
                                        Direccion = hotel.Direccion,
                                        Id1 = hotel.Id1,
                                        Id2 = hotel.Id2,
                                        Id3 = hotel.Id3,
                                        Latitud = hotel.Ubicacion.X,
                                        Longitud = hotel.Ubicacion.Y,
                                        Provincia = hotel.Provincia,
                                        Sitio = hotel.Sitio,
                                        Telefono = hotel.Telefono,
                                    };
        }
    }
}