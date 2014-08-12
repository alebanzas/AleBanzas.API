using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ABServicios.Azure.Storage
{
	public class ImagesStorageInitializer : IStorageInitializer
	{
		private readonly CloudStorageAccount _account;
		private readonly string _documentsContainerName;

		public ImagesStorageInitializer(CloudStorageAccount account, string containerName)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			if (string.IsNullOrEmpty(containerName))
			{
				throw new ArgumentException("The name of the container is requested.", "containerName");
			}
			_account = account;
			_documentsContainerName = containerName.ToLowerInvariant();
		}

		#region IStorageInitializer Members

		public void Initialize()
		{
			CloudBlobClient blobStorageType = _account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(_documentsContainerName);
			container.CreateIfNotExists();
			var perm = new BlobContainerPermissions
			           	{
			           		PublicAccess = BlobContainerPublicAccessType.Container
			           	};
			container.SetPermissions(perm);
		}

		public void Drop()
		{
			CloudBlobClient blobStorageType = _account.CreateCloudBlobClient();
			if (blobStorageType.ListContainers().Select(c => c.Name).Contains(_documentsContainerName))
			{
				CloudBlobContainer container = blobStorageType.GetContainerReference(_documentsContainerName);
				container.Delete();
			}
		}

		#endregion
	}
}