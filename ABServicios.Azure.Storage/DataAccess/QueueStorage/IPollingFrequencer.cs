namespace ABServicios.Azure.Storage.DataAccess.QueueStorage
{
	public interface IPollingFrequencer
	{
		/// <summary>
		/// The current milliseconds.
		/// </summary>
		/// <remarks>> it increase at each call.</remarks>
		int Current { get; }

		void Decrease();
	}
}