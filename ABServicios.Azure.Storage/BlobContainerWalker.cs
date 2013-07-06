using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ABServicios.Azure.Storage
{
	public class BlobContainerWalker
	{
		private readonly CloudStorageAccount account;
		private readonly string containerName;

		public BlobContainerWalker(CloudStorageAccount account, string containerName)
		{
			if (string.IsNullOrWhiteSpace(containerName))
			{
				throw new ArgumentNullException("containerName");
			}
			this.account = account;
			this.containerName = containerName;
		}

		public IEnumerable<Uri> GetAllUri()
		{
			CloudBlobContainer container = account.CreateCloudBlobClient().GetContainerReference(containerName);

			var options = new BlobRequestOptions {UseFlatBlobListing = true, BlobListingDetails = BlobListingDetails.None};
			ResultSegment<IListBlobItem> resultSegment = container.ListBlobsSegmented(options);
			foreach (var blobItem in resultSegment.Results)
			{
				yield return blobItem.Uri;
			}
			ResultContinuation continuationToken = resultSegment.ContinuationToken;

			while (continuationToken != null)
			{
				resultSegment = container.ListBlobsSegmented(2000, continuationToken, options);
				foreach (var blobItem in resultSegment.Results)
				{
					yield return blobItem.Uri;
				}
				continuationToken = resultSegment.ContinuationToken;
			}
		}
	}
}