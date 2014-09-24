using System;
using System.Net;
using System.Net.Mail;

namespace AB.Common.Mail
{
	/// <summary>
	/// Sender General de mails.
	/// </summary>
	public class GeneralMailSender: IMailSender
	{
		private readonly string userName;
		private readonly string password;

		public GeneralMailSender(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentNullException("userName");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("password");
			}
			this.userName = userName;
			this.password = password;
		}

		public void Send(MailMessage mail)
		{
			var client = new SmtpClient { Host = "smtp.gmail.com", EnableSsl = true, Credentials = new NetworkCredential(userName, password) };
			client.Send(mail);
		}
	}
}