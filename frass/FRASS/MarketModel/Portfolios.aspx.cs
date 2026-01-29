using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Formulas;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;
using Telerik.Web.UI;

namespace FRASS.MarketModel
{
	public partial class Portfolios : System.Web.UI.Page
	{
		List<MarketModelData> marketModelData = new List<MarketModelData>();
		List<TimberMarket> timberMarkets;
		List<StumpageGroupQualityCode> qualityCodes;
		LogMarketReportSpecy thisLogMarketReportSpecy;
		StumpageGroup thisStumpageGroup;
		List<v_HistoricLogPrice> historicLogPrices;
		List<StumpagePrice> stumpagePrices;
		DateTime startingDate;
		DateTime endingDate;
		decimal deltaNM;
		decimal deltaN;
		User thisUser;
		StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		RPAPortfolioDataManager dbRPAPortfolioDataManager;
		ParcelDataManager dbParcelDataManager;
		TimberDataManager dbTimberDataManager;
		StandDataManager dbStandDataManager;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				LoadSavedModels();
				LoadFrontPage();
			}
		}
		private void Page_Init(object sender, EventArgs e)
		{
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			dbRPAPortfolioDataManager = RPAPortfolioDataManager.GetInstance();
			dbParcelDataManager = ParcelDataManager.GetInstance();
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbStandDataManager = StandDataManager.GetInstance();

			thisUser = Master.GetCurrentUser();
			marketModelData = dbDeliveredLogMarketModelDataManager.GetMarketModelData().ToList<MarketModelData>();
			timberMarkets = dbTimberDataManager.GetTimberMarkets();
			historicLogPrices = dbTimberDataManager.GetHistoricLogPrice();
			qualityCodes = dbStumpageMarketModelDataManager.GetStumpageGroupQualityCodes();
			stumpagePrices = dbStumpageMarketModelDataManager.GetStumpagePrices();
		}

		private string _multipliers;
		protected string Multipliers
		{
			get
			{
				if (string.IsNullOrEmpty(_multipliers))
				{
					var list = new List<Multiplier>();
					foreach (var m in dbStumpageMarketModelDataManager.GetStumpageModelPortfolioMultipliers())
					{
						list.Add(new Multiplier(m));
					}
					var serializer = new JavaScriptSerializer();
					_multipliers = serializer.Serialize(list);
				}
				return _multipliers;
			}
		}

		[Serializable]
		private class Multiplier
		{
			public int StumpageGroupQualityCodeID { get; private set; }
			public decimal HaulZone3 { get; private set; }
			public decimal HaulZone4 { get; private set; }
			public decimal HaulZone5 { get; private set; }
			public Multiplier(StumpageModelPortfolioMultiplier stumpageModelPortfolioMultiplier)
			{
				StumpageGroupQualityCodeID = stumpageModelPortfolioMultiplier.StumpageGroupQualityCodeID;
				HaulZone3 = stumpageModelPortfolioMultiplier.HaulZone3Multiplier;
				HaulZone4 = stumpageModelPortfolioMultiplier.HaulZone4Multiplier;
				HaulZone5 = stumpageModelPortfolioMultiplier.HaulZone5Multiplier;
			}
		}

		private void LoadFrontPage()
		{
			HiddenField_PortfolioID.Value = "0";
			ResetPanels();
			Panel_FrontPage.Visible = true;
			Button_Next.Visible = false;
			Button_Prev.Visible = false;
			Button_Cancel.Visible = false;
		}
		protected void Button_Next_Click(object sender, EventArgs e)
		{
			if (HiddenField_ModelType.Value == "1")
			{
				if (Panel_Inflation.Visible == true)
				{
					ResetPanels();
					Panel_ReforestationCosts.Visible = true;
					Panel_Header.Visible = false;
				}
				else if (Panel_ReforestationCosts.Visible == true)
				{
					ResetPanels();
					Panel_RoadUseConstruction.Visible = true;
				}
				else if (Panel_RoadUseConstruction.Visible == true)
				{
					ResetPanels();
					Panel_DeliveredLogModel.Visible = true;
					Button_Next.Visible = false;
				}
			}
		}
		protected void Button_Prev_Click(object sender, EventArgs e)
		{
			if (HiddenField_ModelType.Value == "1")
			{
				if (Panel_ReforestationCosts.Visible == true)
				{
					ResetPanels();
					Panel_Inflation.Visible = true;
					Button_Prev.Visible = false;
					Panel_Header.Visible = true;
				}
				else if (Panel_RoadUseConstruction.Visible == true)
				{
					ResetPanels();
					Panel_ReforestationCosts.Visible = true;
					Panel_Header.Visible = true;
				}
				else if (Panel_DeliveredLogModel.Visible == true)
				{
					ResetPanels();
					Panel_RoadUseConstruction.Visible = true;
					Panel_Header.Visible = false;
				}
			}
		}

		protected void Button_DeleteMarketModel_Click(object sender, CommandEventArgs e)
		{
			var marketPortfolio = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(Convert.ToInt32(e.CommandArgument.ToString()));
			dbDeliveredLogMarketModelDataManager.DeleteMarketModelPortfolio(marketPortfolio);
			RadGrid_DeliveredLogModels.Rebind();
			LoadFrontPage();
		}
		protected void Button_DeleteRPAPortfolio_Click(object sender, CommandEventArgs e)
		{
			var rpaPortfolio = dbRPAPortfolioDataManager.GetRPAPortfolio(Convert.ToInt32(e.CommandArgument.ToString()));
			dbRPAPortfolioDataManager.DeleteRPAPortfolio(rpaPortfolio);
			RadGrid_RPAPortfolios.Rebind();
			LoadFrontPage();
		}
		protected void Button_DeleteStumpageModel_Click(object sender, CommandEventArgs e)
		{
			var stumpagePortfolio = dbStumpageMarketModelDataManager.GetStumpageModelPortfolio(Convert.ToInt32(e.CommandArgument.ToString()));
			dbStumpageMarketModelDataManager.DeleteStumpageModelPortfolio(stumpagePortfolio);
			LoadSavedModels();
			LoadFrontPage();
		}

		private void ResetPanels()
		{
			Button_Next.Visible = true;
			Button_Prev.Visible = true;
			Button_Cancel.Visible = true;

			Panel_FrontPage.Visible = false;
			Panel_Header.Visible = false;
			Panel_Inflation.Visible = false;
			Panel_ReforestationCosts.Visible = false;
			Panel_RoadUseConstruction.Visible = false;
			Panel_DeliveredLogModel.Visible = false;
			Panel_StumpageModel.Visible = false;
			Panel_StumpageModelPage2.Visible = false;
		}
		protected void LoadYearDropDowns()
		{
			LoadYearDropDownInflation(marketModelData.OrderBy(uu => uu.Year).ToList<MarketModelData>(), DropDownList_BeginningYear);
			LoadYearDropDownInflation(marketModelData.OrderByDescending(uu => uu.Year).ToList<MarketModelData>(), DropDownList_EndingYear);

			if (DropDownList_BeginningYear.Items.FindByValue(System.DateTime.Now.Year.ToString()) != null)
			{
				DropDownList_BeginningYear.SelectedValue = (System.DateTime.Now.Year - 10).ToString();
				DropDownList_EndingYear.SelectedValue = (System.DateTime.Now.Year).ToString();
			}

			//LoadYearDropDownLPI(marketModelData.Where(uu => uu.MarketModelTypeID == 1).OrderBy(uu => uu.Year).ToList<MarketModelData>(), DropDownList_StartYearLogCostRealPriceAppreciation, "asc");
			//LoadYearDropDownLPI(marketModelData.Where(uu => uu.MarketModelTypeID == 1).OrderByDescending(uu => uu.Year).ToList<MarketModelData>(), DropDownList_EndYearLogCostRealPriceAppreciation, "desc");


			var stumpagePrices = dbStumpageMarketModelDataManager.GetStumpagePrices();
			var stumpageDates = (from d in stumpagePrices select d.MarketDate).Distinct();
			foreach (DateTime date in stumpageDates.OrderBy(uu => uu))
			{
				DropDownList_StumpageModelStartingYear.Items.Add(new ListItem(GetStumpageReportDateValue(date), date.ToShortDateString()));
			}
			foreach (DateTime date in stumpageDates.OrderByDescending(uu => uu))
			{
				DropDownList_StumpageModelEndingYear.Items.Add(new ListItem(GetStumpageReportDateValue(date), date.ToShortDateString()));
			}
		}
		private string GetStumpageReportDateValue(DateTime date)
		{
			var stumpageReportValue = "First Half ";
			if (date.Month == 7)
			{
				stumpageReportValue = "Second Half ";
			}
			return stumpageReportValue + date.Year.ToString();
		}
		protected void LoadSavedModels()
		{
			var stumpages = dbStumpageMarketModelDataManager.GetStumpageModelPortfolios(thisUser);
			Repeater_SavedStumpageLogModels.DataSource = stumpages.OrderBy(uu => uu.PortfolioName);
			Repeater_SavedStumpageLogModels.DataBind();
		}
		
		protected void Repeater_SavedRPAPortfolios_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var model = e.Item.DataItem as RPAPortfolio;
			var LinkButton_RPAName = e.Item.FindControl("LinkButton_RPAName") as LinkButton;
			var Button_DeleteRPAPortfolio = e.Item.FindControl("Button_DeleteRPAPortfolio") as Button;
			var Button_ShareRPAPortfolio = e.Item.FindControl("Button_ShareRPAPortfolio") as Button;
			LinkButton_RPAName.Text = model.PortfolioName;
			LinkButton_RPAName.CommandArgument = model.RPAPortfolioID.ToString();
			Button_DeleteRPAPortfolio.CommandArgument = model.RPAPortfolioID.ToString();
			Button_ShareRPAPortfolio.Attributes.Add("onclick", "radopen('/Popups/SharePortfolio.aspx?type=2&id=" + model.RPAPortfolioID.ToString() + "','ShareModel'); return false;");

		}
		protected void Repeater_SavedStumpageLogModels_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var model = e.Item.DataItem as StumpageModelPortfolio;
			var LinkButton_StumpageModelName = e.Item.FindControl("LinkButton_StumpageModelName") as LinkButton;
			var Button_DeleteStumpageModel = e.Item.FindControl("Button_DeleteStumpageModel") as Button;
			var Button_ShareStumpageModel = e.Item.FindControl("Button_ShareStumpageModel") as Button;
			LinkButton_StumpageModelName.Text = model.PortfolioName;
			LinkButton_StumpageModelName.CommandArgument = model.StumpageModelPortfolioID.ToString();
			Button_DeleteStumpageModel.CommandArgument = model.StumpageModelPortfolioID.ToString();
			Button_ShareStumpageModel.Attributes.Add("onclick", "radopen('/Popups/SharePortfolio.aspx?type=2&id=" + model.StumpageModelPortfolioID.ToString() + "','ShareModel'); return false;");
		}
		protected void LinkButton_MarketModel_Click(object sender, CommandEventArgs e)
		{
			Button_SaveEditsToModel.Visible = true;
			SetUpDeliveredLogModels();
			var marketModelPortfolioID = Convert.ToInt32(e.CommandArgument.ToString());
			var model = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(marketModelPortfolioID);
			var inflationDetail = model.MarketModelPortfolioInflationDetails;
			var costs = model.MarketModelPortfolioCosts;
			var deliveredLogModel = model.MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
			TextBox_DeliveredLogModelName.Text = model.PortfolioName;
			HiddenField_PortfolioID.Value = marketModelPortfolioID.ToString();
			HiddenField_ModelType.Value = "1";
			LoadInflationPanel(inflationDetail);
			LoadCosts(costs);
			LoadDeliveredLogModels(deliveredLogModel);
		}
		protected void LinkButton_RPAPortfolio_Click(object sender, CommandEventArgs e)
		{
			var rpaPortfolioID = Convert.ToInt32(e.CommandArgument.ToString());
			Response.Redirect("RPAPortfolio.aspx?id=" + rpaPortfolioID.ToString(), true);
		}
		protected void LinkButton_StumpageModelName_Click(object sender, CommandEventArgs e)
		{
			Button_SaveEditsToStumpageModel.Visible = true;
			SetUpStumpageModels();
			var stumpageModelPortfolioID = Convert.ToInt32(e.CommandArgument.ToString());
			var model = dbStumpageMarketModelDataManager.GetStumpageModelPortfolio(stumpageModelPortfolioID);
			var inflationDetail = model.StumpageModelPortfolioInflationDetails;
			var costs = model.StumpageModelPortfolioCosts;
			var datas = model.StumpageModelPortfolioRPADatas.ToList<StumpageModelPortfolioRPAData>();
			var values = model.StumpageModelPortfolioValues.ToList<StumpageModelPortfolioValue>();

			TextBox_StumpageModelName.Text = model.PortfolioName;
			HiddenField_PortfolioID.Value = stumpageModelPortfolioID.ToString();
			HiddenField_ModelType.Value = "2";
			LoadInflationPanel(inflationDetail);
			LoadCosts(costs);
			LoadStumpageModelDatas(datas);
			LoadStumpageModelValues(values);
		}

		private void LoadInflationPanel(StumpageModelPortfolioInflationDetail inflationDetail)
		{
			DropDownList_BeginningYear.SelectedValue = inflationDetail.BeginningYear.ToString();
			DropDownList_EndingYear.SelectedValue = inflationDetail.EndingYear.ToString();
			TextBox_CPI.Text = (inflationDetail.LandownerDiscountRate * 100).ToString("N2") + "%";
			TextBox_PPI.Text = (inflationDetail.InflationRate * 100).ToString("N2") + "%";
			TextBox_CPIRate.Text = inflationDetail.LandownerDiscountRate.ToString();
			TextBox_PPIRate.Text = inflationDetail.InflationRate.ToString();
		}
		private void LoadCosts(StumpageModelPortfolioCost costs)
		{
			TextBox_ReforestationCosts.Text = costs.ReforestationCost.ToString();
			TextBox_AccessFeeRock.Text = costs.AccessFeeRock.ToString("N2");
			TextBox_AccessFeeTimber.Text = costs.AccessFeeTimber.ToString("N2");
			TextBox_MaintenanceFeeRock.Text = costs.MaintenanceFeeRockHaul.ToString("N2");
			TextBox_MaintenanceFeeTimber.Text = costs.MaintenanceFeeTimberHaul.ToString("N2");
			TextBox_LoggingRoadConstructionCost.Text = costs.RoadConstructionCosts.ToString();
		}
		private void LoadStumpageModelDatas(List<StumpageModelPortfolioRPAData> datas)
		{
			var itemTops = Repeater_StumpageGroups.Items;
			foreach (RepeaterItem item in itemTops)
			{
				HiddenField HiddenField_StumpageGroupID = item.FindControl("HiddenField_StumpageGroupID") as HiddenField;
				var stumpageGroupID = Convert.ToInt32(HiddenField_StumpageGroupID.Value);
				Repeater Repeater_QualityCodes = item.FindControl("Repeater_QualityCodes") as Repeater;
				if (Repeater_QualityCodes != null)
				{
					foreach (RepeaterItem qcs in Repeater_QualityCodes.Items)
					{
						HiddenField QualityCodeID = qcs.FindControl("QualityCodeID") as HiddenField;
						TextBox TextBox_HaulZone3 = qcs.FindControl("TextBox_HaulZone3") as TextBox;
						TextBox TextBox_HaulZone4 = qcs.FindControl("TextBox_HaulZone4") as TextBox;
						TextBox TextBox_HaulZone5 = qcs.FindControl("TextBox_HaulZone5") as TextBox;
						TextBox TextBox_NewLongevityOfRPAOrRPD = qcs.FindControl("TextBox_NewLongevityOfRPAOrRPD") as TextBox;
						var vals = (from l in datas where l.StumpageGroupID == stumpageGroupID && l.StumpageGroupQualityCodeID == Convert.ToInt32(QualityCodeID.Value) select l).FirstOrDefault();
						DropDownList_StumpageModelStartingYear.SelectedValue = vals.BeginningReport.ToShortDateString();
						DropDownList_StumpageModelEndingYear.SelectedValue = vals.EndingReport.ToShortDateString();
						if (vals.Haul3.HasValue)
						{
							TextBox_HaulZone3.Text = vals.Haul3.Value.ToString("N5");
						}
						if (vals.Haul4.HasValue)
						{
							TextBox_HaulZone4.Text = vals.Haul4.Value.ToString("N5");
						}
						if (vals.Haul5.HasValue)
						{
							TextBox_HaulZone5.Text = vals.Haul5.Value.ToString("N5");
						}
						if (vals.Longevity.HasValue)
						{
							TextBox_NewLongevityOfRPAOrRPD.Text = vals.Longevity.Value.ToString("N1");
						}
					}
				}
			}
		}
		private void LoadStumpageModelValues(List<StumpageModelPortfolioValue> values)
		{
			var itemTops = Repeater_StumpageGroups2.Items;
			foreach (RepeaterItem item in itemTops)
			{
				HiddenField HiddenField_StumpageGroupID = item.FindControl("HiddenField_StumpageGroupID") as HiddenField;
				var stumpageGroupID = Convert.ToInt32(HiddenField_StumpageGroupID.Value);
				Repeater Repeater_QualityCodes = item.FindControl("Repeater_QualityCodes2") as Repeater;
				if (Repeater_QualityCodes != null)
				{
					foreach (RepeaterItem qcs in Repeater_QualityCodes.Items)
					{
						HiddenField QualityCodeID = qcs.FindControl("QualityCodeID") as HiddenField;
						TextBox TextBox_HaulZone3 = qcs.FindControl("TextBox_HaulZone3") as TextBox;
						TextBox TextBox_HaulZone4 = qcs.FindControl("TextBox_HaulZone4") as TextBox;
						TextBox TextBox_HaulZone5 = qcs.FindControl("TextBox_HaulZone5") as TextBox;
						TextBox TextBox_ONA = qcs.FindControl("TextBox_ONA") as TextBox;
						TextBox TextBox_HaulZone3CNV = qcs.FindControl("TextBox_HaulZone3CNV") as TextBox;
						TextBox TextBox_HaulZone4CNV = qcs.FindControl("TextBox_HaulZone4CNV") as TextBox;
						TextBox TextBox_HaulZone5CNV = qcs.FindControl("TextBox_HaulZone5CNV") as TextBox;
						TextBox TextBox_PR = qcs.FindControl("TextBox_PR") as TextBox;
						TextBox TextBox_Notes = qcs.FindControl("TextBox_Notes") as TextBox;
						
						var vals = (from l in values where l.StumpageGroupID == stumpageGroupID && l.StumpageGroupQualityCodeID == Convert.ToInt32(QualityCodeID.Value) select l).FirstOrDefault();
						if (vals.Haul3.HasValue)
						{
							TextBox_HaulZone3.Text = vals.Haul3.Value.ToString("N2");
						}
						if (vals.Haul4.HasValue)
						{
							TextBox_HaulZone4.Text = vals.Haul4.Value.ToString("N2");
						}
						if (vals.Haul5.HasValue)
						{
							TextBox_HaulZone5.Text = vals.Haul5.Value.ToString("N2");
						}
						if (vals.OverheadAndAdmin.HasValue)
						{
							TextBox_ONA.Text = vals.OverheadAndAdmin.Value.ToString("N2");
						}
						if (vals.Haul3CurrentNetValue.HasValue)
						{
							TextBox_HaulZone3CNV.Text = vals.Haul3CurrentNetValue.Value.ToString("N2");
						}
						if (vals.Haul4CurrentNetValue.HasValue)
						{
							TextBox_HaulZone4CNV.Text = vals.Haul4CurrentNetValue.Value.ToString("N2");
						}
						if (vals.Haul5CurrentNetValue.HasValue)
						{
							TextBox_HaulZone5CNV.Text = vals.Haul5CurrentNetValue.Value.ToString("N2");
						}
						if (vals.ProfitAndRisk.HasValue)
						{
							TextBox_PR.Text = vals.ProfitAndRisk.Value.ToString("N2");
						}
						TextBox_Notes.Text = vals.Notes;
					}
				}
			}
		}


		private void LoadInflationPanel(MarketModelPortfolioInflationDetail inflationDetail)
		{
			DropDownList_BeginningYear.SelectedValue = inflationDetail.BeginningYear.ToString();
			DropDownList_EndingYear.SelectedValue = inflationDetail.EndingYear.ToString();
			TextBox_CPI.Text = (inflationDetail.LandownerDiscountRate * 100).ToString("N2") + "%";
			TextBox_PPI.Text = (inflationDetail.InflationRate * 100).ToString("N2") + "%";
			TextBox_CPIRate.Text = inflationDetail.LandownerDiscountRate.ToString();
			TextBox_PPIRate.Text = inflationDetail.InflationRate.ToString();
		}
		private void LoadCosts(MarketModelPortfolioCost costs)
		{
			TextBox_ReforestationCosts.Text = costs.ReforestationCost.ToString();
			TextBox_AccessFeeRock.Text = costs.AccessFeeRock.ToString("N2");
			TextBox_AccessFeeTimber.Text = costs.AccessFeeTimber.ToString("N2");
			TextBox_MaintenanceFeeRock.Text = costs.MaintenanceFeeRockHaul.ToString("N2");
			TextBox_MaintenanceFeeTimber.Text = costs.MaintenanceFeeTimberHaul.ToString("N2");
			TextBox_LoggingRoadConstructionCost.Text = costs.RoadConstructionCosts.ToString();
		}
		private void LoadDeliveredLogModels(List<MarketModelPortfolioDeliveredLogModelDetail> deliveredLogModel)
		{
			var speciesItems = Repeater_DeliveredLogModelSpecies.Items;
			foreach (RepeaterItem species in speciesItems)
			{
				HiddenField HiddenField_LogMarketReportSpeciesID = species.FindControl("HiddenField_LogMarketReportSpeciesID") as HiddenField;
				var speciesID = Convert.ToInt32(HiddenField_LogMarketReportSpeciesID.Value);
				Repeater Repeater_DeliveredLogModelSpecies_Sorts = species.FindControl("Repeater_DeliveredLogModelSpecies_Sorts") as Repeater;
				var items = Repeater_DeliveredLogModelSpecies_Sorts.Items;
				foreach (RepeaterItem item in items)
				{
					HiddenField HiddenField_TimberMarketID = item.FindControl("HiddenField_TimberMarketID") as HiddenField;
					var vals = (from l in deliveredLogModel where l.LogMarketReportSpeciesID == speciesID && l.TimberMarketID == Convert.ToInt32(HiddenField_TimberMarketID.Value) select l).FirstOrDefault();
					if (vals != null)
					{
						TextBox dlp = item.FindControl("TextBox_DeliveredLogPrices") as TextBox;
						TextBox lc = item.FindControl("TextBox_LoggingCosts") as TextBox;
						TextBox hc = item.FindControl("TextBox_HaulingCosts") as TextBox;
						TextBox oha = item.FindControl("TextBox_OverheadAndAdmin") as TextBox;
						TextBox cnv = item.FindControl("TextBox_CurrentNetValue") as TextBox;
						TextBox pnr = item.FindControl("TextBox_ProfitAndRisk") as TextBox;
						TextBox TextBox_Notes = item.FindControl("TextBox_Notes") as TextBox;
						dlp.Text = "";
						lc.Text = "";
						hc.Text = "";
						oha.Text = "";
						cnv.Text = "";
						pnr.Text = "";
						TextBox_Notes.Text = vals.Notes;
						var currentNetValue = 0M;

						if (vals.DeliveredLogPrice.HasValue)
						{
							currentNetValue = vals.DeliveredLogPrice.Value;
							dlp.Text = vals.DeliveredLogPrice.Value.ToString();
						}
						if (vals.LoggingCosts.HasValue)
						{
							currentNetValue = currentNetValue - vals.LoggingCosts.Value;
							lc.Text = vals.LoggingCosts.Value.ToString();
						}
						if (vals.HaulingCosts.HasValue)
						{
							currentNetValue = currentNetValue - vals.HaulingCosts.Value;
							hc.Text = vals.HaulingCosts.Value.ToString();
						}
						if (vals.OverheadAndAdmin.HasValue)
						{
							currentNetValue = currentNetValue - vals.OverheadAndAdmin.Value;
							oha.Text = vals.OverheadAndAdmin.Value.ToString();
						}

						cnv.Text = currentNetValue.ToString();

						if (vals.ProfitAndRisk.HasValue)
						{
							pnr.Text = vals.ProfitAndRisk.Value.ToString();
						}
					}
				}
			}
		}

		protected void Button_Cancel_Click(object sender, EventArgs e)
		{
			LoadFrontPage();
		}
		protected void Button_StartNewDeliveredLogModel_Click(object sender, EventArgs e)
		{
			Button_SaveEditsToModel.Visible = false;
			SetUpDeliveredLogModels();
		}
		private void SetUpDeliveredLogModels()
		{
			HiddenField_ModelType.Value = "1";
			LoadYearDropDowns();
			CalculateInflation();
			Panel_FrontPage.Visible = false;
			Panel_Inflation.Visible = true;
			Panel_ReforestationCosts.Visible = false;
			Panel_DeliveredLogModel.Visible = false;
			Panel_StumpageModel.Visible = false;
			Panel_StumpageModelPage2.Visible = false;
			Button_Next.Visible = true;
			Button_Prev.Visible = false;
			Button_Cancel.Visible = true;
			Panel_Header.Visible = true;
			LoadDeliveredLogModels();
		}

		protected void Button_StartNewRPAPortfolio_Click(object sender, EventArgs e)
		{
			Response.Redirect("RPAPortfolio.aspx?id=0",true);
		}
		private void SetUpRPAPortfolios()
		{
			HiddenField_ModelType.Value = "2";
			Panel_FrontPage.Visible = false;
			LoadYearDropDowns();
			Button_Next.Visible = false;
			Button_Prev.Visible = false;
			Panel_Header.Visible = true;
			Button_Cancel.Visible = true;
		}
		protected void Button_StartNewStumpageModel_Click(object sender, EventArgs e)
		{
			Button_SaveEditsToStumpageModel.Visible = false;
			SetUpStumpageModels();
		}

		private void SetUpStumpageModels()
		{
			HiddenField_ModelType.Value = "2";
			LoadYearDropDowns();
			CalculateInflation();
			Panel_FrontPage.Visible = false;
			Panel_Inflation.Visible = true;
			Button_Next.Visible = true;
			Button_Cancel.Visible = true;
			LoadStumpageModel();
		}

		private void LoadYearDropDownInflation(List<MarketModelData> cpippis, DropDownList ddl)
		{
			ddl.Items.Clear();
			ddl.DataSource = cpippis.Select(uu => uu.Year).Distinct();
			ddl.DataBind();
		}

		private void LoadYearDropDownHistoricLogPrices(DropDownList ddl, string direction)
		{
			List<DateTime> dts = new List<DateTime>();
			foreach (var h in historicLogPrices)
			{
				dts.Add(new DateTime(h.Year, h.Month, 1));
			}
			if (direction == "asc")
			{
				foreach (var d in dts.Distinct().OrderBy(uu => uu))
				{
					ddl.Items.Add(new ListItem(d.ToString("MMMM") + " " + d.Year.ToString(), d.ToShortDateString()));
				}
			}
			else
			{
				foreach (var d in dts.Distinct().OrderByDescending(uu => uu))
				{
					ddl.Items.Add(new ListItem(d.ToString("MMMM") + " " + d.Year.ToString(), d.ToShortDateString()));
				}
			}
		}

		protected void DropDownList_Year_Changed(object sender, EventArgs e)
		{
			CalculateInflation();
		}

		private void CalculateInflation()
		{
			Int32 startYear = Convert.ToInt32(DropDownList_BeginningYear.SelectedValue);
			Int32 endYear = Convert.ToInt32(DropDownList_EndingYear.SelectedValue);

			var startCPI = (from c in marketModelData where c.Year == startYear && c.MarketModelTypeID == 2 && c.Period == 13 select c).FirstOrDefault();
			var endCPI = (from c in marketModelData where c.Year == endYear && c.MarketModelTypeID == 2 select c).OrderByDescending(uu => uu.Period).FirstOrDefault();
			var startPPI = (from c in marketModelData where c.Year == startYear && c.MarketModelTypeID == 3 && c.Period == 13 select c).FirstOrDefault();
			var endPPI = (from c in marketModelData where c.Year == endYear && c.MarketModelTypeID == 3 select c).OrderByDescending(uu => uu.Period).FirstOrDefault();

			Double yearDiff = Convert.ToDouble(endYear - startYear);
			if (startCPI == null || endCPI == null)
			{
				TextBox_CPI.Text = "";
			}
			else
			{
				CalculateInflation(yearDiff, Convert.ToDouble(startCPI.Value), Convert.ToDouble(endCPI.Value), TextBox_CPI, TextBox_CPIRate);
			}
			if (startPPI == null || endPPI == null)
			{
				TextBox_PPI.Text = "";
			}
			else
			{
				CalculateInflation(yearDiff, Convert.ToDouble(startPPI.Value), Convert.ToDouble(endPPI.Value), TextBox_PPI, TextBox_PPIRate);
			}
		}

		private void CalculateInflation(Double yearDiff, Double startVal, Double endVal, TextBox tb, TextBox tbRate)
		{
			var indiv = (endVal / startVal);
			var pow = Math.Pow(Convert.ToDouble(indiv), 1d / yearDiff);
			var answer = (pow - 1) * 100;
			tb.Text = answer.ToString("N2") + "%";
			tbRate.Text = (answer / 100).ToString("N4");
		}

		private void LoadDeliveredLogModels()
		{
			LoadDeliveredLogPrices();
		}
		private void LoadDeliveredLogPrices()
		{
			Repeater_DeliveredLogModelSpecies.DataSource = dbTimberDataManager.GetLogMarketReportSpecies();
			Repeater_DeliveredLogModelSpecies.DataBind();
		}
		private void LoadStumpageModel()
		{
			startingDate = Convert.ToDateTime(DropDownList_StumpageModelStartingYear.SelectedValue);
			endingDate = Convert.ToDateTime(DropDownList_StumpageModelEndingYear.SelectedValue);
			deltaN = GetDeltaN(startingDate, endingDate);
			Repeater_StumpageGroups.DataSource = dbStumpageMarketModelDataManager.GetStumpageGroups();
			Repeater_StumpageGroups.DataBind();
			Repeater_StumpageGroups2.DataSource = dbStumpageMarketModelDataManager.GetStumpageGroups();
			Repeater_StumpageGroups2.DataBind();
		}

		protected void Repeater_DeliveredLogModelSpecies_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			LogMarketReportSpecy logMarketReportSpecy = (LogMarketReportSpecy)e.Item.DataItem;
			if (logMarketReportSpecy != null)
			{
				Label Label_LogMarketSpecies = e.Item.FindControl("Label_LogMarketSpecies") as Label;
				HiddenField HiddenField_LogMarketReportSpeciesID = e.Item.FindControl("HiddenField_LogMarketReportSpeciesID") as HiddenField;
				Repeater Repeater_DeliveredLogModelSpecies_Sorts = e.Item.FindControl("Repeater_DeliveredLogModelSpecies_Sorts") as Repeater;
				
				thisLogMarketReportSpecy = logMarketReportSpecy;

				Label_LogMarketSpecies.Text = logMarketReportSpecy.LogMarketSpecies;
				HiddenField_LogMarketReportSpeciesID.Value = thisLogMarketReportSpecy.LogMarketReportSpeciesID.ToString();
				Repeater_DeliveredLogModelSpecies_Sorts.DataSource = (from t in timberMarkets
											 where (from ts in logMarketReportSpecy.LogMarketReportSpeciesMarkets select ts.TimberMarketID).Contains(t.TimberMarketID)
											 select t).OrderBy(uu => uu.OrderID);
				Repeater_DeliveredLogModelSpecies_Sorts.DataBind();

			}
		}
		protected void Repeater_DeliveredLogModelSpecies_Sorts_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			TimberMarket timberMarket = (TimberMarket)e.Item.DataItem;
			if (timberMarket != null)
			{
				HiddenField HiddenField_TimberMarketID = e.Item.FindControl("HiddenField_TimberMarketID") as HiddenField;
				HiddenField_TimberMarketID.Value = timberMarket.TimberMarketID.ToString();
				Label Label_TimberMarket = e.Item.FindControl("Label_TimberMarket") as Label;
				Label_TimberMarket.Text = timberMarket.Market;


				TextBox TextBox_DeliveredLogPrices = e.Item.FindControl("TextBox_DeliveredLogPrices") as TextBox;
				TextBox TextBox_LoggingCosts = e.Item.FindControl("TextBox_LoggingCosts") as TextBox;
				TextBox TextBox_HaulingCosts = e.Item.FindControl("TextBox_HaulingCosts") as TextBox;
				TextBox TextBox_OverheadAndAdmin = e.Item.FindControl("TextBox_OverheadAndAdmin") as TextBox;
				TextBox TextBox_CurrentNetValue = e.Item.FindControl("TextBox_CurrentNetValue") as TextBox;
				var historicPrice = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID).OrderByDescending(uu=>uu.Year).ThenByDescending(uu=>uu.Month).FirstOrDefault();
				if (historicPrice != null)
				{
					TextBox_DeliveredLogPrices.Text = historicPrice.Price.ToString("N0").Replace(",","");
					TextBox_CurrentNetValue.Text = historicPrice.Price.ToString("N0").Replace(",", "");
				}
				
				

				TextBox_DeliveredLogPrices.Attributes.Add("onkeyup", "setCurrentNetValue('" + TextBox_DeliveredLogPrices.ClientID + "','" + TextBox_LoggingCosts.ClientID + "', '" + TextBox_HaulingCosts.ClientID + "','" + TextBox_OverheadAndAdmin.ClientID + "','" + TextBox_CurrentNetValue.ClientID + "');");
				TextBox_LoggingCosts.Attributes.Add("onkeyup", "setCurrentNetValue('" + TextBox_DeliveredLogPrices.ClientID + "','" + TextBox_LoggingCosts.ClientID + "', '" + TextBox_HaulingCosts.ClientID + "','" + TextBox_OverheadAndAdmin.ClientID + "','" + TextBox_CurrentNetValue.ClientID + "');");
				TextBox_HaulingCosts.Attributes.Add("onkeyup", "setCurrentNetValue('" + TextBox_DeliveredLogPrices.ClientID + "','" + TextBox_LoggingCosts.ClientID + "', '" + TextBox_HaulingCosts.ClientID + "','" + TextBox_OverheadAndAdmin.ClientID + "','" + TextBox_CurrentNetValue.ClientID + "');");
				TextBox_OverheadAndAdmin.Attributes.Add("onkeyup", "setCurrentNetValue('" + TextBox_DeliveredLogPrices.ClientID + "','" + TextBox_LoggingCosts.ClientID + "', '" + TextBox_HaulingCosts.ClientID + "','" + TextBox_OverheadAndAdmin.ClientID + "','" + TextBox_CurrentNetValue.ClientID + "');");
			}
		}

		private decimal GetNominalDeliveredLogPrices(decimal historicLogPrice, decimal actualMarketValue, decimal deltanm)
		{
			var curVal = GetCurrentLogPriceEquivalent(deltanm, historicLogPrice);
			var powerBase = Convert.ToDouble(actualMarketValue / curVal);
			var rooted = Math.Pow(powerBase, Convert.ToDouble(1 / deltanm));
			return Convert.ToDecimal(rooted - 1);
		}
		private decimal GetCurrentLogPriceEquivalent(decimal deltaNM, decimal historicLogPrice)
		{
			var f = Inflation(deltaNM);
			var val1 = 1 + f;
			var val2 = Math.Pow(Convert.ToDouble(val1), Convert.ToDouble(deltaNM));
			var val = Convert.ToDouble(historicLogPrice) * val2;
			return Convert.ToDecimal(val);
		}
		private decimal GetDeltaNM(DateTime startDate, DateTime endDate)
		{
			var dnm = new DeltaNM();
			return dnm.GetDeltaNM(startDate.Year, startingDate.Month, endDate.Year, endDate.Month);
		}
		
		private MarketModelData GetPPI(DateTime date)
		{
			var ppi = (from data in marketModelData
					   where data.Year == date.Year && data.Period == date.Month
					   && data.MarketModelTypeID == 3
					   select data).FirstOrDefault();
			return ppi;
		}
		private MarketModelData GetLPI(DateTime date)
		{
			var lpi = (from data in marketModelData
					   where data.Year == date.Year && data.Period == date.Month
					   && data.MarketModelTypeID == 1
					   select data).FirstOrDefault();
			return lpi;
		}

		private decimal? Inflation(decimal deltaNM)
		{
			var ppi1 = GetPPI(endingDate);
			var ppi2 = GetPPI(startingDate);

			if (ppi1 == null || ppi2 == null)
			{
				return null;
			}
			var ppiNM = Convert.ToDecimal(ppi1.Value);
			var ppi0M = Convert.ToDecimal(ppi2.Value);
			return CalculateInflation(deltaNM, ppiNM, ppi0M);
		}
		private decimal? AdjustLPI(decimal inflation, decimal deltanm)
		{
			var lpiStart = GetLPI(startingDate);
			if (lpiStart == null)
			{
				return null;
			}
			var x = 1 + inflation;
			var lpiNM = Convert.ToDouble(lpiStart.Value) * (Math.Pow(Convert.ToDouble(x), Convert.ToDouble(deltaNM)));
			return Convert.ToDecimal(lpiNM);
		}
		private decimal GetRealPriceAppreciation(decimal curLPI, decimal futureLPI, decimal deltanm)
		{
			return CalculateInflation(deltaNM, curLPI, futureLPI);
		}
		private decimal CalculateInflation(decimal root, decimal numerator, decimal denominator)
		{
			var x = numerator / denominator;
			var baseVal = Math.Pow(Convert.ToDouble(x), Convert.ToDouble((1 / root)));
			return Convert.ToDecimal(baseVal - 1);
		}

		protected void Button_LoggingCostsApply_Click(object sender, EventArgs e)
		{
			ApplyValues(TextBox_LoggingCostsApply.Text, "TextBox_LoggingCosts");
		}
		protected void Button_HaulingCostsApply_Click(object sender, EventArgs e)
		{
			ApplyValues(TextBox_HaulingCostsApply.Text, "TextBox_HaulingCosts");
		}
		protected void Button_OverheadAndAdminApply_click(object sender, EventArgs e)
		{
			ApplyValues(TextBox_OverheadAndAdminApply.Text, "TextBox_OverheadAndAdmin");
		}
		protected void Button_ProfitAndRiskApply_Click(object sender, EventArgs e)
		{
			ApplyValues(TextBox_ProfitAndRiskApply.Text, "TextBox_ProfitAndRisk");
		}
		protected void Button_NotesApply_Click(object sender, EventArgs e)
		{
			ApplyValues(TextBox_NotesApply.Text, "TextBox_Notes");
		}

		private void SaveInflationDetails(MarketModelPortfolio model)
		{

			MarketModelPortfolioInflationDetail inflation;
			if (model.MarketModelPortfolioInflationDetails == null)
			{
				inflation = new MarketModelPortfolioInflationDetail();
			}
			else
			{
				inflation = model.MarketModelPortfolioInflationDetails;
			}
			inflation.BeginningYear = Convert.ToInt32(DropDownList_BeginningYear.SelectedValue);
			inflation.EndingYear = Convert.ToInt32(DropDownList_EndingYear.SelectedValue);
			decimal cpi = 0;
			decimal ppi = 0;
			Decimal.TryParse(TextBox_CPIRate.Text.Trim(), out cpi);
			Decimal.TryParse(TextBox_PPIRate.Text.Trim(), out ppi);
			inflation.LandownerDiscountRate = Convert.ToDecimal(cpi);
			inflation.InflationRate = Convert.ToDecimal(ppi);
			model.MarketModelPortfolioInflationDetails = inflation;
		}
		private void SaveMarketModelCosts(MarketModelPortfolio model)
		{
			MarketModelPortfolioCost costs;
			if (model.MarketModelPortfolioCosts == null)
			{
				costs = new MarketModelPortfolioCost();
			}
			else
			{
				costs = model.MarketModelPortfolioCosts;
			}
			int reforeststationCost = 0;
			decimal accessFeeRock = 0;
			decimal accessFeeTimber = 0;
			decimal maintenanceFeeRock = 0;
			decimal maintenanceFeeTimber = 0;
			int roadConstructionCost = 0;
			Int32.TryParse(TextBox_ReforestationCosts.Text.Trim(), out reforeststationCost);
			Decimal.TryParse(TextBox_AccessFeeRock.Text.Trim(), out accessFeeRock);
			Decimal.TryParse(TextBox_AccessFeeTimber.Text.Trim(), out accessFeeTimber);
			Decimal.TryParse(TextBox_MaintenanceFeeRock.Text.Trim(), out maintenanceFeeRock);
			Decimal.TryParse(TextBox_MaintenanceFeeTimber.Text.Trim(), out maintenanceFeeTimber);
			Int32.TryParse(TextBox_LoggingRoadConstructionCost.Text.Trim(), out roadConstructionCost);
			costs.ReforestationCost = reforeststationCost;
			costs.AccessFeeRock = accessFeeRock;
			costs.AccessFeeTimber = accessFeeTimber;
			costs.MaintenanceFeeRockHaul = maintenanceFeeRock;
			costs.MaintenanceFeeTimberHaul = maintenanceFeeTimber;
			costs.RoadConstructionCosts = roadConstructionCost;
			model.MarketModelPortfolioCosts = costs;
		}
		
		private void SaveDeliveredLogModel(MarketModelPortfolio model)
		{
			var deliveredLogModel = from m in model.MarketModelPortfolioDeliveredLogModelDetails select m;
			foreach (RepeaterItem item in Repeater_DeliveredLogModelSpecies.Items)
			{
				HiddenField HiddenField_LogMarketReportSpeciesID = item.FindControl("HiddenField_LogMarketReportSpeciesID") as HiddenField;
				Repeater Repeater_DeliveredLogModelSpecies_Sorts = item.FindControl("Repeater_DeliveredLogModelSpecies_Sorts") as Repeater;
				var logMarketSpeciesReportSpeciesID = Convert.ToInt32(HiddenField_LogMarketReportSpeciesID.Value);
				if (Repeater_DeliveredLogModelSpecies_Sorts != null)
				{
					foreach (RepeaterItem itemSorts in Repeater_DeliveredLogModelSpecies_Sorts.Items)
					{
						HiddenField HiddenField_TimberMarketID = itemSorts.FindControl("HiddenField_TimberMarketID") as HiddenField;
						MarketModelPortfolioDeliveredLogModelDetail detail;
						var vals = (from l in deliveredLogModel where l.LogMarketReportSpeciesID == logMarketSpeciesReportSpeciesID && l.TimberMarketID == Convert.ToInt32(HiddenField_TimberMarketID.Value) select l).FirstOrDefault();
						if (vals != null)
						{
							detail = vals;
						}
						else
						{
							detail = new MarketModelPortfolioDeliveredLogModelDetail();
						}
						
						detail.LogMarketReportSpeciesID = logMarketSpeciesReportSpeciesID;
						detail.TimberMarketID = Convert.ToInt32(HiddenField_TimberMarketID.Value);
						
						TextBox dlp = itemSorts.FindControl("TextBox_DeliveredLogPrices") as TextBox;
						TextBox lc = itemSorts.FindControl("TextBox_LoggingCosts") as TextBox;
						TextBox hc = itemSorts.FindControl("TextBox_HaulingCosts") as TextBox;
						TextBox oha = itemSorts.FindControl("TextBox_OverheadAndAdmin") as TextBox;
						TextBox cnv = itemSorts.FindControl("TextBox_CurrentNetValue") as TextBox;
						TextBox pnr = itemSorts.FindControl("TextBox_ProfitAndRisk") as TextBox;
						TextBox TextBox_Notes = itemSorts.FindControl("TextBox_Notes") as TextBox;

						int dlpValue = 0;
						int lcValue = 0;
						int hcValue = 0;
						int ohaValue = 0;
						int cnvValue = 0;
						int pnrValue = 0;
						Int32.TryParse(dlp.Text, out dlpValue);
						Int32.TryParse(lc.Text, out lcValue);
						Int32.TryParse(hc.Text, out hcValue);
						Int32.TryParse(oha.Text, out ohaValue);
						Int32.TryParse(cnv.Text, out cnvValue);
						Int32.TryParse(pnr.Text, out pnrValue);
						string notes = TextBox_Notes.Text.Trim();
						detail.DeliveredLogPrice = dlpValue;
						detail.LoggingCosts = lcValue;
						detail.HaulingCosts = hcValue;
						detail.OverheadAndAdmin = ohaValue;
						detail.CurrentNetValue = cnvValue;
						detail.ProfitAndRisk = pnrValue;
						detail.Notes = notes;
						if (vals == null)
						{
							model.MarketModelPortfolioDeliveredLogModelDetails.Add(detail);
						}
					}
				}
			}
		}
		protected void Button_SaveMarketModel_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(TextBox_DeliveredLogModelName.Text))
			{
				MarketModelPortfolio model = new MarketModelPortfolio();
				model.UserID = thisUser.UserID;
				model.CreatedByUserID = thisUser.UserID;
				model.PortfolioName = TextBox_DeliveredLogModelName.Text.Trim();
				model.LastEdited = DateTime.Now;
				SaveInflationDetails(model);
				SaveDeliveredLogModel(model);
				SaveMarketModelCosts(model);
				dbDeliveredLogMarketModelDataManager.AddNewMarketModelPortfolio(model);
				TextBox_DeliveredLogModelName.Text = "";
				LoadSavedModels();
				LoadFrontPage();
				TextBox_DeliveredLogModelName.BackColor = System.Drawing.Color.White;
			}
			else
			{
				TextBox_DeliveredLogModelName.BackColor = System.Drawing.Color.Red;
			}
		}
		
		protected void Button_SaveEditsToModel_Click(object sender, EventArgs e)
		{
			MarketModelPortfolio model = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(Convert.ToInt32(HiddenField_PortfolioID.Value));
			model.PortfolioName = TextBox_DeliveredLogModelName.Text.Trim();
			model.LastEdited = DateTime.Now;
			SaveInflationDetails(model);
			SaveDeliveredLogModel(model);
			SaveMarketModelCosts(model);
			dbDeliveredLogMarketModelDataManager.UpdateMarketModelPortfolio(model);
			RadGrid_DeliveredLogModels.Rebind();
			LoadFrontPage();
		}
		private void ApplyValues(string valueToApply, string textBoxName)
		{
			foreach (RepeaterItem item in Repeater_DeliveredLogModelSpecies.Items)
			{
				Repeater Repeater_DeliveredLogModelSpecies_Sorts = item.FindControl("Repeater_DeliveredLogModelSpecies_Sorts") as Repeater;
				if (Repeater_DeliveredLogModelSpecies_Sorts != null)
				{
					foreach (RepeaterItem itemSorts in Repeater_DeliveredLogModelSpecies_Sorts.Items)
					{
						TextBox tb = itemSorts.FindControl(textBoxName) as TextBox;
						tb.Text = valueToApply;

						TextBox dlp = itemSorts.FindControl("TextBox_DeliveredLogPrices") as TextBox;
						TextBox lc = itemSorts.FindControl("TextBox_LoggingCosts") as TextBox;
						TextBox hc = itemSorts.FindControl("TextBox_HaulingCosts") as TextBox;
						TextBox oha = itemSorts.FindControl("TextBox_OverheadAndAdmin") as TextBox;
						int dlpValue = 0;
						int lcValue = 0;
						int hcValue = 0;
						int ohaValue = 0;
						Int32.TryParse(dlp.Text, out dlpValue);
						Int32.TryParse(lc.Text, out lcValue);
						Int32.TryParse(hc.Text, out hcValue);
						Int32.TryParse(oha.Text, out ohaValue);

						TextBox cnv = itemSorts.FindControl("TextBox_CurrentNetValue") as TextBox;
						cnv.Text = (dlpValue - lcValue - hcValue - ohaValue).ToString();
					}
				}
			}

		}

		protected void DropDownList_StumpageModel_Changed(object sender, EventArgs e)
		{
			startingDate = Convert.ToDateTime(DropDownList_StumpageModelStartingYear.SelectedValue);
			endingDate = Convert.ToDateTime(DropDownList_StumpageModelEndingYear.SelectedValue);
			deltaN = GetDeltaN(startingDate, endingDate);

			var stumpageGroups = Repeater_StumpageGroups.Items;
			foreach (RepeaterItem stumpageGroup in stumpageGroups)
			{
				Repeater Repeater_QualityCodes = stumpageGroup.FindControl("Repeater_QualityCodes") as Repeater;
				HiddenField HiddenField_StumpageGroupID = stumpageGroup.FindControl("HiddenField_StumpageGroupID") as HiddenField;
				thisStumpageGroup = dbStumpageMarketModelDataManager.GetStumpageGroup(Convert.ToInt32(HiddenField_StumpageGroupID.Value));
				var qcs = Repeater_QualityCodes.Items;
				foreach (RepeaterItem qc in qcs)
				{
					Label Label_QualityCodeNumber = qc.FindControl("Label_QualityCodeNumber") as Label;
					TextBox TextBox_SuggestedLongevity = qc.FindControl("TextBox_SuggestedLongevity") as TextBox;
					TextBox_SuggestedLongevity.Text = deltaN.ToString();
					var qualityCode = Convert.ToInt32(Label_QualityCodeNumber.Text);
					var price = (from p in stumpagePrices where (from s in thisStumpageGroup.StumpageGroupSpecies select s.SpeciesID).Contains(p.SpeciesID) && p.MarketDate == startingDate && p.TimberQualityCode == qualityCode select p).FirstOrDefault();
					var priceCur = (from p in stumpagePrices where (from s in thisStumpageGroup.StumpageGroupSpecies select s.SpeciesID).Contains(p.SpeciesID) && p.MarketDate == endingDate && p.TimberQualityCode == qualityCode select p).FirstOrDefault();
					if (price != null && priceCur != null)
					{
						TextBox TextBox_SuggestedZone3 = qc.FindControl("TextBox_SuggestedZone3") as TextBox;
						TextBox TextBox_SuggestedZone4 = qc.FindControl("TextBox_SuggestedZone4") as TextBox;
						TextBox TextBox_SuggestedZone5 = qc.FindControl("TextBox_SuggestedZone5") as TextBox;

						SetHaulingRPA(startingDate, endingDate, price.HaulingZone3, priceCur.HaulingZone3, TextBox_SuggestedZone3);
						SetHaulingRPA(startingDate, endingDate, price.HaulingZone4, priceCur.HaulingZone4, TextBox_SuggestedZone4);
						SetHaulingRPA(startingDate, endingDate, price.HaulingZone5, priceCur.HaulingZone5, TextBox_SuggestedZone5);
					}
				}
			}
		}
		protected void Button_ApplyStumpageModel_Click(object sender, EventArgs e)
		{
			var stumpageGroups = Repeater_StumpageGroups.Items;
			startingDate = Convert.ToDateTime(DropDownList_StumpageModelStartingYear.SelectedValue);
			endingDate = Convert.ToDateTime(DropDownList_StumpageModelEndingYear.SelectedValue);
			deltaNM = GetDeltaN(startingDate, endingDate);
			foreach (RepeaterItem stumps in stumpageGroups)
			{
				Repeater Repeater_QualityCodes = stumps.FindControl("Repeater_QualityCodes") as Repeater;
				var items = Repeater_QualityCodes.Items;
				foreach (RepeaterItem item in items)
				{
					TextBox TextBox_HaulZone3 = item.FindControl("TextBox_HaulZone3") as TextBox;
					TextBox TextBox_HaulZone4 = item.FindControl("TextBox_HaulZone4") as TextBox;
					TextBox TextBox_HaulZone5 = item.FindControl("TextBox_HaulZone5") as TextBox;
					TextBox TextBox_SuggestedZone3 = item.FindControl("TextBox_SuggestedZone3") as TextBox;
					TextBox TextBox_SuggestedZone4 = item.FindControl("TextBox_SuggestedZone4") as TextBox;
					TextBox TextBox_SuggestedZone5 = item.FindControl("TextBox_SuggestedZone5") as TextBox;
					TextBox TextBox_NewLongevityOfRPAOrRPD= item.FindControl("TextBox_NewLongevityOfRPAOrRPD") as TextBox;
				    TextBox TextBox_SuggestedLongevity= item.FindControl("TextBox_SuggestedLongevity") as TextBox;
					TextBox_HaulZone3.Text = TextBox_SuggestedZone3.Text;
					TextBox_HaulZone4.Text = TextBox_SuggestedZone4.Text;
					TextBox_HaulZone5.Text = TextBox_SuggestedZone5.Text;
					TextBox_NewLongevityOfRPAOrRPD.Text = TextBox_SuggestedLongevity.Text;
				}
			}
		}
		protected void Repeater_StumpageGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			StumpageGroup stumpageGroup = (StumpageGroup)e.Item.DataItem;
			if (stumpageGroup != null)
			{
				Label Label_StumpageGroupName = e.Item.FindControl("Label_StumpageGroupName") as Label;
				Label Label_StumpageGroupSpeciesAbbreviations = e.Item.FindControl("Label_StumpageGroupSpeciesAbbreviations") as Label;
				HiddenField HiddenField_StumpageGroupID = e.Item.FindControl("HiddenField_StumpageGroupID") as HiddenField;
				Repeater Repeater_QualityCodes = e.Item.FindControl("Repeater_QualityCodes") as Repeater;
				thisStumpageGroup = stumpageGroup;
				var s = thisStumpageGroup.StumpageGroupSpecies.Select(uu=>uu.Specy);
				Label_StumpageGroupSpeciesAbbreviations.Text = string.Join(", ", s.Select(uu => uu.Abbreviation).Distinct());
				Label_StumpageGroupName.Text = stumpageGroup.StumpageGroupName;
				HiddenField_StumpageGroupID.Value = thisStumpageGroup.StumpageGroupID.ToString();
				

				Repeater_QualityCodes.DataSource = (from t in qualityCodes
													where (from qc in stumpageGroup.StumpageGroupQualityCodes select qc.StumpageGroupID).Contains(t.StumpageGroupID)
															  select t).OrderBy(uu => uu.QualityCodeNumber);
				Repeater_QualityCodes.DataBind();

			}
		}
		protected void Repeater_QualityCodes_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			StumpageGroupQualityCode qc = (StumpageGroupQualityCode)e.Item.DataItem;
			if (qc != null)
			{
				Label Label_QualityCodeNumber = e.Item.FindControl("Label_QualityCodeNumber") as Label;
				TextBox TextBox_NewLongevityOfRPAOrRPD = e.Item.FindControl("TextBox_NewLongevityOfRPAOrRPD") as TextBox;
				Label_QualityCodeNumber.Text = qc.QualityCodeNumber.ToString();
				HiddenField QualityCodeID = e.Item.FindControl("QualityCodeID") as HiddenField;
				QualityCodeID.Value = qc.StumpageGroupQualityCodeID.ToString();
				TextBox_NewLongevityOfRPAOrRPD.Text = deltaN.ToString();
				DateTime starting = Convert.ToDateTime(DropDownList_StumpageModelStartingYear.SelectedValue);
				DateTime ending =  Convert.ToDateTime(DropDownList_StumpageModelEndingYear.SelectedValue);
				var price = (from p in stumpagePrices where (from s in thisStumpageGroup.StumpageGroupSpecies select s.SpeciesID).Contains(p.SpeciesID) && p.MarketDate == starting select p).FirstOrDefault();
				var priceCur = (from p in stumpagePrices where (from s in thisStumpageGroup.StumpageGroupSpecies select s.SpeciesID).Contains(p.SpeciesID) && p.MarketDate == ending select p).FirstOrDefault();
				if (price != null && priceCur != null)
				{
					TextBox TextBox_HaulZone3 = e.Item.FindControl("TextBox_HaulZone3") as TextBox;
					TextBox TextBox_HaulZone4 = e.Item.FindControl("TextBox_HaulZone4") as TextBox;
					TextBox TextBox_HaulZone5 = e.Item.FindControl("TextBox_HaulZone5") as TextBox;

					SetHaulingRPA(starting, ending, price.HaulingZone3, priceCur.HaulingZone3, TextBox_HaulZone3);
					SetHaulingRPA(starting, ending, price.HaulingZone4, priceCur.HaulingZone4, TextBox_HaulZone4);
					SetHaulingRPA(starting, ending, price.HaulingZone5, priceCur.HaulingZone5, TextBox_HaulZone5);
				}
			}
		}

		protected void Repeater_StumpageGroups2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			StumpageGroup stumpageGroup = (StumpageGroup)e.Item.DataItem;
			if (stumpageGroup != null)
			{
				Label Label_StumpageGroupName = e.Item.FindControl("Label_StumpageGroupName") as Label;
				Label Label_StumpageGroupSpeciesAbbreviations = e.Item.FindControl("Label_StumpageGroupSpeciesAbbreviations") as Label;
				HiddenField HiddenField_StumpageGroupID = e.Item.FindControl("HiddenField_StumpageGroupID") as HiddenField;
				Repeater Repeater_QualityCodes2 = e.Item.FindControl("Repeater_QualityCodes2") as Repeater;
				thisStumpageGroup = stumpageGroup;
				var s = thisStumpageGroup.StumpageGroupSpecies.Select(uu => uu.Specy);
				Label_StumpageGroupSpeciesAbbreviations.Text = string.Join(", ", s.Select(uu => uu.Abbreviation).Distinct());
				Label_StumpageGroupName.Text = stumpageGroup.StumpageGroupName;
				HiddenField_StumpageGroupID.Value = thisStumpageGroup.StumpageGroupID.ToString();


				Repeater_QualityCodes2.DataSource = (from t in qualityCodes
													where (from qc in stumpageGroup.StumpageGroupQualityCodes select qc.StumpageGroupID).Contains(t.StumpageGroupID)
													select t).OrderBy(uu => uu.QualityCodeNumber);
				Repeater_QualityCodes2.DataBind();

			}
		}
		protected void Repeater_QualityCodes2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			StumpageGroupQualityCode qc = (StumpageGroupQualityCode)e.Item.DataItem;
			if (qc != null)
			{
				Label Label_QualityCodeNumber = e.Item.FindControl("Label_QualityCodeNumber") as Label;
				HiddenField QualityCodeID = e.Item.FindControl("QualityCodeID") as HiddenField;
				QualityCodeID.Value = qc.StumpageGroupQualityCodeID.ToString();
				TextBox TextBox_NewLongevityOfRPAOrRPD = e.Item.FindControl("TextBox_NewLongevityOfRPAOrRPD") as TextBox;
				Label_QualityCodeNumber.Text = qc.QualityCodeNumber.ToString();
				var price = (from p in stumpagePrices where (from s in thisStumpageGroup.StumpageGroupSpecies select s.SpeciesID).Contains(p.SpeciesID) select p).OrderByDescending(uu=>uu.MarketDate).FirstOrDefault();
				if (price != null)
				{
					TextBox TextBox_HaulZone3 = e.Item.FindControl("TextBox_HaulZone3") as TextBox;
					TextBox TextBox_HaulZone4 = e.Item.FindControl("TextBox_HaulZone4") as TextBox;
					TextBox TextBox_HaulZone5 = e.Item.FindControl("TextBox_HaulZone5") as TextBox;
					TextBox TextBox_ONA = e.Item.FindControl("TextBox_ONA") as TextBox;
					TextBox TextBox_HaulZone3CNV = e.Item.FindControl("TextBox_HaulZone3CNV") as TextBox;
					TextBox TextBox_HaulZone4CNV = e.Item.FindControl("TextBox_HaulZone4CNV") as TextBox;
					TextBox TextBox_HaulZone5CNV = e.Item.FindControl("TextBox_HaulZone5CNV") as TextBox;
					TextBox TextBox_PR = e.Item.FindControl("TextBox_PR") as TextBox;
					TextBox TextBox_Notes = e.Item.FindControl("TextBox_Notes") as TextBox;
					TextBox_HaulZone3.Text = price.HaulingZone3.ToString("N2");
					TextBox_HaulZone4.Text = price.HaulingZone4.ToString("N2");
					TextBox_HaulZone5.Text = price.HaulingZone5.ToString("N2");

					TextBox_HaulZone3.CssClass = "Allocate_" + qc.StumpageGroupQualityCodeID.ToString() + "_" + 3;
					TextBox_HaulZone4.CssClass = "Allocate_" + qc.StumpageGroupQualityCodeID.ToString() + "_" + 4;
					TextBox_HaulZone5.CssClass = "Allocate_" + qc.StumpageGroupQualityCodeID.ToString() + "_" + 5;

					TextBox_HaulZone3.Attributes.Add("onkeyup", "setStumpageNetValue('" + TextBox_HaulZone3.ClientID + "','" + TextBox_ONA.ClientID + "','" + TextBox_HaulZone3CNV.ClientID + "');");
					TextBox_HaulZone4.Attributes.Add("onkeyup", "setStumpageNetValue('" + TextBox_HaulZone4.ClientID + "','" + TextBox_ONA.ClientID + "','" + TextBox_HaulZone4CNV.ClientID + "');");
					TextBox_HaulZone5.Attributes.Add("onkeyup", "setStumpageNetValue('" + TextBox_HaulZone5.ClientID + "','" + TextBox_ONA.ClientID + "','" + TextBox_HaulZone5CNV.ClientID + "');");
					TextBox_HaulZone3CNV.Attributes.Add("onkeyup", "setStumpageNetValue('" + TextBox_HaulZone3.ClientID + "','" + TextBox_ONA.ClientID + "','" + TextBox_HaulZone3CNV.ClientID + "');");
					TextBox_HaulZone4CNV.Attributes.Add("onkeyup", "setStumpageNetValue('" + TextBox_HaulZone4.ClientID + "','" + TextBox_ONA.ClientID + "','" + TextBox_HaulZone4CNV.ClientID + "');");
					TextBox_HaulZone5CNV.Attributes.Add("onkeyup", "setStumpageNetValue('" + TextBox_HaulZone5.ClientID + "','" + TextBox_ONA.ClientID + "','" + TextBox_HaulZone5CNV.ClientID + "');");
					TextBox_ONA.Attributes.Add("onkeyup", "setAllStumpageNetValues('" + TextBox_ONA.ClientID + "','" + TextBox_HaulZone3.ClientID + "','" + TextBox_HaulZone3CNV.ClientID + "', '" + TextBox_HaulZone4.ClientID + "','" + TextBox_HaulZone4CNV.ClientID + "', '" + TextBox_HaulZone5.ClientID + "','" + TextBox_HaulZone5CNV.ClientID + "');");
				}
			}
		}
		private void SetHaulingRPA(DateTime starting, DateTime ending, Decimal startPrice, Decimal currentPrice, TextBox tb)
		{
			try
			{
				var f = GetStumpageModelF(starting, ending);
				var val3 = GetStumpagePriceValue(startPrice, f);
				var rpa3 = GetStumpageRPA(currentPrice, val3);
				tb.Text = rpa3.ToString("N5");
			}
			catch
			{
				tb.Text = "";
			}
		}

		private decimal GetDeltaN(DateTime start, DateTime end)
		{
			decimal dateEnd = end.Year;
			decimal dateStart = start.Year;
			if (end.Month == 7)
			{
				dateEnd = dateEnd + .5M;
			}
			if (start.Month == 7)
			{
				dateStart = dateStart + .5M;
			}
			return dateEnd - dateStart;
		}
		private Decimal GetAveragePPI(DateTime Starting, DateTime Ending)
		{
			var ppis = (from data in marketModelData
						where data.Period != 13 && ((data.ReportDate >= Starting) && (data.ReportDate < Ending))
						&& data.MarketModelTypeID == 3
					   select data).ToList();
			if (ppis.Count() > 0)
			{
				return ppis.Average(uu => uu.Value);
			}
			else
			{
				return 0;
			}
		}
		private Decimal GetStumpagePriceValue(decimal stumpagePrice, decimal f)
		{
			var insideBrackets = 1 + f;
			var powered = Math.Pow(Convert.ToDouble(insideBrackets), Convert.ToDouble(deltaN));
			var val = stumpagePrice * Convert.ToDecimal(powered);
			return val;
		}
		private Decimal GetStumpageRPA(decimal currentValue, decimal adjustValue)
		{
			Decimal val = CalculateInflation(deltaN, currentValue, adjustValue);
			return val;
		}
		private decimal GetStumpageModelF(DateTime Starting, DateTime Ending)
		{

			var startingAveragePPI = GetAveragePPI(GetStumpageSpanStartDate(Starting), Starting);
			var endingAveragePPI = GetAveragePPI(GetStumpageSpanStartDate(Ending), Ending);

			var f = CalculateInflation(deltaN, endingAveragePPI, startingAveragePPI);
			return f;
		}
		private DateTime GetStumpageSpanStartDate(DateTime dateTime)
		{
			DateTime returnDate;
			if (dateTime.Month == 7)
			{
				returnDate = new DateTime(dateTime.Year, 1, 1);
			}
			else
			{
				returnDate = new DateTime(dateTime.Year - 1, 7, 1);
			}
			return returnDate;
		}

		protected void Button_ONAApply_Click(object sender, EventArgs e)
		{
			Decimal onaValue = 0;
			Decimal.TryParse(TextBox_ONAApply.Text.Trim(), out onaValue);
			ApplyStumpageValues(onaValue.ToString("N2"), "TextBox_ONA", true);
		}
		protected void Button_PNRApply_Click(object sender, EventArgs e)
		{
			Decimal prValue = 0;
			Decimal.TryParse(TextBox_PNRApply.Text.Trim(), out prValue);
			ApplyStumpageValues(prValue.ToString("N2"), "TextBox_PR", false);
		}
		protected void Button_StumpageNotesApply_Click(object sender, EventArgs e)
		{
			ApplyStumpageValues(TextBox_StumpageNotesApply.Text.Trim(), "TextBox_Notes", false);
		}
		private void ApplyStumpageValues(string valueToApply, string textBoxName, bool recalc)
		{
			foreach (RepeaterItem items in Repeater_StumpageGroups2.Items)
			{
				Repeater Repeater_QualityCodes2 = items.FindControl("Repeater_QualityCodes2") as Repeater;
				if (Repeater_QualityCodes2 != null)
				{
					foreach (RepeaterItem item in Repeater_QualityCodes2.Items)
					{
						TextBox tb = item.FindControl(textBoxName) as TextBox;
						tb.Text = valueToApply;
						if (recalc)
						{
							RecalculateStumpage(item);
						}
					}
				}
			}
		}
		private void RecalculateStumpage(RepeaterItem item)
		{
			TextBox TextBox_HaulZone3 = item.FindControl("TextBox_HaulZone3") as TextBox;
			TextBox TextBox_HaulZone4 = item.FindControl("TextBox_HaulZone4") as TextBox;
			TextBox TextBox_HaulZone5 = item.FindControl("TextBox_HaulZone5") as TextBox;
			TextBox TextBox_ONA = item.FindControl("TextBox_ONA") as TextBox;
			TextBox TextBox_HaulZone3CNV = item.FindControl("TextBox_HaulZone3CNV") as TextBox;
			TextBox TextBox_HaulZone4CNV = item.FindControl("TextBox_HaulZone4CNV") as TextBox;
			TextBox TextBox_HaulZone5CNV = item.FindControl("TextBox_HaulZone5CNV") as TextBox;

			decimal haul3Value = 0;
			decimal haul4Value = 0;
			decimal haul5Value = 0;
			decimal ohaValue = 0;
			Decimal.TryParse(TextBox_HaulZone3.Text, out haul3Value);
			Decimal.TryParse(TextBox_HaulZone4.Text, out haul4Value);
			Decimal.TryParse(TextBox_HaulZone5.Text, out haul5Value);
			Decimal.TryParse(TextBox_ONA.Text, out ohaValue);

			Decimal newValue3 = haul3Value - ohaValue;
			TextBox_HaulZone3CNV.Text = newValue3.ToString("N2");
			Decimal newValue4 = haul4Value - ohaValue;
			TextBox_HaulZone4CNV.Text = newValue4.ToString("N2");
			Decimal newValue5 = haul5Value - ohaValue;
			TextBox_HaulZone5CNV.Text = newValue5.ToString("N2");
		}

		protected void Button_SaveMarketStumpageModel_Click(object sender, EventArgs e)
		{
			if (TextBox_StumpageModelName.Text.Trim().Length > 0)
			{
				StumpageModelPortfolio model = new StumpageModelPortfolio();
				model.UserID = thisUser.UserID;
				model.PortfolioName = TextBox_StumpageModelName.Text.Trim();
				SaveInflationDetails(model);
				SaveMarketModelCosts(model);
				SaveStumpageModelRPAData(model);
				SaveStumpageModelValues(model);
				dbStumpageMarketModelDataManager.AddNewStumpageModelPortfolio(model);
				TextBox_StumpageModelName.Text = "";
				TextBox_StumpageNotesApply.Text = "";
				TextBox_ONAApply.Text = "";
				TextBox_ProfitAndRiskApply.Text = "";
				LoadSavedModels();
				LoadFrontPage();
				TextBox_DeliveredLogModelName.BackColor = System.Drawing.Color.White;
			}
			else
			{
				TextBox_DeliveredLogModelName.BackColor = System.Drawing.Color.Red;
			}
		}
		protected void Button_SaveEditsToStumpageModel_Click(object sender, EventArgs e)
		{
			StumpageModelPortfolio model = dbStumpageMarketModelDataManager.GetStumpageModelPortfolio(Convert.ToInt32(HiddenField_PortfolioID.Value));
			model.PortfolioName = TextBox_StumpageModelName.Text.Trim();
			SaveInflationDetails(model);
			SaveMarketModelCosts(model);
			SaveStumpageModelRPAData(model);
			SaveStumpageModelValues(model);
			dbStumpageMarketModelDataManager.UpdateStumpageModelPortfolio(model);
			TextBox_StumpageModelName.Text = "";
			TextBox_StumpageNotesApply.Text = "";
			TextBox_ONAApply.Text = "";
			TextBox_ProfitAndRiskApply.Text = "";
			LoadSavedModels();
			LoadFrontPage();
		}

		private void SaveInflationDetails(StumpageModelPortfolio model)
		{

			StumpageModelPortfolioInflationDetail inflation;
			if (model.StumpageModelPortfolioInflationDetails == null)
			{
				inflation = new StumpageModelPortfolioInflationDetail();
			}
			else
			{
				inflation = model.StumpageModelPortfolioInflationDetails;
			}
			inflation.BeginningYear = Convert.ToInt32(DropDownList_BeginningYear.SelectedValue);
			inflation.EndingYear = Convert.ToInt32(DropDownList_EndingYear.SelectedValue);
			decimal cpi = 0;
			decimal ppi = 0;
			Decimal.TryParse(TextBox_CPIRate.Text.Trim(), out cpi);
			Decimal.TryParse(TextBox_PPIRate.Text.Trim(), out ppi);
			inflation.LandownerDiscountRate = Convert.ToDecimal(cpi);
			inflation.InflationRate = Convert.ToDecimal(ppi);
			model.StumpageModelPortfolioInflationDetails = inflation;
		}
		private void SaveMarketModelCosts(StumpageModelPortfolio model)
		{
			StumpageModelPortfolioCost costs;
			if (model.StumpageModelPortfolioCosts == null)
			{
				costs = new StumpageModelPortfolioCost();
			}
			else
			{
				costs = model.StumpageModelPortfolioCosts;
			}
			int reforeststationCost = 0;
			decimal accessFeeRock = 0;
			decimal accessFeeTimber = 0;
			decimal maintenanceFeeRock = 0;
			decimal maintenanceFeeTimber = 0;
			int roadConstructionCost = 0;
			Int32.TryParse(TextBox_ReforestationCosts.Text.Trim(), out reforeststationCost);
			Decimal.TryParse(TextBox_AccessFeeRock.Text.Trim(), out accessFeeRock);
			Decimal.TryParse(TextBox_AccessFeeTimber.Text.Trim(), out accessFeeTimber);
			Decimal.TryParse(TextBox_MaintenanceFeeRock.Text.Trim(), out maintenanceFeeRock);
			Decimal.TryParse(TextBox_MaintenanceFeeTimber.Text.Trim(), out maintenanceFeeTimber);
			Int32.TryParse(TextBox_LoggingRoadConstructionCost.Text.Trim(), out roadConstructionCost);
			costs.ReforestationCost = reforeststationCost;
			costs.AccessFeeRock = accessFeeRock;
			costs.AccessFeeTimber = accessFeeTimber;
			costs.MaintenanceFeeRockHaul = maintenanceFeeRock;
			costs.MaintenanceFeeTimberHaul = maintenanceFeeTimber;
			costs.RoadConstructionCosts = roadConstructionCost;
			model.StumpageModelPortfolioCosts = costs;
		}
		private void SaveStumpageModelRPAData(StumpageModelPortfolio model)
		{
			var datas = from m in model.StumpageModelPortfolioRPADatas select m;
			foreach (RepeaterItem item in Repeater_StumpageGroups.Items)
			{
				HiddenField HiddenField_StumpageGroupID = item.FindControl("HiddenField_StumpageGroupID") as HiddenField;
				var stumpageGroupID = Convert.ToInt32(HiddenField_StumpageGroupID.Value);
				Repeater Repeater_QualityCodes = item.FindControl("Repeater_QualityCodes") as Repeater;
				if (Repeater_QualityCodes != null)
				{
					foreach (RepeaterItem qcs in Repeater_QualityCodes.Items)
					{
						HiddenField QualityCodeID = qcs.FindControl("QualityCodeID") as HiddenField;
						TextBox TextBox_HaulZone3 = qcs.FindControl("TextBox_HaulZone3") as TextBox;
						TextBox TextBox_HaulZone4 = qcs.FindControl("TextBox_HaulZone4") as TextBox;
						TextBox TextBox_HaulZone5 = qcs.FindControl("TextBox_HaulZone5") as TextBox;
						TextBox TextBox_NewLongevityOfRPAOrRPD = qcs.FindControl("TextBox_NewLongevityOfRPAOrRPD") as TextBox;
						StumpageModelPortfolioRPAData data;
						var vals = (from l in datas where l.StumpageGroupID == stumpageGroupID && l.StumpageGroupQualityCodeID == Convert.ToInt32(QualityCodeID.Value) select l).FirstOrDefault();
						if (vals != null)
						{
							data = vals;
						}
						else
						{
							data = new StumpageModelPortfolioRPAData();
						}
						data.BeginningReport = Convert.ToDateTime(DropDownList_StumpageModelStartingYear.SelectedValue);
						data.EndingReport = Convert.ToDateTime(DropDownList_StumpageModelEndingYear.SelectedValue);
						data.StumpageGroupID = stumpageGroupID;
						data.StumpageGroupQualityCodeID = Convert.ToInt32(QualityCodeID.Value);
						decimal haul3;
						decimal haul4;
						decimal haul5;
						decimal longevity;
						if (Decimal.TryParse(TextBox_HaulZone3.Text.Trim(), out haul3))
						{
							data.Haul3 = haul3;
						}
						if (Decimal.TryParse(TextBox_HaulZone4.Text.Trim(), out haul4))
						{
							data.Haul4 = haul4;
						}
						if (Decimal.TryParse(TextBox_HaulZone5.Text.Trim(), out haul5))
						{
							data.Haul5 = haul5;
						}
						if (Decimal.TryParse(TextBox_NewLongevityOfRPAOrRPD.Text.Trim(), out longevity))
						{
							data.Longevity = longevity;
						}
						if (vals == null)
						{
							model.StumpageModelPortfolioRPADatas.Add(data);
						}
					}
				}
			}			
		}
		private void SaveStumpageModelValues(StumpageModelPortfolio model)
		{
			var datas = from m in model.StumpageModelPortfolioValues select m;
			foreach (RepeaterItem item in Repeater_StumpageGroups2.Items)
			{
				HiddenField HiddenField_StumpageGroupID = item.FindControl("HiddenField_StumpageGroupID") as HiddenField;
				var stumpageGroupID = Convert.ToInt32(HiddenField_StumpageGroupID.Value);
				Repeater Repeater_QualityCodes = item.FindControl("Repeater_QualityCodes2") as Repeater;
				if (Repeater_QualityCodes != null)
				{
					foreach (RepeaterItem qcs in Repeater_QualityCodes.Items)
					{
						HiddenField QualityCodeID = qcs.FindControl("QualityCodeID") as HiddenField;
						TextBox TextBox_HaulZone3 = qcs.FindControl("TextBox_HaulZone3") as TextBox;
						TextBox TextBox_HaulZone4 = qcs.FindControl("TextBox_HaulZone4") as TextBox;
						TextBox TextBox_HaulZone5 = qcs.FindControl("TextBox_HaulZone5") as TextBox;
						TextBox TextBox_ONA = qcs.FindControl("TextBox_ONA") as TextBox;
						TextBox TextBox_HaulZone3CNV = qcs.FindControl("TextBox_HaulZone3CNV") as TextBox;
						TextBox TextBox_HaulZone4CNV = qcs.FindControl("TextBox_HaulZone4CNV") as TextBox;
						TextBox TextBox_HaulZone5CNV = qcs.FindControl("TextBox_HaulZone5CNV") as TextBox;
						TextBox TextBox_PR = qcs.FindControl("TextBox_PR") as TextBox;
						TextBox TextBox_Notes = qcs.FindControl("TextBox_Notes") as TextBox;
						StumpageModelPortfolioValue value;
						var vals = (from l in datas where l.StumpageGroupID == stumpageGroupID && l.StumpageGroupQualityCodeID == Convert.ToInt32(QualityCodeID.Value) select l).FirstOrDefault();
						if (vals != null)
						{
							value = vals;
						}
						else
						{
							value = new StumpageModelPortfolioValue();
						}
						value.StumpageGroupID = stumpageGroupID;
						value.StumpageGroupQualityCodeID = Convert.ToInt32(QualityCodeID.Value);
						decimal haul3;
						decimal haul4;
						decimal haul5;
						decimal ona;
						decimal haul3cnv;
						decimal haul4cnv;
						decimal haul5cnv;
						decimal pr;
						value.Notes = TextBox_Notes.Text.Trim();
						if (Decimal.TryParse(TextBox_HaulZone3.Text.Trim(), out haul3))
						{
							value.Haul3 = haul3;
						}
						if (Decimal.TryParse(TextBox_HaulZone4.Text.Trim(), out haul4))
						{
							value.Haul4 = haul4;
						}
						if (Decimal.TryParse(TextBox_HaulZone5.Text.Trim(), out haul5))
						{
							value.Haul5 = haul5;
						}
						if (Decimal.TryParse(TextBox_ONA.Text.Trim(), out ona))
						{
							value.OverheadAndAdmin = ona;
						}
						if (Decimal.TryParse(TextBox_HaulZone3CNV.Text.Trim(), out haul3cnv))
						{
							value.Haul3CurrentNetValue = haul3cnv;
						}
						if (Decimal.TryParse(TextBox_HaulZone4CNV.Text.Trim(), out haul4cnv))
						{
							value.Haul4CurrentNetValue = haul4cnv;
						}
						if (Decimal.TryParse(TextBox_HaulZone5CNV.Text.Trim(), out haul5cnv))
						{
							value.Haul5CurrentNetValue = haul5cnv;
						}
						if (Decimal.TryParse(TextBox_PR.Text.Trim(), out pr))
						{
							value.ProfitAndRisk = pr;
						}
						if (vals == null)
						{
							model.StumpageModelPortfolioValues.Add(value);
						}
					}
				}
			}	
		}

		private void BlankCostRealPriceAppreciationLabels()
		{
			//TextBox_RPA.Text = "";
		}

		protected void Repeater_LogMarketReportSpecies_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			LogMarketReportSpecy logMarketReportSpecy = (LogMarketReportSpecy)e.Item.DataItem;
			if (logMarketReportSpecy != null)
			{
				Label Label_LogMarketSpecies = e.Item.FindControl("Label_LogMarketSpecies") as Label;
				HiddenField HiddenField_LogMarketReportSpeciesID = e.Item.FindControl("HiddenField_LogMarketReportSpeciesID") as HiddenField;


				Repeater Repeater_Sorts = e.Item.FindControl("Repeater_Sorts") as Repeater;
				thisLogMarketReportSpecy = logMarketReportSpecy;

				Label_LogMarketSpecies.Text = logMarketReportSpecy.LogMarketSpecies;
				HiddenField_LogMarketReportSpeciesID.Value = thisLogMarketReportSpecy.LogMarketReportSpeciesID.ToString();
				Repeater_Sorts.DataSource = (from t in timberMarkets
											 where (from ts in logMarketReportSpecy.LogMarketReportSpeciesMarkets select ts.TimberMarketID).Contains(t.TimberMarketID)
											 select t).OrderBy(uu => uu.OrderID);
				Repeater_Sorts.DataBind();

			}
		}
		protected void Repeater_Sorts_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			TimberMarket timberMarket = (TimberMarket)e.Item.DataItem;
			if (timberMarket != null)
			{
				HiddenField HiddenField_TimberMarketID = e.Item.FindControl("HiddenField_TimberMarketID") as HiddenField;
				HiddenField_TimberMarketID.Value = timberMarket.TimberMarketID.ToString();
				Label Label_MarketedAs = e.Item.FindControl("Label_MarketedAs") as Label;
				Label_MarketedAs.Text = thisLogMarketReportSpecy.LogMarketSpeciesAbbreviations;
				Label Label_IncludedSpecies = e.Item.FindControl("Label_IncludedSpecies") as Label;
				var s = thisLogMarketReportSpecy.LogMarketReportSpeciesSpecies.Select(uu => uu.Specy);
				Label_IncludedSpecies.Text = string.Join(", ", s.Select(uu => uu.Abbreviation).Distinct());
				Label Label_Sort = e.Item.FindControl("Label_Sort") as Label;
				Label_Sort.Text = timberMarket.Market;
				TextBox TextBox_RealPriceAppreciationOrDepreciation = e.Item.FindControl("TextBox_RealPriceAppreciationOrDepreciation") as TextBox;
				TextBox TextBox_LongevityOfRPAOrRPD = e.Item.FindControl("TextBox_LongevityOfRPAOrRPD") as TextBox;
				TextBox TextBox_NewLongevityOfRPAOrRPD = e.Item.FindControl("TextBox_NewLongevityOfRPAOrRPD") as TextBox;
				TextBox TextBox_HistoricValues = e.Item.FindControl("TextBox_HistoricValues") as TextBox;
				Button Button_ApplyRow = e.Item.FindControl("Button_ApplyRow") as Button;
				Button_ApplyRow.CommandArgument = thisLogMarketReportSpecy.LogMarketReportSpeciesID.ToString() + "|" + timberMarket.TimberMarketID.ToString();
				var historicPrice = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID && uu.Year == startingDate.Year && uu.Month == startingDate.Month).FirstOrDefault();
				var actualMarketValue = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID && uu.Year == endingDate.Year && uu.Month == endingDate.Month).FirstOrDefault();
				if (historicPrice != null && actualMarketValue != null)
				{
					try
					{
						var f = GetNominalDeliveredLogPrices(historicPrice.Price, actualMarketValue.Price, deltaNM);
						TextBox_HistoricValues.Text = "";
						if (f != 0)
						{
							TextBox_HistoricValues.Text = f.ToString("N4");
							TextBox_RealPriceAppreciationOrDepreciation.Text = f.ToString("N4");
						}
					}
					catch
					{
						TextBox_HistoricValues.Text = "";
					}
				}
				TextBox_LongevityOfRPAOrRPD.Text = deltaNM.ToString("N2");
				TextBox_NewLongevityOfRPAOrRPD.Text = deltaNM.ToString("N2");
			}
		}

		protected void RadGrid_DeliveredLogModels_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			RadGrid_DeliveredLogModels.DataSource = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolios(thisUser); ;
		}

		protected void RadGrid_RPAPortfolios_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			RadGrid_RPAPortfolios.DataSource = dbRPAPortfolioDataManager.GetRPAPortfolios(thisUser);
		}

		protected void RadGrid_DeliveredLogModels_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				var model = e.Item.DataItem as IModelShare;
				var LinkButton_MarketModel = e.Item.FindControl("LinkButton_MarketModel") as LinkButton;
				var Button_DeleteMarketModel = e.Item.FindControl("Button_DeleteMarketModel") as Button;
				var Button_ShareMarketModel = e.Item.FindControl("Button_ShareMarketModel") as Button;
				LinkButton_MarketModel.Text = model.PortfolioName;
				LinkButton_MarketModel.CommandArgument = model.ModelID.ToString();
				Button_DeleteMarketModel.CommandArgument = model.ModelID.ToString();
				Button_ShareMarketModel.Attributes.Add("onclick", "radopen('/Popups/SharePortfolio.aspx?type=1&id=" + model.ModelID.ToString() + "','ShareModel'); return false;");
			}
		}

		protected void RadGrid_RPAPortfolios_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				var model = e.Item.DataItem as IModelShare;
				var LinkButton_RPAPortfolio = e.Item.FindControl("LinkButton_RPAPortfolio") as LinkButton;
				var Button_DeleteRPAPortfolio = e.Item.FindControl("Button_DeleteRPAPortfolio") as Button;
				var Button_ShareRPAPortfolio = e.Item.FindControl("Button_ShareRPAPortfolio") as Button;
				LinkButton_RPAPortfolio.Text = model.PortfolioName;
				LinkButton_RPAPortfolio.CommandArgument = model.ModelID.ToString();
				Button_DeleteRPAPortfolio.CommandArgument = model.ModelID.ToString();
				Button_ShareRPAPortfolio.Attributes.Add("onclick", "radopen('/Popups/SharePortfolio.aspx?type=2&id=" + model.ModelID.ToString() + "','ShareModel'); return false;");
			}
		}
			
	}
}