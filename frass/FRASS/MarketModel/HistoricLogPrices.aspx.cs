using System;
using System.Collections;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;
using Telerik.Web.UI;

namespace FRASS.WebUI.MarketModel
{
	public partial class HistoricLogPrices : System.Web.UI.Page
	{
		bool hadError = false;
		User thisUser;

		protected void Page_Load(object sender, EventArgs e)
		{

		}
		protected void Page_Init(object sender, EventArgs e)
		{
			thisUser = Master.GetCurrentUser();
			if (thisUser.UserType.UserTypeID >= 5)
			{
				RadGrid1.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
				RadGrid1.MasterTableView.GetColumn("EditCommandColumn").Visible = false;
				RadGrid1.MasterTableView.GetColumn("DeleteColumn").Visible = false;
			}
		}

		protected void RadGrid1_InsertCommand(object source, GridCommandEventArgs e)
		{
			var editableItem = ((GridEditableItem)e.Item);
			
			//create new entity
			var price = new HistoricPrice();
			//populate its properties
			Hashtable values = new Hashtable();
			editableItem.ExtractValues(values);
			var year = 0;
			var month = 0;
			var speciesid = 0;
			if (Int32.TryParse(values["Year"].ToString(), out year))
			{
				price.Year = year;
			}
			else
			{
				Label_Message.Text = "Error Saving: Year Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}

			if (Int32.TryParse(values["Month"].ToString(), out month))
			{
				price.Month = month;
			}
			else
			{
				Label_Message.Text = "Error Saving: Month Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
			if (Int32.TryParse(values["LogMarketReportSpeciesID"].ToString(), out speciesid))
			{
				price.LogMarketReportSpeciesID = speciesid;
			}
			else
			{
				Label_Message.Text = "Error Saving: Species Required";
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}

			
			Decimal val;

			if (values["SMPrice"] != null)
			{
				if (Decimal.TryParse(values["SMPrice"].ToString(), out val))
				{
					price.SMPrice = val;
				}
			}
			if (values["Saw2Price"] != null)
			{
				if (Decimal.TryParse(values["Saw2Price"].ToString(), out val))
				{
					price.Saw2Price = val;
				}
			}
			if (values["Saw3Price"] != null)
			{
				if (Decimal.TryParse(values["Saw3Price"].ToString(), out val))
				{
					price.Saw3Price = val;
				}
			}
			if (values["Saw4Price"] != null)
			{
				if (Decimal.TryParse(values["Saw4Price"].ToString(), out val))
				{
					price.Saw4Price = val;
				}
			}
			if (values["Saw4CNPrice"] != null)
			{
				if (Decimal.TryParse(values["Saw4CNPrice"].ToString(), out val))
				{
					price.Saw4CNPrice = val;
				}
			}
			if (values["PulpPrice"] != null)
			{
				if (Decimal.TryParse(values["PulpPrice"].ToString(), out val))
				{
					price.PulpPrice = val;
				}
			}
			if (values["CamprunPrice"] != null)
			{
				if (Decimal.TryParse(values["CamprunPrice"].ToString(), out val))
				{
					price.CamprunPrice = val;
				}
			}
			if (values["Export12Price"] != null)
			{
				if (Decimal.TryParse(values["Export12Price"].ToString(), out val))
				{
					price.Export12Price = val;
				}
			}
			if (values["Export8Price"] != null)
			{
				if (Decimal.TryParse(values["Export8Price"].ToString(), out val))
				{
					price.Export8Price = val;
				}
			}
			if (values["ChipPrice"] != null)
			{
				if (Decimal.TryParse(values["ChipPrice"].ToString(), out val))
				{
					price.ChipPrice = val;
				}
			}
			
			TimberDataManager.GetInstance().AddHistoricLogPrice(price);
		}
		protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
		{
			Label_Message.Text = "";
			var editableItem = ((GridEditableItem)e.Item);
			var Year = (int)editableItem.GetDataKeyValue("Year");
			var Month = (int)editableItem.GetDataKeyValue("Month");
			var LogMarketReportSpeciesID = (int)editableItem.GetDataKeyValue("LogMarketReportSpeciesID");

			//retrive entity form the Db
			HistoricPrice hp = new HistoricPrice();
			hp.Year = Year;
			hp.Month = Month;
			hp.LogMarketReportSpeciesID = LogMarketReportSpeciesID;

			//update entity's state
			editableItem.UpdateValues(hp);
			try
			{
				//submit chanages to Db
				TimberDataManager.GetInstance().EditHistoricLogPrices(hp);
			}
			catch (System.Exception ex)
			{
				Label_Message.Text = "Error Saving: " + ex.Message.ToString();
				Label_Message.ForeColor = System.Drawing.Color.Red;
				hadError = true;
			}
		}
		protected void RadGrid1_DeleteCommand(object source, GridCommandEventArgs e)
		{
			var editableItem = ((GridEditableItem)e.Item);
			var Year = (int)editableItem.GetDataKeyValue("Year");
			var Month = (int)editableItem.GetDataKeyValue("Month");
			var LogMarketReportSpeciesID = (int)editableItem.GetDataKeyValue("LogMarketReportSpeciesID");

			var price = TimberDataManager.GetInstance().GetHistoricLogPrice(Year, Month, LogMarketReportSpeciesID);
			TimberDataManager.GetInstance().DeleteHistoricLogPrice(price);
			RadGrid1.DataBind();
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
		protected void RadGrid1_PreRender(object sender, EventArgs e)
		{
			foreach (var edititem in RadGrid1.MasterTableView.GetItems(GridItemType.EditItem))
			{
				if (edititem is GridDataInsertItem)
				{
					var item = (GridDataInsertItem)edititem;
					foreach (GridColumn col in RadGrid1.MasterTableView.RenderColumns)
					{
						if (col.ColumnType == "GridBoundColumn")
						{
							if (edititem.IsInEditMode)
							{
								TextBox txtbx = (TextBox)item[col.UniqueName].Controls[0];
								txtbx.Width = Unit.Pixel(50);
							}
						}
					}
				}
				else
				{
					var item = (GridDataItem)edititem;
					RadComboBox ddl = (RadComboBox)edititem.FindControl("RadComboBox_LogMarketSpeciesAbbreviations");
					ddl.Enabled = false;
					foreach (GridColumn col in RadGrid1.MasterTableView.RenderColumns)
					{
						if (col.ColumnType == "GridBoundColumn")
						{
							if (edititem.IsInEditMode)
							{
								TextBox txtbx = (TextBox)item[col.UniqueName].Controls[0];
								if (col.UniqueName == "Month" || col.UniqueName == "Year")
								{
									txtbx.Enabled = false;
								}
								txtbx.Width = Unit.Pixel(50);
							}
						}
					}
				}
			}

		}

		private class HistoricPrice : IHistoricPrice
		{
			public int Year { get; set; }
			public int Month { get; set; }
			public int LogMarketReportSpeciesID { get; set; }
			public decimal? SMPrice { get; set; }
			public decimal? Saw2Price { get; set; }
			public decimal? Saw3Price { get; set; }
			public decimal? Saw4Price { get; set; }
			public decimal? Saw4CNPrice { get; set; }
			public decimal? PulpPrice { get; set; }
			public decimal? CamprunPrice { get; set; }
			public decimal? Export12Price { get; set; }
			public decimal? Export8Price { get; set; }
			public decimal? ChipPrice { get; set; }
		}
	}
}