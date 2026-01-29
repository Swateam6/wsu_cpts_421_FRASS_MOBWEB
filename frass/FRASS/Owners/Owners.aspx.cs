using System;
using System.Collections;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.Owners
{
	public partial class Owners : System.Web.UI.Page
	{
		OwnerDataManager db;
		Boolean hadError;
		User thisUser;
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Page_Init(object sender, EventArgs e)
		{
			db = OwnerDataManager.GetInstance();
			thisUser = Master.GetCurrentUser();
			if (thisUser.UserTypeID >= 5)
			{
				RadGrid1.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
				RadGrid1.MasterTableView.Columns[6].Visible = false;
			}
		}

		protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
		{
			Label_Message.Text = "";
			var editableItem = ((GridEditableItem)e.Item);
			var ownerid = (int)editableItem.GetDataKeyValue("OwnerID");

			//retrive entity form the Db
			var owner = db.GetOwner(ownerid);
			if (owner != null)
			{
				//update entity's state
				editableItem.UpdateValues(owner);

				try
				{
					//submit chanages to Db
					db.UpdateOwner(owner);
				}
				catch (System.Exception ex)
				{
					Label_Message.Text = "Error Saving: " + ex.Message.ToString();
					Label_Message.ForeColor = System.Drawing.Color.Red;
					hadError = true;
				}
			}
		}

		protected void RadGrid1_InsertCommand(object source, GridCommandEventArgs e)
		{
			var editableItem = ((GridEditableItem)e.Item);

			//create new entity
			var owner = new Owner();
			//populate its properties
			Hashtable values = new Hashtable();
			editableItem.ExtractValues(values);
			owner.Address = (string)values["Address"];
			owner.City = (string)values["City"];
			owner.Name = (string)values["Name"];
			owner.Zip = (string)values["Zip"];
			owner.Zip4 = (string)values["Zip4"];
			RadComboBox rcb = editableItem.FindControl("RadComboBox_States") as RadComboBox;
			owner.StateID = Convert.ToInt32(rcb.SelectedValue);
			Label_Message.Text = "";
			try
			{
				//submit chanages to Db
				db.AddNewOwner(owner);
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
				Label_Message.Text = "";
			}
		}

		protected void ParcelsDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
		{
			e.Result = db.GetParcelOwnerInfo();

		}
	}
}