using System;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using Microsoft.WindowsAzure.Storage;

namespace ABServicios.Azure.Storage
{
	public static class ExceptionsExtensions
	{
		public static void LogSilentNoHttp(this Exception exception, string logMessage = null)
		{
		    try
		    {
                if (exception == null)
                {
                    return;
                }
                CloudStorageAccount account = AzureAccount.DefaultAccount();
                new QueueStorageInitializer<AppException>(account).Initialize();

                var queueTags = new MessageQueue<AppException>();
                queueTags.Enqueue(new AppException
                {
                    LogMessage = logMessage,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerExceptionMessage = (exception.InnerException != null) ? exception.InnerException.Message : string.Empty
                });
		    }
		    catch (Exception)
		    {
		        //estamos en la B, error del error
		    }
		}
	}
}