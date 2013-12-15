using ABServicios.Api.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class DivisaControllerTests
    {
        [Test]
        public void WhenGetRofexModel()
        {
            var model = CotizacionController.GetRofexModel();

        }
    }

}
