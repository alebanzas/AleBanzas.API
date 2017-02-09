using System;
using ABServicios.Api.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class LiveTrenControllerTests
    {
        [Test]
        public void WhenGetModel()
        {
            var model = LiveTrenController.GetModelFromMinInterior("test", new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=1&rnd={0}&key={1}"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=1"));

        }

    }

}
