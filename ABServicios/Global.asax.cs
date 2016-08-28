using AB.Common.Extensions;
using ABServicios.Services;
using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AB.Common.Wiring;
using AB.Data;
using Microsoft.ApplicationInsights;

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

            //TelemetryConfiguration.CreateDefault();
            //;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DataConfig.StartInitial();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                var context = HttpContext.Current;
                if (context == null) return;

                var exception = context.Server.GetLastError();

                exception.Log(context, ExceptionAction.SendMailAndEnqueue);
                //no hago el clear, porque sino no entra en el customerrors
                //context.Server.ClearError();
            }
            catch (Exception ex)
            {
                (new SystemException("FATAL", ex)).Log(ExceptionAction.SendMail);
                throw;
            }
        }

        protected void Application_End()
        {
            guywire.Dewire();
        }
    }
}
