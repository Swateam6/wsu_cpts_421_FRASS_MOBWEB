using System;
using System.Net.Mail;
using System.Web.UI;
using FRASS.BLL.Mail;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.WebUI.Popups
{
	public partial class SendEmail : System.Web.UI.Page
	{
		User thisUser;
		UserDataManager db;
		protected void Page_Load(object sender, EventArgs e)
		{
			ValidateUser();
		}

		protected void Button_Send_Click(object sender, EventArgs e)
		{
			var mail = new MailMan();
			MailMessage message = new MailMessage();
			message.Subject = TextBox_Subject.Text;
			message.Body = TextBox_Message.Text;
			message.IsBodyHtml = false;
			db = UserDataManager.GetInstance();
			var users = db.GetUsers();

			foreach (var user in users)
			{
				message.Bcc.Add(new MailAddress(user.Email, user.FirstName + " " + user.LastName));
			}
			MailAddress fromAddress = new MailAddress("admin@resource-analysis.com");
			message.From = fromAddress;
			message.To.Add(fromAddress);
			mail.SendMail(message);
			TriggerClose();
		}

		private void ValidateUser()
		{
			if (Session["User"] == null)
			{
				if (Request.Cookies["user"] != null && Request.Cookies["userid"] != null)
				{
					string hash = Request.Cookies["user"].Value;
					string useridString = Request.Cookies["userid"].Value;
					Int32 userid = 0;
					if (Int32.TryParse(useridString, out userid))
					{

						if (hash.Length == 0 || userid == 0)
						{
							TriggerClose();
						}
						else
						{
							db = UserDataManager.GetInstance();
							User u = db.GetUser(hash, userid);
							if (u != null)
							{
								Session["User"] = u;
								Response.Cookies["user"].Value = u.Hash;
								Response.Cookies["user"].Expires = System.DateTime.Now.AddHours(2);
								Response.Cookies["userid"].Value = u.UserID.ToString();
								Response.Cookies["userid"].Expires = System.DateTime.Now.AddHours(2);
								thisUser = u;
							}
							else
							{
								Session["User"] = null;
								Response.Cookies["user"].Value = "";
								Response.Cookies["userid"].Value = "";
								TriggerClose();
							}
						}
					}
					else
					{
						TriggerClose();
					}
				}
				else
				{
					TriggerClose();
				}
			}
			else
			{
				thisUser = (User)Session["User"];
			}
		}

		private void TriggerClose()
		{
			string script = "cancelClick();";
			ScriptManager.RegisterStartupScript(Page, typeof(Page), "script", script, true);
		}
	}
}