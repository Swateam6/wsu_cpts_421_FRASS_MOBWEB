using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FRASS.WebUI.WWW.BLL
{
	public class MailMain
	{
		private string emailAddress = "admin@resource-analysis.com";

		public void SendMail(MailMessage mail)
		{
			mail.From = new MailAddress(emailAddress);

			var smtp = ConfigurationManager.AppSettings["SMTP"].ToString();
			var client = new SmtpClient(smtp);

			var useCreds = ConfigurationManager.AppSettings["SMTPCredentials"].ToString();
			if (useCreds == "true")
			{
				var smtpUsername = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
				var smtpPassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

				NetworkCredential creds = new NetworkCredential(smtpUsername, smtpPassword);
				client.UseDefaultCredentials = false;
				client.Credentials = creds;
			}
			client.Send(mail);

		}
	}
}