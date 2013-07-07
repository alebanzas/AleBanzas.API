using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.Storage;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Consumers
{
	public abstract class MessageBlocksConsumer<TMessage> where TMessage: class
	{
		private readonly int messagesBlockSize;
		private readonly TimeSpan estimatedTimeToProcessMessageBlock;
		protected MessageBlocksConsumer(int messagesBlockSize, TimeSpan estimatedTimeToProcessMessageBlock)
		{
			this.messagesBlockSize = messagesBlockSize;
			this.estimatedTimeToProcessMessageBlock = estimatedTimeToProcessMessageBlock;
		}

		public void PollQueue()
		{
			// TODO: Si hay error encolar mensajes a otra queue (vale la pena?)
			var consumerName = GetType().Name;
			CloudStorageAccount account = AzureAccount.DefaultAccount();
			var queue = new MessageQueue<TMessage>(account);
			var frequencer = GetPoollingFrequencer();
			while (true)
			{
				try
				{
					var messages = queue.Dequeue(messagesBlockSize, estimatedTimeToProcessMessageBlock).ToList();
					if (messages.Count > 0)
					{
						try
						{
							ProcessMessagesGroup(queue, messages);
						}
						catch (Exception e)
						{
							string message = string.Format("{0}: {1}\nStackTrace:{2}", consumerName, e.Message, e.StackTrace);
							Trace.WriteLine(message, "Error");
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
					string message = string.Format("Queue connection of {0}: {1}\nStackTrace:{2}", consumerName, e.Message, e.StackTrace);
					Trace.WriteLine(message, "Error");
					Thread.Sleep(5 * 1000);
				}
			}
		}

		protected virtual PollingFrequencer GetPoollingFrequencer()
		{
			return new PollingFrequencer(estimatedTimeToProcessMessageBlock);
		}

		protected abstract void ProcessMessagesGroup(MessageQueue<TMessage> queue, IEnumerable<QueueMessage<TMessage>> messages);
	}
}