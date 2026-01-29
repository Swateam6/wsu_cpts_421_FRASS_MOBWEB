using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS
{
	public partial class Site : System.Web.UI.MasterPage
	{
		User thisUser;
		bool dontValidate = false;
		protected void Page_Load(object sender, EventArgs e)
		{
			RadMenu1.Items[7].Items[3].Visible = false;
			RadMenu1.Items[6].Visible = false;
			if (dontValidate == false)
			{
				ValidateUser();
				if (!Page.IsPostBack)
				{
					if (thisUser.UserTypeID > 2)
					{
						RadMenu1.Items[10].Visible = false;
					}
					if (thisUser.UserTypeID == 6)
					{
						RadMenu1.Items[7].Items[4].Visible = false;
						RadMenu1.Items[7].Items[1].Visible = false;
						RadMenu1.Items[10].Visible = false;
						RadMenu1.Items[9].Visible = false;
					}
				}
			}
		}
		protected void Page_Init(object sender, EventArgs e)
		{

		}
		public void DontValidate()
		{
			dontValidate = true;
		}
		public void SetMenuBarVisibility(bool isVisible)
		{
			RadMenu1.Visible = isVisible;
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
