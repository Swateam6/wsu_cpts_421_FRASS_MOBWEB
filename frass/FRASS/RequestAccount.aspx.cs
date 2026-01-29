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
	public partial class RequestAccount : System.Web.UI.Page
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
		}

		protected void Button_Submit_Click(object sender, EventArgs e)
		{
			if (!RadioButton_Agree.Checked)
			{
				Label_Message.ForeColor = System.Drawing.Color.Red;
				Label_Message.Font.Bold = true;
				Label_Message.Text = "If you do not agree, an account cannot be created.";
			}
			else
			{
				if (IsValidEmail(TextBox_Email.Text.Trim()))
				{
					var db = UserDataManager.GetInstance();
					var olduser = db.GetUser(TextBox_Email.Text.Trim());
					if (olduser == null)
					{

						MailMan mail = new MailMan();
						User user = new User();
						user.FirstName = TextBox_FirstName.Text.Trim();
						user.LastName = TextBox_LastName.Text.Trim();
						user.Email = TextBox_Email.Text.Trim();
						user.Company = TextBox_Company.Text.Trim();
						user.PhoneNumber = TextBox_PhoneNumber.Text.Trim();
						user.UserStatusID = Convert.ToInt32(UserStatusTypes.Draft);
						user.UserTypeID = Convert.ToInt32(UsersTypes.Viewer);
						user.Password = "NOT SET";

						db.AddNewUser(user, 0);
						mail.SendAccountRequest(user);
						Panel_Form.Visible = false;
						Label_Message.ForeColor = System.Drawing.Color.Green;
						Label_Message.Font.Bold = true;
						Label_Message.Text = "The Request has been submitted and you will be notified once your account has been created.";
					}
					else
					{
						Panel_Form.Visible = false;
						Label_Message.ForeColor = System.Drawing.Color.Green;
						Label_Message.Font.Bold = true;
						Label_Message.Text = "The Request has been submitted and you will be notified once your account has been created.";
					}
				}
				else
				{
					Label_Message.ForeColor = System.Drawing.Color.Red;
					Label_Message.Font.Bold = true;
					Label_Message.Text = "Invalid Email Address Entered";
				}
			}
		}
		protected void Button_Cancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("/default.aspx", true);
		}

		private bool IsValidEmail(string strIn)
		{
			// Return true if strIn is in valid e-mail format.
			return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}
	}
}