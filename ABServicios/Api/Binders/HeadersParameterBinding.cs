using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;
using AB.Common.Helpers;

namespace ABServicios.Api.Binders
{
	public class HeadersParameterBinding : HttpParameterBinding
	{
		private readonly string headerParamName;

		public HeadersParameterBinding(HttpParameterDescriptor descriptor) : base(descriptor)
		{
			headerParamName = string.Concat(ABServiciosHmacBuilder.CustomHeaderPrefix, "-",
			                                descriptor.ParameterName.Replace("xabs_","").Titleize().Dasherize());
		}

		public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext,
		                                         CancellationToken cancellationToken)
		{
			HttpRequestHeaders headers = actionContext.Request.Headers;
			IEnumerable<string> hederValue;
			if (!headers.TryGetValues(headerParamName, out hederValue))
			{
				return TaskHelpers.NullResult();
			}
			// por ahora me limito solo a bindear strings unico o lista
			if (Descriptor.ParameterType.IsAssignableFrom(typeof (IEnumerable<string>)))
			{
				actionContext.ActionArguments[Descriptor.ParameterName] = hederValue.ToArray();
				return TaskHelpers.Completed();
			}
			if (Descriptor.ParameterType == typeof (string))
			{
				actionContext.ActionArguments[Descriptor.ParameterName] = hederValue.FirstOrDefault();
				return TaskHelpers.Completed();
			}
			return TaskHelpers.NullResult();
		}
	}
}