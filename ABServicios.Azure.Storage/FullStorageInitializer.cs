using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using Microsoft.WindowsAzure.Storage;

namespace ABServicios.Azure.Storage
{
	public class FullStorageInitializer
	{
		public static void Initialize()
		{
			CloudStorageAccount account = AzureAccount.DefaultAccount();

            new QueueStorageInitializer<AppException>(account).Initialize();
            new QueueStorageInitializer<MailMessage>(account).Initialize();
            new QueueStorageInitializer<ApiAccessLog>(account).Initialize();
            new QueueStorageInitializer<AppErrorReport>(account).Initialize();
            new QueueStorageInitializer<DenunciaPrecios>(account).Initialize();

            new TableStorageInitializer<TableSampleData>(account).Initialize();
            new TableStorageInitializer<ApiAccessLogData>(account).Initialize();
		}
	}
}