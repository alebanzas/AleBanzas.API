using Microsoft.WindowsAzure.Storage.Blob;

namespace ABServicios.Azure.Storage
{
	public static class BlobExtensions
	{
        public static bool Exists(this CloudBlockBlob blob)
        {
            return blob.Exists();
		}
	}
}