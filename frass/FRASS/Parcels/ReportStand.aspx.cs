using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.BLL.Models;
using FRASS.BLL.Formulas;
using FRASS.Interfaces;

namespace FRASS.WebUI.Parcels
{
	public partial class ReportStand : System.Web.UI.Page
	{
		Int32 parcelID;
		Int32 portfolioID;
		Int32 rpaPortfolioID;
		User thisUser;
		MarketModelPortfolio portfolio;
		RPAPortfolio rpaPortfolio;
		List<RPAPortfolioDetail> RPAPortfolioDetails;
		List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails;
		Parcel parcel;
		Int32 standID;
		decimal acres;
		SEV sev;
		R2 r2;
		R1 r1;
		Int32 TimberGradeID;
		LogMarketReportSpecy LogMarketSpecies;
		List<TimberGrade> TimberGrades;
		List<TimberMarket> TimberMarkets;
		List<LogMarketReportSpecy> LogMarketReportSpecies;
		EconVariables EconVariables;
		RotationGenerator rg;
		GrowthCuts gcs;
		List<int> currentStandYears;
		decimal maxValue;
		decimal overallMax;
		StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		RPAPortfolioDataManager dbRPAPortfolioDataManager;
		ParcelDataManager dbParcelDataManager;
		TimberDataManager dbTimberDataManager;
		StandDataManager dbStandDataManager;

		private DeliveredLogMarketModelDataManager _dbDeliveredLogMarketModel;
		private DeliveredLogMarketModelDataManager dbDeliveredLogMarketModel
		{
			get
			{
				if (_dbDeliveredLogMarketModel == null)
				{
					_dbDeliveredLogMarketModel = DeliveredLogMarketModelDataManager.GetInstance();
				}
				return _dbDeliveredLogMarketModel;
			}
		}

		private RPARealValue _rpaREAL;
		private RPARealValue RPAREAL
		{
			get { return _rpaREAL ?? (_rpaREAL = new RPARealValue()); }
		}
		private List<MarketModelData> _mmd;
		private List<MarketModelData> MarketModelDataList
		{
			get { return _mmd ?? (_mmd = dbDeliveredLogMarketModelDataManager.GetMarketModelData().Where(uu=>uu.MarketModelTypeID==3).ToList<MarketModelData>()); }
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
			if (!Page.IsPostBack)
			{
				LoadRotation1();
				Label_Parcel.Text = parcel.ParcelNumber;
				Label_Stand.Text = standID.ToString();
				Label_MarketModel.Text = portfolio.PortfolioName;
				Label_RPAPortfolio.Text = rpaPortfolio.PortfolioName;
			}
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			dbRPAPortfolioDataManager = RPAPortfolioDataManager.GetInstance();

			dbParcelDataManager = ParcelDataManager.GetInstance();
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbStandDataManager = StandDataManager.GetInstance();

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
			if (Int32.TryParse(Request.QueryString["StandID"].ToString(), out tmpInt))
			{
				standID = tmpInt;
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
		protected void RadTabStrip1_TabClick(object sender, RadTabStripEventArgs e)
		{
			Int32 index = e.Tab.Index;
			switch (index)
			{
				case 0:
					LoadRotation1();
					break;
				case 1:
					LoadRotation2(0);
					break;
				case 2:
					LoadSEV();
					break;
				case 3:
					LoadRedux();
					break;
			}
		}

		private void LoadBasics()
		{
			TimberGrades = dbTimberDataManager.GetTimberGrades();
			TimberMarkets = dbTimberDataManager.GetTimberMarkets();
			LogMarketReportSpecies = dbTimberDataManager.GetLogMarketReportSpecies();
			portfolio = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(portfolioID);
			rpaPortfolio = dbRPAPortfolioDataManager.GetRPAPortfolio(rpaPortfolioID);
			RPAPortfolioDetails = rpaPortfolio.RPAPortfolioDetails.ToList<RPAPortfolioDetail>();
			MarketModelPortfolioDeliveredLogModelDetails = portfolio.MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
			parcel = dbParcelDataManager.GetParcel(parcelID);
			acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standID && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.Acres).FirstOrDefault();
			EconVariables = new EconVariables(portfolio, rpaPortfolio);
		}

		private void LoadSEV()
		{
			LoadBasics();
			sev = new SEV(portfolio, rpaPortfolio,RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, parcel, standID, EconVariables);
			var years = new List<string>();
			for (var year = 5; year <= 200; year += 5)
			{
				years.Add(year.ToString());
			}

			Repeater_Years.DataSource = years;
			Repeater_Years.DataBind();

			Repeater_Species.DataSource = LogMarketReportSpecies;
			Repeater_Species.DataBind();

			Repeater_TotalFutureValue.DataSource = years;
			Repeater_TotalFutureValue.DataBind();

			Repeater_CurrentSEVValue.DataSource = years;
			Repeater_CurrentSEVValue.DataBind();
			CalculateRotaionOptimum();
		}
		private void LoadRotation1()
		{
			LoadBasics();
			r1 = new R1(portfolio, rpaPortfolio, parcel, standID, EconVariables);

			currentStandYears = dbStandDataManager.GetCurrentStandSortYears();

			Repeater_Years_R1.DataSource = currentStandYears;
			Repeater_Years_R1.DataBind();

			Repeater_Species_R1.DataSource = LogMarketReportSpecies;
			Repeater_Species_R1.DataBind();

			Repeater_ValueCosts_R1.DataSource = currentStandYears;
			Repeater_ValueCosts_R1.DataBind();

			Repeater_FutureCosts_R1.DataSource = currentStandYears;
			Repeater_FutureCosts_R1.DataBind();

			Repeater_PeriodicNetRevenue_R1.DataSource = currentStandYears;
			Repeater_PeriodicNetRevenue_R1.DataBind();

			Repeater_AcreFV_R1.DataSource = currentStandYears;
			Repeater_AcreFV_R1.DataBind();

			Repeater_NPV_R1.DataSource = currentStandYears;
			Repeater_NPV_R1.DataBind();
			CalculateR1();

		}
		private void LoadRotation2(int offset)
		{
			LoadBasics();
			var years = new List<string>();
			var rg = new RotationGenerator(portfolio, rpaPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, parcel, standID, EconVariables);
			r2 = rg.GetR2(offset);
			DropDownList_YearOffset.Items.Clear();
			DropDownList_YearOffset.Items.Add(new ListItem("@0", "0"));
			for (var year = 5; year <= 200; year += 5)
			{
				years.Add(year.ToString());
				DropDownList_YearOffset.Items.Add(new ListItem("@" + year.ToString(), year.ToString()));
			}
			DropDownList_YearOffset.SelectedValue = offset.ToString();

			Repeater_Years_R2.DataSource = years;
			Repeater_Years_R2.DataBind();

			Repeater_Species_R2.DataSource = LogMarketReportSpecies;
			Repeater_Species_R2.DataBind();

			Repeater_ValueCosts_R2.DataSource = years;
			Repeater_ValueCosts_R2.DataBind();

			Repeater_FutureCosts_R2.DataSource = years;
			Repeater_FutureCosts_R2.DataBind();

			Repeater_AcreFV_R2.DataSource = years;
			Repeater_AcreFV_R2.DataBind();

			Repeater_NPV_R2.DataSource = years;
			Repeater_NPV_R2.DataBind();

			CalculateR2();
		}
		private void LoadRedux()
		{
			LoadBasics();
			rg = new RotationGenerator(portfolio, rpaPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, parcel, standID, EconVariables);
			var OuterList = rg.RunGenerator(out overallMax);
			Label_SEVYears.Text = "(" + rg.SEV.GetSEVRotationOptimum().Year.ToString() + ")";
			Repeater_Grow0.DataSource = OuterList;
			Repeater_Grow0.DataBind();
		}

		protected void Repeater_Years_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Year = e.Item.FindControl("Label_Year") as Label;
			Label_Year.Text = e.Item.DataItem.ToString();
		}
		protected void Repeater_Species_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			LogMarketSpecies = (LogMarketReportSpecy)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Repeater_Sorts = e.Item.FindControl("Repeater_Sorts") as Repeater;
			Label_Species.Text = LogMarketSpecies.LogMarketSpecies;

			var timbergrades = sev.SEVItems.Where(uu => uu.LogMarketReportSpeciesID == LogMarketSpecies.LogMarketReportSpeciesID).OrderBy(uu=>uu.OrderID).Select(uu => uu.TimberGradeID).Distinct();

			Repeater_Sorts.DataSource = timbergrades;
			Repeater_Sorts.DataBind();

		}
		protected void Repeater_Sorts_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			TimberGradeID = (int)e.Item.DataItem;
			TimberGradeID = (int)e.Item.DataItem;
			var timbergrades = (from t in TimberGrades where t.TimberGradeID == TimberGradeID select t).FirstOrDefault();
			var tm = (from t in TimberMarkets where t.TimberMarketID == timbergrades.TimberMarketID select t).FirstOrDefault();

			var items = sev.SEVItems.Where(uu => uu.LogMarketReportSpeciesID == LogMarketSpecies.LogMarketReportSpeciesID && uu.TimberGradeID == TimberGradeID).ToList();

			var details2 = RPAPortfolioDetails.Where(uu => uu.TimberMarketID == timbergrades.TimberMarketID && uu.LogMarketReportSpeciesID == LogMarketSpecies.LogMarketReportSpeciesID).FirstOrDefault();
			var ppiNominalDate = MarketModelDataList.Where(uu => uu.Year == details2.BeginningDate.Year && uu.Period == details2.BeginningDate.Month).FirstOrDefault().Value;
			var realValue = RPAREAL.GetRealValueFromTodaysPPI(details2.BeginningRealValue, ppiNominalDate, ppiToday);

			if (items.Count() > 0)
			{
				var Label_Market = e.Item.FindControl("Label_Market") as Label;
				var Label_Abbreviation = e.Item.FindControl("Label_Abbreviation") as Label;
				var Label_SortCode = e.Item.FindControl("Label_SortCode") as Label;


				var Repeater_FutureValueTBL = e.Item.FindControl("Repeater_FutureValueTBL") as Repeater;

				Label_Market.Text = tm.Market;
				Label_Abbreviation.Text = timbergrades.Specy.Abbreviation;
				Label_SortCode.Text = timbergrades.SortCode;

				var vals = new List<SEVItem>();
				var details = MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
				for (var ry = 5; ry <= 200; ry += 5)
				{
					var v = (from va in items where va.Year == ry select va).FirstOrDefault();
					if (v == null)
					{
						v = new SEVItem(portfolio, rpaPortfolio, details, LogMarketSpecies.LogMarketReportSpeciesID, tm.TimberMarketID, TimberGradeID, 0, ry, tm.OrderID.Value, acres, 0, 0, 0, realValue);						
					}
					vals.Add(v);
				}
				Repeater_FutureValueTBL.DataSource = vals;
				Repeater_FutureValueTBL.DataBind();
			}
		}
		protected void Repeater_FutureValueTBL_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var sevItem = (SEVItem)e.Item.DataItem;
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			var maxValue = sev.GetMaxValue(sevItem.Year, sevItem.LogMarketReportSpeciesID, sevItem.TimberMarketID, ppiToday);
			
			var currentRotation = sevItem.GetNetFutureSEV(maxValue);
			if (currentRotation > 0)
			{
				var str = "";
				str = currentRotation.ToString("C2");
				Label_Value.Text = str;
			}
		}
		protected void Repeater_TotalFutureValue_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int year = Convert.ToInt16(e.Item.DataItem.ToString());
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			var answer = sev.GetTotalFutureValue(year);
			if (answer > 0)
			{
				Label_Value.Text = answer.ToString("C2");
			}
		}
		protected void Repeater_CurrentSEVValue_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int year = Convert.ToInt16(e.Item.DataItem.ToString());
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			var answer = sev.GetCurrentSEVValue(year);
			if (answer > 0)
			{
				Label_Value.Text = answer.ToString("C2");
			}
		}
		
		protected void Repeater_Years_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Year = e.Item.FindControl("Label_Year") as Label;
			Label_Year.Text = e.Item.DataItem.ToString();
		}
		protected void Repeater_Species_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			LogMarketSpecies = (LogMarketReportSpecy)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Repeater_Sorts_R2 = e.Item.FindControl("Repeater_Sorts_R2") as Repeater;
			Label_Species.Text = LogMarketSpecies.LogMarketSpecies;
			var timbergrades = r2.R2Items.Where(uu => uu.LogMarketReportSpeciesID == LogMarketSpecies.LogMarketReportSpeciesID).OrderBy(uu => uu.OrderID).Select(uu => uu.TimberGradeID).Distinct();

			Repeater_Sorts_R2.DataSource = timbergrades;
			Repeater_Sorts_R2.DataBind();
			
		}
		protected void Repeater_Sorts_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			TimberGradeID = (int)e.Item.DataItem;
			var timbergrades = (from t in TimberGrades where t.TimberGradeID == TimberGradeID select t).FirstOrDefault();
			var tm = (from t in TimberMarkets where t.TimberMarketID == timbergrades.TimberMarketID select t).FirstOrDefault();

			var items = r2.R2Items.Where(uu => uu.LogMarketReportSpeciesID == LogMarketSpecies.LogMarketReportSpeciesID && uu.TimberMarketID == tm.TimberMarketID).OrderBy(uu => uu.Year).ToList();
			
			if (items.Count() > 0)
			{
				var Label_Market = e.Item.FindControl("Label_Market") as Label;
				var Label_Abbreviation = e.Item.FindControl("Label_Abbreviation") as Label;
				var Label_SortCode = e.Item.FindControl("Label_SortCode") as Label;
				
				var Repeater_FutureValueTBL_R2 = e.Item.FindControl("Repeater_FutureValueTBL_R2") as Repeater;

				Label_Market.Text = tm.Market;
				Label_Abbreviation.Text = timbergrades.Specy.Abbreviation;
				Label_SortCode.Text = timbergrades.SortCode;
				var vals = new List<R2Item>();
				for (var ry = 5; ry <= 200; ry += 5)
				{
					var v = (from va in items where va.Year == ry && va.TimberGradeID == TimberGradeID select va).FirstOrDefault();		
					if (v == null)
					{
						v = new R2Item(portfolio, rpaPortfolio, LogMarketSpecies.LogMarketReportSpeciesID, tm.TimberMarketID, TimberGradeID, 0, ry, tm.OrderID.Value, acres);						
					}
					vals.Add(v);
				}
				Repeater_FutureValueTBL_R2.DataSource = vals;
				Repeater_FutureValueTBL_R2.DataBind();
			}
		}
		protected void Repeater_FutureValueTBL_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var riItem = (R2Item)e.Item.DataItem;
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			var maxValue = r2.GetMaxValue(riItem.Year, riItem.LogMarketReportSpeciesID, riItem.TimberMarketID);
			var value = riItem.Summary(maxValue);
			if (value > 0)
			{
				var str = "";
				str = value.ToString("C2");
				Label_Value.Text = str;
			}
		}
		protected void Repeater_ValueCosts_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int year = Convert.ToInt16(e.Item.DataItem.ToString());
			var Label_Value = e.Item.FindControl("Label_Value") as Label;

			decimal totals = r2.GetValueCosts(year);
						
			Label_Value.Text = "--";
			if (totals > 0)
			{
				Label_Value.Text = totals.ToString("C0");
			}
		}
		protected void Repeater_FutureCosts_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int year = Convert.ToInt16(e.Item.DataItem.ToString());
			decimal totals = r2.GetAdditionalCosts(year);
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			if (totals > 0)
			{
			    Label_Value.Text = totals.ToString("C2");
			}
		}
		protected void Repeater_AcreFV_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int year = Convert.ToInt16(e.Item.DataItem.ToString());
			decimal totals = r2.GetAcreFV(year);
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = totals.ToString("C2");
		}
		protected void Repeater_NPV_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int year = Convert.ToInt16(e.Item.DataItem.ToString());
			decimal totals = r2.GetNPV(year);
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = totals.ToString("C2");
		}

		protected void Repeater_Years_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Year = e.Item.FindControl("Label_Year") as Label;
			Label_Year.Text = e.Item.DataItem.ToString();
		}
		protected void Repeater_Species_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			LogMarketSpecies = (LogMarketReportSpecy)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Repeater_Sorts_R1 = e.Item.FindControl("Repeater_Sorts_R1") as Repeater;
			Label_Species.Text = LogMarketSpecies.LogMarketSpecies;
			var timbergrades = r1.R1Items.Where(uu => uu.LogMarketReportSpeciesID == LogMarketSpecies.LogMarketReportSpeciesID).OrderBy(uu => uu.OrderID).Select(uu => uu.TimberGradeID).Distinct();

			Repeater_Sorts_R1.DataSource = timbergrades;
			Repeater_Sorts_R1.DataBind();

		}
		protected void Repeater_Sorts_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			TimberGradeID = (int)e.Item.DataItem;
			var timbergrades = (from t in TimberGrades where t.TimberGradeID == TimberGradeID select t).FirstOrDefault();
			var logspecies = (from l in LogMarketReportSpecies where l.LogMarketReportSpeciesID == LogMarketSpecies.LogMarketReportSpeciesID select l).FirstOrDefault();
			var tm = (from t in TimberMarkets where t.TimberMarketID == timbergrades.TimberMarketID select t).FirstOrDefault();
			
			var items = r1.R1Items.Where(uu => uu.LogMarketReportSpeciesID == logspecies.LogMarketReportSpeciesID && uu.TimberMarketID == tm.TimberMarketID).OrderBy(uu => uu.Year).ToList();
			
			if (items.Count() > 0)
			{
				var Label_Market = e.Item.FindControl("Label_Market") as Label;
				var Label_Abbreviation = e.Item.FindControl("Label_Abbreviation") as Label;
				var Label_SortCode = e.Item.FindControl("Label_SortCode") as Label;

				var Repeater_FutureValueTBL_R1 = e.Item.FindControl("Repeater_FutureValueTBL_R1") as Repeater;

				Label_Market.Text = tm.Market;
				Label_Abbreviation.Text = timbergrades.Specy.Abbreviation;
				Label_SortCode.Text = timbergrades.SortCode;
				var vals = new List<R1Item>();
				var index = -5;
				foreach (var ry in currentStandYears.OrderBy(uu=>uu))
				{
					index = index + 5;
					var v = (from va in items where va.Year == ry && va.TimberGradeID == TimberGradeID select va).FirstOrDefault();
					if (v == null)
					{
						v = new R1Item(portfolio, rpaPortfolio, logspecies.LogMarketReportSpeciesID, tm.TimberMarketID, TimberGradeID, 0, ry, index, tm.OrderID.Value, acres);
					}
					vals.Add(v);
				}
				Repeater_FutureValueTBL_R1.DataSource = vals;
				Repeater_FutureValueTBL_R1.DataBind();
			}
		}
		protected void Repeater_FutureValueTBL_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var riItem = (R1Item)e.Item.DataItem;
			var firstYear = currentStandYears.Min(uu => uu);
			var year = riItem.Year - firstYear;
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			var maxValue = r1.GetMaxValue(year, riItem.LogMarketReportSpeciesID, riItem.TimberMarketID);
			var value = riItem.Summary(maxValue);
			if (value > 0)
			{
				var str = "";
				str = value.ToString("C2");
				Label_Value.Text = str;
			}
		}
		protected void Repeater_ValueCosts_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int calendarYear = Convert.ToInt16(e.Item.DataItem.ToString());
			var firstYear = currentStandYears.Min(uu => uu);
			var year = calendarYear - firstYear;
			var Label_Value = e.Item.FindControl("Label_Value") as Label;

			decimal totals = r1.GetValueCosts(calendarYear, year);

			Label_Value.Text = "--";
			if (totals > 0)
			{
				Label_Value.Text = totals.ToString("C0");
			}
		}
		protected void Repeater_FutureCosts_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int calendarYear = Convert.ToInt16(e.Item.DataItem.ToString());
			var firstYear = currentStandYears.Min(uu => uu);
			var year = calendarYear - firstYear;
			decimal totals = r1.GetAdditionalCosts(calendarYear, year);
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			if (totals > 0)
			{
				Label_Value.Text = totals.ToString("C2");
			}
		}
		protected void Repeater_PeriodicNetRevenue_R1_ItemDataBound(Object sender, RepeaterItemEventArgs e)
		{
			int calendarYear = Convert.ToInt16(e.Item.DataItem.ToString());
			var firstYear = currentStandYears.Min(uu => uu);
			var year = calendarYear - firstYear;
			decimal totals = r1.GetPeriodicNetRevenue(calendarYear, year);
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = "--";
			if (totals > 0)
			{
				Label_Value.Text = totals.ToString("C2");
			}

		}
		protected void Repeater_AcreFV_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int calendarYear = Convert.ToInt16(e.Item.DataItem.ToString());
			var firstYear = currentStandYears.Min(uu => uu);
			var year = calendarYear - firstYear;
			decimal totals = r1.GetAcreFV(calendarYear, year);
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = totals.ToString("C2");
		}
		protected void Repeater_NPV_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			int calendarYear = Convert.ToInt16(e.Item.DataItem.ToString());
			var firstYear = currentStandYears.Min(uu => uu);
			var year = calendarYear - firstYear;
			decimal totals = r1.GetNPV(calendarYear, year);
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			Label_Value.Text = totals.ToString("C2");
		}

		protected void Repeater_Grow0_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			gcs = (GrowthCuts)e.Item.DataItem;
			var Label_Title = e.Item.FindControl("Label_Title") as Label;

			Label_Title.Text = gcs.Title;
			maxValue = gcs.MaxValue;
			var Repeater_Values_Redux = e.Item.FindControl("Repeater_Values_Redux") as Repeater;
			Repeater_Values_Redux.DataSource = gcs.Cuts.OrderBy(uu => uu.Year);
			Repeater_Values_Redux.DataBind();
		}
		protected void Repeater_Values_Redux_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var item = (GrowthCut)e.Item.DataItem;
			var Label_Total = e.Item.FindControl("Label_Total") as Label;
			var Label_R1 = e.Item.FindControl("Label_R1") as Label;
			var Label_R2 = e.Item.FindControl("Label_R2") as Label;
			var Label_R2Year = e.Item.FindControl("Label_R2Year") as Label;
			var Label_SEV = e.Item.FindControl("Label_SEV") as Label;

			Label_Total.Text = item.Total.RotationOptimum.ToString("C2");
			if (overallMax == item.Total.RotationOptimum)
			{
				{
					Label_Total.ForeColor = System.Drawing.Color.Red;
					Label_Total.Font.Bold = true;
				}
			}
			else if (maxValue == item.Total.RotationOptimum)
			{
				Label_Total.ForeColor = System.Drawing.Color.Green;
				Label_Total.Font.Bold = true;
			}

			
			Label_R1.Text = item.R1.RotationOptimum.ToString("C2");
			Label_R2.Text = item.R2.RotationOptimum.ToString("C2");
			Label_R2Year.Text = item.Year.ToString("")  + " (" + (item.Year + gcs.HarvestYear).ToString() + ")";
			Label_SEV.Text = item.SEV.RotationOptimum.ToString("C2");
		}

		private void CalculateR2()
		{
			var answer = r2.GetFVMax();
			Label_FVMaxR2.Text = answer.RotationOptimum.ToString("C0");
			Label_R2.Text = answer.Year.ToString();
			HyperLink_R2.Attributes.Add("onclick", "radopen(\"/Parcels/Charts/R2Chart.aspx?RPAPortfolioID=" + rpaPortfolioID + "&MarketModelPortfolioID=" + portfolio.MarketModelPortfolioID.ToString() + "&ParcelID=" + parcelID.ToString() + "&StandID=" + standID.ToString() + "\",\"RadWindow1\"); return false;");
		}
		private void CalculateR1()
		{
			var minYear = dbStandDataManager.GetCurrentStandSortYears().Min();
			var answer = r1.GetFVMax();
			Label_FVMaxR1.Text = answer.RotationOptimum.ToString("C0");
			Label_R1.Text = (minYear + answer.Year).ToString();
			HyperLink_R1.Attributes.Add("onclick", "radopen(\"/Parcels/Charts/R1Chart.aspx?RPAPortfolioID=" + rpaPortfolioID + "&MarketModelPortfolioID=" + portfolio.MarketModelPortfolioID.ToString() + "&ParcelID=" + parcelID.ToString() + "&StandID=" + standID.ToString() + "\",\"RadWindow1\"); return false;");
		}
		private void CalculateRotaionOptimum()
		{
			var answer = sev.GetSEVRotationOptimum();
			Label_SEVRotationOptimum.Text = answer.RotationOptimum.ToString("C0");
			Label_OptimalRotationLength.Text = answer.Year.ToString();
			HyperLink_Sev.Attributes.Add("onclick", "radopen(\"/Parcels/Charts/SEVChart.aspx?RPAPortfolioID=" + rpaPortfolioID + "&MarketModelPortfolioID=" + portfolio.MarketModelPortfolioID.ToString() + "&ParcelID=" + parcelID.ToString() + "&StandID=" + standID.ToString() + "\",\"RadWindow1\"); return false;");
		}

		protected void DropDownList_YearOffset_SelectedIndexChanged(object sender, EventArgs e)
		{
			var offset = Convert.ToInt32(DropDownList_YearOffset.SelectedValue);
			LoadRotation2(offset);
		}
	}
}