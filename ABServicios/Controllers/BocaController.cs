using System.Web.Mvc;
using ABServicios.Attributes;

namespace ABServicios.Controllers
{
    [NeedRelationalPersistence]
    public class BocaController : BaseController
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
