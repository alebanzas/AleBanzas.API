﻿using System;
using System.Web.Mvc;
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
        // GET: /Ping/
        [NeedRelationalPersistence]
        public ActionResult Ping()
        {
            try
            {
                (new SubteController()).Index();
                (new TrenesController()).Index();
                (new DivisaController()).Index();
                (new BicicletasController()).Index();
                (new SUBEController()).RecargaAll();
                (new SUBEController()).VentaAll();
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
