using System;
using System.Collections;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.MarketModel
{
	public partial class CPIPPI : System.Web.UI.Page
	{
		DeliveredLogMarketModelDataManager db;
		bool hadError = false;
		User thisUser;

		protected void Page_Load(object sender, EventArgs e)
		{

		}
		protected void Page_Init(object sender, EventArgs e)
		{
			db = DeliveredLogMarketModelDataManager.GetInstance();
			thisUser = Master.GetCurrentUser();
			if (thisUser.UserType.UserTypeID > 3)
			{
				RadGrid1.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
				RadGrid1.Columns[6].Visible = false;
				RadGrid1.Columns[5].Visible = false;
			}
		}
		protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
		{
			Label_Message.Text = "";
			var editableItem = ((GridEditableItem)e.Item);
			var year = (int)editableItem.GetDataKeyValue("MarketModelDataID");

			//retrive entity form the Db
			var cpippi = db.GetMarketModelDataByID(year);
			if (cpippi != null)
			{
				//update entity's state
				editableItem.UpdateValues(cpippi);

				try
				{
					//submit chanages to Db
					db.UpdateCPIPPI(cpippi);
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
			var marketModelData = new MarketModelData();
			//populate its properties
			Hashtable values = new Hashtable();
			editableItem.ExtractValues(values);
			marketModelData.Year = Convert.ToInt32(values["Year"].ToString());
			marketModelData.Period = Convert.ToInt32(values["Period"].ToString());
			marketModelData.SeriesID = (string)values["SeriesID"];
			marketModelData.Value = Convert.ToDecimal(values["Value"].ToString());
			marketModelData.MarketModelTypeID = Convert.ToInt32(values["MarketModelTypeID"].ToString());
			db.AddNewMarketModelData(marketModelData);
		}

		protected void RadGrid1_ItemDataBound(object source, GridItemEventArgs e)
		{
			if (hadError == false)
			{
				Label_Message.Text = "";
			}
		}

		protected void RadGrid1_DeleteCommand(object source, GridCommandEventArgs e)
		{
			GridDataItem dataItem = (GridDataItem)e.Item;
			var id = Convert.ToInt32(dataItem.GetDataKeyValue("MarketModelDataID").ToString());
			db.DeleteMarketModelData(id);
		}
	}
}