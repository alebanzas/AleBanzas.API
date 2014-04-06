using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public abstract class MessageBlockPollQueueConsumer<TMessage> : AbstractQueueConsumer<TMessage> where TMessage : class
	{
		protected MessageBlockPollQueueConsumer(IQueueMessageBlocksConsumer<TMessage> consumer)
		{
			if (consumer == null)
			{
				throw new ArgumentNullException("consumer");
			}
			Consumer = consumer;
		}

		protected IQueueMessageBlocksConsumer<TMessage> Consumer { get; private set; }

		protected virtual string ConsumerName
		{
			get { return Consumer.GetType().Name; }
		}

		protected abstract CloudStorageAccount QueueAccount { get; }

		public override void PollQueue()
		{
			CloudStorageAccount account = QueueAccount;
			var queue = new MessageQueue<TMessage>(account);
			var queueRemover = GetQueueRemover(queue);
			IPollingFrequencer frequencer = Frequencer;
			bool useDefaultTimeout = !Consumer.EstimatedTimeToProcessMessageBlock.HasValue;
			TimeSpan defaultTimeout = useDefaultTimeout ? TimeSpan.FromSeconds(30) : Consumer.EstimatedTimeToProcessMessageBlock.Value;
			while (true)
			{
				try
				{
					var messages = useDefaultTimeout ? queue.Dequeue(Consumer.BlockSize).ToList() : queue.Dequeue(Consumer.BlockSize, defaultTimeout).ToList();
					if (messages.Count > 0)
					{
						try
						{
							Consumer.ProcessMessagesGroup(queueRemover, messages);
						}
						catch (Exception e)
						{
							OnProcessMessageLogException(messages, e);
						}
						finally
						{
							frequencer.Decrease();
						}
					}
					else
					{
						Thread.Sleep(frequencer.Current);
					}
				}
				catch (Exception e)
				{
					OnDequeueLogException(e);
					Thread.Sleep(5*1000);
				}
			}
		}

		protected virtual IQueueMessageRemover<TMessage> GetQueueRemover(MessageQueue<TMessage> queue)
		{
			return new QueueMessageRemover<TMessage>(queue);
		}

		protected virtual void OnDequeueLogException(Exception e)
		{
			string message = string.Format("Queue connection of {0}: {1}\nStackTrace:{2}", ConsumerName, e.Message, e.StackTrace);
			Trace.WriteLine(message, "Error");
		}

		protected virtual void OnProcessMessageLogException(IEnumerable<QueueMessage<TMessage>> messages, Exception e)
		{
			string errorMessage = string.Format("{0}: {1}\nStackTrace:{2}", ConsumerName, e.Message, e.StackTrace);
			Trace.WriteLine(errorMessage, "Error");
		}

		protected class QueueMessageRemover<TMessage> : IQueueMessageRemover<TMessage> where TMessage : class
		{
			private readonly MessageQueue<TMessage> queue;

			public QueueMessageRemover(MessageQueue<TMessage> queue)
			{
				this.queue = queue;
			}

			public void RemoveProcessedMessages(IEnumerable<QueueMessage<TMessage>> sucefullyProcessedMessages)
			{
				queue.Remove(sucefullyProcessedMessages);
			}

			public void RemoveProcessedMessage(QueueMessage<TMessage> sucefullyProcessedMessage)
			{
				queue.Remove(sucefullyProcessedMessage);
			}
		}
	}
}