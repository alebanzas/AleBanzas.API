using System;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ABServicios.Azure.Storage.DataAccess.BlobStorage
{
	public class BaseDocumentDao<TDocument> where TDocument:class
	{
		// TODO : Hacer esta clase abstract con el metodo InitializeStorageContainer
		private readonly IDocumentSerializer<TDocument> _documentSerializer;
		private readonly Func<CloudStorageAccount> _accountgetter;

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
			_accountgetter = accountgetter;
			_documentSerializer = documentSerializer;
		}

		protected CloudStorageAccount Account { get { return _accountgetter(); } }

		protected virtual string DocumentsContainerName { get; private set; }

		public TDocument Get(string documentName)
		{
			CloudBlobClient blobStorageType = Account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(DocumentsContainerName);
			var blobAddressUri = documentName;

			var serializationStream = new MemoryStream();
			//try
			//{
			//	container.GetBlobReference(blobAddressUri).DownloadToStream(serializationStream);
			//}
			//catch (StorageClientException e)
			//{
			//	if (e.StatusCode == HttpStatusCode.NotFound)
			//	{
			//		return null;
			//	}
			//	throw;
			//}
            //TODO: revisar que tipo de exception tira
		    try
		    {
                container.GetBlockBlobReference(blobAddressUri).DownloadToStream(serializationStream);
		    }
		    catch (Exception)
		    {
		        return null;
		    }
			return _documentSerializer.Deserialize(serializationStream);
		}

		public void Persist(string documentName, TDocument document)
		{
			var blobAddressUri = documentName;
			CloudBlobClient blobStorageType = Account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(DocumentsContainerName);
            CloudBlockBlob blobReference = container.GetBlockBlobReference(blobAddressUri);
            AdjustBlobAttribute(blobReference);
		    blobReference.UploadFromStream(_documentSerializer.Serialize(document));
		}

	    protected virtual void AdjustBlobAttribute(CloudBlockBlob blobReference)
	    {
	    }

	    public void Remove(string documentName)
		{
			CloudBlobClient blobStorageType = Account.CreateCloudBlobClient();
			CloudBlobContainer container = blobStorageType.GetContainerReference(DocumentsContainerName);

			container.GetBlockBlobReference(documentName).DeleteIfExists();
		}
	}
}