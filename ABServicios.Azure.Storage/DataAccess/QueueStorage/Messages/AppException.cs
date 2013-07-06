namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages
{
	public class AppException
	{
		public string Url { get; set; }
		public string UrlReferrer { get; set; }
		public string LogMessage { get; set; }
		public string Message { get; set; }

		public string StackTrace { get; set; }

		public string InnerExceptionMessage { get; set; }
	}
}