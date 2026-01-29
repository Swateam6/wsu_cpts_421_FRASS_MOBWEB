using System;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.Parcels
{
	public partial class Parcels : System.Web.UI.Page
	{
		protected void Button_ExportExcel_Click(object sender, EventArgs e)
		{
			ConfigureExport();
			RadGrid1.MasterTableView.ExportToExcel();
		}
		protected void Button_ExportWord_Click(object sender, EventArgs e)
		{
			ConfigureExport();
			RadGrid1.MasterTableView.ExportToWord();
		}
		private void ConfigureExport()
		{
			RadGrid1.ExportSettings.ExportOnlyData = true;
			RadGrid1.ExportSettings.IgnorePaging = true;
			RadGrid1.ExportSettings.OpenInNewWindow = true;
		}
		protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			var list = ParcelDataManager.GetInstance().GetParcelListingItems();
			RadGrid1.DataSource = list;
		}
	}
}