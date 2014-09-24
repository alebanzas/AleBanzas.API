using System;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class TablePersister<TDataRow> : IPersister<TDataRow> where TDataRow : TableDataRow
	{
		private readonly CloudTable table;
		private readonly string entityTableName = typeof(TDataRow).AsTableStorageName();

        public TablePersister(CloudTableClient tableClient)
		{
            if (tableClient == null)
			{
                throw new ArgumentNullException("cloudTableClient");
			}
            table = tableClient.GetTableReference(entityTableName);
		}

		public TDataRow Get(string partitionKey, string rowKey)
		{
		    try
		    {
                var retrieveOperation = TableOperation.Retrieve<TDataRow>(partitionKey, rowKey);

                var retrievedResult = table.Execute(retrieveOperation);

                return (TDataRow)retrievedResult.Result;
		    }
		    catch (StorageException e)
		    {
                if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
		        {
		            return null;
		        }
		        throw;
		    }
		}

		public void Add(TDataRow dataRow)
		{
		    var op = TableOperation.Insert(dataRow);
            table.Execute(op);
		}

		public void Update(TDataRow dataRow)
        {
            var op = TableOperation.Replace(dataRow);
            table.Execute(op);
		}

		public void Delete(string partitionKey, string rowKey)
		{
			var entity = Get(partitionKey, rowKey);
		    if (entity == null) return;

		    var op = TableOperation.Delete(entity);
		    table.Execute(op);
		}

		public void Delete(TDataRow dataRow)
		{
			Delete(dataRow.PartitionKey, dataRow.RowKey);
		}
	}
}