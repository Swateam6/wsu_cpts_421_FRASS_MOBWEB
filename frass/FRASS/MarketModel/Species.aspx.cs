using System;
using System.Collections;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.MarketModel
{
	public partial class Species : System.Web.UI.Page
	{
		bool hadError = false;
		User thisUser;
		TimberDataManager db;
		protected void Page_Load(object sender, EventArgs e)
		{

		}
		protected void Page_Init(object sender, EventArgs e)
		{
			db = TimberDataManager.GetInstance();
			thisUser = Master.GetCurrentUser();
			if (thisUser.UserType.UserTypeID >= 5)
			{
				RadGrid1.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
				RadGrid1.MasterTableView.GetColumn("EditCommandColumn").Visible = false;
			}
		}
		protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
		{
			Label_Message.Text = "";
			var editableItem = ((GridEditableItem)e.Item);
			var speciesID = (int)editableItem.GetDataKeyValue("SpeciesID");

			//retrive entity form the Db
			var species = db.GetSpecies(speciesID);
			if (species != null)
			{
				//update entity's state
				editableItem.UpdateValues(species);

				try
				{
					//submit chanages to Db
					db.UpdateSpecies(species);
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
			var species = new Specy();
			//populate its properties
			Hashtable values = new Hashtable();
			editableItem.ExtractValues(values);
			species.CommonName = (string)values["CommonName"];
			if (species.CommonName == null)
			{
				species.CommonName = "";
			}
			species.Abbreviation = (string)values["Abbreviation"];
			if (species.Abbreviation == null)
			{
				species.Abbreviation = "";
			}
			species.Latin = (string)values["Latin"];
			if (species.Latin == null)
			{
				species.Latin = "";
			}
			species.LatinAbbreviation = (string)values["LatinAbbreviation"];
			if (species.LatinAbbreviation == null)
			{
				species.LatinAbbreviation = "";
			}
			species.Taxa = (string)values["Taxa"];
			if (species.Taxa == null)
			{
				species.Taxa = "";
			}
			db.AddNewSpecies(species);
		}

		protected void RadGrid1_ItemDataBound(object source, GridItemEventArgs e)
		{
			if (hadError == false)
			{
				Label_Message.Text = "";
			}
		}
	}
}