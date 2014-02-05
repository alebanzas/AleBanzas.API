using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using ABServicios.Azure.QueuesConsumers;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ABServicios.Azure.Worker1
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            QueueConsumerFor<MailMessage>.WithinCurrentThread.Using(new MailsMessagesSender())
                                                                                        .With(PollingFrequencer.For(MailsMessagesSender.EstimatedTime))
                                                                                        .StartConsimung();
            QueueConsumerFor<ApiAccessLog>.WithinCurrentThread.Using(new ApiAccessLogSaver())
                                                                                        .With(PollingFrequencer.For(ApiAccessLogSaver.EstimatedTime))
                                                                                        .StartConsimung();


            //QueueConsumerFor<AzureChristmasVoteLog>.WithinCurrentThread.Using(new AzureChristmasVoteLogDespegar())
            //                                                                            .With(PollingFrequencer.For(AzureChristmasVoteLogDespegar.EstimatedTime))
            //                                                                            .StartConsimung();

            //QueueConsumerFor<AzureChristmasVoteLog>.WithinCurrentThread.Using(new AzureChristmasVoteLogSaver())
            //                                                                            .With(PollingFrequencer.For(AzureChristmasVoteLogSaver.EstimatedTime))
            //                                                                            .StartConsimung();


            var waitForNothing = TimeSpan.FromSeconds(1);
            while (true)
            {
                Thread.Sleep(waitForNothing);
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            WaitForRoleAvailability();

            InitializeAzureStorage();

            return base.OnStart();
        }

        private static void WaitForRoleAvailability()
        {
            bool accountAvailable = false;
            do
            {
                try
                {
                    AzureAccount.DefaultAccount();
                    accountAvailable = true;
                }
                catch (RoleEnvironmentException)
                {
                    Thread.Sleep(3000);
                }
            } while (!accountAvailable);
        }

        private static void InitializeAzureStorage()
        {
            bool storageInitialized = false;
            do
            {
                try
                {
                    FullStorageInitializer.Initialize();
                    storageInitialized = true;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Storage services initialization failure. Message: '{0}'", e.Message);
                    Thread.Sleep(5000);
                }
            } while (!storageInitialized);
        }
    }
}
