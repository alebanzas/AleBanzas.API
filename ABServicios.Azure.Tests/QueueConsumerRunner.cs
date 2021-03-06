﻿using ABServicios.Azure.QueuesConsumers;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using NUnit.Framework;

namespace ABServicios.Azure.Tests
{
    public class QueueConsumerRunner
    {
        [Test]
        public void FullStorageInitializerTest()
        {
            FullStorageInitializer.Initialize();
        }

        [Test]
        public void StartEnqueuingMailsMessages()
        {
            var i = 1;
            while (i <= 5)
            {
                AzureQueue.Enqueue(new MailMessage
                {
                    From = "noreply@abhosting.com.ar",
                    Subject = "TEST" + i,
                    To = "alebanzas@outlook.com",
                    Body = "<b>probando</b> el mensaje",
                });
                i++;
            }
        }

        [Test]
        public void StartConsumigMailsMessagesSender()
        {
            QueueConsumerFor<MailMessage>.WithinCurrentThread.Using(new MailsMessagesSender())
                                            .With(PollingFrequencer.For(MailsMessagesSender.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigApiAccessLogSaver()
        {
            QueueConsumerFor<ApiAccessLog>.WithinCurrentThread.Using(new ApiAccessLogSaver())
                                            .With(PollingFrequencer.For(ApiAccessLogSaver.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigDenunciaPreciosSaver()
        {
            QueueConsumerFor<DenunciaPrecios>.WithinCurrentThread.Using(new DenunciaPreciosSaver())
                                            .With(PollingFrequencer.For(DenunciaPreciosSaver.EstimatedTime))
                                            .StartConsimung();
        }

        [Test]
        public void StartConsumigTrenEnEstacionReduceDuplicates()
        {
            QueueConsumerFor<TrenEnEstacion>.WithinCurrentThread.Using(new TrenEnEstacionReduceDuplicates())
                                            .With(PollingFrequencer.For(TrenEnEstacionReduceDuplicates.EstimatedTime))
                                            .StartConsimung();
        }
    }
}
