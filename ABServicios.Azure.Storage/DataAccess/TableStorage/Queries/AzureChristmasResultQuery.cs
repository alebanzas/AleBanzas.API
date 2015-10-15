using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
    public class AzureChristmasResultQuery
    {
        private CloudTable _table;
        private CloudTable _tableResults;

	    public AzureChristmasResultQuery(CloudStorageAccount account)
		{
            var client = account.CreateCloudTableClient();
            _table = client.GetTableReference(typeof(AzureChristmasVoteLogData).AsTableStorageName());
            _tableResults = client.GetTableReference(typeof(AzureChristmasVoteUserResultData).AsTableStorageName());
		}

        public VotacionModel GetResults(List<AzureChristmasVoteLogData> list)
        {
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

        public List<AzureChristmasVoteLogData> GetList()
        {
            IQueryable<AzureChristmasVoteLogData> apiAccessLogDatas =
                (from data in _table.CreateQuery<AzureChristmasVoteLogData>()
                    where
                        (!data.PartitionKey.Equals("127.0.0.1") && !data.PartitionKey.Equals("127.0.0.2") &&
                         !data.PartitionKey.Equals("localhost") && !data.PartitionKey.Equals("127.255.0.0") &&
                         !data.Ip.Equals("127.0.0.1"))
                    select data);

            var list = apiAccessLogDatas.ToList();
            return list;
        }

        public List<string> GetUsersFromReferal(string referal)
	    {
            IQueryable<string> apiAccessLogDatas = (from data in _table.CreateQuery<AzureChristmasVoteLogData>()
	                                                                   where
	                                                                        data.Referal == referal &&
	                                                                       (!data.PartitionKey.Equals("127.0.0.1") && !data.PartitionKey.Equals("127.0.0.2") &&
	                                                                        !data.PartitionKey.Equals("localhost") && !data.PartitionKey.Equals("127.255.0.0") && !data.Ip.Equals("127.0.0.1"))
	                                                                   select data.PartitionKey);

            return apiAccessLogDatas.ToList().Distinct().ToList();
	    }

	    public VotacionModel GetResultsToView()
	    {
	        var votacionItems = new List<VotacionItem>(
                _tableResults.CreateQuery<AzureChristmasVoteUserResultData>().ToList().Select(x => new VotacionItem
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
                if (votacionItem.Nombre == null || !votacionItem.Nombre.EndsWith(".azurewebsites.net") || !Uri.TryCreate("http://" + votacionItem.Nombre, UriKind.Absolute, out referal))
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