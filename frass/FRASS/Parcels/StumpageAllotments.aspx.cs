using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using FRASS.DAL.DataManager;
using FRASS.DAL;
using FRASS.Interfaces;
using FRASS.BLL.Models;


namespace FRASS.WebUI.Parcels
{
	public partial class StumpageAllotments : System.Web.UI.Page
	{
		String allotmentNo;
		Int32 parcelID;
		User thisUser;
		Parcel parcel;
		StumpageModelPortfolio portfolio;
		EconVariables EconVariables;
		List<StumpageGroup> StumpageGroups;
		List<TimberMarket> TimberMarkets;
		List<StumpageGroupQualityCode> StumpageGroupQualityCodes;
		List<Int32> CalendarYears;
		List<Int32> ReportYears;
		Int32 HaulZoneID;
		int EarliestHarvestDate = 3000;
		int minYear = 0;
		decimal TotalPresentValue = 0M;
		List<Parcel> Parcels;

		StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		ParcelDataManager dbParcelDataManager;
		TimberDataManager dbTimberDataManager;
		StandDataManager dbStandDataManager;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				LoadModels();
				LoadPage(Convert.ToInt32(DropDownList_StumpageModels.SelectedValue));
			}
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			dbParcelDataManager = ParcelDataManager.GetInstance();
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbStandDataManager = StandDataManager.GetInstance();

			int tmpInt2;
			thisUser = Master.GetCurrentUser();
			parcelID = Convert.ToInt32(Request.QueryString["parcelid"].ToString());
			if (Request.QueryString["AllotmentNumber"] != null && Int32.TryParse(Request.QueryString["StumpageModelPortfolioID"].ToString(), out tmpInt2))
			{
				allotmentNo = Request.QueryString["AllotmentNumber"].ToString();
				portfolio = dbStumpageMarketModelDataManager.GetStumpageModelPortfolio(tmpInt2);
			}
			else
			{
				Response.Redirect("/Parcels/Parcels.aspx", true);
			}
		}
		protected void LoadPage(int portfolioID)
		{
			portfolio = dbStumpageMarketModelDataManager.GetStumpageModelPortfolio(portfolioID);
			parcel = dbParcelDataManager.GetParcel(parcelID);
			EconVariables = new EconVariables(portfolio);
			StumpageGroups = dbStumpageMarketModelDataManager.GetStumpageGroups();
			TimberMarkets = dbTimberDataManager.GetTimberMarkets();
			CalendarYears = dbStandDataManager.GetCurrentStandSortYears();
			StumpageGroupQualityCodes = dbStumpageMarketModelDataManager.GetStumpageGroupQualityCodes();
			minYear = CalendarYears.Min(uu => uu);
			ReportYears = new List<int>();
			HaulZoneID = parcel.ParcelHaulZones.FirstOrDefault().HaulZoneID;
			for (var year = 5; year <= 200; year += 5)
			{
				ReportYears.Add(year);
			}
		}
		protected void Button_ExportPDF_Click(object sender, EventArgs e)
		{
			Response.Redirect("/PDFs/StumpageModelAllotments.aspx?ParcelID=" + parcelID.ToString() + "&StumpageModelPortfolioID=" + portfolio.StumpageModelPortfolioID.ToString() + "&AllotmentNumber=" + allotmentNo.ToString(), true);
		}
		protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			var parcelAllotments = dbParcelDataManager.GetParcelAllotmentByAllotmentNumber(allotmentNo, Convert.ToInt32(Request.QueryString["ParcelID"].ToString()));
			var parcel = dbParcelDataManager.GetParcel(Convert.ToInt32(Request.QueryString["ParcelID"].ToString()));
			Parcels = new List<Parcel>();
			Panel_Errors.Visible = false;
			if (parcelAllotments.Count == 0)
			{
				RadGrid1.Visible = false;
				Button_ExportDBF.Visible = false;
				Panel_Errors.Visible = true;
				Label_ErrorMessage.Text = "No Parcels were found with this Allotment Number: " + allotmentNo;
			}
			else
			{

				if (parcel.ParcelAllotments.Count > 1)
				{
					RadGrid1.Visible = false;
					Button_ExportDBF.Visible = false;
					Panel_Errors.Visible = true;
					Label_ErrorMessage.Text = "There are multiple Allotment Numbers on Parcel #" + parcel.ParcelNumber;
				}
				else if (parcelAllotments.FirstOrDefault().ParcelAllotmentShares.Count() == 0)
				{
					RadGrid1.Visible = false;
					Button_ExportDBF.Visible = false;
					Panel_Errors.Visible = true;
					Label_ErrorMessage.Text = "The current (1999) database for Allotment Number: " + allotmentNo + " does not list Allotment Owner names.";
				}
				else
				{
					Parcels.Add(parcel);
				}
			}
			RadGrid1.DataSource = Parcels;
		}
		protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{
				parcel = (Parcel)e.Item.DataItem;
				LoadHeaderSection(e.Item);
				LoadHarvestVolumesValueSummary(e.Item);
				LoadAllottees(e.Item);
			}
		}

		protected void Button_ApplyStumpageModel_Click(object sender, EventArgs e)
		{
			parcelID = Convert.ToInt32(Request.QueryString["parcelid"].ToString());
			Response.Redirect(string.Format("/Parcels/StumpageAllotments.aspx?ParcelID={2}&AllotmentNumber={0}&StumpageModelPortfolioID={1}", allotmentNo, DropDownList_StumpageModels.SelectedValue, parcelID.ToString()), true);
		}
		protected void Button_ApplyMarketModel_Click(object sender, EventArgs e)
		{
			parcelID = Convert.ToInt32(Request.QueryString["parcelid"].ToString());
			Response.Redirect(string.Format("/Parcels/Allotments.aspx?ParcelID={2}&AllotmentNumber={0}&MarketModelPortfolioID={1}", allotmentNo, DropDownList_MarketModels.SelectedValue, parcelID.ToString()), true);
		}

		protected void Repeater_HarvestData_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var standid = (int)e.Item.DataItem;
			var acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Sum(uu => uu.Acres);
			var Label_StandID = e.Item.FindControl("Label_StandID") as Label;
			var Label_Acres = e.Item.FindControl("Label_Acres") as Label;
			var Label_HarvestYear = e.Item.FindControl("Label_HarvestYear") as Label;
			var Label_NPV = e.Item.FindControl("Label_NPV") as Label;
			var Label_RotationLength_R2 = e.Item.FindControl("Label_RotationLength_R2") as Label;
			var Label_NPV_R2 = e.Item.FindControl("Label_NPV_R2") as Label;
			var Label_RotationLength_SEV = e.Item.FindControl("Label_RotationLength_SEV") as Label;
			var Label_SEV = e.Item.FindControl("Label_SEV") as Label;
			var Label_StandValue = e.Item.FindControl("Label_StandValue") as Label;
			var Label_StandValue_PerAcre = e.Item.FindControl("Label_StandValue_PerAcre") as Label;

			Label_StandID.Text = standid.ToString();
			Label_Acres.Text = acres.ToString("N2");

			List<IStandData> CurrentStandData = dbStandDataManager.GetStandDataCurrent(standid, parcelID);
			List<IStandData> FutureStandData = dbStandDataManager.GetStandDataFuture(standid, parcelID);

			var sg = new StumpageGenerator(parcel, standid, portfolio, StumpageGroups, CurrentStandData, FutureStandData, TimberMarkets, HaulZoneID, minYear, EconVariables, StumpageGroupQualityCodes, CalendarYears, ReportYears);

			decimal overallMax;
			var OuterList = sg.GetGrowthCuts(out overallMax);
			var o = OuterList.Where(uu => uu.MaxValue == overallMax).FirstOrDefault();
			if (o == null)
			{
				e.Item.Visible = false;
			}
			if (o != null)
			{
				var gc = o.Cuts.Where(uu => uu.Total.RotationOptimum == overallMax).FirstOrDefault();
				if (gc != null)
				{
					Label_HarvestYear.Text = o.HarvestYear.ToString();
					Label_NPV.Text = gc.R1.RotationOptimum.ToString("C0");


					Label_RotationLength_R2.Text = gc.Year.ToString();
					Label_NPV_R2.Text = gc.R2.RotationOptimum.ToString("C0");
					decimal sevMax = 0M;
					Label_RotationLength_SEV.Text = sg.StumpageSEV.GetMaxSEV(out sevMax).ToString();
					Label_SEV.Text = gc.SEV.RotationOptimum.ToString("C2");
				}
				if (EarliestHarvestDate > o.HarvestYear)
				{
					EarliestHarvestDate = o.HarvestYear;
				}
				if (EarliestHarvestDate < minYear)
				{
					EarliestHarvestDate = minYear;
				}
			}

			if (acres > 0)
			{
				var valPerAcre = overallMax / acres;
				Label_StandValue_PerAcre.Text = valPerAcre.ToString("C0");
			}

			Label_StandValue.Text = overallMax.ToString("C0");
			TotalPresentValue = TotalPresentValue + overallMax;

		}
		protected void Repeater_Allottees_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var allottee = (ParcelAllotmentShare)e.Item.DataItem;
			var Label_LastName = e.Item.FindControl("Label_LastName") as Label;
			var Label_FirstName = e.Item.FindControl("Label_FirstName") as Label;
			var Label_AllotteeNumber = e.Item.FindControl("Label_AllotteeNumber") as Label;
			var Label_UndivdedInteresetShare = e.Item.FindControl("Label_UndivdedInteresetShare") as Label;
			var Label_ProratedValue = e.Item.FindControl("Label_ProratedValue") as Label;
			var Label_FractionationScalar = e.Item.FindControl("Label_FractionationScalar") as Label;
			var Label_ValueAdjustedForFractionation = e.Item.FindControl("Label_ValueAdjustedForFractionation") as Label;
			Label_FirstName.Text = allottee.ParcelAllottee.FirstName;
			Label_LastName.Text = allottee.ParcelAllottee.LastName;
			Label_AllotteeNumber.Text = allottee.ParcelAllottee.AllotteeNumber.ToString();
			var share = allottee.Share;
			Label_UndivdedInteresetShare.Text = share.ToString("N5");
			var proratedValue = TotalPresentValue * share;
			Label_ProratedValue.Text = proratedValue.ToString("C2");
			var fractionalShare = GetFractionalShare(share);
			Label_FractionationScalar.Text = fractionalShare.ToString("N1");
			Label_ValueAdjustedForFractionation.Text = (proratedValue * fractionalShare).ToString("C2");
		}

		private void LoadHeaderSection(GridItem item)
		{
			StringBuilder txt = new StringBuilder();
			Label Label_User = item.FindControl("Label_User") as Label;
			Label Label_ParcelNum = item.FindControl("Label_ParcelNum") as Label;
			Label Label_Acres = item.FindControl("Label_Acres") as Label;
			Label Label_Hectares = item.FindControl("Label_Hectares") as Label;
			Label Label_OwnerCategory = item.FindControl("Label_OwnerCategory") as Label;
			Label Label_AllotmentNumber = item.FindControl("Label_AllotmentNumber") as Label;
			Label Label_Owner = item.FindControl("Label_Owner") as Label;
			Label Label_Township = item.FindControl("Label_Township") as Label;
			Label Label_Range = item.FindControl("Label_Range") as Label;
			Label Label_Section = item.FindControl("Label_Section") as Label;
			Label Label_LegalDesc = item.FindControl("Label_LegalDesc") as Label;
			Label Label_County = item.FindControl("Label_County") as Label;
			Label Label_CountyAssessorLandValue = item.FindControl("Label_CountyAssessorLandValue") as Label;
			Label Label_CountyAssessorBuildingValue = item.FindControl("Label_CountyAssessorBuildingValue") as Label;

			Label_User.Text = thisUser.FirstName + " " + thisUser.LastName;
			Label_ParcelNum.Text = parcel.ParcelNumber;
			Label_Acres.Text = String.Format("{0:0.0}", parcel.Acres);
			Label_Hectares.Text = String.Format("{0:0.0}", parcel.Hectares);

			var ot = parcel.ParcelAllotments.Select(uu => uu.OwnerType).FirstOrDefault();
			Label_OwnerCategory.Text = ot.OwnerType1.ToString();

			StringBuilder ownerStringBuilder = new StringBuilder();


			var owner = new List<string>();
			foreach (var parcelOwner in parcel.ParcelOwners.Select(uu => uu.Owner).Distinct())
			{
				var str = parcelOwner.Name + "<br/>";
				str = str + parcelOwner.Address + "<br/>";
				if (parcelOwner.State != null)
				{
					str = str + parcelOwner.City + ", " + parcelOwner.State.StateInitial + " " + parcelOwner.Zip;
				}
				else
				{
					str = str + parcelOwner.City + parcelOwner.Zip;
				}
				if (parcelOwner.Zip4.Trim() != string.Empty)
				{
					str = str + "-" + parcelOwner.Zip4;
				}
				str = str + "<br/>";
				owner.Add(str);
			}
			var ct = 0;
			foreach (var str in owner.OrderBy(uu => uu).Distinct())
			{
				if (ct > 0)
				{
					ownerStringBuilder.Append("<br/>");
				}
				ownerStringBuilder.Append(str);
				ct = ct + 1;
			}

			Label_Owner.Text = ownerStringBuilder.ToString();

			StringBuilder townshipSB = new StringBuilder();
			StringBuilder rangeSB = new StringBuilder();
			StringBuilder sectionSB = new StringBuilder();
			StringBuilder countySB = new StringBuilder();
			StringBuilder allotnumbers = new StringBuilder();

			foreach (var trs in parcel.ParcelAllotments.Select(uu => uu.Town).Distinct())
			{
				townshipSB.Append(trs.ToString() + "<br/>");
			}
			foreach (var trs in parcel.ParcelAllotments.Select(uu => uu.Range).Distinct())
			{
				rangeSB.Append(trs.ToString() + "<br/>");
			}
			foreach (var trs in parcel.ParcelAllotments.Select(uu => uu.Section).Distinct())
			{
				sectionSB.Append(trs.ToString() + "<br/>");
			}
			foreach (var pc in parcel.ParcelCounties.Select(uu => uu.County.County1).Distinct())
			{
				countySB.Append(pc + "<br/>");
			}
			foreach (var an in parcel.ParcelAllotments.Select(uu => uu.AllotmentNumber).Distinct())
			{
				allotnumbers.Append(an + "<br/>");
			}

			Label_AllotmentNumber.Text = allotnumbers.ToString();

			Label_Township.Text = townshipSB.ToString();
			Label_Range.Text = rangeSB.ToString();
			Label_Section.Text = sectionSB.ToString();

			Label_County.Text = countySB.ToString();
			Label_CountyAssessorBuildingValue.Text = "$" + parcel.ParcelLegals.Sum(uu => uu.BuildingValue);
			Label_CountyAssessorLandValue.Text = "$" + parcel.ParcelLegals.Sum(uu => uu.LandValue);

			if (ot.OwnerType1 == "AiT" || ot.OwnerType1 == "T" || ot.OwnerType1 == "Q")
			{

				foreach (var pa in parcel.ParcelAllotments)
				{
					foreach (var pal in pa.ParcelAllotmentLegals)
					{
						Label_LegalDesc.Text += pal.Legal + "<br/>";
					}

				}
			}
			else
			{
				foreach (var pl in parcel.ParcelLegals.OrderBy(uu => uu.Legal))
				{
					Label_LegalDesc.Text += pl.Legal + "<br/>";
				}
			}
		}
		private void LoadHarvestVolumesValueSummary(GridItem item)
		{
			parcel = (Parcel)item.DataItem;
			var Repeater_HarvestData = item.FindControl("Repeater_HarvestData") as Repeater;

			var Label_RoadSchedule = item.FindControl("Label_RoadSchedule") as Label;
			var Label_RoadCurrentCost = item.FindControl("Label_RoadCurrentCost") as Label;
			var Label_RoadFuture = item.FindControl("Label_RoadFuture") as Label;
			var Label_RoadDiscountCost = item.FindControl("Label_RoadDiscountCost") as Label;


			var Label_TotalCommercialAcres = item.FindControl("Label_TotalCommercialAcres") as Label;
			var Label_TotalValue = item.FindControl("Label_TotalValue") as Label;
			var Label_TotalValueAcre = item.FindControl("Label_TotalValueAcre") as Label;

			var Label_TotalAcresForested = item.FindControl("Label_TotalAcresForested") as Label;
			var Label_TotalValueForestedPerAcre = item.FindControl("Label_TotalValueForestedPerAcre") as Label;

			var Label_TotalAcresEntireParcel = item.FindControl("Label_TotalAcresEntireParcel") as Label;
			var Label_TotalalueEntireParcelPerAcre = item.FindControl("Label_TotalalueEntireParcelPerAcre") as Label;

			var Label_TotalBareLand = item.FindControl("Label_TotalBareLand") as Label;
			var Label_BareLandValue = item.FindControl("Label_BareLandValue") as Label;
			var Label_TotalValuePareLandPerAcre = item.FindControl("Label_TotalValuePareLandPerAcre") as Label;

			Repeater_HarvestData.DataSource = parcel.ParcelRiparians.Where(uu => uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.STD_ID).ToList<int>().Distinct();
			Repeater_HarvestData.DataBind();

			SetRoadCosts(Label_RoadSchedule, Label_RoadCurrentCost, Label_RoadFuture, Label_RoadDiscountCost);

			SetTotalValueBasedOnOperableCommercialAcres(Label_TotalCommercialAcres, Label_TotalValue, Label_TotalValueAcre);
			SetValuePerForestedAcre(Label_TotalAcresForested, Label_TotalValueForestedPerAcre);
			SetValuePerEntireParcel(Label_TotalAcresEntireParcel, Label_TotalalueEntireParcelPerAcre);
			SetValueBareLandValue(Label_TotalBareLand, Label_BareLandValue, Label_TotalValuePareLandPerAcre);
		}
		private void LoadAllottees(GridItem item)
		{
			var Repeater_Allottees = item.FindControl("Repeater_Allottees") as Repeater;
			Repeater_Allottees.DataSource = dbParcelDataManager.GetParcelAllotmentSharesByParcelID(parcel.ParcelID);
			Repeater_Allottees.DataBind();
		}

		private void SetRoadCosts(Label Label_RoadSchedule, Label Label_RoadCurrentCost, Label Label_RoadFuture, Label Label_RoadDiscountCost)
		{
			var distance = parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary;

			if (distance.HasValue && distance.Value > 0)
			{
				distance = (distance / 1000) * 0.621371192237334M;
				var currentCosts = EconVariables.NewRoad * distance.Value;
				var years = EarliestHarvestDate - 2 - DateTime.Now.Year;
				var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + EconVariables.RateOfInflation), years));
				var futureCosts = currentCosts * power;
				var discount = futureCosts / (Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation ) * (1 + EconVariables.RealDiscount)), years)));
				Label_RoadSchedule.Text = (EarliestHarvestDate - 2).ToString();
				if (EarliestHarvestDate == minYear)
				{
					Label_RoadSchedule.Text = (EarliestHarvestDate).ToString();
				}

				Label_RoadCurrentCost.Text = currentCosts.ToString("C2");
				Label_RoadFuture.Text = futureCosts.ToString("C2");
				Label_RoadDiscountCost.Text = (-1 * discount).ToString("C2");
				TotalPresentValue = TotalPresentValue - discount;
			}
			else
			{
				Label_RoadSchedule.Text = "--";
				Label_RoadCurrentCost.Text = "--";
				Label_RoadFuture.Text = "--";
				Label_RoadDiscountCost.Text = "--";
			}

		}
		private void SetTotalValueBasedOnOperableCommercialAcres(Label Label_TotalCommercialAcres, Label Label_TotalValue, Label Label_TotalValueAcre)
		{
			var standids = dbParcelDataManager.GetReportTimberStatistics(parcel.ParcelID, System.DateTime.Now.Year).Where(uu => uu.StandStats == StandStats.Operable).Select(uu => uu.Stand_ID).Distinct().ToList<int>();

			var opacres = (from p in parcel.ParcelRiparians
						   where p.StandStatID == Convert.ToInt32(StandStats.Operable) && (from s in standids select s).Contains(p.STD_ID)
						   select p).Sum(uu => uu.Acres);
			var valperacre = 0M;
			if (opacres > 0)
			{
				valperacre = TotalPresentValue / opacres;
			}

			Label_TotalCommercialAcres.Text = opacres.ToString("N1");
			Label_TotalValue.Text = TotalPresentValue.ToString("C0");
			Label_TotalValueAcre.Text = valperacre.ToString("C0");

		}
		private void SetValuePerForestedAcre(Label Label_TotalAcresForested, Label Label_TotalValueForestedPerAcre)
		{
			var standids = dbParcelDataManager.GetReportTimberStatistics(parcel.ParcelID, System.DateTime.Now.Year).Where(uu => uu.StandStats == StandStats.Operable).Select(uu => uu.Stand_ID).Distinct().ToList<int>();
			var acres = (from p in parcel.ParcelRiparians
						 where (from s in standids select s).Contains(p.STD_ID)
						 select p).Sum(uu => uu.Acres);
			var valperacre = 0M;
			if (acres > 0)
			{
				valperacre = TotalPresentValue / acres;
			}

			Label_TotalAcresForested.Text = acres.ToString("N1");
			Label_TotalValueForestedPerAcre.Text = valperacre.ToString("C0");
		}
		private void SetValuePerEntireParcel(Label Label_TotalAcresEntireParcel, Label Label_TotalalueEntireParcelPerAcre)
		{
			var acres = parcel.Acres;
			var valperacre = 0M;
			if (acres > 0)
			{
				valperacre = TotalPresentValue / acres;
			}
			Label_TotalAcresEntireParcel.Text = acres.ToString("N1");
			Label_TotalalueEntireParcelPerAcre.Text = valperacre.ToString("C0");
		}
		private void SetValueBareLandValue(Label Label_TotalBareLand, Label Label_BareLandValue, Label Label_TotalValuePareLandPerAcre)
		{
			var econVariables = new EconVariables(portfolio);
			var tots = 0M;
			foreach (var standid in parcel.ParcelRiparians.Select(uu => uu.STD_ID).Distinct())
			{
				List<IStandData> CurrentStandData = dbStandDataManager.GetStandDataCurrent(standid, parcelID);
				List<IStandData> FutureStandData = dbStandDataManager.GetStandDataFuture(standid, parcelID);

				var sg = new StumpageGenerator(parcel, standid, portfolio, StumpageGroups, CurrentStandData, FutureStandData, TimberMarkets, HaulZoneID, minYear, EconVariables, StumpageGroupQualityCodes, CalendarYears, ReportYears);
				var maxSEV = 0M;
				var year = sg.StumpageSEV.GetMaxSEV(out maxSEV);
				tots += maxSEV;
			}
			var acres = parcel.Acres;
			var valperacre = tots / acres;
			Label_TotalBareLand.Text = acres.ToString("N1");
			Label_BareLandValue.Text = tots.ToString("C0");
			Label_TotalValuePareLandPerAcre.Text = valperacre.ToString("C0");
		}

		private decimal GetFractionalShare(decimal share)
		{
			var f = 0M;
			if (share > .9M)
			{
				f = 1M;
			}
			else if (share > .8M)
			{
				f = .9M;
			}
			else if (share > .7M)
			{
				f = .85M;
			}
			else if (share > .6M)
			{
				f = .8M;
			}
			else if (share > .5M)
			{
				f = .7M;
			}
			else if (share > .4M)
			{
				f = .6M;
			}
			else if (share > .3M)
			{
				f = .5M;
			}
			else if (share > .2M)
			{
				f = .3M;
			}
			else if (share > .1M)
			{
				f = .1M;
			}
			return f;
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
				DropDownList_StumpageModels.SelectedValue = portfolio.StumpageModelPortfolioID.ToString();
			}
		}
	}
}