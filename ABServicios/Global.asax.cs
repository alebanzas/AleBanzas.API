using ABServicios.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AB.Common.Wiring;
using AB.Data;
using ABServicios.Azure.Storage;
using ABServicios.Controllers;
using ABServicios.Extensions;
using ABServicios.Services;

namespace ABServicios
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IGuyWire guywire;
        private static ICacheProvider cacheProvider = new WebCache();

        protected void Application_Start()
        {
            guywire = new MvcGuyWire();
            guywire.Wire();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DataConfig.StartInitial();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context == null) return;

            var ex = context.Server.GetLastError();

            ex.Log(context);

            //no hago el clear, porque sino no entra en el customerrors
            //context.Server.ClearError();
        }

        protected void Application_End()
        {
            guywire.Dewire();
        }
    }
}
