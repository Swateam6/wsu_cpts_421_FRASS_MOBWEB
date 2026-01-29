using System;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.Admin
{
	public partial class SiteSettings : System.Web.UI.Page
	{
		SiteDataManager db;
		bool hadError = false;
		User thisUser;
		protected void Page_Load(object sender, EventArgs e)
		{

		}
		protected void Page_Init(object sender, EventArgs e)
		{
			db = SiteDataManager.GetInstance();
			thisUser = Master.GetCurrentUser();
			if (thisUser.UserTypeID > 2)
			{
				Response.Redirect("/welcome.aspx", true);
			}
		}
		protected void RadGrid1_UpdateCommand(object source, GridCommandEventArgs e)
		{
			Label_Message.Text = "";
			var editableItem = ((GridEditableItem)e.Item);
			var siteSettingID = (int)editableItem.GetDataKeyValue("SiteSettingID");
			SiteSetting ss = db.GetSiteSetting(siteSettingID);
			editableItem.UpdateValues(ss);
			db.UpdateSiteSettings(ss);
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