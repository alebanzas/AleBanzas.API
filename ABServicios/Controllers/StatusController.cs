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

        public ActionResult Ok()
        {
            return new HttpStatusCodeResult(200);
        }


        //
        // GET: /Status/Check

        public ActionResult Check(string versionId)
        {
            if (versionId == "1.4.2.0d")
            {
                return new HttpStatusCodeResult(426, "probando mensaje por aca");    
            }

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
