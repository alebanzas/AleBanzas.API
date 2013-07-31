using System.Web.Mvc;
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

            cache.Evict<BicicletasStatusModel>(BicicletasController.CacheKey);

            cache.Evict<TrenesStatusModel>(TrenesController.CacheKey);

            cache.Evict<SubteStatusModel>(SubteController.CacheKey);

            cache.Evict<DivisaModel>(DivisaController.CacheKey);
            
            
            return new HttpStatusCodeResult(200);
        }
    }
}
