namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages
{
	public class MailMessage
	{
		public string MakerFactoryKey { get; set; }
		public string SenderFactoryKey { get; set; }
		public string SubjectType { get; set; }
		public string SubjectId { get; set; }
		public dynamic MakerData { get; set; }
	}
}