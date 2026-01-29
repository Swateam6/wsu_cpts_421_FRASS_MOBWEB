using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;

namespace FRASS
{
	public partial class _default : System.Web.UI.Page
	{
		SiteDataManager db;

		protected void Page_Load(object sender, EventArgs e)
		{
			Master.SetMenuBarVisibility(false);
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			db = SiteDataManager.GetInstance();
			Master.DontValidate();
			if (!Page.IsPostBack)
			{
				if (Session["User"] != null)
				{
					var u = (User)Session["User"];
					db.AddLog(DatabaseIDs.LogTypes.Login, null, "Logout: " + u.FirstName + " " + u.LastName);
				}
				Session["siteLogin"] = null;
				Session["User"] = null;
				Response.Cookies["user"].Value = "";
				Response.Cookies["userid"].Value = "";
			}
		}
		protected void Button_SiteCredentials_Click(object sender, EventArgs e)
		{
			SiteLogin();
		}
		private void SiteLogin()
		{
			bool verified = VerifySitePassword();
			if (verified)
			{
				Session["siteLogin"] = "Verified";
				Panel_Login.Visible = true;
				Panel_SiteLogin.Visible = false;
			}
			else
			{
				Label_SitePassword_Error.Text = "Invalid Credentials";
			}
		}
		private bool VerifySitePassword()
		{
			var pwd = TextBox_SitePassword.Text;
			var actual = SiteDataManager.GetInstance().GetSiteSetting(1);
			if (pwd == actual.Value)
			{
				return true;
			}
			else
			{
				db.AddLog(DatabaseIDs.LogTypes.Login,null, "Site Password Fail: " + pwd);
				return false;
			}
		}
		protected void Button_Login_Click(object sender, EventArgs e)
		{
			if (!IsValidEmail(TextBox_Email.Text))
			{
				Label_Error.Text = "Enter a valid e-mail address.";
			}
			else
			{
				User u = UserDataManager.GetInstance().Login(TextBox_Email.Text, TextBox_Password.Text);
				if (u == null)
				{
					db.AddLog(DatabaseIDs.LogTypes.Login,null, "Login Fail: " + TextBox_Email.Text);
					Label_Error.Text = "Login Failed";
				}
				else
				{
					if (u.UserStatusID == Convert.ToInt32(UserStatusTypes.NeedsPasswordReset))
					{
						Response.Redirect("PasswordReset.aspx?token=" + u.Hash, true);
					}
					else
					{
						db.AddLog(DatabaseIDs.LogTypes.Login, u.UserID, "Successful Login: " + u.FirstName + " " + u.LastName);
						db.AddLog(DatabaseIDs.LogTypes.Login, u.UserID, Request.ServerVariables["REMOTE_ADDR"].ToString());
						Session["User"] = u;
						Response.Cookies["user"].Value = u.Hash;
						Response.Cookies["user"].Expires = System.DateTime.Now.AddMinutes(15);
						Response.Cookies["userid"].Value = u.UserID.ToString();
						Response.Cookies["userid"].Expires = System.DateTime.Now.AddMinutes(15);
						Response.Redirect("welcome.aspx", true);
					}
				}
			}
		}

		private bool IsValidEmail(string strIn)
		{
			// Return true if strIn is in valid e-mail format.
			return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}
	}
}