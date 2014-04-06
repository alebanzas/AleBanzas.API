using ABServicios.Api.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class BicicletasControllerTests
    {
        [Test]
        public void WhenGetBicicletasModel()
        {
            var model = BicicletaController.GetModel();

        }
    }

}
