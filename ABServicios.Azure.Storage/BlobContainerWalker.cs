using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ABServicios.Azure.Storage
{
	public class BlobContainerWalker
	{
		private readonly CloudStorageAccount _account;
		private readonly string _containerName;

		public BlobContainerWalker(CloudStorageAccount account, string containerName)
		{
			if (string.IsNullOrWhiteSpace(containerName))
			{
				throw new ArgumentNullException("containerName");
			}
			this._account = account;
			this._containerName = containerName;
		}

		public IEnumerable<Uri> GetAllUri()
		{
			CloudBlobContainer container = _account.CreateCloudBlobClient().GetContainerReference(_containerName);
            BlobRequestOptions options = new BlobRequestOptions();
            OperationContext operationContext = new OperationContext();
            
            BlobResultSegment resultSegment = container.ListBlobsSegmented(string.Empty, true, BlobListingDetails.None, 2000, null, options, operationContext);
			foreach (var blobItem in resultSegment.Results)
			{
				yield return blobItem.Uri;
			}
            BlobContinuationToken continuationToken = resultSegment.ContinuationToken;

			while (continuationToken != null)
			{
                resultSegment = container.ListBlobsSegmented(string.Empty, true, BlobListingDetails.None, 2000, continuationToken, options, operationContext);
				foreach (var blobItem in resultSegment.Results)
				{
					yield return blobItem.Uri;
				}
				continuationToken = resultSegment.ContinuationToken;
			}
		}
	}
}