using System;
using System.Collections;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.MarketModel
{
	public partial class HistoricStumpagePrices : System.Web.UI.Page
	{
		StumpageMarketModelDataManager db;
		bool hadError = false;
		User thisUser;

		protected void Page_Load(object sender, EventArgs e)
		{
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			db = StumpageMarketModelDataManager.GetInstance();
			thisUser = Master.GetCurrentUser();
			if (thisUser.UserType.UserTypeID >= 5)
			{
				RadGrid1.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
				RadGrid1.MasterTableView.GetColumn("EditCommandColumn").Visible = false;
				RadGrid1.MasterTableView.GetColumn("DeleteColumn").Visible = false;
			}
		}
		protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
		{
			var editableItem = ((GridEditableItem)e.Item);
			var stumpagePricesID = (int)editableItem.GetDataKeyValue("StumpagePricesID");
			var price = db.GetStumpagePrice(stumpagePricesID);
			Hashtable values = new Hashtable();
			editableItem.ExtractValues(values);

			price = GetFormData(values, price);
			if (!hadError)
			{
				db.UpdateStumpagePrice(price);
				RadGrid1.DataBind();
			}
		}

		protected void RadGrid1_DeleteCommand(object source, GridCommandEventArgs e)
		{
			var editableItem = ((GridEditableItem)e.Item);
			var stumpagePricesID = (int)editableItem.GetDataKeyValue("StumpagePricesID");
			
			var price = db.GetStumpagePrice(stumpagePricesID);
			db.DeleteStumpagePrice(price);
			RadGrid1.DataBind();
		}

		protected void RadGrid1_InsertCommand(object source, GridCommandEventArgs e)
		{
			var editableItem = ((GridEditableItem)e.Item);
			var price = new StumpagePrice();
			Hashtable values = new Hashtable();
			editableItem.ExtractValues(values);
			
			price = GetFormData(values, price);
			

			if (!hadError)
			{
				db.AddStumpagePrice(price);
				RadGrid1.DataBind();
			}
			

		}

		private StumpagePrice GetFormData(Hashtable values, StumpagePrice price)
		{
			var marketdate = new DateTime();
			var timberquality = 0;
			var zone1 = 0M;
			var zone2 = 0M;
			var zone3 = 0M;
			var zone4 = 0M;
			var zone5 = 0M;
			price.SpeciesID = Convert.ToInt32(values["SpeciesID"].ToString());
			if (values["MarketDate"] != null && DateTime.TryParse(values["MarketDate"].ToString(), out marketdate))
			{
				price.MarketDate = marketdate;
				if ((marketdate.Month == 1 || marketdate.Month == 7) == false)
				{
					Label_Message.Text = "Error Saving: Market Date must be in January or July";
					Label_Message.ForeColor = System.Drawing.Color.Red;
					hadError = true;
				}
			}
			else
			{
				Label_Message.Text = "Error Saving: Market Date must be in January or July";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			if (values["TimberQualityCode"] != null && Int32.TryParse(values["TimberQualityCode"].ToString(), out timberquality))
			{
				price.TimberQualityCode = timberquality;
			}
			if (price.TimberQualityCode < 1 || price.TimberQualityCode > 4)
			{
				Label_Message.Text = "Error Saving: Invalid Timber Quality Code";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			if (values["HaulingZone1"] != null)
			{
				if (Decimal.TryParse(values["HaulingZone1"].ToString(), out zone1))
				{
					price.HaulingZone1 = zone1;
				}
			}
			if (price.HaulingZone1 == 0)
			{
				Label_Message.Text = "Error Saving: Price for Hauling Zone 1 Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			if (values["HaulingZone2"] != null)
			{
				if (Decimal.TryParse(values["HaulingZone2"].ToString(), out zone2))
				{
					price.HaulingZone2 = zone2;
				}
			}
			if (price.HaulingZone2 == 0)
			{
				Label_Message.Text = "Error Saving: Price for Hauling Zone 2 Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			if (values["HaulingZone3"] != null)
			{
				if (Decimal.TryParse(values["HaulingZone3"].ToString(), out zone3))
				{
					price.HaulingZone3 = zone3;
				}
			}
			if (price.HaulingZone3 == 0)
			{
				Label_Message.Text = "Error Saving: Price for Hauling Zone 3 Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			if (values["HaulingZone4"] != null)
			{
				if (Decimal.TryParse(values["HaulingZone4"].ToString(), out zone4))
				{
					price.HaulingZone4 = zone4;
				}
			}
			if (price.HaulingZone4 == 0)
			{
				Label_Message.Text = "Error Saving: Price for Hauling Zone 4 Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			if (values["HaulingZone5"] != null)
			{
				if (Decimal.TryParse(values["HaulingZone5"].ToString(), out zone5))
				{
					price.HaulingZone5 = zone5;
				}
			}
			if (price.HaulingZone5 == 0)
			{
				Label_Message.Text = "Error Saving: Price for Hauling Zone 5 Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			return price;
		}

		protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
		{
			RadGrid1.DataSource = db.GetStumpagePrices();
		}
		protected void RadGrid1_ItemDataBound(object source, GridItemEventArgs e)
		{
			if (hadError == false)
			{
				Label_Message.Text = "";
			}
			if ((e.Item is GridDataInsertItem && e.Item.OwnerTableView.IsItemInserted) || (e.Item is GridEditFormItem && e.Item.IsInEditMode))
			{
				RadGrid1.MasterTableView.GetColumn("EditCommandColumn").Visible = true;
				foreach (GridDataItem dataItem in RadGrid1.MasterTableView.Items)
				{
					((LinkButton)dataItem["EditCommandColumn"].Controls[0]).Visible = false;
				}
			}
			else
			{
				RadGrid1.MasterTableView.GetColumn("EditCommandColumn").Visible = thisUser.UserType.UserTypeID != 5;			
			} 
		}

		private string GetStumpageReportDateValue(DateTime date)
		{
			var stumpageReportValue = "First Half ";
			if (date.Month == 7)
			{
				stumpageReportValue = "Second Half ";
			}
			return stumpageReportValue + date.Year.ToString();
		}
	}
}