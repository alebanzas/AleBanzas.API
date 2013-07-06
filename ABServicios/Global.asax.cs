using System.Web.Mvc;
using System.Web.Routing;
using AB.Common.Wiring;
using AB.Data;
using ABServicios.Azure.Storage;
using ABServicios.Services;

namespace ABServicios
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static IGuyWire guywire;
        private static ICacheProvider cacheProvider = new WebCache();

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            guywire = new MvcGuyWire();
            guywire.Wire();

            AreaRegistration.RegisterAllAreas();

            FullStorageInitializer.Initialize();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_End()
        {
            guywire.Dewire();
        }
    }
}