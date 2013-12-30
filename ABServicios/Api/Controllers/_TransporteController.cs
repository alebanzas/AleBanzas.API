using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AB.Common.Helpers;
using ABServicios.Api.Extensions;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.DataInterfaces.Queries;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;
using NetTopologySuite.Geometries;

namespace ABServicios.Api.Controllers
{
    public class _TransporteController : ABServicios.Controllers.BaseController
    {
        private readonly IRepository<Transporte> _transportesRepo;

        public _TransporteController()
        {
            _transportesRepo = ServiceLocator.Current.GetInstance<IRepository<Transporte>>();
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
        public ActionResult ColectivoPorLinea(string linea, bool puntos = false)
        {
            return Json(_transportesRepo.Where(x => x.Tipo == TiposTransporte.Colectivo && x.Linea == linea.ToUrl()).ToList().Select(x => x.ToTransporteViewModel(puntos)), JsonRequestBehavior.AllowGet);
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
