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
        public AzureChristmasResultQuery(CloudStorageAccount account)
		{
		    var client = account.CreateCloudTableClient();
            _tableContext = client.GetTableServiceContext();
		}

        public VotacionModel GetResults()
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

            var rr = new VotacionModel();

            foreach (IGrouping<string, AzureChristmasVoteLogData> item in group.OrderByDescending(x => x.Count()))
            {
                if (item.Key.ToLowerInvariant().Contains("google")) continue;

                int count = item.Distinct(new AzureChristmasVoteLogDataComparer()).Count();

                rr.Lista.Add(new VotacionItem
                {
                    Nombre = item.Key,
                    Visitas = count,
                });
            }

            rr.Lista = rr.Lista.OrderByDescending(x => x.Visitas).ToList();

            return rr;
		}

	    public List<string> GetUsersFromReferal(string referal)
	    {
            var queryable = _tableContext.CreateQuery<AzureChristmasVoteLogData>(typeof(AzureChristmasVoteLogData).AsTableStorageName());

            IQueryable<AzureChristmasVoteLogData> apiAccessLogDatas = (from data in queryable
                                                                       where
                                                                            data.Referal == referal &&
                                                                           (!data.PartitionKey.Equals("127.0.0.1") && !data.PartitionKey.Equals("127.0.0.2") &&
                                                                            !data.PartitionKey.Equals("localhost") && !data.PartitionKey.Equals("127.255.0.0") && !data.Ip.Equals("127.0.0.1"))
                                                                       select data);
            var result = apiAccessLogDatas.AsTableServiceQuery(_tableContext).Execute();

            return result.GroupBy(x => x.UserId).Select(x => x.Key).ToList();
	    }

	    public VotacionModel GetResultsToView()
	    {
            var queryable = _tableContext.CreateQuery<AzureChristmasVoteUserResultData>(typeof(AzureChristmasVoteUserResultData).AsTableStorageName());

	        var votacionItems = new List<VotacionItem>(
	            queryable.AsTableServiceQuery(_tableContext).Execute().Select(x => new VotacionItem
	            {
	                Nombre = x.UserId,
	                Puntos = x.Puntos,
	                Visitas = x.Visitas,
	                VisitasReferidas = x.VisitasReferidos,
	            }).OrderByDescending(x => (x.Visitas + x.VisitasReferidas)));

            var votacionItemsFiltrado = new List<VotacionItem>();

	        foreach (var votacionItem in votacionItems)
	        {
                Uri referal;
                if (votacionItem.Nombre == null || !votacionItem.Nombre.EndsWith(".cloudapp.net") || !Uri.TryCreate("http://" + votacionItem.Nombre, UriKind.Absolute, out referal))
                {
                    continue;
                }
	            votacionItemsFiltrado.Add(votacionItem);
	        }

	        return new VotacionModel
            {
                Lista =
                    votacionItemsFiltrado,
            };
	    }
	}
}