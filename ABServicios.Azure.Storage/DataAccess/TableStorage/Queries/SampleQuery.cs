using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
	public class SampleQuery
	{
		private readonly TableServiceContext tableContext;
        private readonly TablePersister<TableSampleData> tableSamplePersister;

		public SampleQuery(CloudStorageAccount account)
		{
			tableContext = new TableServiceContext(account.TableEndpoint.ToString(), account.Credentials);
            tableSamplePersister = new TablePersister<TableSampleData>(tableContext);
		}

        public TableSampleData GetResultsFromPublicacion(string ex1, int ex2)
		{
            var row = tableSamplePersister.Get(TableSampleData.GetPartionKeyFor(ex1, ex2), TableSampleData.GetRowKey(ex1, ex2));

			return row;
		}

		public void GetResultsFromPackage(string ex1, int ex2)
		{
            var pk = TableSampleData.GetPartionKeyFor(ex1, ex2);

            var queryable = tableContext.CreateQuery<TableSampleData>(typeof(TableSampleData).AsTableStorageName());

			var result = (from data in queryable where data.PartitionKey == pk select data).AsTableServiceQuery().Execute();
		}
	}
}