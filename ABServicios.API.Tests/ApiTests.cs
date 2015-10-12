using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using ABServicios.Api.Models;
using NUnit.Framework;

namespace ABServicios.API.Tests
{
    public class ApiTests
    {
        [Test]
        public void WhenGetReservasThenHttp200()
        {
            var httpClient = Api.GetHttpClient();

            HttpResponseMessage result = httpClient.GetAsync("http://api.alebanzas.com.ar/api/reservas").Result;
            var serializer = new DataContractJsonSerializer(typeof(ReservasModel));
            var o = (ReservasModel)serializer.ReadObject(result.Content.ReadAsStreamAsync().Result);

        }

    }
}
