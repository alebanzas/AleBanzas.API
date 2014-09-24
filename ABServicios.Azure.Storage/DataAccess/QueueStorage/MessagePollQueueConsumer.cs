using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public abstract class MessagePollQueueConsumer<TMessage> : AbstractQueueConsumer<TMessage> where TMessage : class
	{
		protected MessagePollQueueConsumer(IQueueMessageConsumer<TMessage> consumer)
		{
			if (consumer == null)
			{
				throw new ArgumentNullException("consumer");
			}
			Consumer = consumer;
		}

		protected IQueueMessageConsumer<TMessage> Consumer { get; private set; }

		protected virtual string ConsumerName
		{
			get { return Consumer.GetType().Name; }
		}

		public override void PollQueue()
		{
			CloudStorageAccount account = QueueAccount;
			var queue = new MessageQueue<TMessage>(account);
			var frequencer = Frequencer;
			bool useDefaultTimeout = !Consumer.EstimatedTimeToProcessMessageBlock.HasValue;
			TimeSpan defaultTimeout = useDefaultTimeout ?  TimeSpan.FromSeconds(1): Consumer.EstimatedTimeToProcessMessageBlock.Value;
			while (true)
			{
				try
				{
					var message = useDefaultTimeout ? queue.Dequeue() : queue.Dequeue(defaultTimeout);
					if (message!= null)
					{
						try
						{
							Consumer.ProcessMessages(message);
							queue.Remove(message);
						}
						catch (Exception e)
						{
							OnProcessMessageLogException(message, e);
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
					Thread.Sleep(5 * 1000);
				}
			}
		}

		protected abstract CloudStorageAccount QueueAccount { get; }
		
		protected virtual void OnDequeueLogException(Exception e)
		{
			string message = string.Format("Queue connection of {0}: {1}\nStackTrace:{2}", ConsumerName, e.Message, e.StackTrace);
			Trace.WriteLine(message, "Error");
		}

		protected virtual void OnProcessMessageLogException(QueueMessage<TMessage> message, Exception e)
		{
			string errorMessage = string.Format("{0}: {1}\nStackTrace:{2}", ConsumerName, e.Message, e.StackTrace);
			Trace.WriteLine(errorMessage, "Error");
		}
	}
}