using Microsoft.WindowsAzure.Storage;

namespace ABServicios.Azure.Storage
{
	public static class AzureAccount
	{
		private const string DefaultDataConnectionString = "ABSConnectionString";
        
		public static CloudStorageAccount DefaultAccount()
		{
		    CloudStorageAccount account;
            return CloudStorageAccount.TryParse(DefaultDataConnectionString, out account) ? 
                            account :
                            CloudStorageAccount.DevelopmentStorageAccount;
		}
	}
}