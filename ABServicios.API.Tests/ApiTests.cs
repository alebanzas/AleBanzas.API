using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ABServicios.Api.Controllers;
using ABServicios.Api.Models;
using ABServicios.BLL.Entities;
using ABServicios.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ABServicios.API.Tests
{
    public class ApiTests
    {
        [Test]
        public void WhenGetSUBE()
        {
            var r = (new RecargaSubeController()).Get(-58,-34);
            Assert.IsNotNull(r.First());
        }

        [Test]
        public void WhenGetBicicletas()
        {
            var r = BicicletaController.GetModel();
            Assert.IsNotNull(r.Estaciones.First());
        }

        [Test]
        public void WhenGetBicicletasThenHttp200()
        {
            var httpClient = Api.GetHttpClient();
            var result = httpClient.GetAsync("/bicicleta".ToAbsoluteUri()).Result;
            var value = result.Content.ReadAsStringAsync().Result;
            var r = JsonConvert.DeserializeObject<AvionesTerminalStatusModel>(value);

            Assert.IsNotNull(r.Arribos.First());
        }

        [Test]
        public void WhenGetAviones()
        {
            var r = AvionController.GetModel(TerminalesAereas.Aeroparque);
            Assert.IsNotNull(r.Arribos.First());

            r = AvionController.GetModel(TerminalesAereas.Ezeiza);
            Assert.IsNotNull(r.Arribos.First());
        }

        [Test]
        public void WhenGetAvionesThenHttp200()
        {
            var httpClient = Api.GetHttpClient();
            var result = httpClient.GetAsync("/avion/arribos".ToAbsoluteUri(new {t = "EZE"})).Result;
            var value = result.Content.ReadAsStringAsync().Result;
            var r = JsonConvert.DeserializeObject<AvionesTerminalStatusModel>(value);

            Assert.IsNotNull(r.Arribos.First());
        }

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
        public void WhenGetCotizacionTasas()
        {
            var r = CotizacionController.GetTasasModel();

            foreach (var divisaViewModel in r.Divisas)
            {
                Console.WriteLine(divisaViewModel.Nombre + "|" + divisaViewModel.ValorVenta + "|" +
                                  divisaViewModel.ValorCompra);
            }

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
        public void WhenGetCotizacionRofex()
        {
            var r = CotizacionController.GetRofexModel();

            foreach (var divisaViewModel in r.Divisas)
            {
                Console.WriteLine(divisaViewModel.Nombre + "|" + divisaViewModel.ValorVenta + "|" +
                                  divisaViewModel.ValorCompra);
            }

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
        public void WhenGetCotizacionDivisas()
        {
            var r = CotizacionController.GetModel();

            foreach (var divisaViewModel in r.Divisas)
            {
                Console.WriteLine(divisaViewModel.Nombre + "|" + divisaViewModel.ValorVenta + "|" +
                                  divisaViewModel.ValorCompra);
            }

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