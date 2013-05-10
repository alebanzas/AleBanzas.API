using System.Web.Mvc;
using ABServicios.Attributes;

namespace ABServicios.Controllers
{
    [NeedRelationalPersistence]
    public class BocaController : Controller
    {
        //
        // GET: /Boca/

        public ActionResult Index()
        {
            return new HttpStatusCodeResult(200);
            //return new HttpStatusCodeResult(404);
        }
    }
}
