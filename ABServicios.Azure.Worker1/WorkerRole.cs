using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ABServicios.Azure.QueuesConsumers;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace ABServicios.Azure.Worker1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public readonly AzureChristmasResultQuery query;
        private static TableServiceContext _tableContext;
        private static TablePersister<AzureChristmasVoteUserResultData> _tablePersister;
        public WorkerRole()
        {
            query = new AzureChristmasResultQuery(AzureAccount.DefaultAccount());
            
            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tableContext = new TableServiceContext(tableClient);
            _tablePersister = new TablePersister<AzureChristmasVoteUserResultData>(_tableContext);
        }


        public override void Run()
        {
            Trace.TraceInformation("ABServicios.Azure.Worker1 is running");
            
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ABServicios.Azure.Worker1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ABServicios.Azure.Worker1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ABServicios.Azure.Worker1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            QueueConsumerFor<ApiAccessLog>.WithStandaloneThread.Using(new ApiAccessLogSaver())
                                                                                        .With(PollingFrequencer.For(ApiAccessLogSaver.EstimatedTime))
                                                                                        .StartConsimung();

            QueueConsumerFor<MailMessage>.WithStandaloneThread.Using(new MailsMessagesSender())
                                                                                        .With(PollingFrequencer.For(MailsMessagesSender.EstimatedTime))
                                                                                        .StartConsimung();


            QueueConsumerFor<AzureChristmasVoteLog>.WithStandaloneThread.Using(new AzureChristmasVoteLogSaver())
                                                                                        .With(PollingFrequencer.For(AzureChristmasVoteLogSaver.EstimatedTime))
                                                                                        .StartConsimung();
            
            QueueConsumerFor<PuntosProcesados>.WithStandaloneThread.Using(new AzureChristmasPuntosPorUsuario())
                                                                                        .With(PollingFrequencer.For(AzureChristmasPuntosPorUsuario.EstimatedTime))
                                                                                        .StartConsimung();

            //QueueConsumerFor<AzureChristmasRefreshReferal>.WithStandaloneThread.Using(new AzureChristmasReferalRefresh())
            //                                                                            .With(PollingFrequencer.For(AzureChristmasReferalRefresh.EstimatedTime))
            //                                                                            .StartConsimung();

            QueueConsumerFor<TrenEnEstacion>.WithStandaloneThread.Using(new TrenEnEstacionReduceDuplicates())
                                                                                        .With(PollingFrequencer.For(TrenEnEstacionReduceDuplicates.EstimatedTime))
                                                                                        .StartConsimung();


            // TODO: logica de tareas como implementacion de consumers
            while (!cancellationToken.IsCancellationRequested)
            {
                CalculateAzureChristmasResults();


                Trace.TraceInformation("Working");
                await Task.Delay(1000 * 60 * 5);
            }
        }

        private void CalculateAzureChristmasResults()
        {
            var results = query.GetResults();

            //TODO: por cada resultado, armo o actualizo la tabla
            //TODO: tuneo la query para que traiga por fecha y hora
        }
    }
}
