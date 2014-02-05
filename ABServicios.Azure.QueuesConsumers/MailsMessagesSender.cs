using System;
using AB.Common.Mail;
using ABServicios.Azure.Storage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage;
using ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages;
using System.Diagnostics;

namespace ABServicios.Azure.QueuesConsumers
{
	public class MailsMessagesSender : IQueueMessageConsumer<MailMessage>
	{
		public static readonly TimeSpan EstimatedTime = TimeSpan.FromSeconds(40);

	    public TimeSpan? EstimatedTimeToProcessMessageBlock { get; private set; }

	    public void ProcessMessages(QueueMessage<MailMessage> message)
		{
            try
            {
                var mailSender = new GeneralMailSender("noreply@abhosting.com.ar", "Y@361]96KB{CPiw");
                var mailMessage = new System.Net.Mail.MailMessage(message.Data.From, message.Data.To, message.Data.Subject, message.Data.Body);

                // si el mensaje es null significa que el maker controló algunas situaciones y no hay nada para enviar y el mensaje se puede remover de la queue
                mailSender.Send(mailMessage);
            }
            catch (Exception e)
            {
                e.LogSilentNoHttp(string.Format("Enviando mail key {0}", "GeneralMailSender"));
                string exmsg = string.Format("Enviando mail : {0}\nStackTrace:{1}", e.Message, e.StackTrace);
                Trace.WriteLine(exmsg, "Error");
                if (message.DequeueCount < 20)
                {
                    throw;
                }
            }
		}
	}
}