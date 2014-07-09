namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public interface IQueueConsumer
	{
		IQueueConsumer With(IPollingFrequencer frequencer);
		void StartConsimung();
		void StopConsimung();
	}
}