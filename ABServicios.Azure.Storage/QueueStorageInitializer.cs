using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ABServicios.Azure.Storage
{
	/// <summary>
	/// Initialize a queue storage specific for a message type.
	/// </summary>
	/// <typeparam name="TMessage">The typeof the message</typeparam>
	public class QueueStorageInitializer<TMessage> : IStorageInitializer where TMessage : class
	{
		private readonly CloudStorageAccount account;
		private readonly string queueName = typeof(TMessage).Name.ToLowerInvariant();

		public QueueStorageInitializer(CloudStorageAccount account)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			this.account = account;
		}

		public void Initialize()
		{
			CloudQueueClient queueClient = account.CreateCloudQueueClient();
			CloudQueue queue = queueClient.GetQueueReference(queueName);
			queue.CreateIfNotExists();
		}

		public void Drop()
		{
			CloudQueueClient queueClient = account.CreateCloudQueueClient();
			if (queueClient.ListQueues().Select(c => c.Name).Contains(queueName))
			{
				CloudQueue queue = queueClient.GetQueueReference(queueName);
				queue.Delete();
			}
		}
	}
}