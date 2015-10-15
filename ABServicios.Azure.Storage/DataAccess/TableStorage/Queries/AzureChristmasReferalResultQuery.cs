using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ABServicios.Azure.Storage.DataAccess.TableStorage.Queries
{
    public class AzureChristmasReferalResultQuery
    {
        private CloudTable _table;
        private CloudTable _tableTuple;

        public AzureChristmasReferalResultQuery(CloudStorageAccount account)
		{
            var client = account.CreateCloudTableClient();
            _table = client.GetTableReference(typeof(AzureChristmasVoteReferalResultData).AsTableStorageName());
            _tableTuple = client.GetTableReference(typeof(AzureChristmasVoteLogReferalData).AsTableStorageName());
        }

        public VotacionModel GetResults()
        {
            var query = _table.CreateQuery<AzureChristmasVoteReferalResultData>();

            var list = query.Where(x => x.PartitionKey == "PK").ToList().OrderByDescending(x => x.Puntos);

            var rr = new VotacionModel();

            foreach (var item in list)
            {
                rr.Lista.Add(new VotacionItem
                {
                    Nombre = item.ReferalId,
                    Puntos = item.Puntos,
                });
            }

            return rr;
        }

        public IEnumerable<IGrouping<string, AzureChristmasVoteLogReferalData>> GetToGenerateResults()
        {
            var query = _tableTuple.CreateQuery<AzureChristmasVoteLogReferalData>();

            return query.ToList().GroupBy(x => x.ReferalId);
        }
    }
}