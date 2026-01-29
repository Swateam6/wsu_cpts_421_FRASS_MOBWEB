using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.WebUI.Reports
{
	public partial class MyReports : System.Web.UI.Page
	{
		User thisUser;
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadData();
			} 
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			thisUser = Master.GetCurrentUser();
			if (!Page.IsPostBack)
			{
				ReportDataManager.GetInstance().ClearOldReports();
			}
		}

		protected void RadGrid1_ItemDataBound(object source, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				var item = (GridDataItem)e.Item;
				var report = (DataRowView)e.Item.DataItem;
				if (report.Row.ItemArray[5].ToString().ToLower() == "false")
				{
					((HyperLink)item["HyperLink_Title"].Controls[0]).Enabled = false;
				}
			}
		}

		protected void RadGrid1_DeleteCommand(object source, GridCommandEventArgs e)
		{
			GridDataItem dataItem = (GridDataItem)e.Item;
			var id = Convert.ToInt32(dataItem.GetDataKeyValue("ReportID").ToString());
			ReportDataManager.GetInstance().DeleteReport(id);
			LoadData();
			RadGrid1.DataBind();
		}

		private void LoadData()
		{
			RadGrid1.DataSource = GetDataTable(string.Format(@"
				select r.ReportID, r.Title, p.ParcelNumber, rt.Type, r.DateCreated, r.Completed
				From Reports r
				inner join Parcels p on r.ParcelID = p.ParcelID
				inner join ReportTypes rt on r.ReportTypeID = rt.ReportTypeID
				where r.UserID = {0}",thisUser.UserID.ToString()));
		}

		public DataTable GetDataTable(string query)
		{
			var ConnString = ConfigurationManager.ConnectionStrings["FRASS.DAL.Properties.Settings.FRASSConnectionString"].ConnectionString;
			var conn = new SqlConnection(ConnString);
			var adapter = new SqlDataAdapter();
			adapter.SelectCommand = new SqlCommand(query, conn);

			var myDataTable = new DataTable();

			conn.Open();
			try
			{
				adapter.Fill(myDataTable);
			}
			finally
			{
				conn.Close();
			}

			return myDataTable;
		}
	}
}