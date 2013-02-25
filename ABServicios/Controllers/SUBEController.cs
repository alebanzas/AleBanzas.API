using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;

namespace ABServicios.Controllers
{
    public class SUBEController : Controller
    {
        private readonly IRepository<RecargaSUBE> _recargaSUBERepo;
        private readonly IRepository<VentaSUBE> _ventaSUBERepo;

        public SUBEController()
        {
            _recargaSUBERepo = ServiceLocator.Current.GetInstance<IRepository<RecargaSUBE>>();
            _ventaSUBERepo = ServiceLocator.Current.GetInstance<IRepository<VentaSUBE>>();
		}

        //
        // GET: /SUBE/

        public ActionResult Index()
        {
            return View();
        }

    }
}
