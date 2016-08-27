using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using ABServicios.Api.Controllers;
using ABServicios.Api.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ABServicios.API.Tests
{
    public class ApiTests
    {
        [Test]
        public void WhenGetTren()
        {
            var r = TrenController.GetModel();
            Assert.IsNotNull(r.Lineas.First());
        }

        [Test]
        public void WhenGetTrenesThenHttp200()
        {
            var httpClient = Api.GetHttpClient();
            var result = httpClient.GetAsync("/tren".ToAbsoluteUri()).Result;
            var value = result.Content.ReadAsStringAsync().Result;
            var r = JsonConvert.DeserializeObject<TrenesStatusModel>(value);

            Assert.IsNotNull(r.Lineas.First());
        }

        [Test]
        public void WhenGetCotizacion()
        {
            var r = CotizacionController.GetModel();
            Assert.IsNotNull(r.Divisas.First());
        }

        [Test]
        public void WhenGetTasasThenHttp200()
        {
            var httpClient = Api.GetHttpClient();
            var result = httpClient.GetAsync("/cotizacion/tasas".ToAbsoluteUri()).Result;
            var value = result.Content.ReadAsStringAsync().Result;
            var r = JsonConvert.DeserializeObject<DivisaModel>(value);

            Assert.IsNotNull(r.Divisas.First());
        }

        [Test]
        public void WhenGetRofexThenHttp200()
        {
            var httpClient = Api.GetHttpClient();
            var result = httpClient.GetAsync("/cotizacion/rofex".ToAbsoluteUri()).Result;
            var value = result.Content.ReadAsStringAsync().Result;
            var r = JsonConvert.DeserializeObject<DivisaModel>(value);

            Assert.IsNotNull(r.Divisas.First());
        }

        [Test]
        public void WhenGetDivisasThenHttp200()
        {
            var httpClient = Api.GetHttpClient();
            var result = httpClient.GetAsync("/cotizacion/divisas".ToAbsoluteUri()).Result;
            var value = result.Content.ReadAsStringAsync().Result;
            var r = JsonConvert.DeserializeObject<DivisaModel>(value);

            Assert.IsNotNull(r.Divisas.First());
        }

    }
}
