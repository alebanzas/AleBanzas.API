using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Web.Mvc;
using reCAPTCHA.MVC;

namespace AzureContest.Web.Controllers
{
    public class CodigoController : Controller
    {
        // GET: Codigo
        public ActionResult Index()
        {
            return View("Index", new CodigoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidator(ErrorMessage = "¿Sos un robot?")]
        public ActionResult Index(CodigoModel form)
        {
            if (ModelState.IsValid)
            {
                return Get(form.Nombre, form.Apellido, form.Email);
            }

            return PartialView("Form", form);
        }

        public ActionResult Get(string nombre, string apellido, string email)
        {
            var httpClient = new HttpClient();

            var request = httpClient.GetAsync($"http://api.alebanzas.com.ar/api/dreamspark?nombre={nombre}&apellido={apellido}&email={email}");

            var codigo = request.Result.Content.ReadAsStringAsync().Result;

            return PartialView("Get", codigo);
        }

    }

    public class CodigoModel
    {
        [Required(ErrorMessage = "*")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "*")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
    }
}