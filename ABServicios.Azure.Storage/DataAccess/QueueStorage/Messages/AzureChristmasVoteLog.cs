using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages
{
    public class AzureChristmasVoteLog
    {
        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public string Ip { get; set; }

        public string Referer { get; set; }
         
        public string Referal { get; set; }
    }
}
