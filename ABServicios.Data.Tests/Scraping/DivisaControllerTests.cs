using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABServicios.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class DivisaControllerTests
    {
        [Test]
        public void WhenGetRofexModel()
        {
            var model = DivisaController.GetRofexModel();

        }
    }

}
