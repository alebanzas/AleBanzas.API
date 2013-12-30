using System.Web.Mvc;
using System.Web.Routing;
using ABServicios.Extensions;

namespace ABServicios
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Redirect("subte", "/api/subte");
            routes.Redirect("trenes", "/api/tren");
            routes.Redirect("divisa", "/api/cotizacion");
            routes.Redirect("divisa/rofex", "/api/cotizacion/rofex");
            routes.Redirect("transporte/cercano", "/api/transporte/");
            routes.Redirect("transporte/porlinea", "/api/transporte/");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
