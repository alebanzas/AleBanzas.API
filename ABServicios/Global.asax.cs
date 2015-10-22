using AB.Common.Extensions;
using ABServicios.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AB.Common.Wiring;
using AB.Data;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace ABServicios
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private TelemetryClient _telemetry;
        private static IGuyWire guywire;
        private static ICacheProvider cacheProvider = new WebCache();
        

        protected void Application_Start()
        {
            _telemetry = new TelemetryClient();
            _telemetry.Context.InstrumentationKey = ConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];
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
            var context = HttpContext.Current;
            if (context == null) return;

            var ex = context.Server.GetLastError();

            ex.Log(context);

            _telemetry.TrackException(ex);
            //no hago el clear, porque sino no entra en el customerrors
            //context.Server.ClearError();
        }

        protected void Application_End()
        {
            guywire.Dewire();
        }
    }
}
