using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Models;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;
using Telerik.Web.UI;

namespace FRASS.WebUI.Parcels
{
	public partial class StumpageReportStand : System.Web.UI.Page
	{
		StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		ParcelDataManager dbParcelDataManager;
		TimberDataManager dbTimberDataManager;
		StandDataManager dbStandDataManager;

		Int32 parcelID;
		Int32 portfolioID;
		User thisUser;
		StumpageModelPortfolio portfolio;
		Parcel parcel;
		Int32 standID;
		decimal acres;
		List<IStandData> currentStandData;
		List<IStandData> futureStandData;
		List<TimberMarket> timberMarkets;
		StumpageGroup stumpageGroup;
		StumpageGroupQualityCode stumpageGroupQualityCode;
		StumpageGenerator stumpageGenerator;
		Int32 minYear;
		decimal maxValue;
		GrowthCuts growthCuts;
		EconVariables EconVariables;
		List<int> currentStandYears;
		List<int> reportYears;
		decimal overallMax;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				LoadRotation1();
				Label_Parcel.Text = parcel.ParcelNumber;
				Label_Stand.Text = standID.ToString();
				Label_MarketModel.Text = portfolio.PortfolioName;
			}
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
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
			if (Int32.TryParse(Request.QueryString["StumpageModelPortfolioID"].ToString(), out tmpInt))
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
					LoadRedux(0);
					break;
			}
		}

		private void LoadBasics()
		{
			portfolio = dbStumpageMarketModelDataManager.GetStumpageModelPortfolio(portfolioID);
			parcel = dbParcelDataManager.GetParcel(parcelID);
			acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standID && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.Acres).FirstOrDefault();
			EconVariables = new EconVariables(portfolio);
			currentStandData = dbStandDataManager.GetStandDataCurrent(standID, parcelID);
			futureStandData = dbStandDataManager.GetStandDataFuture(standID, parcelID);
			timberMarkets = dbTimberDataManager.GetTimberMarkets();
			currentStandYears = dbStandDataManager.GetCurrentStandSortYears();
			minYear = currentStandYears.Min(uu => uu);
			var StumpageGroupQualityCodes = dbStumpageMarketModelDataManager.GetStumpageGroupQualityCodes();
			var stumpageGroups = dbStumpageMarketModelDataManager.GetStumpageGroups();
			var haulZoneID = parcel.ParcelHaulZones.FirstOrDefault().HaulZoneID;
			reportYears = new List<int>();
			for (var year = 5; year <= 200; year += 5)
			{
				reportYears.Add(year);
			}
			stumpageGenerator = new StumpageGenerator(parcel, standID, portfolio, stumpageGroups, currentStandData, futureStandData, timberMarkets, haulZoneID, minYear, EconVariables, StumpageGroupQualityCodes, currentStandYears, reportYears);
		}
		private void LoadRotation1()
		{
			LoadBasics();

			Repeater_Years_R1.DataSource = currentStandYears;
			Repeater_Years_R1.DataBind();

			Repeater_Species_R1.DataSource = dbStumpageMarketModelDataManager.GetStumpageGroups().OrderBy(uu => uu.StumpageGroupName);
			Repeater_Species_R1.DataBind();

			Repeater_Stand.DataSource = currentStandYears;
			Repeater_Stand.DataBind();

			Repeater_NetFV.DataSource = currentStandYears;
			Repeater_NetFV.DataBind();

			Repeater_NPV.DataSource = currentStandYears;
			Repeater_NPV.DataBind();

			CalculateR1();
		}
		private void LoadRotation2(int offset)
		{
			LoadBasics();
			DropDownList_YearOffset.Items.Clear();
			DropDownList_YearOffset.Items.Add(new ListItem("@0", "0"));
			DropDownList_YearOffset.Items.Add(new ListItem("@3", "3"));
			foreach(var year in reportYears.OrderBy(uu=>uu))
			{
				DropDownList_YearOffset.Items.Add(new ListItem("@" + year.ToString(), year.ToString()));
			}
			DropDownList_YearOffset.SelectedValue = offset.ToString();

			Repeater_Years_R2.DataSource = reportYears;
			Repeater_Years_R2.DataBind();

			Repeater_Species_R2.DataSource = dbStumpageMarketModelDataManager.GetStumpageGroups().OrderBy(uu => uu.StumpageGroupName);
			Repeater_Species_R2.DataBind();

			Repeater_Stand_R2.DataSource = reportYears;
			Repeater_Stand_R2.DataBind();

			Repeater_NetFV_R2.DataSource = reportYears;
			Repeater_NetFV_R2.DataBind();

			Repeater_NPV_R2.DataSource = reportYears;
			Repeater_NPV_R2.DataBind();

			CalculateR2();
		}
		private void LoadSEV()
		{
			LoadBasics();
			Repeater_Years_SEV.DataSource = reportYears;
			Repeater_Years_SEV.DataBind();

			Repeater_Species_SEV.DataSource = dbStumpageMarketModelDataManager.GetStumpageGroups().OrderBy(uu => uu.StumpageGroupName);
			Repeater_Species_SEV.DataBind();

			Repeater_Stand_SEV.DataSource = reportYears;
			Repeater_Stand_SEV.DataBind();

			Repeater_NetFV_SEV.DataSource = reportYears;
			Repeater_NetFV_SEV.DataBind();

			Repeater_SEV.DataSource = reportYears;
			Repeater_SEV.DataBind();
			CalculateSEV();
		}
		private void LoadRedux(int offset)
		{
			LoadBasics();
			var sevVal = 0M;
			var sevYear = stumpageGenerator.StumpageSEV.GetMaxSEV(out sevVal);
			Label_SEVYears.Text = "(" + sevYear.ToString() + ")";
			overallMax = 0M;
			Repeater_Redux.DataSource = stumpageGenerator.GetGrowthCuts(out overallMax);
			Repeater_Redux.DataBind();
		}
		protected void Repeater_Years_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Year = e.Item.FindControl("Label_Year") as Label;
			Label_Year.Text = e.Item.DataItem.ToString();
		}
		protected void Repeater_Species_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			stumpageGroup = (StumpageGroup)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Repeater_QualityCodes_R1 = e.Item.FindControl("Repeater_QualityCodes_R1") as Repeater;
			Label_Species.Text = stumpageGroup.StumpageGroupName;
			Repeater_QualityCodes_R1.DataSource = stumpageGroup.StumpageGroupQualityCodes.OrderBy(uu => uu.QualityCodeNumber);
			Repeater_QualityCodes_R1.DataBind();
		}
		protected void Repeater_QualityCodes_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			stumpageGroupQualityCode = (StumpageGroupQualityCode)e.Item.DataItem;
			var Label_CommonNames = e.Item.FindControl("Label_CommonNames") as Label;
			var Label_QualityCodeNumber = e.Item.FindControl("Label_QualityCodeNumber") as Label;
			var Repeater_FutureValueTBL_R1 = e.Item.FindControl("Repeater_FutureValueTBL_R1") as Repeater;

			Label_CommonNames.Text = GetSpeicesList(stumpageGroupQualityCode.StumpageGroup);
			Label_QualityCodeNumber.Text = stumpageGroupQualityCode.QualityCodeNumber.ToString();
			Repeater_FutureValueTBL_R1.DataSource = currentStandYears;
			Repeater_FutureValueTBL_R1.DataBind();

		}
		protected void Repeater_FutureValueTBL_R1_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var sg = (StumpageGroupIDs)stumpageGroup.StumpageGroupID;
			var price = 0M;
			var qc = stumpageGenerator.StumpageR1.GetQualityCodePriceForYearAndGroup(sg, year, out price);
			Label_Value.Text = "--";
			if (qc == stumpageGroupQualityCode.QualityCodeNumber && price > 0)
			{
				Label_Value.Text = price.ToString("C2");
			}
			
		}
		protected void Repeater_Stand_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var cost = stumpageGenerator.StumpageR1.GetStandValue(year);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}
		protected void Repeater_NetFV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var cost = stumpageGenerator.StumpageR1.GetNetFV(year);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}
		protected void Repeater_NPV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var cost = stumpageGenerator.StumpageR1.GetNPV(year);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}
		private string GetSpeicesList(StumpageGroup stumpageGroup)
		{
			var str = "";

			foreach (var species in stumpageGroup.StumpageGroupSpecies)
			{
				str = str + species.Specy.Abbreviation + ", ";
			}

			return str.Substring(0,str.Length - 2);
		}

		protected void DropDownList_YearOffset_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadRotation2(Convert.ToInt32(DropDownList_YearOffset.SelectedValue));
		}
		protected void Repeater_Years_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Year = e.Item.FindControl("Label_Year") as Label;
			Label_Year.Text = e.Item.DataItem.ToString();
		}
		protected void Repeater_Species_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			stumpageGroup = (StumpageGroup)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Repeater_QualityCodes_R2 = e.Item.FindControl("Repeater_QualityCodes_R2") as Repeater;
			Label_Species.Text = stumpageGroup.StumpageGroupName;
			Repeater_QualityCodes_R2.DataSource = stumpageGroup.StumpageGroupQualityCodes.OrderBy(uu => uu.QualityCodeNumber);
			Repeater_QualityCodes_R2.DataBind();
		}
		protected void Repeater_QualityCodes_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			stumpageGroupQualityCode = (StumpageGroupQualityCode)e.Item.DataItem;
			var Label_CommonNames = e.Item.FindControl("Label_CommonNames") as Label;
			var Label_QualityCodeNumber = e.Item.FindControl("Label_QualityCodeNumber") as Label;
			var Repeater_FutureValueTBL_R2 = e.Item.FindControl("Repeater_FutureValueTBL_R2") as Repeater;

			Label_CommonNames.Text = GetSpeicesList(stumpageGroupQualityCode.StumpageGroup);
			Label_QualityCodeNumber.Text = stumpageGroupQualityCode.QualityCodeNumber.ToString();
			Repeater_FutureValueTBL_R2.DataSource = reportYears;
			Repeater_FutureValueTBL_R2.DataBind();
		}
		protected void Repeater_FutureValueTBL_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var sg = (StumpageGroupIDs)stumpageGroup.StumpageGroupID;
			var price = 0M;
			var offset = Convert.ToInt32(DropDownList_YearOffset.SelectedValue);
			var qc = stumpageGenerator.StumpageR2.GetQualityCodePriceForYearAndGroup(sg, year, offset, out price);
			Label_Value.Text = "--";
			if (qc == stumpageGroupQualityCode.QualityCodeNumber && price > 0)
			{
				Label_Value.Text = price.ToString("C2");
			}
		}
		protected void Repeater_Stand_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var offset = Convert.ToInt32(DropDownList_YearOffset.SelectedValue);
			var cost = stumpageGenerator.StumpageR2.GetStandValue(year, offset);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}
		protected void Repeater_NetFV_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var offset = Convert.ToInt32(DropDownList_YearOffset.SelectedValue);
			var cost = stumpageGenerator.StumpageR2.GetNetFV(year, offset);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}
		protected void Repeater_NPV_R2_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var offset = Convert.ToInt32(DropDownList_YearOffset.SelectedValue);
			var cost = stumpageGenerator.StumpageR2.GetNPV(year, offset);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}

		protected void Repeater_Years_SEV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Year = e.Item.FindControl("Label_Year") as Label;
			Label_Year.Text = e.Item.DataItem.ToString();
		}
		protected void Repeater_Species_SEV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			stumpageGroup = (StumpageGroup)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Repeater_QualityCodes_SEV = e.Item.FindControl("Repeater_QualityCodes_SEV") as Repeater;
			Label_Species.Text = stumpageGroup.StumpageGroupName;
			Repeater_QualityCodes_SEV.DataSource = stumpageGroup.StumpageGroupQualityCodes.OrderBy(uu => uu.QualityCodeNumber);
			Repeater_QualityCodes_SEV.DataBind();
		}
		protected void Repeater_QualityCodes_SEV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			stumpageGroupQualityCode = (StumpageGroupQualityCode)e.Item.DataItem;
			var Label_CommonNames = e.Item.FindControl("Label_CommonNames") as Label;
			var Label_QualityCodeNumber = e.Item.FindControl("Label_QualityCodeNumber") as Label;
			var Repeater_FutureValueTBL_SEV = e.Item.FindControl("Repeater_FutureValueTBL_SEV") as Repeater;

			Label_CommonNames.Text = GetSpeicesList(stumpageGroupQualityCode.StumpageGroup);
			Label_QualityCodeNumber.Text = stumpageGroupQualityCode.QualityCodeNumber.ToString();
			Repeater_FutureValueTBL_SEV.DataSource = reportYears;
			Repeater_FutureValueTBL_SEV.DataBind();
		}
		protected void Repeater_FutureValueTBL_SEV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var sg = (StumpageGroupIDs)stumpageGroup.StumpageGroupID;
			var price = 0M;
			var qc = stumpageGenerator.StumpageSEV.GetQualityCodePriceForYearAndGroup(sg, year, out price);
			Label_Value.Text = "--";
			if (qc == stumpageGroupQualityCode.QualityCodeNumber && price > 0)
			{
				Label_Value.Text = price.ToString("C2");
			}
		}
		protected void Repeater_Stand_SEV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var cost = stumpageGenerator.StumpageSEV.GetStandValue(year);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}
		protected void Repeater_NetFV_SEV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var cost = stumpageGenerator.StumpageSEV.GetNetFV(year);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}
		protected void Repeater_SEV_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Value = e.Item.FindControl("Label_Value") as Label;
			var year = Convert.ToInt32(e.Item.DataItem.ToString());
			var cost = stumpageGenerator.StumpageSEV.GetSEV(year);
			Label_Value.Text = "--";
			if (cost > 0)
			{
				Label_Value.Text = cost.ToString("C2");
			}
		}

		protected void Repeater_Redux_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			growthCuts = (GrowthCuts)e.Item.DataItem;
			var Label_Title = e.Item.FindControl("Label_Title") as Label;
			var Repeater_Values_Redux = e.Item.FindControl("Repeater_Values_Redux") as Repeater;
			Label_Title.Text = growthCuts.Title;
			maxValue = growthCuts.MaxValue;
			Repeater_Values_Redux.DataSource = growthCuts.Cuts.OrderBy(uu=>uu.Year);
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
			Label_R2Year.Text = item.Year.ToString("") + " (" + (item.Year + growthCuts.HarvestYear).ToString() + ")";
			Label_SEV.Text = item.SEV.RotationOptimum.ToString("C2");
		}

		private void CalculateR1()
		{
			var maxNPV = 0M;
			var year = stumpageGenerator.StumpageR1.GetMaxNPV(out maxNPV);
			Label_MaxNPV.Text = maxNPV.ToString("C2") + " (" + year.ToString() + ")";
		}
		private void CalculateR2()
		{
			var maxNPV = 0M;
			var offset = Convert.ToInt32(DropDownList_YearOffset.SelectedValue);
			var year = stumpageGenerator.StumpageR2.GetMaxNPV(offset, out maxNPV);
			Label_MaxNPV_R2.Text = maxNPV.ToString("C2") + " (" + year.ToString() + ")";
		}
		private void CalculateSEV()
		{
			var maxSEV = 0M;
			var year = stumpageGenerator.StumpageSEV.GetMaxSEV(out maxSEV);
			Label_MaxSEV.Text = maxSEV.ToString("C2") + " (" + year.ToString() + ")";
		}
	}
}