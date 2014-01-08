using System;
using System.Web.Mvc;
using ABServicios.Api.Controllers;
using ABServicios.Attributes;

namespace ABServicios.Controllers
{
    public class StatusController : BaseController
    {
        //
        // GET: /Status/Test

        public ActionResult Test()
        {
            return View();
        }


        //
        // GET: /Status/Check

        public ActionResult Check()
        {
            return new HttpStatusCodeResult(200);
        }

        //
        // GET: /Ping/
        [NeedRelationalPersistence]
        public ActionResult Ping()
        {
            try
            {
                (new SubteController()).Get();
                (new TrenController()).Get();
                (new CotizacionController()).Divisas();
                (new BicicletaController()).Get();
                (new RecargaSubeController()).Get(-54, -38, 10);
                (new VentaSubeController()).Get(-54, -38, 10);
            }
            catch(Exception ex)
            {
                string statusDescription = (ex.InnerException != null ? ex.InnerException.Message : "") + ex.Message;
                return new HttpStatusCodeResult(500, statusDescription.Length >= 512 ? statusDescription.Substring(0, 511) : statusDescription);
            }
            return new HttpStatusCodeResult(200, "OK");
        }

    }
}
