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
        public void StartConsumigAzureChristmasVoteLogSaver()
        {
            QueueConsumerFor<AzureChristmasVoteLog>.WithinCurrentThread.Using(new AzureChristmasVoteLogSaver())
                                                                                        .With(PollingFrequencer.For(AzureChristmasVoteLogSaver.EstimatedTime))
                                                                                        .StartConsimung();
        }

        [Test]
        public void StartConsumigAzureChristmasVoteDespegar()
        {
            QueueConsumerFor<AzureChristmasVoteLog>.WithinCurrentThread.Using(new AzureChristmasVoteLogDespegar())
                                                                                        .With(PollingFrequencer.For(AzureChristmasVoteLogDespegar.EstimatedTime))
                                                                                        .StartConsimung();
        }
	}
}