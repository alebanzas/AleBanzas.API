using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public interface IQueueConsumerBuilder<TMessage> where TMessage : class
	{
		IQueueConsumer Using(IQueueMessageBlocksConsumer<TMessage> consumer);
		IQueueConsumer Using(IQueueMessageConsumer<TMessage> consumer);
	}

	public class QueueConsumerFor<TMessage> where TMessage : class
	{
		protected class QueueConsumerBuilder<TMessage> : IQueueConsumerBuilder<TMessage> where TMessage : class
		{
			public IQueueConsumer Using(IQueueMessageBlocksConsumer<TMessage> consumer)
			{
				if (consumer == null)
				{
					throw new ArgumentNullException("consumer");
				}
				return new QueueMessageBlockConsumer<TMessage>(consumer);
			}

			public IQueueConsumer Using(IQueueMessageConsumer<TMessage> consumer)
			{
				if (consumer == null)
				{
					throw new ArgumentNullException("consumer");
				}
				return new QueueMessageConsumer<TMessage>(consumer);
			}
		}
		protected class CurrentThreadQueueConsumerBuilder<TMessage>:IQueueConsumerBuilder<TMessage> where TMessage : class
		{
			public IQueueConsumer Using(IQueueMessageBlocksConsumer<TMessage> consumer)
			{
				if (consumer == null)
				{
					throw new ArgumentNullException("consumer");
				}
				return new CurrentThreadQueueMessageBlockConsumer<TMessage>(consumer);
			}

			public IQueueConsumer Using(IQueueMessageConsumer<TMessage> consumer)
			{
				if (consumer == null)
				{
					throw new ArgumentNullException("consumer");
				}
				return new CurrentThreadQueueMessageConsumer<TMessage>(consumer);
			}
		}

		public static IQueueConsumer Using(IQueueMessageBlocksConsumer<TMessage> consumer)
		{
			return new QueueConsumerBuilder<TMessage>().Using(consumer);
		}

		public static IQueueConsumer Using(IQueueMessageConsumer<TMessage> consumer)
		{
			return new QueueConsumerBuilder<TMessage>().Using(consumer);
		}

		public static IQueueConsumerBuilder<TMessage> WithinCurrentThread
		{
			get { return new CurrentThreadQueueConsumerBuilder<TMessage>(); }
		}

		public static IQueueConsumerBuilder<TMessage> WithStandaloneThread
		{
			get { return new QueueConsumerBuilder<TMessage>(); }
		}
	}
}