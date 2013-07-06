using System;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class TableStorageInitializer<TTableEntity> : IStorageInitializer where TTableEntity : class
	{
		private readonly CloudStorageAccount account;
		private readonly string entityTableName = typeof (TTableEntity).AsTableStorageName();

		public TableStorageInitializer(CloudStorageAccount account)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			this.account = account;
			var properties =
				typeof (TTableEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(pi => pi.Name).
					ToList();
			if (!properties.Contains("PartitionKey") || !properties.Contains("RowKey") || !properties.Contains("Timestamp"))
			{
				throw new ArgumentOutOfRangeException("TTableEntity",
				                                      "The type of the entity is not a valid Azure entity type.(it should contain at least the three required public properties: PartitionKey,RowKey,Timestamp");
			}
		}

		public void Initialize()
		{
			var client = new CloudTableClient(account.TableEndpoint.ToString(), account.Credentials);
			client.CreateTableIfNotExist(entityTableName);
			// Execute conditionally for development storage only
			if (client.BaseUri.IsLoopback)
			{
				var instance = Activator.CreateInstance(typeof (TTableEntity), true);
				client.InitializeTableSchemaFromEntity(entityTableName, instance);
			}
		}

		public void Drop()
		{
			var client = new CloudTableClient(account.TableEndpoint.ToString(), account.Credentials);
			client.DeleteTableIfExist(entityTableName);
		}
	}
}