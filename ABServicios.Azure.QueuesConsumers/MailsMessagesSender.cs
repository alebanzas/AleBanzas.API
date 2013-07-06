using System;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;

namespace ABServicios.Azure.QueuesConsumers
{
	public class MailsMessagesSender : IQueueMessageConsumer<MailMessage>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(40);

	    public TimeSpan? EstimatedTimeToProcessMessageBlock { get; private set; }

	    public void ProcessMessages(QueueMessage<MailMessage> message)
		{
			
		}
	}
}