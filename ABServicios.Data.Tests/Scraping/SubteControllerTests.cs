using System;
using ABServicios.Api.Controllers;
using NUnit.Framework;

namespace ABServicios.Data.Tests.Scraping
{
    public class SubteControllerTests
    {
        [Test]
        public void WhenGetModel()
        {
            var model = SubteController.GetModel();

        }

        [Test]
        public void WhenSegsToStr()
        {
            Console.WriteLine(SegToMinStr(540));
            Console.WriteLine(SegToMinStr(545));
            Console.WriteLine(SegToMinStr(550));

        }
        private string SegToMinStr(int num)
        {
            if (num == 0) return string.Empty;

            int min = num / 60;
            var seg = (int) Math.Round(((num / 60f) - min) * 60f, 0);
            if (seg == 0)
                return "(cada " + min + " mins)";

            return "(cada " + min + ":" + seg.ToString("D2") + " mins)";
        }
    }

}
