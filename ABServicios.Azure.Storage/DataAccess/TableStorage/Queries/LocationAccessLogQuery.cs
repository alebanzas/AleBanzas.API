using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
	public class LocationAccessLogQuery
	{
		private readonly TableServiceContext _tableContext;
        private readonly TablePersister<ApiAccessLogData> _tablePersister;

        public LocationAccessLogQuery(CloudStorageAccount account)
		{
		    var client = account.CreateCloudTableClient();
			_tableContext = client.GetTableServiceContext();
            _tablePersister = new TablePersister<ApiAccessLogData>(client);
		}
        
		public IList<ApiAccessLogData> GetResultsFromDate(DateTime date)
		{
            var pk = date.ToString("yyyyMMdd");

            var queryable = _tableContext.CreateQuery<ApiAccessLogData>(typeof(ApiAccessLogData).AsTableStorageName());

			var result = (from data in queryable where data.PartitionKey == pk select data).AsTableServiceQuery(_tableContext).Execute();

		    return result.ToList();
		}
	}
}