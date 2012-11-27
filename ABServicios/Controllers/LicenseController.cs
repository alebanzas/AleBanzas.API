using System.Web.Mvc;

namespace ABServicios.Controllers
{
    public class LicenseController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CPanel()
        {
            //return new HttpUnauthorizedResult();
            return new HttpStatusCodeResult(200);
        }
    }
}
