using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ABServicios.Azure.QueuesConsumers;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace ABServicios.Azure.Worker1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

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


            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
