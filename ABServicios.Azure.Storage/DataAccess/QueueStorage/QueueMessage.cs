using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public class QueueMessage<TMessage>
	{
		public string Id { get; internal set; }
		public string PopReceipt { get; internal set; }
		public DateTime? InsertionTime { get; internal set; }
		public DateTime? ExpirationTime { get; internal set; }
		public DateTime? NextVisibleTime { get; internal set; }
		public int DequeueCount { get; internal set; }
		public TMessage Data { get; set; }
	}
}