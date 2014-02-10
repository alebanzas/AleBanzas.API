using ABServicios.Api.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class ReservasControllerTests
    {
        [Test]
        public void WhenGetModel()
        {
            var model = ReservasController.GetModelFromBCRA();

        }

    }

}
