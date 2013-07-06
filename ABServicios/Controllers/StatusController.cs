using System;
using System.Web.Mvc;
using ABServicios.Attributes;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;

namespace ABServicios.Controllers
{
    public class StatusController : BaseController
    {
        private IRepository<Hotel> _hotelRepo;

        public StatusController()
		{
            _hotelRepo = ServiceLocator.Current.GetInstance<IRepository<Hotel>>();
		}

        //
        // GET: /Status/

        public ActionResult Index()
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
