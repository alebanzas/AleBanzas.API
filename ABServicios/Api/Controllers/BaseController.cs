using System.Globalization;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ABServicios.Api.Controllers
{
    public class BaseController : ApiController
    {
        protected override void Initialize(HttpControllerContext requestContext)
        {
            base.Initialize(requestContext);
            var culture = CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
