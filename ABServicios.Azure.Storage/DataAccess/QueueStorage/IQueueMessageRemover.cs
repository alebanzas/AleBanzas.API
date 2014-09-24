using System.Collections.Generic;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public interface IQueueMessageRemover<TMessage>
	{
		void RemoveProcessedMessages(IEnumerable<QueueMessage<TMessage>> sucefullyProcessedMessages);
		void RemoveProcessedMessage(QueueMessage<TMessage> sucefullyProcessedMessage);
	}
}