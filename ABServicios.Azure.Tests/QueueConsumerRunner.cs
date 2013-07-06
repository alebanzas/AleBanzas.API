using ABServicios.Azure.QueuesConsumers;
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
	}
}