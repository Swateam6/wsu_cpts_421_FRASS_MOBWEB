using System;
using System.Collections;
using System.Web.UI.WebControls;
using FRASS.BLL.Mail;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.Admin
{
	public partial class Users : System.Web.UI.Page
	{
		UserDataManager db;
		bool hadError = false;
		User thisUser;
		protected void Page_Load(object sender, EventArgs e)
		{
			SendMessage.OpenerElementID = Button_SendEmail.ClientID;
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			db = UserDataManager.GetInstance();
			thisUser = Master.GetCurrentUser();
			if (thisUser.UserTypeID >= 5)
			{
				RadGrid1.DataSource = UsersDataSourceActive;
			}
			else
			{
                RadGrid1.DataSource = UsersDataSource;
            }
			if (thisUser.UserTypeID == 6)
			{
				Response.Redirect("/welcome.aspx", true);
			}
			else if (thisUser.UserTypeID == 5)
			{
				RadGrid1.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
				Button_SendEmail.Visible = false;
			}

		}
		protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
		{
			Label_Message.Text = "";
			var editableItem = ((GridEditableItem)e.Item);
			var userid = (int)editableItem.GetDataKeyValue("UserID");

			//retrive entity form the Db
			var user = db.GetUser(userid);
			if (user != null)
			{
				var statusid = user.UserStatusID;
				//update entity's state
				editableItem.UpdateValues(user);

				try
				{
					//submit chanages to Db
					db.UpdateUser(user);
					if (statusid != user.UserStatusID && user.UserStatusID == Convert.ToInt32(UserStatusTypes.Active))
					{
						var newPASS = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
						user.Password = newPASS;
						db.UpdatePassword(user);
						db.SetUserStatusTypes(user, UserStatusTypes.NeedsPasswordReset);
						db.SetUserCreatedBy(user, thisUser);
						var mail = new MailMan();
						mail.SendActivateUser(user);
						Label_Message.Text = "A New Password has been emailed to " + user.FirstName + " " + user.LastName + " at " + user.Email + ".";
						Label_Message.ForeColor = System.Drawing.Color.Green;
					}
				}
				catch (System.Exception ex)
				{
					Label_Message.Text = "Error Saving: " + ex.Message.ToString();
					Label_Message.ForeColor = System.Drawing.Color.Red;
					hadError = true;
				}
			}
		}
		protected void RadGrid1_DeleteCommand(object source, GridCommandEventArgs e)
		{
			Label_Message.Text = "";
			var editableItem = ((GridEditableItem)e.Item);
			var userid = (int)editableItem.GetDataKeyValue("UserID");
			db.DeleteUser(userid);
		}
		protected void RadGrid1_InsertCommand(object source, GridCommandEventArgs e)
		{
			var editableItem = ((GridEditableItem)e.Item);
			
			//create new entity
			var user = new User();
			//populate its properties
			Hashtable values = new Hashtable();
			editableItem.ExtractValues(values);
			user.FirstName = (string)values["FirstName"];
			user.LastName = (string)values["LastName"];
			user.Email = (string)values["Email"];
			RadComboBox rcb = editableItem.FindControl("RadComboBox_UserTypeID") as RadComboBox;
			RadComboBox rcb2 = editableItem.FindControl("RadComboBox_UserStatusID") as RadComboBox;
			user.UserTypeID = Convert.ToInt32(rcb.SelectedValue);
			user.UserStatusID = Convert.ToInt32(rcb2.SelectedValue);

			var newPASS = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
			user.Password = newPASS;
			Label_Message.Text = "";
			try
			{
				//submit chanages to Db
				db.AddNewUser(user, thisUser.UserID);
				var mail = new MailMan();
				mail.SendNewUserEmail(user);
				Label_Message.Text = "User " + user.FirstName + " " + user.LastName + " has been created an a password was sent to " + user.Email + ".";
				Label_Message.ForeColor = System.Drawing.Color.Green;
			}
			catch (System.Exception ex)
			{
				Label_Message.Text = "Error Saving: " + ex.Message.ToString();
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
				
		}

		protected void RadGrid1_ItemDataBound(object source, GridItemEventArgs e)
		{
			if (hadError == false)
			{
				//Label_Message.Text = "";
			}
			if (e.Item is GridEditableItem && e.Item.IsInEditMode)
			{
				var Button_PasswordReset = e.Item.FindControl("Button_PasswordReset") as Button;
				var RadComboBox_UserTypeID = (RadComboBox)e.Item.FindControl("RadComboBox_UserTypeID");
				GridEditableItem item = (GridEditableItem)e.Item;
				if (e.Item is GridDataInsertItem)
				{
					Button_PasswordReset.Visible = false;
					FilterUserTypes(RadComboBox_UserTypeID);
				}
				else if (e.Item is GridDataItem)
				{

					var userid = (int)item.GetDataKeyValue("UserID");
					if (thisUser.UserID == userid)
					{
						RadComboBox_UserTypeID.Enabled = false;
						RadComboBox_UserTypeID.Skin = "Web20";
						RadComboBox_UserTypeID.ForeColor = System.Drawing.Color.Black;

						var RadComboBox_UserStatusID = (RadComboBox)e.Item.FindControl("RadComboBox_UserStatusID");
						RadComboBox_UserStatusID.Enabled = false;
						RadComboBox_UserStatusID.Skin = "Web20";
						RadComboBox_UserStatusID.ForeColor = System.Drawing.Color.Black;
					}
					Button_PasswordReset.CommandArgument = userid.ToString();
					FilterUserTypes(RadComboBox_UserTypeID);
				}
			}
			else if (e.Item is GridDataItem)
			{
				
				var item = (GridDataItem)e.Item;
				var userid = (int)item.GetDataKeyValue("UserID");
				var userTypeID = (int)item.GetDataKeyValue("UserTypeID");
				if (thisUser.UserTypeID != 1)
				{
					RadGrid1.Columns[9].Visible = false;
					RadGrid1.Columns[4].Visible = false;
					RadGrid1.Columns[3].Visible = false;
				}
				if (thisUser.UserTypeID >= 5)
				{
					(item["EditCommandColumn"].Controls[0] as LinkButton).Visible = (thisUser.UserID == userid);
				}
				else if (thisUser.UserTypeID > 1)
				{
					if (userTypeID == 1)
					{
						(item["EditCommandColumn"].Controls[0] as LinkButton).Visible = false;
					}
					else if (userTypeID == 2 && thisUser.UserTypeID > 2)
					{
						(item["EditCommandColumn"].Controls[0] as LinkButton).Visible = false;
					}
				}		
			}
		}
		private void FilterUserTypes(RadComboBox RadComboBox_UserTypeID)
		{
			if (thisUser.UserTypeID > 1)
			{
				var su = RadComboBox_UserTypeID.FindItemByValue("1");
				if (su != null)
				{
					RadComboBox_UserTypeID.Items.Remove(su.Index);
				}
			}
			if (thisUser.UserTypeID > 2)
			{
				var au = RadComboBox_UserTypeID.FindItemByValue("2");
				if (au != null)
				{
					RadComboBox_UserTypeID.Items.Remove(au.Index);
				}
			}
		}
		protected void Button_PasswordReset_Click(object source, CommandEventArgs e)
		{
			var user = db.GetUser(Convert.ToInt32(e.CommandArgument.ToString()));
			var newPASS = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
			user.Password = newPASS;
			db.UpdatePassword(user);
			db.SetUserStatusTypes(user, UserStatusTypes.NeedsPasswordReset);
			var mail = new MailMan();
			mail.SendPasswordReset(user);
			Label_Message.Text = "A New Password has been emailed to " + user.FirstName + " " + user.LastName + " at " + user.Email + ".";
			Label_Message.ForeColor = System.Drawing.Color.Green;
		}

	}
}