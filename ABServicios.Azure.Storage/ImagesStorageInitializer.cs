using System;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ABServicios.Azure.Storage
{
	public class ImagesStorageInitializer : IStorageInitializer
	{
		private readonly CloudStorageAccount account;
		private readonly string documentsContainerName;

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
			this.account = account;
			documentsContainerName = containerName.ToLowerInvariant();
		}

		#region IStorageInitializer Members

		public void Initialize()
		{
			CloudBlobClient blobStorageType = account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(documentsContainerName);
			container.CreateIfNotExist();
			var perm = new BlobContainerPermissions
			           	{
			           		PublicAccess = BlobContainerPublicAccessType.Container
			           	};
			container.SetPermissions(perm);
		}

		public void Drop()
		{
			CloudBlobClient blobStorageType = account.CreateCloudBlobClient();
			if (blobStorageType.ListContainers().Select(c => c.Name).Contains(documentsContainerName))
			{
				CloudBlobContainer container = blobStorageType.GetContainerReference(documentsContainerName);
				container.Delete();
			}
		}

		#endregion
	}
}