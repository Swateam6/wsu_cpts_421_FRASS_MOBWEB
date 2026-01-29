using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Models;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;
using FRASS.Services.Proxies;

namespace FRASS.WebUI.Parcels
{
	public partial class StumpageApplied : System.Web.UI.Page
	{
		DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		ParcelDataManager dbParcelDataManager;

		Int32 parcelID;
		Int32 portfolioID;
		User thisUser;
		Parcel parcel;
		StumpageGroup stumpageGroup;
		StumpageModelPortfolio portfolio;

		protected void Page_Load(object sender, EventArgs e)
		{
			ScriptManager.GetCurrent(Page).RegisterPostBackControl(Button_ExportPDF);
			LoadPage();
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbParcelDataManager = ParcelDataManager.GetInstance();		

			thisUser = Master.GetCurrentUser();
			Int32 tmpInt;
			if (Int32.TryParse(Request.QueryString["ParcelID"].ToString(), out tmpInt))
			{
				parcelID = tmpInt;
				parcel = dbParcelDataManager.GetParcel(parcelID);
			}
			else
			{
				Server.Transfer("/Parcels/Parcels.aspx");
			}
			if (Int32.TryParse(Request.QueryString["StumpageModelPortfolioID"].ToString(), out tmpInt))
			{
				portfolioID = tmpInt;
			}
			else
			{
				Server.Transfer("/Parcels/Parcels.aspx");
			}
		}
		protected void Button_ExportToPDF_Click(object sender, EventArgs e)
		{
			Response.Redirect("/PDFs/StumpageModelReportApplied.aspx?ParcelID=" + parcelID.ToString() + "&StumpageModelPortfolioID=" + portfolioID.ToString(), true);
		}
		protected void Button_ExportFullReportToPDF_Click(object sender, EventArgs e)
		{
			var proxy = new ReportServiceProxy();
			proxy.GenerateFullParcelReportStumpageMarketModel(thisUser.UserID, parcelID, portfolioID);
		}
		private void LoadPage()
		{
			portfolio = dbStumpageMarketModelDataManager.GetStumpageModelPortfolio(portfolioID);
			var econ = new EconVariables(portfolio);
			Label_MarketModelName.Text = portfolio.PortfolioName;
			Label_RateOfInflation.Text = (100 * econ.RateOfInflation).ToString("N2") + "%";
			Label_LandownerDiscountRate.Text = (100 * econ.RealDiscount).ToString("N2") + "%";
			Label_ReforestationCost.Text = econ.ReforestionCosts.ToString("C0") + "/Acre";
			Label_AccessFee.Text = econ.AccessFee.ToString("C2") + "/MBF/Mile";
			Label_MaintenanceFee.Text = econ.MaintenanceFee.ToString("C2") + "/MBF/Mile";
			Label_NewLoggingRoadConstruction.Text = econ.NewRoad.ToString("C0") + "/Mile";
			Label_ParcelNumber.Text = parcel.ParcelNumber;


			var stumpageGroups = dbStumpageMarketModelDataManager.GetStumpageGroups();
			Repeater_Species.DataSource = stumpageGroups.OrderBy(uu => uu.StumpageGroupName);
			Repeater_Species.DataBind();

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
				DropDownList_MarketModels.Items.Add(new ListItem("--- Delivered Log Market Portfolios ---", "0"));
				foreach (var model in marketModels.OrderBy(uu => uu.PortfolioName))
				{
					DropDownList_MarketModels.Items.Add(new ListItem(model.PortfolioName, model.ModelID.ToString()));
				}
			}

			var stumpageModels = dbStumpageMarketModelDataManager.GetStumpageModelPortfolios(thisUser);
			if (stumpageModels.Count() > 0)
			{
				DropDownList_StumpageModels.Visible = true;
				Button_ApplyStumpageModels.Visible = true;
				DropDownList_StumpageModels.Items.Clear();
				foreach (var model in stumpageModels.OrderBy(uu => uu.PortfolioName))
				{
					DropDownList_StumpageModels.Items.Add(new ListItem(model.PortfolioName, model.StumpageModelPortfolioID.ToString()));
				}
				DropDownList_StumpageModels.SelectedValue = portfolioID.ToString();
			}
		}
		
		protected void Button_ViewMarketValueReport_Click(object sender, EventArgs e)
		{
			var url = string.Format("/Parcels/StumpageValueReport.aspx?ParcelID={0}&StumpageModelPortfolioID={1}", parcelID.ToString(), portfolioID.ToString());
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
			if (DropDownList_MarketModels.SelectedValue != "0")
			{
				var url = string.Format("/Parcels/ReportApplied.aspx?ParcelID={0}&MarketModelPortfolioID={1}", parcelID, DropDownList_MarketModels.SelectedValue);
				Response.Redirect(url, true);
			}
		}
		protected void Button_ViewStandReport_Click(object sender, EventArgs e)
		{
			var id = RadComboBox_Stands.SelectedValue;
			var url = "/Parcels/StumpageReportStand.aspx?ParcelID=" + parcelID.ToString() + "&StumpageModelPortfolioID=" + portfolioID.ToString() + "&StandID=" + id.ToString();
			Response.Redirect(url, true);
		}

		protected void Repeater_Species_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			stumpageGroup = (StumpageGroup)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			Label_Species.Text = stumpageGroup.StumpageGroupName;
			

			var Repeater_Vals = e.Item.FindControl("Repeater_Vals") as Repeater;
			Repeater_Vals.DataSource = stumpageGroup.StumpageGroupQualityCodes.OrderBy(uu => uu.QualityCodeNumber);
			Repeater_Vals.DataBind();			
		}
		protected void Repeater_Vals_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var qualityCode = (StumpageGroupQualityCode)e.Item.DataItem;
			var Label_QualityCode = e.Item.FindControl("Label_QualityCode") as Label;
			var Label_HaulZone3_RPA = e.Item.FindControl("Label_HaulZone3_RPA") as Label;
			var Label_HaulZone4_RPA = e.Item.FindControl("Label_HaulZone4_RPA") as Label;
			var Label_HaulZone5_RPA = e.Item.FindControl("Label_HaulZone5_RPA") as Label;
			var Label_Longevity = e.Item.FindControl("Label_Longevity") as Label;
			var Label_HaulZone3_Price = e.Item.FindControl("Label_HaulZone3_Price") as Label;
			var Label_HaulZone4_Price = e.Item.FindControl("Label_HaulZone4_Price") as Label;
			var Label_HaulZone5_Price = e.Item.FindControl("Label_HaulZone5_Price") as Label;
			var Label_ONA = e.Item.FindControl("Label_ONA") as Label;
			var Label_PR = e.Item.FindControl("Label_PR") as Label;
			Label_QualityCode.Text = qualityCode.QualityCodeNumber.ToString();
			var rpas = (from p in portfolio.StumpageModelPortfolioRPADatas
					   where p.StumpageGroupQualityCodeID == qualityCode.StumpageGroupQualityCodeID
					   select p).FirstOrDefault();

			if (rpas != null)
			{
				Label_HaulZone3_RPA.Text = rpas.Haul3.HasValue ? rpas.Haul3.Value.ToString("N4") : "--";
				Label_HaulZone4_RPA.Text = rpas.Haul4.HasValue ? rpas.Haul4.Value.ToString("N4") : "--";
				Label_HaulZone5_RPA.Text = rpas.Haul5.HasValue ? rpas.Haul5.Value.ToString("N4") : "--";
				Label_Longevity.Text = rpas.Longevity.HasValue ? rpas.Longevity.Value.ToString("") : "--";
			}
			var prices = (from p in portfolio.StumpageModelPortfolioValues
						  where p.StumpageGroupQualityCodeID == qualityCode.StumpageGroupQualityCodeID
						  select p).FirstOrDefault();
			if (prices != null)
			{
				Label_HaulZone3_Price.Text = prices.Haul3.HasValue ? prices.Haul3.Value.ToString("C0") : "--";
				Label_HaulZone4_Price.Text = prices.Haul4.HasValue ? prices.Haul4.Value.ToString("C0") : "--";
				Label_HaulZone5_Price.Text = prices.Haul5.HasValue ? prices.Haul5.Value.ToString("C0") : "--";
				Label_ONA.Text = prices.OverheadAndAdmin.HasValue ? prices.OverheadAndAdmin.Value.ToString("C0") : "--";
				Label_PR.Text = prices.ProfitAndRisk.HasValue ? prices.ProfitAndRisk.Value.ToString("N0") + "%" : "--";
			}
			
		}
	}
}