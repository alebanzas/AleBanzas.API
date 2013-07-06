using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public class QueueMessageConsumer<TMessage> : MessagePollQueueConsumer<TMessage> where TMessage : class
	{
		public QueueMessageConsumer(IQueueMessageConsumer<TMessage> consumer) : base(consumer) {}

		protected override Thread CreateThreadForPolling()
		{
			var consumerName = ConsumerName;
			Trace.WriteLine("Starting " + consumerName, "Information");

			return new Thread(PollQueue) { Name = consumerName };
		}

		protected override CloudStorageAccount QueueAccount
		{
			get { return AzureAccount.DefaultAccount(); }
		}
	}
}