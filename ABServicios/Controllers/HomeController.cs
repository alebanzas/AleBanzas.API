using System.Web.Mvc;
using ABServicios.Api.Controllers;
using ABServicios.Api.Models;
using ABServicios.Models;
using ABServicios.Services;

namespace ABServicios.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return HttpNotFound();
        }

        public ActionResult RemoveAll()
        {
            var cache = new WebCache();

            cache.Evict<BicicletasStatusModel>(BicicletaController.CacheKey);
            cache.Evict<TrenesStatusModel>(TrenController.CacheKey);
            cache.Evict<SubteStatusModel>(SubteController.CacheKey);
            cache.Evict<DivisaModel>(CotizacionController.CacheKey);
            cache.Evict<DivisaModel>(CotizacionController.CacheKeyRofex);
            
            
            return new HttpStatusCodeResult(200);
        }
    }
}
