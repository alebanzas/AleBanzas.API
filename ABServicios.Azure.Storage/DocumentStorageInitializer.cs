using System;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ABServicios.Azure.Storage
{
	public class DocumentStorageInitializer : IStorageInitializer
	{
		private readonly CloudStorageAccount account;
		private readonly string documentsContainerName;

		public DocumentStorageInitializer(CloudStorageAccount account, string documentsContainerName)
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

		public virtual string DocumentsContainerName
		{
			get { return documentsContainerName; }
		}

        public void Initialize()
        {
            Initialize(BlobContainerPublicAccessType.Off);
        }

	    public void Initialize(BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off)
		{
			CloudBlobClient blobStorageType = account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(documentsContainerName);
			container.CreateIfNotExists();
			var perm = new BlobContainerPermissions
			{
                PublicAccess = accessType
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
	}

	public class DocumentStorageInitializer<TDocument> : DocumentStorageInitializer where TDocument : class
	{
		public DocumentStorageInitializer(CloudStorageAccount account)
			: base(account, typeof(TDocument).Name.ToLowerInvariant())
		{
		}
	}
}