using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABServicios.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class BicicletasControllerTests
    {
        [Test]
        public void WhenGetBicicletasModel()
        {
            var model = BicicletasController.GetModel();

        }
    }

}
