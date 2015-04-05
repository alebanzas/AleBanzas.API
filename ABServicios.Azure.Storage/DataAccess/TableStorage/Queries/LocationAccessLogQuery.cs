using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
	public class LocationAccessLogQuery
	{
	    private readonly CloudTable _table;

	    public LocationAccessLogQuery(CloudStorageAccount account)
		{
		    var client = account.CreateCloudTableClient();
            _table = client.GetTableReference(typeof(ApiAccessLogData).AsTableStorageName());
		}
        
		public IList<ApiAccessLogData> GetResultsFromDate(DateTime date)
		{
            var pk = date.ToString("yyyyMMdd");

            var queryable = _table.CreateQuery<ApiAccessLogData>();

			var result = (from data in queryable where data.PartitionKey == pk select data);

		    return result.ToList();
		}
	}
}