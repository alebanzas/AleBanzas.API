using System;
using System.Collections.Generic;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public interface IQueueMessageBlocksConsumer<TMessage>
	{
		TimeSpan? EstimatedTimeToProcessMessageBlock { get; }
		int BlockSize { get; }
		void ProcessMessagesGroup(IQueueMessageRemover<TMessage> messagesRemover, IEnumerable<QueueMessage<TMessage>> messages);
	}
}