using System;
using System.IO;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.WebUI.PDFs
{
	public partial class StumpageModelValueReport : System.Web.UI.Page
	{
		User thisUser;
		string fileName = "";

		protected void Page_Load(object sender, EventArgs e)
		{
			thisUser = GetCurrentUser();
			var report = GetReport();

			Response.Clear();
			Response.ClearHeaders();
			Response.ClearContent();
			Response.Charset = "UTF-8";
			Response.AddHeader("Content-Length", report.Length.ToString());
			Response.AddHeader("content-disposition", String.Format("attachment;filename={0}.pdf", fileName));
			Response.ContentType = "application/pdf";
			Response.BinaryWrite(report.ToArray());
			Response.Flush();
			Response.End();
		}

		private MemoryStream GetReport()
		{
			var parcelID = Convert.ToInt32(Request.QueryString["ParcelID"].ToString());
			var portfolioID = Convert.ToInt32(Request.QueryString["StumpageModelPortfolioID"].ToString());
			var baseFilePath = Server.MapPath("/");
			var db = ParcelDataManager.GetInstance();
			var dbPortfolio = StumpageMarketModelDataManager.GetInstance();
			var timberstats = db.GetReportTimberStatistics(parcelID, System.DateTime.Now.Year);
			var report = new FRASS.Reports.StumpageMarketValueReport(thisUser, db.GetParcel(parcelID), dbPortfolio.GetStumpageModelPortfolio(portfolioID), db.GetCurrentParcelTimberSummary(parcelID, System.DateTime.Now.Year), timberstats, baseFilePath);
			fileName = "Parcel_" + report.Parcel.ParcelNumber + "_Portfolio_" + report.Portfolio.PortfolioName;
			return report.GetReport();
		}

		private void ValidateUser()
		{
			if (Session["User"] == null)
			{
				if (Request.Cookies["user"] != null && Request.Cookies["userid"] != null)
				{
					string hash = Request.Cookies["user"].Value;
					string useridString = Request.Cookies["userid"].Value;
					Int32 userid = 0;
					if (Int32.TryParse(useridString, out userid))
					{

						if (hash.Length == 0 || userid == 0)
						{
							Response.Redirect("/default.aspx?type=3", true);
						}
						else
						{
							User u = UserDataManager.GetInstance().GetUser(hash, userid);
							if (u != null)
							{
								Session["User"] = u;
								Response.Cookies["user"].Value = u.Hash;
								Response.Cookies["user"].Expires = System.DateTime.Now.AddHours(2);
								Response.Cookies["userid"].Value = u.UserID.ToString();
								Response.Cookies["userid"].Expires = System.DateTime.Now.AddHours(2);
								thisUser = u;
							}
							else
							{
								Session["User"] = null;
								Response.Cookies["user"].Value = "";
								Response.Cookies["userid"].Value = "";
								Response.Redirect("/default.aspx?type=2", true);
							}
						}
					}
					else
					{
						Response.Redirect("/default.aspx?type=1", true);
					}
				}
				else
				{
					Response.Redirect("/default.aspx?type=1", true);
				}
			}
			else
			{
				thisUser = (User)Session["User"];
			}
		}
		public User GetCurrentUser()
		{
			if (thisUser == null)
			{
				ValidateUser();
			}
			return thisUser;
		}
	}
}