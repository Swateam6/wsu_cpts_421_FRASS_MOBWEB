using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.UI;
using FRASS.BLL.Mail;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.WebUI.Popups
{
    public partial class SharePortfolio : System.Web.UI.Page
    {
        User thisUser;
        DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
        RPAPortfolioDataManager dbRPAPortfolioDataManager;
        UserDataManager dbUserDataManager;

        // Use ViewState to persist values across postbacks
        private int PortfolioID
        {
            get { return ViewState["PortfolioID"] != null ? (int)ViewState["PortfolioID"] : 0; }
            set { ViewState["PortfolioID"] = value; }
        }

        private string PortfolioType
        {
            get { return ViewState["PortfolioType"] as string; }
            set { ViewState["PortfolioType"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
            dbRPAPortfolioDataManager = RPAPortfolioDataManager.GetInstance();
            dbUserDataManager = UserDataManager.GetInstance();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidateUser();

            if (thisUser == null)
            {
                TriggerClose();
                return;
            }

            if (!Page.IsPostBack)
            {
                // Parse query string only on initial load
                string typeParam = Request.QueryString["type"];
                string idParam = Request.QueryString["id"];

                if (typeParam == "1" || typeParam == "2")
                {
                    PortfolioType = typeParam;
                    int parsedId;
                    if (!string.IsNullOrEmpty(idParam) && Int32.TryParse(idParam, out parsedId))
                    {
                        PortfolioID = parsedId;

                        if (PortfolioType == "1")
                        {
                            LoadMarketModel();
                        }
                        else if (PortfolioType == "2")
                        {
                            LoadRPAModel();
                        }
                    }
                    else
                    {
                        TriggerClose();
                    }
                }
                else
                {
                    TriggerClose();
                }
            }
        }

        private void LoadMarketModel()
        {
            var portfolio = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(PortfolioID);
            if (portfolio == null)
            {
                TriggerClose();
                return;
            }
            var users = dbUserDataManager.GetUsers().Where(uu => uu.UserID != thisUser.UserID && uu.UserStatusID == 1);
            RadListBox_Shared.Items.Clear();
            foreach (var user in users.OrderBy(uu => uu.FirstName).ThenBy(uu => uu.LastName))
            {
                var item = new Telerik.Web.UI.RadListBoxItem(user.FirstName + " " + user.LastName, user.UserID.ToString());
                RadListBox_Shared.Items.Add(item);
            }
            Page.Title = "Share Market Model Portfolio: " + portfolio.PortfolioName;
        }
        private void LoadRPAModel()
        {
            var portfolio = dbRPAPortfolioDataManager.GetRPAPortfolio(PortfolioID);
            if (portfolio == null)
            {
                TriggerClose();
                return;
            }
            var users = dbUserDataManager.GetUsers().Where(uu => uu.UserID != thisUser.UserID && uu.UserStatusID == 1);
            RadListBox_Shared.Items.Clear();
            foreach (var user in users.OrderBy(uu => uu.FirstName).ThenBy(uu => uu.LastName))
            {
                var item = new Telerik.Web.UI.RadListBoxItem(user.FirstName + " " + user.LastName, user.UserID.ToString());
                RadListBox_Shared.Items.Add(item);
            }
            Page.Title = "RPA Portfolio: " + portfolio.PortfolioName;
        }

        protected void Button_Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (thisUser == null)
                {
                    Label_Message.Text = "Session expired. Please close and try again.";
                    return;
                }

                if (PortfolioType == "1")
                {
                    SaveMarketModelShares();
                }
                else if (PortfolioType == "2")
                {
                    SaveRPAPortfolioShares();
                }

                // Clear the selected items after successful share
                foreach (Telerik.Web.UI.RadListBoxItem item in RadListBox_Shared.Items)
                {
                    item.Checked = false;
                }

                Label_Message.Text = "Model Shared";
            }
            catch (Exception ex)
            {
                Label_Message.Text = "Error: " + ex.Message;
            }
        }

        private void SaveMarketModelShares()
        {
            var portfolio = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(PortfolioID);
            if (portfolio == null) return;

            foreach (var item in RadListBox_Shared.CheckedItems)
            {
                try
                {
                    var userID = Convert.ToInt32(item.Value);
                    dbDeliveredLogMarketModelDataManager.CopyModel(PortfolioID, userID);
                    SendNotifications(userID, "Delivered Log Market Portfolio - " + portfolio.PortfolioName);
                }
                catch
                {
                    // Continue with other recipients if one fails
                }
            }
        }
        private void SaveRPAPortfolioShares()
        {
            var portfolio = dbRPAPortfolioDataManager.GetRPAPortfolio(PortfolioID);
            if (portfolio == null) return;

            foreach (var item in RadListBox_Shared.CheckedItems)
            {
                try
                {
                    var userID = Convert.ToInt32(item.Value);
                    dbRPAPortfolioDataManager.CopyModel(PortfolioID, userID);
                    SendNotifications(userID, "RPA Portfolio - " + portfolio.PortfolioName);
                }
                catch
                {
                    // Continue with other recipients if one fails
                }
            }
        }

        protected void Button_Cancel_Click(object sender, EventArgs e)
        {
            TriggerClose();
        }
        private void SendNotifications(int userId, string title)
        {
            if (thisUser == null) return;

            var user = dbUserDataManager.GetUser(userId);
            if (user == null || string.IsNullOrEmpty(user.Email)) return;

            var fromAddress = new MailAddress("admin@resource-analysis.com");
            var mail = new MailMan();
            var message = new MailMessage();
            message.Subject = title;
            message.Body = thisUser.FirstName + " " + thisUser.LastName + " has just shared the " + title + " with you.";
            message.To.Add(user.Email);
            message.From = fromAddress;
            mail.SendMail(message);
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
                            TriggerClose();
                        }
                        else
                        {
                            User u = dbUserDataManager.GetUser(hash, userid);
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
                                TriggerClose();
                            }
                        }
                    }
                    else
                    {
                        TriggerClose();
                    }
                }
                else
                {
                    TriggerClose();
                }
            }
            else
            {
                thisUser = (User)Session["User"];
            }
        }
        private void TriggerClose()
        {
            string script = "cancelClick();";
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "script", script, true);
        }
    }
}