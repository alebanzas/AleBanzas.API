using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AB.Common.Helpers;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using GeoAPI.Geometries;
using Microsoft.Practices.ServiceLocation;
using NetTopologySuite.Geometries;

namespace ABServicios.Controllers
{
    public class TransporteController : Controller
    {
        private readonly IRepository<Transporte> _transportesRepo;

        public TransporteController()
        {
            _transportesRepo = ServiceLocator.Current.GetInstance<IRepository<Transporte>>();
        }

        //
        // GET: /Transporte/

        [NeedRelationalPersistence]
        public ActionResult All(int cant = 100)
        {
            return Json(_transportesRepo.Take(cant).ToList().Select(ConvertTo), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult Subte()
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Subte).ToList().Select(ConvertTo), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult Tren()
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Tren).ToList().Select(ConvertTo), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult TrenPorLinea(string linea)
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Tren && x.Linea == linea.ToUrl()).ToList().Select(ConvertTo), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult PorId(Guid id)
        {
            return Json(ConvertTo(_transportesRepo.FirstOrDefault(x => x.ID == id)), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult ColectivoPorLinea(string linea)
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Colectivo && x.Linea == linea.ToUrl()).ToList().Select(ConvertTo), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult Cercano(double lat, double lon, string tipo, int cant = int.MaxValue, int caminar = 800)
        {
            var minCaminar = (double)caminar / 1080000; //paso a grados

            var source = new Point(lat, lon);

            IEnumerable<Transporte> transportes = _transportesRepo;

            IEnumerable<Transporte> query = transportes.Where(point =>
                                    source.Distance(point.Ubicacion) < minCaminar);

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                query = query.Where(x => x.Tipo == TiposTransporte.ByNickName(tipo));
            }

            var result = query.Take(cant).ToList().Select(ConvertTo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private TransporteViewModel ConvertTo(Transporte transporte)
        {
            return new TransporteViewModel
            {
                TipoNickName = transporte.Tipo.Nombre.ToUrl(),
                Nombre = transporte.Nombre,
                Linea = transporte.Linea,
                Ramal = transporte.Ramal,
                Puntos = transporte.Ubicacion.Coordinates.ToPuntoViewModel(),
                RecorridoText = transporte.RecorridoText,
            };
        }
    }

    public static class CoordinatesExtensions
    {
        public static List<PuntoViewModel> ToPuntoViewModel(this Coordinate[] source)
        {
            return source.Select(x => new PuntoViewModel
                {
                    X = x.X,
                    Y = x.Y,
                }).ToList();
        }
    }

    public class PuntoViewModel
    {
        public double X { get; set; }

        public double Y { get; set; }
    }

    public class TransporteViewModel
    {
        public string TipoNickName { get; set; }

        public string Nombre { get; set; }

        public string Linea { get; set; }

        public string Ramal { get; set; }

        public List<PuntoViewModel> Puntos { get; set; }

        public string RecorridoText { get; set; }
    }

}
