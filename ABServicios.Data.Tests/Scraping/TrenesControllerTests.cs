using ABServicios.Api.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class TrenesControllerTests
    {
        [Test]
        public void WhenGetModel()
        {
            var model = TrenController.GetModel();

        }

    }

}
