using System;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ABServicios.Api.Binders
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
	public class FromHeaderAttribute : ParameterBindingAttribute
	{
		public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException("parameter");
			}

			return new HeadersParameterBinding(parameter);
		}
	}
}