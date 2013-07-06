using System;
using System.IO;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ABServicios.Azure.Storage.DataAccess.BlobStorage
{
	public class BaseDocumentDao<TDocument> where TDocument:class
	{
		// TODO : Hacer esta clase abstract con el metodo InitializeStorageContainer
		private readonly IDocumentSerializer<TDocument> documentSerializer;
		private Func<CloudStorageAccount> accountgetter;

		public BaseDocumentDao(CloudStorageAccount account)
			: this(account, new JsonDocumentSerializer<TDocument>())
		{
		}

		public BaseDocumentDao(CloudStorageAccount account, IDocumentSerializer<TDocument> documentSerializer) : this(()=>account, documentSerializer)
		{
		}

		public BaseDocumentDao(Func<CloudStorageAccount> accountgetter)
			: this(accountgetter, new JsonDocumentSerializer<TDocument>())
		{
		}

		public BaseDocumentDao(Func<CloudStorageAccount> accountgetter, IDocumentSerializer<TDocument> documentSerializer)
		{
			DocumentsContainerName = typeof(TDocument).Name.ToLowerInvariant();
			if (accountgetter == null)
			{
				throw new ArgumentNullException("accountgetter");
			}
			if (documentSerializer == null)
			{
				throw new ArgumentNullException("documentSerializer");
			}
			this.accountgetter = accountgetter;
			this.documentSerializer = documentSerializer;
		}

		protected CloudStorageAccount Account { get { return accountgetter(); } }

		protected virtual string DocumentsContainerName { get; private set; }

		public TDocument Get(string documentName)
		{
			CloudBlobClient blobStorageType = Account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(DocumentsContainerName);
			var blobAddressUri = documentName;

			var serializationStream = new MemoryStream();
			try
			{
				container.GetBlobReference(blobAddressUri).DownloadToStream(serializationStream);
			}
			catch (StorageClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
				{
					return null;
				}
				throw;
			}
			return documentSerializer.Deserialize(serializationStream);
		}

		public void Persist(string documentName, TDocument document)
		{
			var blobAddressUri = documentName;
			CloudBlobClient blobStorageType = Account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(DocumentsContainerName);
            CloudBlob blobReference = container.GetBlobReference(blobAddressUri);
            AdjustBlobAttribute(blobReference);
		    blobReference.UploadFromStream(documentSerializer.Serialize(document));
		}

	    protected virtual void AdjustBlobAttribute(CloudBlob blobReference)
	    {
	    }

	    public void Remove(string documentName)
		{
			CloudBlobClient blobStorageType = Account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(DocumentsContainerName);

			container.GetBlobReference(documentName).DeleteIfExists();
		}
	}
}