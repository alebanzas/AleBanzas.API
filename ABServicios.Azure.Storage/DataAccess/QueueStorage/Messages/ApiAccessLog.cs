using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages
{
    public class ApiAccessLog
    {
        public DateTime DateTime { get; set; }
        public string Host { get; set; }
        public string PathAndQuery { get; set; }
        public string Request { get; set; }
        public string FullUrl { get; set; }
	}
}