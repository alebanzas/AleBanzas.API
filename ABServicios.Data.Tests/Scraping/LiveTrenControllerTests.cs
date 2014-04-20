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
            var model = LiveTrenController.GetModelFromMinInterior(new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/ajax_arribos.php?ramal=1&rnd=5E5HvXkCkDz2JW0H&key=v%23v%23QTUtWp%23MpWRy80Q0knTE10I30kj%23FNyZ"), new Uri("http://trenes.mininterior.gov.ar/v2_pg/arribos/index.php?ramal=1"));

        }

    }

}
