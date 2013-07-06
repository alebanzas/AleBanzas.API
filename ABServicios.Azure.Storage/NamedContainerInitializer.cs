using System;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ABServicios.Azure.Storage
{
	public class NamedContainerInitializer : IStorageInitializer
	{
		private readonly CloudStorageAccount account;
		private readonly string documentsContainerName;

		public NamedContainerInitializer(CloudStorageAccount account, string documentsContainerName)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			if (string.IsNullOrWhiteSpace(documentsContainerName))
			{
				throw new ArgumentNullException("documentsContainerName");
			}
			this.account = account;
			this.documentsContainerName = documentsContainerName.ToLowerInvariant();
		}

		#region IStorageInitializer Members

		public void Initialize()
		{
			CloudBlobClient blobStorageType = account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(documentsContainerName);
			container.CreateIfNotExist();
			var perm = new BlobContainerPermissions
			           {
			           	PublicAccess = BlobContainerPublicAccessType.Off
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