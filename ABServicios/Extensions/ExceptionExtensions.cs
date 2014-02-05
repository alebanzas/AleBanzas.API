using System;
using System.Web;
using AB.Common.Mail;
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
        public string LogMessage { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerExceptionMessage { get; set; }
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
            var url = "Not available";
            var urlreferer = "Not available";

            if (context != null)
            {
                url = context.Request.Url.ToString();
            }

            if (context != null && context.Request.UrlReferrer != null)
            {
                urlreferer = context.Request.UrlReferrer.ToString();
            }

            var message = new AppException
            {
                CustomMessage = string.IsNullOrWhiteSpace(customMessage) ? "ABServicios.Error" : customMessage,
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
                    var mailSender = new GeneralMailSender("noreply@abhosting.com.ar", "Y@361]96KB{CPiw");
                    var mailMessage = new System.Net.Mail.MailMessage("noreply@abhosting.com.ar", "alebanzas@outlook.com", message.CustomMessage, message.Exception + "\n\n" + message.LogMessage + "\n\n" + message.Message + "\n\n" + message.StackTrace + "\n\n" + message.Url + "\n\n" + message.UrlReferrer + "\n\n" + message.InnerExceptionMessage);

                    // si el mensaje es null significa que el maker controló algunas situaciones y no hay nada para enviar y el mensaje se puede remover de la queue
                    mailSender.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                //estamos en la B, error del error
            }
        }
    }
}