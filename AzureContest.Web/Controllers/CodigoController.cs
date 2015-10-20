using System.Net.Http;
using System.Web.Mvc;

namespace AzureContest.Web.Controllers
{
    public class CodigoController : Controller
    {
        // GET: Codigo
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Get(string nombre, string apellido, string email)
        {
            var httpClient = new HttpClient();

            var request = httpClient.GetAsync($"http://api.alebanzas.com.ar/api/dreamspark?nombre={nombre}&apellido={apellido}&email={email}");

            var codigo = request.Result.Content.ReadAsStringAsync().Result;

            return PartialView("Get", codigo);
        }

    }
}