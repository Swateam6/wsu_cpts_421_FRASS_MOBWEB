using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FRASS
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string msg = Request.QueryString["msg"];
            if (!string.IsNullOrEmpty(msg))
            {
                Label_ErrorMessage.Text = Server.HtmlEncode(msg);
            }
            else
            {
                Label_ErrorMessage.Text = "An unexpected error occurred.";
            }
        }
    }
}
