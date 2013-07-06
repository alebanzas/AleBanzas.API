using System;
using System.Threading;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public abstract class AbstractQueueConsumer<TMessage> : IQueueConsumer
	{
		private IPollingFrequencer frequencer;
		private Thread pollingThread;

		public IQueueConsumer With(IPollingFrequencer theFrequencer)
		{
			if (theFrequencer == null)
			{
				throw new ArgumentNullException("theFrequencer");
			}
			this.frequencer = theFrequencer;
			return this;
		}

		public virtual IPollingFrequencer Frequencer
		{
			get
			{
				if(frequencer == null)
				{
					// The Azure default for message-timeout visibility is 30"
					const int defaultFloor = 60 * 1000; // one minute
					const int defaultCeiling = 10 * 60 * 1000; // 10 minutes
					frequencer = new PollingFrequencer(defaultFloor, defaultCeiling);
				}
				return frequencer;
			}
		}

		public void StartConsimung()
		{
			pollingThread = CreateThreadForPolling();
			if (pollingThread != null)
			{
				pollingThread.Start();
			}
			else
			{
				PollQueue();
			}
		}

		public void StopConsimung()
		{
			if (pollingThread != null)
			{
				pollingThread.Abort();
			}
		}

		protected abstract Thread CreateThreadForPolling();
		public abstract void PollQueue();
	}
}