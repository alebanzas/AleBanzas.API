using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AB.Common.Helpers;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using GeoAPI.Geometries;
using Microsoft.Practices.ServiceLocation;
using NetTopologySuite.Geometries;

namespace ABServicios.Controllers
{
    public class TransporteController : BaseController
    {
        private readonly IRepository<Transporte> _transportesRepo;

        public TransporteController()
        {
            _transportesRepo = ServiceLocator.Current.GetInstance<IRepository<Transporte>>();
        }

        //
        // GET: /Transporte/

        [NeedRelationalPersistence]
        public ActionResult All(int cant = 100, bool puntos = false)
        {
            return Json(_transportesRepo.Take(cant).ToList().Select(x => x.ToTransporteViewModel(puntos)), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult Subte(bool puntos = false)
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Subte).ToList().Select(x => x.ToTransporteViewModel(puntos)), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult Tren(bool puntos = false)
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Tren).ToList().Select(x => x.ToTransporteViewModel(puntos)), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult TrenPorLinea(string linea, bool puntos = false)
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Tren && x.Linea == linea.ToUrl()).ToList().Select(x => x.ToTransporteViewModel(puntos)), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult PorId(Guid id, bool puntos = false)
        {
            return Json(_transportesRepo.FirstOrDefault(x => x.ID == id).ToTransporteViewModel(puntos), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult PorLinea(string linea, bool puntos = false)
        {
            return Json(_transportesRepo.Where(x => x.Linea == linea.ToUrl()).ToList().Select(x => x.ToTransporteViewModel(puntos)), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult ColectivoPorLinea(string linea, bool puntos = false)
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Colectivo && x.Linea == linea.ToUrl()).ToList().Select(x => x.ToTransporteViewModel(puntos)), JsonRequestBehavior.AllowGet);
        }

        [NeedRelationalPersistence]
        public ActionResult Cercano(double lat, double lon, string tipo, int cant = int.MaxValue, int caminar = 800, bool puntos = false)
        {
            var query = ServiceLocator.Current.GetInstance<IGetTransporteCercanoQuery>();

            var list = query.GetMasCercanos(new Point(lon, lat), caminar);

            var result = list.Select(x => x.ToTransporteViewModel(puntos));

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }


    public static class TransporteExtensions
    {
        public static TransporteViewModel ToTransporteViewModel(this Transporte transporte, bool incluyePuntos = false)
        {
            return new TransporteViewModel
                    {
                        ID = transporte.ID,
                        TipoNickName = transporte.Tipo.Nombre.ToUrl(),
                        Nombre = transporte.Nombre,
                        Linea = transporte.Linea,
                        Ramal = transporte.Ramal,
                        Puntos = incluyePuntos ? transporte.Ubicacion.Coordinates.ToPuntoViewModel() : new List<PuntoViewModel>(),
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
        public Guid ID { get; set; }

        public string TipoNickName { get; set; }

        public string Nombre { get; set; }

        public string Linea { get; set; }

        public string Ramal { get; set; }

        public List<PuntoViewModel> Puntos { get; set; }

        public string RecorridoText { get; set; }
    }

}
