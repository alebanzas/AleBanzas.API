using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
	public class AzureChristmasResultQuery
	{
		private readonly TableServiceContext _tableContext;
        private readonly TablePersister<AzureChristmasVoteLogData> _tablePersister;

        public AzureChristmasResultQuery(CloudStorageAccount account)
		{
		    var client = account.CreateCloudTableClient();
			_tableContext = client.GetTableServiceContext();
            _tablePersister = new TablePersister<AzureChristmasVoteLogData>(_tableContext);
		}

        public IEnumerable<IGrouping<string, AzureChristmasVoteLogData>> GetResults()
		{
            var queryable = _tableContext.CreateQuery<AzureChristmasVoteLogData>(typeof(AzureChristmasVoteLogData).AsTableStorageName());

            IQueryable<AzureChristmasVoteLogData> apiAccessLogDatas = (from data in queryable
                where
                    (!data.PartitionKey.Equals("127.0.0.1") && !data.PartitionKey.Equals("127.0.0.2") &&
                     !data.PartitionKey.Equals("localhost") && !data.PartitionKey.Equals("127.255.0.0") && !data.Ip.Equals("127.0.0.1"))
                select data);
            var result = apiAccessLogDatas.AsTableServiceQuery(_tableContext).Execute();

		    var list = result.ToList();


		    var group = list.GroupBy(x => x.UserId);

		    return group;
		}
	}
}