using System.Web.Mvc;

namespace ABServicios.Controllers
{
    public class HomeController : Controller
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
    }
}
