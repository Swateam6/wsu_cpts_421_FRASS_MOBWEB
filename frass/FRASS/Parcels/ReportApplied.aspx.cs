using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Formulas;
using FRASS.BLL.Models;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;
using FRASS.Services.Proxies;
using FRASS.Services.Proxies.ReportService;
using FRASS.Services.ReportService;

namespace FRASS.WebUI.Parcels
{
    public partial class ReportApplied : System.Web.UI.Page
    {
        DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
        StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
        ParcelDataManager dbParcelDataManager;
        RPAPortfolioDataManager dbRPADataManager;

        Int32 parcelID;
        Int32 portfolioID;
        Int32 rpaPortfolioID;
        User thisUser;
        LogMarketReportSpecy specy;
        MarketModelPortfolio portfolio;
        RPAPortfolio rpaPortfolio;
        TimberMarket timberMarket;
        private DeltaNM _DeltaNM;
        private DeltaNM DeltaNM
        {
            get { return _DeltaNM ?? (_DeltaNM = new DeltaNM()); }
        }
        private RPARealValue _rpaREAL;
        private RPARealValue RPAREAL
        {
            get { return _rpaREAL ?? (_rpaREAL = new RPARealValue()); }
        }
        private List<MarketModelData> _mmd;
        private List<MarketModelData> MarketModelDataList
        {
            get { return _mmd ?? (_mmd = dbDeliveredLogMarketModelDataManager.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).ToList<MarketModelData>()); }
        }
        private decimal? _ppiToday;
        private decimal ppiToday
        {
            get
            {
                if (!_ppiToday.HasValue)
                {
                    _ppiToday = MarketModelDataList.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault().Value;
                }
                return _ppiToday.Value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(Button_ExportPDF);
            if (!Page.IsPostBack)
            {
                LoadPage();
            }

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
            dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
            dbParcelDataManager = ParcelDataManager.GetInstance();
            dbRPADataManager = RPAPortfolioDataManager.GetInstance();

            thisUser = Master.GetCurrentUser();
            Int32 tmpInt;
            if (Int32.TryParse(Request.QueryString["ParcelID"].ToString(), out tmpInt))
            {
                parcelID = tmpInt;
            }
            else
            {
                Server.Transfer("/Parcels/Parcels.aspx");
            }
            if (Int32.TryParse(Request.QueryString["MarketModelPortfolioID"].ToString(), out tmpInt))
            {
                portfolioID = tmpInt;
            }
            else
            {
                Server.Transfer("/Parcels/Parcels.aspx");
            }
            if (Int32.TryParse(Request.QueryString["RPAPortfolioID"].ToString(), out tmpInt))
            {
                rpaPortfolioID = tmpInt;
            }
            else
            {
                Server.Transfer("/Parcels/Parcels.aspx");
            }
        }
        protected void Button_ExportToPDF_Click(object sender, EventArgs e)
        {
            Response.Redirect("/PDFs/MarketModelReportApplied.aspx?ParcelID=" + parcelID.ToString() + "&MarketModelPortfolioID=" + portfolioID.ToString() + "&RPAPortfolioID=" + rpaPortfolioID.ToString(), true);
        }
        protected void Button_ExportFullReportToPDF_Click(object sender, EventArgs e)
        {
            try
            {
                var reportService = new ReportService();
                //var proxy = new ReportServiceProxy();
                reportService.GenerateFullParcelReportDeliveredMarketModel(thisUser.UserID, parcelID, portfolioID, rpaPortfolioID);
                //proxy.GenerateFullParcelReportDeliveredMarketModel(thisUser.UserID, parcelID, rpaPortfolioID);
            }
            catch (Exception ex)
            {
                var user = (User)Session["User"];
                var UserID = user.UserID;
                var str = "Error in: " + Request.Url.ToString() + "<br/><br/>Exception: " + ex.GetBaseException().Message + "<br/><br/>Stack Trace: " + ex.GetBaseException().StackTrace;
                SiteDataManager.GetInstance().AddLog(DatabaseIDs.LogTypes.Error, UserID, str);
            }

        }
        private void LoadPage()
        {
            portfolio = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(portfolioID);
            rpaPortfolio = dbRPADataManager.GetRPAPortfolio(rpaPortfolioID);
            Label_RPAPortfolioName.Text = rpaPortfolio.PortfolioName;

            var econ = new EconVariables(portfolio, rpaPortfolio);
            Label_MarketModelName.Text = portfolio.PortfolioName;

            Label_RateOfInflation.Text = (100 * econ.RateOfInflation).ToString("N2") + "%";
            Label_LandownerDiscountRate.Text = (100 * econ.RealDiscount).ToString("N2") + "%";
            Label_ReforestationCost.Text = econ.ReforestionCosts.ToString("C0") + "/Acre";
            Label_AccessFee.Text = econ.AccessFee.ToString("C2") + "/MBF/Mile";
            Label_MaintenanceFee.Text = econ.MaintenanceFee.ToString("C2") + "/MBF/Mile";
            Label_NewLoggingRoadConstruction.Text = econ.NewRoad.ToString("C0") + "/Mile";

            var deliveredLogModels = portfolio.MarketModelPortfolioDeliveredLogModelDetails;
            Repeater_Sorts.DataSource = deliveredLogModels.Select(uu => uu.LogMarketReportSpecy).Distinct();
            Repeater_Sorts.DataBind();

            if (!Page.IsPostBack)
            {
                LoadStands();
                LoadModels();
            }
        }
        private void LoadStands()
        {
            var parcel = dbParcelDataManager.GetParcel(parcelID);
            var stands = parcel.ParcelRiparians.Where(uu => uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.STD_ID).ToList<int>();
            RadComboBox_Stands.DataSource = stands.Distinct().OrderBy(uu => uu);
            RadComboBox_Stands.DataBind();
        }
        private void LoadModels()
        {
            var marketModels = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolios(thisUser);
            if (marketModels.Count() > 0)
            {
                DropDownList_MarketModels.Visible = true;
                Button_ApplyMarketModel.Visible = true;
                DropDownList_MarketModels.Items.Clear();
                foreach (var model in marketModels.OrderBy(uu => uu.PortfolioName))
                {
                    DropDownList_MarketModels.Items.Add(new ListItem(model.PortfolioName, model.ModelID.ToString()));
                }
                DropDownList_MarketModels.SelectedValue = portfolioID.ToString();
            }

            var stumpageModels = dbStumpageMarketModelDataManager.GetStumpageModelPortfolios(thisUser);
            if (stumpageModels.Count() > 0)
            {
                DropDownList_StumpageModels.Visible = true;
                Button_ApplyStumpageModels.Visible = true;
                DropDownList_StumpageModels.Items.Clear();
                DropDownList_StumpageModels.Items.Add(new ListItem("--- Stumpage Market Portfolios ---", "0"));
                foreach (var model in stumpageModels.OrderBy(uu => uu.PortfolioName))
                {
                    DropDownList_StumpageModels.Items.Add(new ListItem(model.PortfolioName, model.StumpageModelPortfolioID.ToString()));
                }
            }

            var rpaPortfolios = dbRPADataManager.GetRPAPortfolios(thisUser);
            if (rpaPortfolios.Count > 0)
            {
                DropDownList_RPA.Visible = true;
                DropDownList_RPA.Items.Clear();
                DropDownList_RPA.Items.Add(new ListItem("--- RPA Portfolios ---", "0"));
                DropDownList_RPA.Items.Add(new ListItem("No RPA Portfolio", "-1"));
                foreach (var model in rpaPortfolios.OrderBy(uu => uu.PortfolioName))
                {
                    DropDownList_RPA.Items.Add(new ListItem(model.PortfolioName, model.ModelID.ToString()));
                }
                DropDownList_RPA.SelectedValue = rpaPortfolioID.ToString();
            }
        }

        protected void Button_ViewMarketValueReport_Click(object sender, EventArgs e)
        {
            var url = string.Format("/Parcels/MarketValueReport.aspx?ParcelID={0}&MarketModelPortfolioID={1}&RPAPortfolioID={2}", parcelID.ToString(), portfolioID.ToString(), rpaPortfolioID.ToString());
            Response.Redirect(url, true);
        }
        protected void Button_ApplyStumpageModel_Click(object sender, EventArgs e)
        {
            if (DropDownList_StumpageModels.SelectedValue != "0")
            {
                var url = string.Format("/Parcels/StumpageApplied.aspx?ParcelID={0}&StumpageModelPortfolioID={1}", parcelID, DropDownList_StumpageModels.SelectedValue);
                Response.Redirect(url, true);
            }
        }
        protected void Button_ApplyMarketModel_Click(object sender, EventArgs e)
        {
            if (DropDownList_MarketModels.SelectedValue != "0" && DropDownList_RPA.SelectedValue != "0")
            {
                var url = string.Format("/Parcels/ReportApplied.aspx?ParcelID={0}&MarketModelPortfolioID={1}&RPAPortfolioID={2}", parcelID, DropDownList_MarketModels.SelectedValue, DropDownList_RPA.SelectedValue);
                Response.Redirect(url, true);
            }
        }
        protected void Repeater_Sorts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            specy = (LogMarketReportSpecy)e.Item.DataItem;
            var HyperLink_Sort = e.Item.FindControl("HyperLink_Sort") as HyperLink;
            HyperLink_Sort.Text = specy.LogMarketSpecies;
            HyperLink_Sort.Attributes.Add("onclick", "radopen(\"/Parcels/Charts/SortValuesChart.aspx?RPAPortfolioID=" + rpaPortfolioID + "&MarketModelPortfolioID=" + portfolio.MarketModelPortfolioID.ToString() + "&LogMarketReportSpeciesID=" + specy.LogMarketReportSpeciesID + "\",\"RadWindow1\"); return false;");

            var Repeater_Vals = e.Item.FindControl("Repeater_Vals") as Repeater;
            Repeater_Vals.DataSource = rpaPortfolio.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == specy.LogMarketReportSpeciesID).Select(uu => uu.TimberMarket).OrderBy(uu => uu.OrderID);
            Repeater_Vals.DataBind();
        }
        protected void Repeater_Vals_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var Label_Sort = e.Item.FindControl("Label_Sort") as Label;
            var Label_MarketValue = e.Item.FindControl("Label_MarketValue") as Label;
            var Label_RPA = e.Item.FindControl("Label_RPA") as Label;
            var Label_Longevity = e.Item.FindControl("Label_Longevity") as Label;
            var Label_PR = e.Item.FindControl("Label_PR") as Label;
            var Label_ONA = e.Item.FindControl("Label_ONA") as Label;
            var Label_LoggingCosts = e.Item.FindControl("Label_LoggingCosts") as Label;
            var Label_HaulingCosts = e.Item.FindControl("Label_HaulingCosts") as Label;

            var Label_2011Future = e.Item.FindControl("Label_2011Future") as Label;
            var Label_2020Future = e.Item.FindControl("Label_2020Future") as Label;
            var Label_2040Future = e.Item.FindControl("Label_2040Future") as Label;
            var Label_2060Future = e.Item.FindControl("Label_2060Future") as Label;
            var Label_2080Future = e.Item.FindControl("Label_2080Future") as Label;



            timberMarket = (TimberMarket)e.Item.DataItem;

            var real = TimberSortValue.GetRealPriceDetails(rpaPortfolio, specy, timberMarket);
            var delivered = TimberSortValue.GetDeliveredLogPrices(portfolio, specy, timberMarket);
            decimal price = 0;
            decimal pnr = 0;
            decimal adjustedValue = 0;
            decimal rpa = 0;
            decimal longevity = 0;
            int ona = 0;
            int loggingcosts = 0;
            int haulcosts = 0;

            if (delivered.DeliveredLogPrice.HasValue)
            {
                price = delivered.DeliveredLogPrice.Value;
            }

            if (delivered.ProfitAndRisk.HasValue)
            {
                pnr = Convert.ToDecimal(delivered.ProfitAndRisk.Value) / 100M;
            }
            if (delivered.OverheadAndAdmin.HasValue)
            {
                ona = delivered.OverheadAndAdmin.Value;
            }
            if (delivered.LoggingCosts.HasValue)
            {
                loggingcosts = delivered.LoggingCosts.Value;
            }
            if (delivered.HaulingCosts.HasValue)
            {
                haulcosts = delivered.HaulingCosts.Value;
            }

            rpa = real.RPA;
            longevity = real.Longevity;

            var detail = rpaPortfolio.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == specy.LogMarketReportSpeciesID && uu.TimberMarketID == timberMarket.TimberMarketID).FirstOrDefault();
            var ppiNominalDate = MarketModelDataList.Where(uu => uu.Year == detail.BeginningDate.Year && uu.Period == detail.BeginningDate.Month).FirstOrDefault().Value;
            var realValue = price;
            if (rpaPortfolio.RPAPortfolioID != -1)
            {
                realValue = RPAREAL.GetRealValueFromTodaysPPI(detail.BeginningRealValue, ppiNominalDate, ppiToday);
            }
            adjustedValue = price;

            Label_Sort.Text = timberMarket.Market;

            Label_MarketValue.Text = price.ToString("C0");
            Label_RPA.Text = rpa.ToString("N4");
            Label_Longevity.Text = longevity.ToString("N2");
            Label_PR.Text = pnr.ToString("N2");
            Label_ONA.Text = ona.ToString("C0");

            Label_LoggingCosts.Text = loggingcosts.ToString("C0");
            Label_HaulingCosts.Text = haulcosts.ToString("C0");

            var firstYear = DateTime.Now.Year;
            var secondYear = 0;
            var thirdYear = 0;
            var fourthYear = 0;
            var fifthYear = 0;

            for (var ict = 1; ict <= 5; ict++)
            {
                if (secondYear == 0 && (firstYear + ict) % 5 == 0)
                {
                    secondYear = firstYear + ict;
                }
            }
            if (secondYear % 10 == 0)
            {
                thirdYear = secondYear + 10;
            }
            else
            {
                thirdYear = secondYear + 5;
            }
            fourthYear = thirdYear + 10;
            fifthYear = fourthYear + 10;

            var startYear = detail.BeginningDate.Year;

            Label_2011.Text = firstYear.ToString();
            Label_2020.Text = secondYear.ToString();
            Label_2040.Text = thirdYear.ToString();
            Label_2060.Text = fourthYear.ToString();
            Label_2080.Text = fifthYear.ToString();

            var rpa2011 = TimberSortValue.GetFutureRPA(rpa, 0, longevity);
            var rpa2020 = TimberSortValue.GetFutureRPA(rpa, secondYear - startYear, longevity);
            var rpa2040 = TimberSortValue.GetFutureRPA(rpa, thirdYear - startYear, longevity);
            var rpa2060 = TimberSortValue.GetFutureRPA(rpa, fourthYear - startYear, longevity);
            var rpa2080 = TimberSortValue.GetFutureRPA(rpa, fifthYear - startYear, longevity);

            var inflationRate = portfolio.MarketModelPortfolioInflationDetails.InflationRate;

            var rpaInflation2011 = TimberSortValue.GetInflationRPA(rpa2011, inflationRate, 0);
            var rpaInflation2020 = TimberSortValue.GetInflationRPA(rpa2020, inflationRate, secondYear - firstYear);
            var rpaInflation2040 = TimberSortValue.GetInflationRPA(rpa2040, inflationRate, thirdYear - firstYear);
            var rpaInflation2060 = TimberSortValue.GetInflationRPA(rpa2060, inflationRate, fourthYear - firstYear);
            var rpaInflation2080 = TimberSortValue.GetInflationRPA(rpa2080, inflationRate, fifthYear - firstYear);

            Label_2011Future.Text = (rpaInflation2011 * adjustedValue).ToString("C0");
            Label_2020Future.Text = (rpaInflation2020 * realValue).ToString("C0");
            Label_2040Future.Text = (rpaInflation2040 * realValue).ToString("C0");
            Label_2060Future.Text = (rpaInflation2060 * realValue).ToString("C0");
            Label_2080Future.Text = (rpaInflation2080 * realValue).ToString("C0");
        }
        protected void Button_ViewStandReport_Click(object sender, EventArgs e)
        {
            var id = RadComboBox_Stands.SelectedValue;
            var url = "/Parcels/ReportStand.aspx?ParcelID=" + parcelID.ToString() + "&RPAPortfolioID=" + rpaPortfolioID.ToString() + "&MarketModelPortfolioID=" + portfolioID.ToString() + "&StandID=" + id.ToString();
            Response.Redirect(url, true);
        }

    }


}