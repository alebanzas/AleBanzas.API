using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using GisSharpBlog.NetTopologySuite.Geometries;
using Microsoft.Practices.ServiceLocation;
using NHibernate;

namespace ABServicios.Controllers
{
    [NeedRelationalPersistence]
    public class SUBEController : Controller
    {
        private readonly IRepository<RecargaSUBE> _recargaSUBERepo;
        private readonly IRepository<VentaSUBE> _ventaSUBERepo;

        public SUBEController()
        {
            _recargaSUBERepo = ServiceLocator.Current.GetInstance<IRepository<RecargaSUBE>>();
            _ventaSUBERepo = ServiceLocator.Current.GetInstance<IRepository<VentaSUBE>>();
		}


        public ActionResult RecargaAll()
        {
            IEnumerable<RecargaSUBE> puntos = _recargaSUBERepo;

            return Json(puntos.Select(ConvertTo).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult VentaAll()
        {
            IEnumerable<VentaSUBE> puntos = _ventaSUBERepo;

            return Json(puntos.Select(ConvertTo).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test()
        {
            var ventaSUBE = new VentaSUBE
            {
                ID = Guid.NewGuid(),
                Ubicacion = new Point(DateTime.Now.Minute, DateTime.Now.Second),
                Nombre = DateTime.UtcNow.ToShortTimeString(),
            };
            
            var sessionFactory = ServiceLocator.Current.GetInstance<ISessionFactory>();
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                _ventaSUBERepo.Add(ventaSUBE);
                
                tx.Commit();
            }

            return Json(ventaSUBE, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecargaNear(double lat, double lon, int cant = 1)
        {
            var source = new Point(lat, lon);

            IEnumerable<RecargaSUBE> puntos = _recargaSUBERepo;

            var result =
                puntos.ToList()
                      .OrderBy(point => source.Distance(point.Ubicacion))
                      .Take(cant).Select(ConvertTo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VentaNear(double lat, double lon, int cant = 1)
        {
            var source = new Point(lat, lon);

            IEnumerable<VentaSUBE> puntos = _ventaSUBERepo;

            var result =
                puntos.ToList()
                      .OrderBy(point => source.Distance(point.Ubicacion))
                      .Take(cant).Select(ConvertTo);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        //public ActionResult PorBarrio(string barrio)
        //{
        //    IEnumerable<Hotel> hotels = _hotelRepo.Where(x => x.Barrio.Equals(barrio));

        //    return Json(hotels.Select(ConvertTo).ToList());
        //}

        private RecargaSUBEViewModel ConvertTo(RecargaSUBE hotel)
        {
            return new RecargaSUBEViewModel
            {
                Latitud = hotel.Ubicacion != null ? hotel.Ubicacion.X : 10,
                Longitud = hotel.Ubicacion != null ? hotel.Ubicacion.Y : 10,
                Nombre = hotel.Nombre.ToUpperInvariant(),
            };
        }


        private VentaSUBEViewModel ConvertTo(VentaSUBE hotel)
        {
            return new VentaSUBEViewModel
            {
                Latitud = hotel.Ubicacion.X,
                Longitud = hotel.Ubicacion.Y,
                Nombre = hotel.Nombre.ToUpperInvariant(),
            };
        }
   

    }

    public class RecargaSUBEViewModel
    {
        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public string Nombre { get; set; }
    }

    public class VentaSUBEViewModel
    {
        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public string Nombre { get; set; }
    }
}
