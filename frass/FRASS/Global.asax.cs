using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using FRASS.BLL.Mail;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;

namespace FRASS.WebUI
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{

		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var exception = Server.GetLastError() as Exception;
			Int32? UserID = null;
			try
			{
				if (Session["User"] != null)
				{
					var user = (User)Session["User"];
					UserID = user.UserID;
				}

				// Handle request validation errors gracefully
				if (exception is System.Web.HttpRequestValidationException)
				{
					// Clear the error
					Server.ClearError();
					// Redirect to an error page or show a message
					Response.Redirect("~/Error.aspx?msg=Invalid input detected. Please avoid using HTML tags or special characters in form fields.", false);
					return;
				}

				var str = "Error in: " + Request.Url.ToString() + "<br/><br/>Exception: " + exception.GetBaseException().Message + "<br/><br/>Stack Trace: " + exception.GetBaseException().StackTrace;
				SiteDataManager.GetInstance().AddLog(DatabaseIDs.LogTypes.Error, UserID, str);
				MailMan.SendErrorEmail(str);
			}
			catch(Exception){}
		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}
