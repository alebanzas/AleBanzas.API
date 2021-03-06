﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Geocoding;
using Geocoding.Google;

namespace ABServicios.Api.Controllers
{
    public class GeocoderController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // GET api/Geocoder/?id=ecuador 1419
        [ApiAuthorize]
        public List<GeocoderResult> Get(string id)
        {
            Bounds boundsBias = new Bounds(new Location(-34.937171, -59.248109), new Location(-34.150454, -57.872254));

            return GetAddress(id, boundsBias: boundsBias);
        }

        // GET api/Geocoder/?address=ecuador 1419
        [ApiAuthorize]
        public List<GeocoderResult> Get(string address, int count = 10,string zone = "ar")
        {
            return GetAddress(address, zone).Take(count).ToList();
        }

        private List<GeocoderResult> GetAddress(string address, string zone = "ar", Bounds boundsBias = null)
        {
            try
            {
                IGeocoder geocoder = new GoogleGeocoder
                {
                    //ApiKey = "AIzaSyA4guD0ambG70ooNV5D_Cg8zR42GK1rP_I",
                    Language = "es",
                    RegionBias = zone,
                    BoundsBias = boundsBias,
                };

                IEnumerable<Address> result = geocoder.Geocode(address).Where(x => IsInBound(boundsBias, new Location(x.Coordinates.Latitude, x.Coordinates.Longitude)));

                return result.Select(x => new GeocoderResult
                {
                    Nombre = x.FormattedAddress,
                    X = x.Coordinates.Latitude,
                    Y = x.Coordinates.Longitude,
                }).ToList();
            }
            catch (Exception)
            {
                return new List<GeocoderResult>();
            }
        }

        private bool IsInBound(Bounds boundsBias, Location location)
        {
            if (boundsBias == null) return true;

            return (boundsBias.SouthWest.Latitude < location.Latitude) && (location.Latitude < boundsBias.NorthEast.Latitude) &&
                   (boundsBias.SouthWest.Longitude < location.Longitude) && (location.Longitude < boundsBias.NorthEast.Longitude);

        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            throw Request.CreateExceptionResponse(HttpStatusCode.MethodNotAllowed, string.Empty);
        }
    }

    public class GeocoderResult
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Nombre { get; set; }
    }
}