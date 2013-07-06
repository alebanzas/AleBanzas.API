using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public interface IQueueMessageConsumer<TMessage>
	{
		TimeSpan? EstimatedTimeToProcessMessageBlock { get; }
		void ProcessMessages(QueueMessage<TMessage> message);
	}
}