using FRASS.DAL.DataManager;
using Telerik.Web.UI;

namespace FRASS.WebUI.Owners
{
	public partial class Allottees : System.Web.UI.Page
	{
		protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			OwnerDataManager db = OwnerDataManager.GetInstance();
			var list = db.GetAllottees();
			RadGrid1.DataSource = list;
		}
	}
}