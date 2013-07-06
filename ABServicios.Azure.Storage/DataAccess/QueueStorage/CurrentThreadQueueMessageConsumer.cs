using System.Diagnostics;
using Microsoft.WindowsAzure;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public class CurrentThreadQueueMessageConsumer<TMessage> : MessagePollQueueConsumer<TMessage> where TMessage : class
	{
		public CurrentThreadQueueMessageConsumer(IQueueMessageConsumer<TMessage> consumer) : base(consumer) {}

		protected override System.Threading.Thread CreateThreadForPolling()
		{
			var consumerName = ConsumerName;
			Trace.WriteLine("Within CurrentThread Start " + consumerName, "Information");

			return null;
		}

		protected override CloudStorageAccount QueueAccount
		{
			get { return AzureAccount.DefaultAccount(); }
		}
	}
}