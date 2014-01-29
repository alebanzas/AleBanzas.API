using ABServicios.Api.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class DivisaControllerTests
    {
        [Test]
        public void WhenGetModel()
        {
            var model = CotizacionController.GetModel();

        }

        [Test]
        public void WhenGetRofexModel()
        {
            var model = CotizacionController.GetRofexModel();

        }
    }

}
