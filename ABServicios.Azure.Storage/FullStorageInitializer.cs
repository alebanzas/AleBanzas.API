﻿using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using Microsoft.WindowsAzure;

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
            
			new TableStorageInitializer<TableSampleData>(account).Initialize();
		}
	}
}