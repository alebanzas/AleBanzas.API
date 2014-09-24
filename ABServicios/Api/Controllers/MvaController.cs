using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using ScrapySharp.Extensions;

namespace ABServicios.Api.Controllers
{
    public class MvaController : ApiController
    {
        public IEnumerable<MvaElement> Get(Uri url)
        {
            if (!url.AbsoluteUri.Contains("microsoftvirtualacademy.com/Profile.aspx?alias="))
                throw Request.CreateExceptionResponse(HttpStatusCode.BadRequest, string.Empty);

            var profileStatus = GetProfileStatus(url);
            if (profileStatus == null)
                throw Request.CreateExceptionResponse(HttpStatusCode.NotFound, string.Empty);

            return profileStatus;
        }

        private IEnumerable<MvaElement> GetProfileStatus(Uri url)
        {
            var courses = new List<MvaElement>
            {
                new MvaElement { Url = new Uri("http://www.microsoftvirtualacademy.com/training-courses/what-s-new-in-visual-studio-2013-jump-start"), Finalizado = false },
                new MvaElement { Url = new Uri("http://www.microsoftvirtualacademy.com/training-courses/construyendo-aplicaciones-en-windows-phone-8"), Finalizado = false },
                new MvaElement { Url = new Uri("http://www.microsoftvirtualacademy.com/training-courses/diseno-de-aplicaciones-de-windows-8-en-html-5"), Finalizado = false },
            };

            var html = new Scraper(url, Encoding.UTF7).GetNodes();

            var hasMicrosite = html.CssSelect("#microsite").Any();
            if (!hasMicrosite) return null;

            var elements = html.CssSelect(".approved-study-name"); 

            foreach (var element in elements)
            {
                try
                {
                    var href = element.CssSelect("a").FirstOrDefault().Attributes["href"].Value;
                    foreach (var mvaElement in courses.Where(x => href.Contains(x.Url.AbsoluteUri)))
                    {
                        mvaElement.Finalizado = true;
                    }
                }
                catch
                {}
            }

            return courses;
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

    public class MvaElement
    {
        public Uri Url { get; set; }
        public bool Finalizado { get; set; }
    }
}