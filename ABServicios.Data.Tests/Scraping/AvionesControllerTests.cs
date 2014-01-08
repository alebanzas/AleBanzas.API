using ABServicios.Api.Controllers;
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

            var model = AvionController.GetModel(ta);


        }
    }

}
