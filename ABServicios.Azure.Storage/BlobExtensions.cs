using Microsoft.WindowsAzure.StorageClient;

namespace ABServicios.Azure.Storage
{
	public static class BlobExtensions
	{
		public static bool Exists(this CloudBlob blob)
		{
			try
			{
				blob.FetchAttributes();
				return true;
			}
			catch (StorageClientException e)
			{
				if (e.ErrorCode == StorageErrorCode.ResourceNotFound || e.ErrorCode == StorageErrorCode.BlobNotFound)
				{
					return false;
				}
				else
				{
					throw;
				}
			}
		}
	}
}