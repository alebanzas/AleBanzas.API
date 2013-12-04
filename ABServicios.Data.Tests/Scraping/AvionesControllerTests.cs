using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABServicios.Controllers;
using NUnit.Framework;
using ABServicios.BLL.Entities;

namespace ABServicios.Data.Tests.Scraping
{
    public class AvionesControllerTests
    {
        [Test]
        public void WhenGetAvionesModel()
        {
            var ta = new TerminalAerea(){ NickName = "EZE" };

            var model = AvionesController.GetModel(ta);


        }
    }

}
