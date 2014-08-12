using System;

namespace ABServicios.Azure.Storage.DataAccess.QueueStorage.Messages
{
    public class AppErrorReport
    {
        public string AppId { get; set; }

        public string AppVersion { get; set; }

        public string InstallationId { get; set; }

        public string ErrorDetail { get; set; }

        public string UserMessage { get; set; }

        public DateTime Date { get; set; }

        public Guid TrackingId { get; set; }
    }
}
