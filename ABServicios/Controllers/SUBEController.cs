using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;
using NetTopologySuite.Geometries;

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

            return Json(puntos.Select(x => x.ToRecargaSUBEViewModel()).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult VentaAll()
        {
            IEnumerable<VentaSUBE> puntos = _ventaSUBERepo;

            return Json(puntos.Select(x => x.ToVentaSUBEViewModel()).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecargaNear(double lat, double lon, int cant = 1)
        {
            var query = ServiceLocator.Current.GetInstance<IGetSUBECercanoQuery>();

            var list = query.GetRecargaMasCercanos(new Point(lon, lat), cant);

            var result = list.Select(x => x.ToRecargaSUBEViewModel());

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VentaNear(double lat, double lon, int cant = 1)
        {
            var query = ServiceLocator.Current.GetInstance<IGetSUBECercanoQuery>();

            var list = query.GetVentaMasCercanos(new Point(lon, lat), cant);

            var result = list.Select(x => x.ToVentaSUBEViewModel());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
    
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
