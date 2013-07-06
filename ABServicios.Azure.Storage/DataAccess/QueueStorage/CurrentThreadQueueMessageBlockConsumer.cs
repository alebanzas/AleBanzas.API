using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public class CurrentThreadQueueMessageBlockConsumer<TMessage> : MessageBlockPollQueueConsumer<TMessage> where TMessage : class
	{
		public CurrentThreadQueueMessageBlockConsumer(IQueueMessageBlocksConsumer<TMessage> consumer) : base(consumer) { }

		protected override Thread CreateThreadForPolling()
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