using System;
using System.Web;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;

namespace ABServicios.Extensions
{
    public enum ExceptionAction
    {
        Enqueue,
        SendMail,
        SendMailAndEnqueue,
    }

    public class AppException
    {
        public Exception Exception { get; set; }
        public string CustomMessage { get; set; }

        public string Url { get; set; }
        public string UrlReferrer { get; set; }
    }

    public static class ExceptionExtensions
    {
        public static void Log(this Exception exception, ExceptionAction action = ExceptionAction.Enqueue)
        {
            Log(exception, null, string.Empty, action);
        }

        public static void Log(this Exception exception, HttpContext context, ExceptionAction action = ExceptionAction.Enqueue)
        {
            Log(exception, context, string.Empty, action);
        }

        public static void Log(this Exception exception, HttpContext context, string customMessage, ExceptionAction action = ExceptionAction.Enqueue)
        {
            var url = context.Request.Url.ToString();
            var urlreferer = "Not available";

            if (context.Request.UrlReferrer != null)
            {
                urlreferer = context.Request.UrlReferrer.ToString();
            }

            var message = new AppException
            {
                CustomMessage = customMessage,
                Exception = exception,
                Url = url,
                UrlReferrer = urlreferer,
            };

            try
            {
                if (ExceptionAction.Enqueue.Equals(action) || ExceptionAction.SendMailAndEnqueue.Equals(action))
                {
                    AzureQueue.Enqueue(message);
                }
            }
            catch (Exception ex)
            {
                //estamos en la B, error del error
            }

            try
            {
                if (ExceptionAction.SendMail.Equals(action) || ExceptionAction.SendMailAndEnqueue.Equals(action))
                {
                    //TODO: envio de mail con exception
                }
            }
            catch (Exception ex)
            {
                //estamos en la B, error del error
            }
        }
    }
}