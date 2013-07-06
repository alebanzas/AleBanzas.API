using System.Web.Mvc;
using ABServicios.Models;
using ABServicios.Services;

namespace ABServicios.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            //var hotel = new Hotel
            //                  {
            //                      Nombre = "PRUEBA",
            //                      Descripcion = DateTime.Now.ToString(CultureInfo.InvariantCulture),
            //                      Ubicacion = new Point(1, 2),
            //                  };

            //var cfg = new Configuration().Configure();
            //var sessionFactory = cfg.BuildSessionFactory();
            //using (var session = sessionFactory.OpenSession())
            //{
            //    using (var tx = session.BeginTransaction())
            //    {
            //        session.SaveOrUpdate(hotel);

            //        tx.Commit();
            //    }
            //}
            return View();
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
