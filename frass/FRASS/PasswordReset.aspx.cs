using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Mail;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS
{
	public partial class PasswordReset : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Master.SetMenuBarVisibility(false);
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			Master.DontValidate();
			if (Session["siteLogin"] == null)
			{
				Response.Redirect("/default.aspx", true);
			}
			else if (Session["siteLogin"].ToString() != "Verified")
			{
				Response.Redirect("/default.aspx", true);
			}
			if (!Page.IsPostBack)
			{
				if (Request.QueryString["token"] != null)
				{
					User u = UserDataManager.GetInstance().GetUserByHash(Request.QueryString["token"].ToString());
					if (u == null)
					{
						Response.Redirect("/default.aspx", true);
					}
					else
					{
						Panel_Reset.Visible = true;
						Panel_Form.Visible = false;
					}
				}
				else
				{
					Panel_Reset.Visible = false;
					Panel_Form.Visible = true;
				}
			}
		}
		protected void Button_SaveNewPassword_Click(object sender, EventArgs e)
		{
			if (TextBox_Password1.Text.Trim().Length >= 8)
			{
				if (TextBox_Password1.Text.Trim() == TextBox_Password2.Text.Trim())
				{
					if (IsPasswordStrong(TextBox_Password1.Text.Trim()))
					{
						var db = UserDataManager.GetInstance();
						User u = db.GetUserByHash(Request.QueryString["token"].ToString());
						u.Password = TextBox_Password1.Text.Trim();
						u.UserStatusID = Convert.ToInt32(UserStatusTypes.Active);
						u.WrongPasswordCount = 0;
						db.UpdatePassword(u);
						Session["User"] = u;
						Response.Cookies["user"].Value = u.Hash;
						Response.Cookies["user"].Expires = System.DateTime.Now.AddMinutes(15);
						Response.Cookies["userid"].Value = u.UserID.ToString();
						Response.Cookies["userid"].Expires = System.DateTime.Now.AddMinutes(15);
						Response.Redirect("welcome.aspx", true);
					}
				}
				else
				{
					SetFailMessage("Passwords do not match.");
				}
			}
			else
			{
				SetFailMessage("Passwords must be at least 8 characters long.");
			}
		}

		private bool IsPasswordStrong(string password)
		{
			if (password.Length >= 8)
			{
				if (Regex.IsMatch(password, "[abcdefghijklmnopqrstuvwxyz]"))
				{
					if (Regex.IsMatch(password, "[ABCDEFGHIJKLMNOPQRSTUVWXYZ]"))
					{
						if (Regex.IsMatch(password, "[0123456789]"))
						{
							if (Regex.IsMatch(password, "[^a-z0-9A-Z]"))
							{
								return true;
							}
							else
							{
								SetFailMessage("Passwords must be at least 8 characters long; with at least one lower case, at least one upper case, at least one number, and at least one special character (!@#$%^&*).");
							}
						}
						else
						{
							SetFailMessage("Passwords must be at least 8 characters long; with at least one lower case, at least one upper case, at least one number, and at least one special character (!@#$%^&*).");
						}
					}
					else
					{
						SetFailMessage("Passwords must be at least 8 characters long; with at least one lower case, at least one upper case, at least one number, and at least one special character (!@#$%^&*).");
					}
				}
				else
				{
					SetFailMessage("Passwords must be at least 8 characters long; with at least one lower case, at least one upper case, at least one number, and at least one special character (!@#$%^&*).");
				}
			}
			return false;
		}

		protected void Button_Submit_Click(object sender, EventArgs e)
		{
			if (IsValidEmail(TextBox_Email.Text.Trim()))
			{
				MailMan mail = new MailMan();
				var db = UserDataManager.GetInstance();
				User user = db.GetUser(TextBox_FirstName.Text.Trim(), TextBox_LastName.Text.Trim(), TextBox_Email.Text.Trim());
				if (user == null)
				{
					SetFailMessage("User Not Found");
				}
				else
				{
					if (user.UserStatusID == Convert.ToInt32(UserStatusTypes.Active) || user.UserStatusID == Convert.ToInt32(UserStatusTypes.NeedsPasswordReset))
					{
						user.Password = user.GetHash(System.DateTime.Now.Ticks.ToString()).ToUpper().Substring(0, 8) + "$a";
						user.UserStatusID = Convert.ToInt32(UserStatusTypes.NeedsPasswordReset);
						user.WrongPasswordCount = 0;
						db.UpdatePassword(user);
						mail.SendPasswordReset(user);
						Panel_Form.Visible = false;
						SetSuccessMessage("A new password has been emailed to you.");
					}
					else
					{
						SetFailMessage("Your account is not Active, please contact an Administrator to reactivate your account.");
					}

				}

			}
			else
			{
				SetFailMessage("Invalid Email Address Entered");
			}
		}
		protected void Button_Cancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("/default.aspx", true);
		}
		private void SetSuccessMessage(string message)
		{
			Label_Message.ForeColor = System.Drawing.Color.Green;
			Label_Message.Font.Bold = true;
			Label_Message.Text = message;
		}
		private void SetFailMessage(string message)
		{
			Label_Message.ForeColor = System.Drawing.Color.Red;
			Label_Message.Text = message;
		}
		private bool IsValidEmail(string strIn)
		{
			// Return true if strIn is in valid e-mail format.
			return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}
	}
}