using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Net;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;

namespace FRASS.BLL.Mail
{
	public class MailMan
	{
		string emailAddress = "admin@resource-analysis.com";

		public static void SendErrorEmail(string error)
		{
			SiteSetting ss1 = SiteDataManager.GetInstance().GetSiteSetting(DatabaseIDs.SiteSettings.AdminEmail);
			SiteSetting ss2 = SiteDataManager.GetInstance().GetSiteSetting(DatabaseIDs.SiteSettings.ErrorEmail);
			MailMessage mail = new MailMessage(ss1.Value, ss2.Value);
			mail.Subject = "FRASS Error";
			mail.Body = error;
			mail.IsBodyHtml = true;
			var mm = new MailMan();
			mm.SendMail(mail);
		}

		public void SendMail(MailMessage mail)
		{
			try
			{
				var appSettings = new AppSettingsReader();
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
				SiteDataManager.GetInstance().AddLog(DatabaseIDs.LogTypes.EmailSent, null, "Email Sent: " + mail.Subject + "<br/><br/>" + mail.To.First().Address + "<br/><br/>" + mail.Body);
			}
			catch (Exception ex)
			{
				SiteDataManager.GetInstance().AddLog(DatabaseIDs.LogTypes.EmailSent, null, "Email Failed: " + mail.Subject + "<br/><br/>" + mail.To.First().Address + "<br/><br/>" + mail.Body + "<br/><br/>" + ex.Message);
				//throw ex;
			}
		}

		public void SendAccountRequest(User user)
		{
			SiteSetting ss = SiteDataManager.GetInstance().GetSiteSetting(2);
			MailMessage mail = new MailMessage(emailAddress, ss.Value);
			mail.Subject = "FRASS Account Request";
			mail.Body = user.FirstName + " " + user.LastName + " (" + user.Email + ") Has Requested an Account.";
			mail.IsBodyHtml = true;
			SendMail(mail);
		}

		public void SendPasswordReset(User user)
		{
			MailMessage mail = new MailMessage(emailAddress, user.Email);
			mail.Subject = "FRASS Password Reset";
			mail.Body = user.FirstName + " " + user.LastName + ", your password has been reset.  Your temporary password id: " + user.Password + "<br /><br />http://frass.forest-econometrics.com";
			mail.IsBodyHtml = true;
			mail.To.Add(user.Email);
			SendMail(mail);
		}

		public void SendNewUserEmail(User user)
		{
			MailMessage mail = new MailMessage(emailAddress, user.Email);
			mail.Subject = "A FRASS Account has been created for you.";
			mail.Body = user.FirstName + " " + user.LastName + ", an account has been created for you at http://frass.forest-econometrics.com.  A temporary password has been set to: " + user.Password + "<br /><br />";
			mail.IsBodyHtml = true;
			mail.To.Add(user.Email);
			SendMail(mail);
		}

		public void SendActivateUser(User user)
		{
			MailMessage mail = new MailMessage(emailAddress, user.Email);
			mail.Subject = "Your FRASS Account has been activated.";
			mail.Body = user.FirstName + " " + user.LastName + ", your account has been approved for you at http://frass.forest-econometrics.com.  A temporary password has been set to: " + user.Password + "<br /><br />";
			mail.IsBodyHtml = true;
			mail.To.Add(user.Email);
			SendMail(mail);
		}

		public void SendReportCompleted(User user, string title)
		{
			var days = SiteDataManager.GetInstance().GetSiteSetting(DatabaseIDs.SiteSettings.ReportExpirationDays).Value;
			MailMessage mail = new MailMessage(emailAddress, user.Email);
			mail.Subject = "Report Completed";
			mail.Body = user.FirstName + " " + user.LastName + ", your report, " + title + " has been completed.  Please look under your My Reports menu item to view this report.  It will expire in " + days + " days.<br /><br />http://frass.forest-econometrics.com";
			mail.IsBodyHtml = true;
			mail.To.Add(user.Email);
			SendMail(mail);
		}

		public void SendBLSUpdatedEmail()
		{
			SiteSetting ss1 = SiteDataManager.GetInstance().GetSiteSetting(DatabaseIDs.SiteSettings.AdminEmail);
			SiteSetting ss2 = SiteDataManager.GetInstance().GetSiteSetting(DatabaseIDs.SiteSettings.ErrorEmail);
			MailMessage mail = new MailMessage();
			mail.From = new MailAddress(ss2.Value);
			mail.To.Add(ss1.Value);
			mail.To.Add(ss2.Value);
			mail.Subject = "FRASS BLS Updated";
			mail.Body = "FRASS BLS Data Updated Automatically Updated";
			mail.IsBodyHtml = true;
			var mm = new MailMan();
			mm.SendMail(mail);
		}
	}
}