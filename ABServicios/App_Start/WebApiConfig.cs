using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using ABServicios.Api;
using ABServicios.Api.Binders;
using ABServicios.BLL.DataInterfaces;
using ABServicios.BLL.Entities;
using Microsoft.Practices.ServiceLocation;

namespace ABServicios
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional, lat = RouteParameter.Optional, lon = RouteParameter.Optional },
                    constraints: null,
                    handler: new AccessLogHandler
                    {
                        InnerHandler = new HmacAuthenticationHandler(ServiceLocator.Current.GetInstance<IRepository<Application>>())
                        {
                            InnerHandler = new HttpControllerDispatcher(config)
                        }
                    }
            );
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.MediaTypeMappings.Clear();
            foreach (var minetype in FormattingConfiguration.ContentTypesJsonEnabled())
            {
                json.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", minetype.Item1, StringComparison.InvariantCultureIgnoreCase, false, minetype.Item2));
            }
            var xml = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            xml.MediaTypeMappings.Clear();
            foreach (var minetype in FormattingConfiguration.ContentTypesXmlEnabled())
            {
                xml.MediaTypeMappings.Add(new RequestHeaderMapping("Accept", minetype.Item1, StringComparison.InvariantCultureIgnoreCase, false, minetype.Item2));
            }

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //TODO: API
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            //config.EnableSystemDiagnosticsTracing();

            var parameterBindingRules = config.ParameterBindingRules;
            parameterBindingRules.Add(x => x.ParameterName.StartsWith("xabs_") ? new HeadersParameterBinding(x) : null);
        }
    }
}
