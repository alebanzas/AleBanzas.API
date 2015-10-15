using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.QueuesConsumers;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using ABServicios.Azure.Storage.DataAccess.TableStorage.Queries;
using ABServicios.Azure.Storage.DataAccess.TableStorage;
using ABServicios.Azure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;
using System.Linq;

namespace ABServicios.Azure.Worker1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public readonly AzureChristmasResultQuery query;
        public readonly AzureChristmasReferalResultQuery queryR;
        private static TablePersister<AzureChristmasVoteUserResultData> _tablePersister;
        private static TablePersister<AzureChristmasVoteReferalResultData> _tableReferalPersister;
        public WorkerRole()
        {
            query = new AzureChristmasResultQuery(AzureAccount.DefaultAccount());
            queryR = new AzureChristmasReferalResultQuery(AzureAccount.DefaultAccount());

            var tableClient = AzureAccount.DefaultAccount().CreateCloudTableClient();
            _tablePersister = new TablePersister<AzureChristmasVoteUserResultData>(tableClient);
            _tableReferalPersister = new TablePersister<AzureChristmasVoteReferalResultData>(tableClient);
            
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
            
            //QueueConsumerFor<PuntosProcesados>.WithStandaloneThread.Using(new AzureChristmasPuntosPorUsuario())
            //                            .With(PollingFrequencer.For(AzureChristmasPuntosPorUsuario.EstimatedTime))
            //                            .StartConsimung();
            
            QueueConsumerFor<AzureChristmasRefreshReferal>.WithStandaloneThread.Using(new AzureChristmasReferalRefresh())
                                          .With(PollingFrequencer.For(AzureChristmasReferalRefresh.EstimatedTime))
                                          .StartConsimung();
            
            QueueConsumerFor<TrenEnEstacion>.WithStandaloneThread.Using(new TrenEnEstacionReduceDuplicates())
                                            .With(PollingFrequencer.For(TrenEnEstacionReduceDuplicates.EstimatedTime))
                                            .StartConsimung();

            await CalculateAzureChristmasResults();

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000, cancellationToken);
            }
        }

        public async Task CalculateAzureChristmasResults()
        {
            while (true)
            {
                Trace.TraceInformation("{0} CalculateAzureChristmasResults", DateTime.UtcNow.ToString("s"));
                var list = query.GetList();
                var results = query.GetResults(list);

                foreach (var votacionItem in results.Lista)
                {
                    try
                    {
                        var i = _tablePersister.Get(AzureChristmasVoteUserResultData.PKey, votacionItem.Nombre);

                        if (i == null)
                        {
                            await _tablePersister.AddAsync(new AzureChristmasVoteUserResultData(votacionItem.Nombre)
                            {
                                Visitas = votacionItem.Visitas,
                            });
                        }
                        else
                        {
                            if (i.Visitas == votacionItem.Visitas) continue;

                            i.Visitas = votacionItem.Visitas;
                            await _tablePersister.UpdateAsync(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Error al procesar " + votacionItem.Nombre + " " + ex.Message);
                        throw;
                    }
                }

                //referal
                var groups = queryR.GetToGenerateResults();

                foreach (var @group in groups)
                {
                    try
                    {
                        
                        var i = _tableReferalPersister.Get(AzureChristmasVoteReferalResultData.PKey, group.Key);

                        if (i == null)
                        {
                            await _tableReferalPersister.AddAsync(new AzureChristmasVoteReferalResultData(group.Key)
                            {
                                Puntos = 1,
                            });
                        }
                        else
                        {
                            i.Puntos = group.Count();
                            await _tableReferalPersister.UpdateAsync(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Error al procesar referal " + group.Key + " " + ex.Message);
                        throw;
                    }
                }

                



                //TODO: tuneo la query para que traiga por fecha y hora

                Thread.Sleep(1000*60);
            }
        }
    }
}
