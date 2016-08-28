using System.Web.Mvc;
using System.Web.Routing;
using AB.Common.Extensions;

namespace ABServicios
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Redirect("subte", "/api/subte");
            routes.Redirect("trenes", "/api/tren");
            routes.Redirect("bicicletas", "/api/bicicleta");
            routes.Redirect("divisa", "/api/cotizacion/divisas");
            routes.Redirect("api/cotizacion", "/api/cotizacion/divisas");
            routes.Redirect("divisa/rofex", "/api/cotizacion/rofex");
            routes.Redirect("transporte/cercano", "/api/transporte/");
            routes.Redirect("transporte/porlinea", "/api/transporte/");
            routes.Redirect("sube/recarganear", "/api/recargasube/");
            routes.Redirect("sube/ventanear", "/api/ventasube/");
            routes.Redirect("aviones/arribos", "/api/avion/arribos");
            routes.Redirect("aviones/partidas", "/api/avion/partidas");
            routes.Redirect("azurelog", "/status/ok");
            routes.Redirect("home/index", "/status/ok");
            routes.Redirect("dreamspark", "/status/ok");
            routes.Redirect("report/error", "/status/ok");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Status", action = "Ok", id = UrlParameter.Optional }
            );
        }
    }
}
