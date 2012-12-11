using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using ABServicios.Models;
using Microsoft.Practices.ServiceLocation;

namespace ABServicios.Controllers
{
    [NeedRelationalPersistence]
    public class HotelController : Controller
    {
        private readonly IRepository<Hotel> _hotelRepo;

        public HotelController()
		{
            _hotelRepo = ServiceLocator.Current.GetInstance<IRepository<Hotel>>();
		}

        public ActionResult All()
        {
            IEnumerable<Hotel> hotels = _hotelRepo;

            return Json(hotels.Select(ConvertTo).ToList());
        }

        public ActionResult Near(double lat, double lon)
        {
            var point = new Point(lat, lon);

            IEnumerable<Hotel> hotels = _hotelRepo;

            var masCercano = new Hotel();

            foreach (var h in hotels.ToList().Where(h => point.Distance(h.Ubicacion) < point.Distance(masCercano.Ubicacion)))
            {
                masCercano = h;
            }

            return Json(ConvertTo(masCercano));
        }

        public ActionResult PorBarrio(string barrio)
        {
            IEnumerable<Hotel> hotels = _hotelRepo.Where(x => x.Barrio.Equals(barrio));

            return Json(hotels.Select(ConvertTo).ToList());
        }

        private HotelViewModel ConvertTo(Hotel hotel)
        {
            return new HotelViewModel
                       {
                           Barrio = hotel.Barrio,
                           Ciudad = hotel.Ciudad,
                           Descripcion = hotel.Descripcion,
                           Direccion = hotel.Direccion,
                           Id1 = hotel.Id1,
                           Id2 = hotel.Id2,
                           Id3 = hotel.Id3,
                           Latitud = hotel.Ubicacion.X,
                           Longitud = hotel.Ubicacion.Y,
                           Nombre = hotel.Nombre,
                           Provincia = hotel.Provincia,
                           Sitio = hotel.Sitio,
                           Telefono = hotel.Telefono,
                       };
        }
   
    }
}
