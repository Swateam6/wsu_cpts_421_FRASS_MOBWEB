using System;
using System.IO;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.WebUI.PDFs
{
	public partial class ParcelReportFull : System.Web.UI.Page
	{
		User thisUser;
		string fileName = "";

		protected void Page_Load(object sender, EventArgs e)
		{
			var reportID = Convert.ToInt32(Request.QueryString["ReportID"].ToString());
			var db = ReportDataManager.GetInstance();
			var report = db.GetReport(reportID);
			if (report.Completed)
			{
				var buffer = (byte[])report.PDF.ToArray();
				var docStream = new MemoryStream(buffer);

				thisUser = GetCurrentUser();
				fileName = report.Title.Replace(" ", "_") + ".pdf";
				Response.Clear();
				Response.ClearHeaders();
				Response.ClearContent();
				Response.Charset = "UTF-8";
				Response.AddHeader("Content-Length", docStream.Length.ToString());
				Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName));
				Response.ContentType = "application/pdf";
				Response.BinaryWrite(docStream.ToArray());
				Response.Flush();
				Response.End();
			}
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