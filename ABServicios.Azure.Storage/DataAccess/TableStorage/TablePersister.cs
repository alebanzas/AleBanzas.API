using System;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage
{
	public class TablePersister<TDataRow> : IPersister<TDataRow> where TDataRow : TableDataRow
	{
		private readonly TableServiceContext tableContext;
		private readonly string entityTableName = typeof(TDataRow).AsTableStorageName();

		public TablePersister(TableServiceContext tableContext)
		{
			if (tableContext == null)
			{
				throw new ArgumentNullException("tableContext");
			}
			this.tableContext = tableContext;
		}

		public TDataRow Get(string partitionKey, string rowKey)
		{
			try
			{
				var query =
					(from te in tableContext.CreateQuery<TDataRow>(entityTableName)
					 where te.PartitionKey == partitionKey && te.RowKey == rowKey
					 select te).AsTableServiceQuery(tableContext);
				return query.Execute().SingleOrDefault();
			}
			catch (DataServiceQueryException e)
			{
				var inner = e.InnerException as DataServiceClientException;
				if (inner != null && inner.StatusCode == (int)HttpStatusCode.NotFound)
				{
					return null;
				}
				throw;
			}
		}

		public void Add(TDataRow dataRow)
		{
			tableContext.AddObject(entityTableName, dataRow);
		}

		public void Update(TDataRow dataRow)
		{
			tableContext.UpdateObject(dataRow);
		}

		public void Delete(string partitionKey, string rowKey)
		{
			var entity = Get(partitionKey, rowKey);
			if (entity != null)
			{
				tableContext.DeleteObject(entity);
			}
		}

		public void Delete(TDataRow dataRow)
		{
			Delete(dataRow.PartitionKey, dataRow.RowKey);
		}
	}
}