using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ABServicios.Azure.Storage
{
	public class NamedContainerInitializer : IStorageInitializer
	{
		private readonly CloudStorageAccount _account;
		private readonly string _documentsContainerName;

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
			_account = account;
			_documentsContainerName = documentsContainerName.ToLowerInvariant();
		}

		#region IStorageInitializer Members

		public void Initialize()
		{
			CloudBlobClient blobStorageType = _account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(_documentsContainerName);
			container.CreateIfNotExists();
			var perm = new BlobContainerPermissions
			           {
			           	PublicAccess = BlobContainerPublicAccessType.Off
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