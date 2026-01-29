using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Formulas;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.WebUI.MarketModel
{
	public partial class RPAPortfolioPage : System.Web.UI.Page
	{
		private User thisUser;
		private int LogMarketReportSpeciesID;
		private TimberDataManager dbTimberDataManager;
		private DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		private List<MarketModelData> MarketModelDataList;
		private List<TimberMarket> TimberMarkets;
		private LogMarketReportSpecy thisLogMarketReportSpecy;
		private DeltaNM DeltaNMCalculator;
		private RPAPortfolioDataManager dbRPAPortfolioDataManager;
		private int RPAPortfolioID;
		private RPAPortfolio Portfolio;

		private List<DateTime> _ppiDateTimes;
		private List<DateTime> PPIDateTimes
		{
			get
			{
				if (_ppiDateTimes == null)
				{
					List<DateTime> dts = new List<DateTime>();
					foreach (var h in MarketModelDataList.Where(uu => uu.Period != 13))
					{
						dts.Add(new DateTime(h.Year, h.Period, 1));
					}
					_ppiDateTimes = dts.Distinct().ToList<DateTime>();
				}
				return _ppiDateTimes;
			}
		}
		private ListItem[] GetPPIDateTimesAsc(int minYear, int minMonth, int maxYear, int maxMonth)
		{
			var listPPIDateTimesAsc = new List<ListItem>();
			foreach (var d in PPIDateTimes.Where(uu=> (uu.Year > minYear || (uu.Year == minYear && uu.Month >= minMonth)) && ((uu.Year < maxYear) || (uu.Year == maxYear && uu.Month <= maxMonth))).OrderBy(uu => uu))
			{
				listPPIDateTimesAsc.Add(new ListItem(d.ToString("MMMM") + " " + d.Year.ToString(), d.ToShortDateString()));
			}
			return listPPIDateTimesAsc.ToArray();
		}
		private ListItem[] GetPPIDateTimesDesc(int minYear, int minMonth, int maxYear, int maxMonth)
		{
			var listPPIDateTimesDesc = new List<ListItem>();
			foreach (var d in PPIDateTimes.Where(uu=> (uu.Year > minYear || (uu.Year == minYear && uu.Month >= minMonth)) && ((uu.Year < maxYear) || (uu.Year == maxYear && uu.Month <= maxMonth))).OrderByDescending(uu => uu))
			{
				listPPIDateTimesDesc.Add(new ListItem(d.ToString("MMMM") + " " + d.Year.ToString(), d.ToShortDateString()));
			}
			return listPPIDateTimesDesc.ToArray();
		}

		private RPARealValue _rpaREAL;
		private RPARealValue RPAREAL
		{
			get { return _rpaREAL ?? (_rpaREAL = new RPARealValue()); }
		}

		private decimal? _ppiToday;
		private decimal ppiToday
		{
			get
			{
				if (!_ppiToday.HasValue)
				{
					_ppiToday = MarketModelDataList.ToList<MarketModelData>().OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault().Value;
				}
				return _ppiToday.Value;
			}
		}

		private DeltaNM _deltaNM;
		private DeltaNM deltaNM
		{
			get { return _deltaNM ?? (_deltaNM = new DeltaNM()); }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				LoadLogMarketRealPriceAppreciation();
				var ppi = dbDeliveredLogMarketModelDataManager.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault();
				System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
				Label_RealValueDate1.Text = mfi.GetAbbreviatedMonthName(ppi.Period).ToUpper() + " " + ppi.Year.ToString();
				Label_RealValueDate2.Text = mfi.GetAbbreviatedMonthName(ppi.Period).ToUpper() + " " + ppi.Year.ToString();
					
			}
		}
		private void Page_Init(object sender, EventArgs e)
		{
			thisUser = Master.GetCurrentUser();
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			TimberMarkets = dbTimberDataManager.GetTimberMarkets();
			MarketModelDataList = dbDeliveredLogMarketModelDataManager.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).ToList<MarketModelData>();
			DeltaNMCalculator = new DeltaNM();
			dbRPAPortfolioDataManager = RPAPortfolioDataManager.GetInstance();
			SetRPAPortfolio();
		}

		private void SetRPAPortfolio()
		{
			if (!Int32.TryParse(Request.QueryString["id"], out RPAPortfolioID))
			{
				RPAPortfolioID = 0;
				
			}
			Button_SaveEditRPAModel.Visible = false;
			if (RPAPortfolioID > 0)
			{
				Portfolio = dbRPAPortfolioDataManager.GetRPAPortfolio(RPAPortfolioID);
				TextBox_RPAModelName.Text = Portfolio.PortfolioName;
				Button_SaveEditRPAModel.Visible = true;
			}
		}

		protected void Button_Cancel_Click(object sender, EventArgs e)
		{
			Response.Redirect("/MarketModel/Portfolios.aspx", true);
		}

		protected void Button_SaveEditRPAModel_Click(object sender, EventArgs e)
		{
			var model = dbRPAPortfolioDataManager.GetRPAPortfolio(RPAPortfolioID);
			model.PortfolioName = TextBox_RPAModelName.Text.Trim();
			model.LastEdited = DateTime.Now;
			SetRPADetails(model);
			dbRPAPortfolioDataManager.UpdateRPAPortfolio(model);
			Response.Redirect("/MarketModel/Portfolios.aspx", true);
		}

		protected void Button_SaveNewRPAModel_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(TextBox_RPAModelName.Text))
			{
				var model = new FRASS.DAL.RPAPortfolio();
				model.UserID = thisUser.UserID;
				model.CreatedByUserID = thisUser.UserID;
				model.PortfolioName = TextBox_RPAModelName.Text.Trim();
				model.LastEdited = DateTime.Now;
				SetRPADetails(model);
				dbRPAPortfolioDataManager.AddNewRPAPortfolio(model);
				Response.Redirect("/MarketModel/Portfolios.aspx", true);
			}
			else
			{
				TextBox_RPAModelName.BackColor = System.Drawing.Color.Red;
			}
		}

		private void SetRPADetails(FRASS.DAL.RPAPortfolio portfolio)
		{
			foreach (RepeaterItem item in Repeater_RPAMarketCalculations.Items)
			{
				var HiddenField_LogMarketReportSpeciesID = item.FindControl("HiddenField_LogMarketReportSpeciesID") as HiddenField;
				var Repeater_SortsRPA = item.FindControl("Repeater_SortsRPA") as Repeater;
				if (Repeater_SortsRPA != null)
				{
					foreach (RepeaterItem itemSorts in Repeater_SortsRPA.Items)
					{
						var HiddenField_TimberMarketID = itemSorts.FindControl("HiddenField_TimberMarketID") as HiddenField;
						var DropDownList_BeginningDate = itemSorts.FindControl("DropDownList_BeginningDate") as DropDownList;
						var TextBox_BeginningRealValue = itemSorts.FindControl("TextBox_BeginningRealValue") as TextBox;
						var DropDownList_EndindingDate = itemSorts.FindControl("DropDownList_EndindingDate") as DropDownList;
						var TextBox_EndingRealValue = itemSorts.FindControl("TextBox_EndingRealValue") as TextBox;
						
						var detail = new RPAPortfolioDetail();
						var vals = (from l in portfolio.RPAPortfolioDetails where l.LogMarketReportSpeciesID == Convert.ToInt32(HiddenField_LogMarketReportSpeciesID.Value) && l.TimberMarketID == Convert.ToInt32(HiddenField_TimberMarketID.Value) select l).FirstOrDefault();
						if (vals != null)
						{
							detail = vals;
						}
						
						var beginningDate = Convert.ToDateTime(DropDownList_BeginningDate.SelectedValue);
						var ppiNominalDate = MarketModelDataList.Where(uu => uu.Year == beginningDate.Year && uu.Period == beginningDate.Month).FirstOrDefault().Value;

						detail.RPAPortfolioID = portfolio.RPAPortfolioID;
						detail.LogMarketReportSpeciesID = Convert.ToInt32(HiddenField_LogMarketReportSpeciesID.Value);
						detail.TimberMarketID = Convert.ToInt32(HiddenField_TimberMarketID.Value);
						detail.BeginningDate = beginningDate;
						detail.BeginningRealValue = RPAREAL.GetNominalValueFromReal(Convert.ToDecimal(TextBox_BeginningRealValue.Text), ppiNominalDate, ppiToday);
						detail.EndingDate = Convert.ToDateTime(DropDownList_EndindingDate.SelectedValue);
						detail.EndingRealValue = RPAREAL.GetNominalValueFromReal(Convert.ToDecimal(TextBox_EndingRealValue.Text), ppiNominalDate, ppiToday);
						
						var longevity = deltaNM.GetDeltaNM(detail.BeginningDate.Year, detail.BeginningDate.Month, detail.EndingDate.Year, detail.EndingDate.Month);
						detail.RPA = RPAREAL.GetRPA(detail.BeginningRealValue, detail.EndingRealValue, longevity);
						detail.Longevity = longevity;
						
						if (vals == null)
						{
							portfolio.RPAPortfolioDetails.Add(detail);
						} 
					}
				}
			}		
		}

		private void LoadLogMarketRealPriceAppreciation()
		{
			Repeater_RPAMarketCalculations.DataSource = dbTimberDataManager.GetLogMarketReportSpecies();
			Repeater_RPAMarketCalculations.DataBind();
		}

		protected void Repeater_RPAMarketCalculations_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			thisLogMarketReportSpecy = (LogMarketReportSpecy)e.Item.DataItem;
			if (thisLogMarketReportSpecy != null)
			{
				Label Label_LogMarketSpecies = e.Item.FindControl("Label_LogMarketSpecies") as Label;
				HiddenField HiddenField_LogMarketReportSpeciesID = e.Item.FindControl("HiddenField_LogMarketReportSpeciesID") as HiddenField;

				Label_LogMarketSpecies.Text = thisLogMarketReportSpecy.LogMarketSpecies;
				HiddenField_LogMarketReportSpeciesID.Value = thisLogMarketReportSpecy.LogMarketReportSpeciesID.ToString();
				LogMarketReportSpeciesID = thisLogMarketReportSpecy.LogMarketReportSpeciesID;

				Repeater Repeater_SortsRPA = e.Item.FindControl("Repeater_SortsRPA") as Repeater;
				Repeater_SortsRPA.DataSource = (from t in TimberMarkets
												where (from ts in thisLogMarketReportSpecy.LogMarketReportSpeciesMarkets select ts.TimberMarketID).Contains(t.TimberMarketID)
												select t).OrderBy(uu => uu.OrderID);
				Repeater_SortsRPA.DataBind();
			}
		}

		protected void Repeater_SortsRPA_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var timberMarket = (TimberMarket)e.Item.DataItem;
			
			if (timberMarket != null)
			{
				var HiddenField_TimberMarketID = e.Item.FindControl("HiddenField_TimberMarketID") as HiddenField;
				var LinkButton_Sort = e.Item.FindControl("LinkButton_Sort") as LinkButton;
				var DropDownList_BeginningDate = e.Item.FindControl("DropDownList_BeginningDate") as DropDownList;
				var DropDownList_EndindingDate = e.Item.FindControl("DropDownList_EndindingDate") as DropDownList;
				var LinkButton_ViewChart = e.Item.FindControl("LinkButton_ViewChart") as LinkButton;
				var Label_RPA = e.Item.FindControl("Label_RPA") as Label;
				var Label_Longevity = e.Item.FindControl("Label_Longevity") as Label;
				var TextBox_BeginningRealValue = e.Item.FindControl("TextBox_BeginningRealValue") as TextBox;
				var TextBox_EndingRealValue = e.Item.FindControl("TextBox_EndingRealValue") as TextBox;

				HiddenField_TimberMarketID.Value = timberMarket.TimberMarketID.ToString();
				LinkButton_Sort.Text = timberMarket.Market;

				var firstDate = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID).OrderBy(uu => uu.Year).ThenBy(uu => uu.Month).FirstOrDefault();
				var lastDate = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID).OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Month).FirstOrDefault();

				DropDownList_BeginningDate.Items.AddRange(GetPPIDateTimesAsc(firstDate.Year, firstDate.Month, lastDate.Year, lastDate.Month));
				DropDownList_EndindingDate.Items.AddRange(GetPPIDateTimesDesc(firstDate.Year, firstDate.Month, lastDate.Year, lastDate.Month));

				if (Portfolio != null)
				{
					var detail = Portfolio.RPAPortfolioDetails.Where(uu=>uu.TimberMarketID == timberMarket.TimberMarketID && uu.LogMarketReportSpeciesID == thisLogMarketReportSpecy.LogMarketReportSpeciesID).FirstOrDefault();

					DropDownList_BeginningDate.SelectedValue = detail.BeginningDate.ToShortDateString();

					var ppiNominalDate = MarketModelDataList.Where(uu => uu.Year == detail.BeginningDate.Year && uu.Period == detail.BeginningDate.Month).FirstOrDefault().Value;

					TextBox_BeginningRealValue.Text = RPAREAL.GetRealValueFromTodaysPPI(detail.BeginningRealValue, ppiNominalDate, ppiToday).ToString("F2");
					DropDownList_EndindingDate.SelectedValue = detail.EndingDate.ToShortDateString();
					TextBox_EndingRealValue.Text = RPAREAL.GetRealValueFromTodaysPPI(detail.EndingRealValue, ppiNominalDate, ppiToday).ToString("F2");
					Label_RPA.Text = detail.RPA.ToString("N4");
					Label_Longevity.Text = detail.Longevity.ToString("N2");

					var clickString = string.Format("LaunchTimberMarketRPAChart('{0}','{1}','{2}','{3}', '{4}','{5}',{6},{7}); return false;", DropDownList_BeginningDate.ClientID, DropDownList_EndindingDate.ClientID, TextBox_BeginningRealValue.ClientID, TextBox_EndingRealValue.ClientID, Label_RPA.ClientID, Label_Longevity.ClientID, timberMarket.TimberMarketID, thisLogMarketReportSpecy.LogMarketReportSpeciesID);
					LinkButton_Sort.OnClientClick = clickString;
				}
				else
				{

					var startingDate = Convert.ToDateTime(DropDownList_BeginningDate.Items[0].Value);
					var endingDate = Convert.ToDateTime(DropDownList_EndindingDate.Items[0].Value);

					var historicPrice = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID && uu.Year == startingDate.Year && uu.Month == startingDate.Month).FirstOrDefault();
					var currentPrice = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID && uu.Year == endingDate.Year && uu.Month == endingDate.Month).FirstOrDefault();

					if (currentPrice == null)
					{
						currentPrice = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID).OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Month).FirstOrDefault();
						endingDate = new DateTime(currentPrice.Year, currentPrice.Month, 1);
						DropDownList_EndindingDate.SelectedValue = endingDate.ToShortDateString();
					}

					if (historicPrice == null)
					{
						historicPrice = thisLogMarketReportSpecy.HistoricLogPrices.Where(uu => uu.TimberMarketID == timberMarket.TimberMarketID).OrderBy(uu => uu.Year).ThenBy(uu => uu.Month).FirstOrDefault();
						startingDate = new DateTime(historicPrice.Year, historicPrice.Month, 1);
						DropDownList_BeginningDate.SelectedValue = startingDate.ToShortDateString();
					}
					
					var deltaNM = DeltaNMCalculator.GetDeltaNM(startingDate.Year, startingDate.Month, endingDate.Year, endingDate.Month);

					Label_RPA.Text = "N/A";
					LinkButton_Sort.Enabled = false;
					LinkButton_ViewChart.Visible = false;

					var startingPPI = MarketModelDataList.Where(uu =>  uu.Year == startingDate.Year && uu.Period == startingDate.Month).FirstOrDefault();
					var endingPPI = MarketModelDataList.Where(uu => uu.Year == endingDate.Year && uu.Period == endingDate.Month).FirstOrDefault();
					var currentPPI = MarketModelDataList.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault();

					if (endingPPI == null)
					{
						endingPPI = currentPPI;
					}

					var realPrice1 = RPAREAL.GetRPARealValue(historicPrice.Price,
									new DateTime(startingDate.Year, startingDate.Month, 1),
									new DateTime(endingDate.Year, endingDate.Month, 1),
									startingPPI.Value,
									currentPPI.Value
								);
					var realPrice2 = RPAREAL.GetRPARealValue(currentPrice.Price,
									new DateTime(startingDate.Year, startingDate.Month, 1),
									new DateTime(endingDate.Year, endingDate.Month, 1),
									endingPPI.Value,
									currentPPI.Value
									
								);

					TextBox_BeginningRealValue.Text = realPrice1.ToString("F2");
					TextBox_EndingRealValue.Text = realPrice2.ToString("F2");
					var rpa = RPAREAL.GetRPA(realPrice1, realPrice2, deltaNM);
					Label_RPA.Text = rpa.ToString("N4");
					Label_Longevity.Text = deltaNM.ToString("N2");
					var clickString = string.Format("LaunchTimberMarketRPAChart('{0}','{1}','{2}','{3}', '{4}','{5}',{6},{7}); return false;", DropDownList_BeginningDate.ClientID, DropDownList_EndindingDate.ClientID, TextBox_BeginningRealValue.ClientID, TextBox_EndingRealValue.ClientID, Label_RPA.ClientID, Label_Longevity.ClientID, timberMarket.TimberMarketID, thisLogMarketReportSpecy.LogMarketReportSpeciesID);
					LinkButton_Sort.OnClientClick = clickString;					
				}

				var funcStart = string.Format("GetPriceStart('{0}','{1}','{2}','{3}', '{4}','{5}',{6},{7});", DropDownList_BeginningDate.ClientID, DropDownList_EndindingDate.ClientID, TextBox_BeginningRealValue.ClientID, TextBox_EndingRealValue.ClientID, Label_RPA.ClientID, Label_Longevity.ClientID, timberMarket.TimberMarketID, thisLogMarketReportSpecy.LogMarketReportSpeciesID);
				var funcEnd = string.Format("GetPriceEnd('{0}','{1}','{2}','{3}', '{4}','{5}',{6},{7});", DropDownList_BeginningDate.ClientID, DropDownList_EndindingDate.ClientID, TextBox_BeginningRealValue.ClientID, TextBox_EndingRealValue.ClientID, Label_RPA.ClientID, Label_Longevity.ClientID, timberMarket.TimberMarketID, thisLogMarketReportSpecy.LogMarketReportSpeciesID);
				DropDownList_BeginningDate.Attributes.Add("onchange", funcStart);
				DropDownList_EndindingDate.Attributes.Add("onchange", funcEnd);

				var func2 = string.Format("GetRPA('{0}','{1}','{2}','{3}', '{4}','{5}',{6},{7})", DropDownList_BeginningDate.ClientID, DropDownList_EndindingDate.ClientID, TextBox_BeginningRealValue.ClientID, TextBox_EndingRealValue.ClientID, Label_RPA.ClientID, Label_Longevity.ClientID, timberMarket.TimberMarketID, thisLogMarketReportSpecy.LogMarketReportSpeciesID);
				TextBox_BeginningRealValue.Attributes.Add("onkeyup", func2);
				TextBox_EndingRealValue.Attributes.Add("onkeyup", func2);

				LinkButton_ViewChart.Visible = true;
				var func3 = string.Format("LaunchChart('{0}','{1}','{2}','{3}', '{4}','{5}',{6},{7}); return false;", DropDownList_BeginningDate.ClientID, DropDownList_EndindingDate.ClientID, TextBox_BeginningRealValue.ClientID, TextBox_EndingRealValue.ClientID, Label_RPA.ClientID, Label_Longevity.ClientID, timberMarket.TimberMarketID, thisLogMarketReportSpecy.LogMarketReportSpeciesID);
				LinkButton_ViewChart.OnClientClick = func3;
			}
		}
	}
}