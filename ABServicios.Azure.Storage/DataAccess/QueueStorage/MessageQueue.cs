using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
    public static class AzureQueue
    {
        public static void Enqueue<Tm>(Tm message) where Tm : class
        {
            var mq = new MessageQueue<Tm>();
            mq.Enqueue(message);
        }
    }

    /// <summary>
	/// Generic base class for messages.
	/// </summary>
	/// <typeparam name="TMessage">The type of the message.</typeparam>
	/// <remarks>
	/// The <typeparamref name="TMessage"/> have to be JSON serializable.
	/// </remarks>
	public class MessageQueue<TMessage> where TMessage : class
	{
		private const int MaxMessageBlockAllowedByAzure = 32;
		private readonly CloudQueueClient queueClient;
		private readonly string queueName = typeof (TMessage).Name.ToLowerInvariant();

	    public MessageQueue() : this(AzureAccount.DefaultAccount()) {}

		public MessageQueue(CloudStorageAccount account)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			queueClient = account.CreateCloudQueueClient();
		}

		public int ApproximateMessageCount
		{
			get
			{
				CloudQueue queueRef = queueClient.GetQueueReference(queueName);
				queueRef.RetrieveApproximateMessageCount();
				if (queueRef.ApproximateMessageCount.HasValue)
				{
					return queueRef.ApproximateMessageCount.Value;
				}
				return 0;
			}
		}

		public void Enqueue(TMessage messageContent)
		{
			if (messageContent == null)
			{
				throw new ArgumentNullException("messageContent");
			}
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			var message = new CloudQueueMessage(SerializeObjectAsString(messageContent));
			queueRef.AddMessage(message);
		}

		public QueueMessage<TMessage> Dequeue()
		{
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			CloudQueueMessage message = queueRef.GetMessage();
			return ConvertToQueueMessage(message);
		}

		public IEnumerable<QueueMessage<TMessage>> Dequeue(int messagesCount)
		{
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			IEnumerable<CloudQueueMessage> messages = queueRef.GetMessages(messagesCount < MaxMessageBlockAllowedByAzure ? messagesCount : MaxMessageBlockAllowedByAzure);
			return messages.Select(x => ConvertToQueueMessage(x));
		}

		public QueueMessage<TMessage> Dequeue(TimeSpan timeout)
		{
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			CloudQueueMessage message = queueRef.GetMessage(timeout);
			return ConvertToQueueMessage(message);
		}

		public IEnumerable<QueueMessage<TMessage>> Dequeue(int messagesCount, TimeSpan timeout)
		{
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			IEnumerable<CloudQueueMessage> messages = queueRef.GetMessages(messagesCount < MaxMessageBlockAllowedByAzure ? messagesCount:MaxMessageBlockAllowedByAzure, timeout);
			return messages.Select(x => ConvertToQueueMessage(x));
		}

		public QueueMessage<TMessage> Peek()
		{
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			CloudQueueMessage message = queueRef.PeekMessage();
			return ConvertToQueueMessage(message);
		}

		public IEnumerable<QueueMessage<TMessage>> Peek(int messagesCount)
		{
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			IEnumerable<CloudQueueMessage> messages = queueRef.PeekMessages(messagesCount < MaxMessageBlockAllowedByAzure ? messagesCount : MaxMessageBlockAllowedByAzure);
			return messages.Select(x => ConvertToQueueMessage(x));
		}

		public void Remove(QueueMessage<TMessage> queueMessage)
		{
			if (queueMessage == null)
			{
				throw new ArgumentNullException("queueMessage");
			}
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			queueRef.DeleteMessage(queueMessage.Id, queueMessage.PopReceipt);
		}

		public void Remove(IEnumerable<QueueMessage<TMessage>> queueMessages)
		{
			if (queueMessages == null)
			{
				throw new ArgumentNullException("queueMessages");
			}
			CloudQueue queueRef = queueClient.GetQueueReference(queueName);
			foreach (var queueMessage in queueMessages)
			{
				queueRef.DeleteMessage(queueMessage.Id, queueMessage.PopReceipt);
			}
		}

		protected virtual string SerializeObjectAsString(TMessage messageContent)
		{
			// a subclass can gzipr the message (GZipStream) where the serialized TMessage is > 8KB
			return JsonConvert.SerializeObject(messageContent);
		}

		protected virtual TMessage DeserializeObjectFromString(string messageContent)
		{
			// a subclass can de-gzip the message
			return JsonConvert.DeserializeObject<TMessage>(messageContent);
		}

		protected virtual QueueMessage<TMessage> ConvertToQueueMessage(CloudQueueMessage message)
		{
			if (message == null)
			{
				return null;
			}
			string messageContent = message.AsString;
			return new QueueMessage<TMessage>
			       {
			       	Id = message.Id,
			       	PopReceipt = message.PopReceipt,
			       	DequeueCount = message.DequeueCount,
			       	InsertionTime = message.InsertionTime,
			       	ExpirationTime = message.ExpirationTime,
			       	NextVisibleTime = message.NextVisibleTime,
			       	Data = DeserializeObjectFromString(messageContent)
			       };
		}
	}
}