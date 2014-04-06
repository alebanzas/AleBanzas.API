using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace ABServicios.Api
{
	public static class HttpRequestExtensions
	{
		public static HttpResponseException InvalidModelState(this HttpRequestMessage source, ModelStateDictionary modelState)
		{
			return new HttpResponseException(source.CreateResponse(HttpStatusCode.BadRequest, new HttpError(modelState, false)));
		}

		public static HttpResponseException CreateExceptionResponse(this HttpRequestMessage source, HttpStatusCode statusCode, string message = null)
		{
			var httpError = message == null ? new HttpError() : new HttpError(message);
			return new HttpResponseException(source.CreateResponse(statusCode, httpError));
		}
	}
}