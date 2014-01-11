using System;
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

        // GET api/<controller>/ecuador 1419
        public List<GeocoderResult> Get(string id)
        {
            try
            {
                IGeocoder geocoder = new GoogleGeocoder
                {
                    //ApiKey = "AIzaSyA4guD0ambG70ooNV5D_Cg8zR42GK1rP_I",
                    Language = "es",
                    RegionBias = "ar",
                    BoundsBias = new Bounds(new Location(-34.937171, -57.872254), new Location(-34.150454, -59.248109)),
                };

                return geocoder.Geocode(id).Select(x => new GeocoderResult
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