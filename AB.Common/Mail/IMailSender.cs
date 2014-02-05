using System.Net.Mail;

namespace AB.Common.Mail
{
    public interface IMailSender
    {
        void Send(MailMessage mail);
    }
}