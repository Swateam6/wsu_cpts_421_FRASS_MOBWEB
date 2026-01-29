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
using FRASS.DAL.DataManager;
using FRASS.DAL;
using FRASS.BLL.Models;
using FRASS.Interfaces;


namespace FRASS.Parcels
{
	public partial class MarketValueReport : System.Web.UI.Page
	{
		StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		RPAPortfolioDataManager dbRPADataManager;
		ParcelDataManager dbParcelDataManager;
		TimberDataManager dbTimberDataManager;
		StandDataManager dbStandDataManager;

		Int32 parcelID;
		User thisUser;
		Parcel parcel;
		MarketModelPortfolio portfolio;
		RPAPortfolio rpaPortfolio;
		private List<RPAPortfolioDetail> RPAPortfolioDetails;
		private List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails;
		EconVariables EconVariables;
		List<TimberGrade> TimberGrades;
		List<TimberMarket> TimberMarkets;
		List<LogMarketReportSpecy> LogMarketReportSpecies;
		List<LogMarketReportSpeciesMarket> LogMarkets;
		int EarliestHarvestDate = 3000;
		int minYear = 0;
		decimal TotalPresentValue = 0M;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				LoadModels();
				EconVariables = new EconVariables(portfolio, rpaPortfolio);
				TimberGrades = dbTimberDataManager.GetTimberGrades();
				TimberMarkets = dbTimberDataManager.GetTimberMarkets();
				minYear = dbStandDataManager.GetCurrentStandSortYears().Min();
				LogMarketReportSpecies = dbTimberDataManager.GetLogMarketReportSpecies();
				LogMarkets = dbStandDataManager.GetLogMarketReportSpeciesMarkets();
			}
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			dbRPADataManager = RPAPortfolioDataManager.GetInstance();
			dbParcelDataManager = ParcelDataManager.GetInstance();
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbStandDataManager = StandDataManager.GetInstance();

			thisUser = Master.GetCurrentUser();
			Int32 tmpInt=0;
			Int32 tmpInt2=0;
			Int32 tmpInt3=0;
			var portfolioID = 0;
			var rpaPortfolioID = 0;
			if (Int32.TryParse(Request.QueryString["ParcelID"].ToString(), out tmpInt) && Int32.TryParse(Request.QueryString["MarketModelPortfolioID"].ToString(), out tmpInt2)&& Int32.TryParse(Request.QueryString["RPAPortfolioID"].ToString(), out tmpInt3))
			{
				parcelID = tmpInt;
				portfolioID = tmpInt2;
				rpaPortfolioID = tmpInt3;
			}
			else
			{
				Server.Transfer("/welcome.aspx");
			}
			portfolio = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(portfolioID);
			rpaPortfolio = dbRPADataManager.GetRPAPortfolio(rpaPortfolioID);
			RPAPortfolioDetails = rpaPortfolio.RPAPortfolioDetails.ToList<RPAPortfolioDetail>();
			MarketModelPortfolioDeliveredLogModelDetails = portfolio.MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
		}
		
		protected void Button_ApplyMarketModel_Click(object sender, EventArgs e)
		{
			Response.Redirect("/Parcels/MarketValueReport.aspx?ParcelID=" + parcelID.ToString() + "&MarketModelPortfolioID=" + DropDownList_MarketModels.SelectedValue + "&RPAPortfolioID=" + DropDownList_RPA.SelectedValue, true);
		}
		protected void Button_ApplyStumpageModel_Click(object sender, EventArgs e)
		{
			Response.Redirect("/Parcels/StumpageValueReport.aspx?ParcelID=" + parcelID.ToString() + "&StumpageModelPortfolioID=" + DropDownList_StumpageModels.SelectedValue, true);
		}
		protected void Button_ExportPDF_Click(object sender, EventArgs e)
		{
			Response.Redirect("/PDFs/MarketValueReport.aspx?ParcelID=" + parcelID.ToString() + "&MarketModelPortfolioID=" + DropDownList_MarketModels.SelectedValue + "&RPAPortfolioID=" + DropDownList_RPA.SelectedValue, true);
		}
		protected void Button_ViewAllotments_Click(object sender, EventArgs e)
		{
			parcel = dbParcelDataManager.GetParcel(parcelID);
			var p = parcel.ParcelAllotments.FirstOrDefault().AllotmentNumber;
			Response.Redirect(string.Format("/Parcels/Allotments.aspx?ParcelID={2}&AllotmentNumber={0}&MarketModelPortfolioID={1}&RPAPortfolioID={3}", p, Request.QueryString["MarketModelPortfolioID"].ToString(), parcelID.ToString(), Request.QueryString["RPAPortfolioID"].ToString()), true);
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
				DropDownList_MarketModels.SelectedValue = portfolio.MarketModelPortfolioID.ToString();
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
				DropDownList_RPA.SelectedValue = rpaPortfolio.RPAPortfolioID.ToString();
			}
		}
		protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			List<Parcel> parcels = new List<Parcel>();
			parcel = dbParcelDataManager.GetParcel(parcelID);
			parcels.Add(parcel);
			RadGrid1.DataSource = parcels;
		}
		protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridHeaderItem)
			{
				var header = e.Item as GridHeaderItem;
				header["Col1"].Text = "Forest Resource Analysis System Software Reporting System:<br/>Delivered Log Market Model - " + portfolio.PortfolioName + ": " + rpaPortfolio.PortfolioName;
			}
			else if (e.Item is GridDataItem)
			{
				var Label_User = e.Item.FindControl("Label_User") as Label;
				var Label_ParcelNumber = e.Item.FindControl("Label_ParcelNumber") as Label;
				var Label_Acres = e.Item.FindControl("Label_Acres") as Label;
				var Image_AcreActionSize = e.Item.FindControl("Image_AcreActionSize") as Image;
				var Label_AcreActions = e.Item.FindControl("Label_AcreActions") as Label;

				var Label_Township = e.Item.FindControl("Label_Township") as Label;
				var Label_Range = e.Item.FindControl("Label_Range") as Label;
				var Label_Section = e.Item.FindControl("Label_Section") as Label;
				var Label_County = e.Item.FindControl("Label_County") as Label;
				var Label_LegalDescription = e.Item.FindControl("Label_LegalDescription") as Label;

				var Repeater_Zoning = e.Item.FindControl("Repeater_Zoning") as Repeater;

				var Label_NorthernSpotedOwl = e.Item.FindControl("Label_NorthernSpotedOwl") as Label;
				var Image_NorthernSpotedOwl = e.Item.FindControl("Image_NorthernSpotedOwl") as Image;
				var Label_MarbledMurrelet = e.Item.FindControl("Label_MarbledMurrelet") as Label;
				var Image_MarbledMurrelet = e.Item.FindControl("Image_MarbledMurrelet") as Image;
				var Label_Eagle = e.Item.FindControl("Label_Eagle") as Label;
				var Image_Eagle = e.Item.FindControl("Image_Eagle") as Image;

				var Label_ExistingSurfaceLengthFeets = e.Item.FindControl("Label_ExistingSurfaceLengthFeets") as Label;
				var Label_ExistingSurfaceLengthMiles = e.Item.FindControl("Label_ExistingSurfaceLengthMiles") as Label;

				var Label_ExistingMainHaulLengthFeet = e.Item.FindControl("Label_ExistingMainHaulLengthFeet") as Label;
				var Label_ExistingMainHaulLengthMiles = e.Item.FindControl("Label_ExistingMainHaulLengthMiles") as Label;

				var Label_ExistingPavedLengthFeet = e.Item.FindControl("Label_ExistingPavedLengthFeet") as Label;
				var Label_ExistingPavedLengthMiles = e.Item.FindControl("Label_ExistingPavedLengthMiles") as Label;

				if (parcel.ParcelRoadUseLengths != null)
				{
					if (parcel.ParcelRoadUseLengths.SurfaceLength.HasValue)
					{
						Label_ExistingSurfaceLengthFeets.Text = (parcel.ParcelRoadUseLengths.SurfaceLength.Value * 3.280839895013123M).ToString("N0");
						Label_ExistingSurfaceLengthMiles.Text = ((parcel.ParcelRoadUseLengths.SurfaceLength.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadUseLengths.MainHaulLength.HasValue)
					{
						Label_ExistingMainHaulLengthFeet.Text = (parcel.ParcelRoadUseLengths.MainHaulLength.Value * 3.280839895013123M).ToString("N0");
						Label_ExistingMainHaulLengthMiles.Text = ((parcel.ParcelRoadUseLengths.MainHaulLength.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadUseLengths.PavedLength.HasValue)
					{
						Label_ExistingPavedLengthFeet.Text = (parcel.ParcelRoadUseLengths.PavedLength.Value * 3.280839895013123M).ToString("N0");
						Label_ExistingPavedLengthMiles.Text = ((parcel.ParcelRoadUseLengths.PavedLength.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
				}
				var Label_DistToNearest_Feet = e.Item.FindControl("Label_DistToNearest_Feet") as Label;
				var Label_DistToNearest_Miles = e.Item.FindControl("Label_DistToNearest_Miles") as Label;
				var Label_NearRoadUse = e.Item.FindControl("Label_NearRoadUse") as Label;

				var Label_ToSurface_Feet = e.Item.FindControl("Label_ToSurface_Feet") as Label;
				var Label_ToSurface_Miles = e.Item.FindControl("Label_ToSurface_Miles") as Label;

				var Label_MainHaul_Feet = e.Item.FindControl("Label_MainHaul_Feet") as Label;
				var Label_MainHaul_Miles = e.Item.FindControl("Label_MainHaul_Miles") as Label;

				Label_NearRoadUse.Text = parcel.ParcelRoadDistances.NearRoadUseFromParcelBoundary;

				if (parcel.ParcelRoadDistances != null)
				{
					if (parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
					{
						Label_DistToNearest_Feet.Text = (parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value * 3.280839895013123M).ToString("N0");
						Label_DistToNearest_Miles.Text = ((parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadDistances.MainHaulToPaved.HasValue)
					{
						Label_ToSurface_Feet.Text = (parcel.ParcelRoadDistances.MainHaulToPaved.Value * 3.280839895013123M).ToString("N0");
						Label_ToSurface_Miles.Text = ((parcel.ParcelRoadDistances.MainHaulToPaved.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadDistances.ToMainHaul.HasValue)
					{
						Label_MainHaul_Feet.Text = (parcel.ParcelRoadDistances.ToMainHaul.Value * 3.280839895013123M).ToString("N0");
						Label_MainHaul_Miles.Text = ((parcel.ParcelRoadDistances.ToMainHaul.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
				}
				var Repeater_HarvestData = e.Item.FindControl("Repeater_HarvestData") as Repeater;

				var Label_RoadSchedule = e.Item.FindControl("Label_RoadSchedule") as Label;
				var Label_RoadCurrentCost = e.Item.FindControl("Label_RoadCurrentCost") as Label;
				var Label_RoadFuture = e.Item.FindControl("Label_RoadFuture") as Label;
				var Label_RoadDiscountCost = e.Item.FindControl("Label_RoadDiscountCost") as Label;


				var Label_TotalCommercialAcres = e.Item.FindControl("Label_TotalCommercialAcres") as Label;
				var Label_TotalValue = e.Item.FindControl("Label_TotalValue") as Label;
				var Label_TotalValueAcre = e.Item.FindControl("Label_TotalValueAcre") as Label;

				var Label_TotalAcresForested = e.Item.FindControl("Label_TotalAcresForested") as Label;
				var Label_TotalValueForestedPerAcre = e.Item.FindControl("Label_TotalValueForestedPerAcre") as Label;

				var Label_TotalAcresEntireParcel = e.Item.FindControl("Label_TotalAcresEntireParcel") as Label;
				var Label_TotalalueEntireParcelPerAcre = e.Item.FindControl("Label_TotalalueEntireParcelPerAcre") as Label;

				var Label_TotalBareLand = e.Item.FindControl("Label_TotalBareLand") as Label;
				var Label_BareLandValue = e.Item.FindControl("Label_BareLandValue") as Label;
				var Label_TotalValuePareLandPerAcre = e.Item.FindControl("Label_TotalValuePareLandPerAcre") as Label;

				// var Repeater_HarvestReport = e.Item.FindControl("Repeater_HarvestReport") as Repeater;

				Label_User.Text = thisUser.FirstName + " " + thisUser.LastName;
				Label_ParcelNumber.Text = parcel.ParcelNumber;
				Label_Acres.Text = String.Format("{0:0.0}", parcel.Acres);
				SetAcreActions(parcel.Acres, Image_AcreActionSize, Label_AcreActions);

				SetTownshipRangeSectionCounty(Label_Township, Label_Range, Label_Section, Label_County);
				SetLegalDescription(Label_LegalDescription);

				Repeater_Zoning.DataSource = parcel.ParcelZonings;
				Repeater_Zoning.DataBind();

				SetEndangeredSpecies(Label_NorthernSpotedOwl, Image_NorthernSpotedOwl, Label_MarbledMurrelet, Image_MarbledMurrelet, Label_Eagle, Image_Eagle);

				//SetRoads_DistToNearest(Label_NearRoadUse, Label_DistToNearest_Meters, Label_DistToNearest_Feet, Label_DistToNearest_Miles);
				//SetRoads_ToMainHaul(Label_ToMainHaul_Meters, Label_ToMainHaul_Feet, Label_ToMainHaul_Miles);
				//SetRoads_MainHaulToPaved(Label_MainHaulToPaved_Meters, Label_MainHaulToPaved_Feet, Label_MainHaulToPaved_Miles);

				Repeater_HarvestData.DataSource = parcel.ParcelRiparians.Where(uu => uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.STD_ID).ToList<int>().Distinct();
				Repeater_HarvestData.DataBind();

				SetRoadCosts(Label_RoadSchedule, Label_RoadCurrentCost, Label_RoadFuture, Label_RoadDiscountCost);

				var stats = dbParcelDataManager.GetReportTimberStatistics(parcel.ParcelID, System.DateTime.Now.Year);
				SetTotalValueBasedOnOperableCommercialAcres(Label_TotalCommercialAcres, Label_TotalValue, Label_TotalValueAcre, stats);
				SetValuePerForestedAcre(Label_TotalAcresForested, Label_TotalValueForestedPerAcre, stats);
				SetValuePerEntireParcel(Label_TotalAcresEntireParcel, Label_TotalalueEntireParcelPerAcre);
				SetValueBareLandValue(Label_TotalBareLand, Label_BareLandValue, Label_TotalValuePareLandPerAcre);
				// Repeater_HarvestReport.DataSource = parcel.ParcelRiparians.Where(uu => uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.STD_ID).Distinct();
				// Repeater_HarvestReport.DataBind();
			}
		}

		private void SetAcreActions(decimal acres, Image image, Label label)
		{
			if (acres < 10M)
			{
				label.Text = "This parcel appears to be below the size needed to represent a commercial timber production parcel and may not be considered a viable Highest and Best Use for timber production.";
				image.ImageUrl = "/images/32_stop.png";
			}
			else if (acres < 20M)
			{
				label.Text = "This parcel may be marginal in size needed to represent a commercial timber production parcel. Caution should be applied in reference to timber production costs for this parcel.";
				image.ImageUrl = "/images/32_warn.png";
			}
			else
			{
				label.Text = "This parcel appears to meet the size requirements for a valid commercial timber production parcel.";
				image.ImageUrl = "/images/32_ok.png";
			}
		}
		private void SetOwnerCategory(Label label)
		{
			IEnumerable<OwnerType> ot = parcel.ParcelAllotments.Select(uu => uu.OwnerType).Distinct();
			StringBuilder ownerTypeStringBuilder = new StringBuilder();
			foreach (var ownerType in ot.Select(uu=>uu.OwnerType1).Distinct())
			{
				ownerTypeStringBuilder.Append(ownerType + "<br/>");
			}
			label.Text = ownerTypeStringBuilder.ToString();
		}
		private void SetAllotmentOwners(Label label)
		{
			StringBuilder allotnumbers = new StringBuilder();
			foreach (var an in parcel.ParcelAllotments.Select(uu => uu.AllotmentNumber).Distinct())
			{
				allotnumbers.Append(an + "<br/>");
			}
			label.Text = allotnumbers.ToString();
		}
		private void SetTownshipRangeSectionCounty(Label label_Township, Label label_Range, Label label_Section, Label label_County)
		{
			StringBuilder townshipSB = new StringBuilder();
			StringBuilder rangeSB = new StringBuilder();
			StringBuilder sectionSB = new StringBuilder();
			StringBuilder countySB = new StringBuilder();

			foreach (var trs in parcel.ParcelAllotments.Select(uu=>uu.Town).Distinct())
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
			

			foreach (var pc in parcel.ParcelCounties.Select(uu=>uu.County.County1).Distinct())
			{
				countySB.Append(pc + "<br/>");
			}
			

			label_Township.Text = townshipSB.ToString();
			label_Range.Text = rangeSB.ToString();
			label_Section.Text = sectionSB.ToString();
			label_County.Text = countySB.ToString();
		}
		private void SetLegalDescription(Label label)
		{
			var ot = parcel.ParcelAllotments.Select(uu => uu.OwnerType).FirstOrDefault();
			if (ot.OwnerType1 == "AiT" || ot.OwnerType1 == "T" || ot.OwnerType1 == "Q")
			{

				foreach (var pa in parcel.ParcelAllotments)
				{
					foreach (var pal in pa.ParcelAllotmentLegals)
					{
						label.Text += pal.Legal + "<br/>";
					}

				}
			}
			else
			{
				foreach (var pl in parcel.ParcelLegals.OrderBy(uu => uu.Legal))
				{
					label.Text += pl.Legal + "<br/>";
				}
			}
		}
		protected void Repeater_ZoningItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var parcelZoning = e.Item.DataItem as ParcelZoning;
			var Label_Zoning = e.Item.FindControl("Label_Zoning") as Label;
			var Label_ZoningAcres = e.Item.FindControl("Label_ZoningAcres") as Label;
			var Image_Zoning = e.Item.FindControl("Image_Zoning") as Image;
			var Label_ZoningAction = e.Item.FindControl("Label_ZoningAction") as Label;
			Label_Zoning.Text = parcelZoning.ParcelZoningType.Zoning;
			Label_ZoningAcres.Text = parcelZoning.Acres.ToString("N1");

			if (parcelZoning.ParcelZoningType.Zoning == "Wilderness" || parcelZoning.ParcelZoningType.Zoning == "Commercial" || parcelZoning.ParcelZoningType.Zoning == "Residential")
			{
				Image_Zoning.ImageUrl = "/images/32_stop.png";
				Label_ZoningAction.Text = "This parcel is located in a Zoning category that is not consistent with commercial timber production.";

			}
			else if (parcelZoning.ParcelZoningType.Zoning == "Industrial")
			{
				Image_Zoning.ImageUrl = "/images/32_warn.png";
				Label_ZoningAction.Text = "This parcel's use for commercial timber production may require a special use permit.";
			}
			else
			{
				Image_Zoning.ImageUrl = "/images/32_ok.png";
				Label_ZoningAction.Text = "This parcel's use for commercial timber production is consistent with the Forestry Zoning Category.";
			}

		}
		private void SetEndangeredSpecies(Label label_owl, Image image_owl, Label label_Murrelett, Image image_Murrelett, Label label_Eagle, Image image_eagle)
		{
			var warn = "<strong>Caution: </strong>Timber harvest timing may be restricted to times when the species is not nesting.";
			var ok = "No restrictions apply";

			label_owl.Text = ok;
			image_owl.ImageUrl = "/images/32_ok.png";

			label_Eagle.Text = ok;
			image_eagle.ImageUrl = "/images/32_ok.png";

			label_Murrelett.Text = ok;
			image_Murrelett.ImageUrl = "/images/32_ok.png";
			var mp = parcel.ParcelMurreletts.Select(uu => uu.MM_Suita);
			foreach(var m in mp)
			{
				if (m == "Core Habitat")
				{
					label_Murrelett.Text = warn;
					image_Murrelett.ImageUrl = "/images/32_warn.png";
				}
			}

			
			
		}
		private void SetRoads_DistToNearest(Label Label_NearRoadUse, Label Label_DistToNearest_Meters, Label Label_DistToNearest_Feet, Label Label_DistToNearest_Miles)
		{
			Label_NearRoadUse.Text = parcel.ParcelRoadDistances.NearRoadUseFromParcelBoundary;

			if (parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
			{
				Label_DistToNearest_Meters.Text = parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value.ToString("N2");
				Label_DistToNearest_Feet.Text = (parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value * 3.280839895013123M).ToString("N2");
				Label_DistToNearest_Miles.Text = ((parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value / 1000) * 0.621371192237334M).ToString("N2");
			}	
		}
		private void SetRoads_ToMainHaul(Label Label_ToMainHaul_Meters, Label Label_ToMainHaul_Feet, Label Label_ToMainHaul_Miles)
		{
			if (parcel.ParcelRoadDistances.ToMainHaul.HasValue)
			{
				Label_ToMainHaul_Meters.Text = parcel.ParcelRoadDistances.ToMainHaul.Value.ToString("N2");
				Label_ToMainHaul_Feet.Text = (parcel.ParcelRoadDistances.ToMainHaul.Value * 3.280839895013123M).ToString("N2");
				Label_ToMainHaul_Miles.Text = ((parcel.ParcelRoadDistances.ToMainHaul.Value / 1000) * 0.621371192237334M).ToString("N2");
			}
		}
		private void SetRoads_MainHaulToPaved(Label Label_MainHaulToPaved_Meters, Label Label_MainHaulToPaved_Feet, Label Label_MainHaulToPaved_Miles)
		{
			if (parcel.ParcelRoadDistances.MainHaulToPaved.HasValue)
			{
				Label_MainHaulToPaved_Meters.Text = parcel.ParcelRoadDistances.MainHaulToPaved.Value.ToString("N2");
				Label_MainHaulToPaved_Feet.Text = (parcel.ParcelRoadDistances.MainHaulToPaved.Value * 3.280839895013123M).ToString("N2");
				Label_MainHaulToPaved_Miles.Text = ((parcel.ParcelRoadDistances.MainHaulToPaved.Value / 1000) * 0.621371192237334M).ToString("N2");
			}
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
				var discount = futureCosts / (Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation) * (1 + EconVariables.RealDiscount)),years)));
				Label_RoadSchedule.Text = (EarliestHarvestDate - 2).ToString();
				if (EarliestHarvestDate == DateTime.Now.Year)
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
		private void SetTotalValueBasedOnOperableCommercialAcres(Label Label_TotalCommercialAcres, Label Label_TotalValue, Label Label_TotalValueAcre, List<IReportParcelTimberStatistic> stats)
		{
			var standids = stats.Where(uu => uu.StandStats == StandStats.Operable).Select(uu => uu.Stand_ID).Distinct().ToList<int>();

			var opacres =  (from p in parcel.ParcelRiparians
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
		private void SetValuePerForestedAcre(Label Label_TotalAcresForested, Label Label_TotalValueForestedPerAcre, List<IReportParcelTimberStatistic> stats)
		{
			var standids = stats.Where(uu => uu.StandStats == StandStats.Operable || uu.StandStats == StandStats.ActChan || uu.StandStats == StandStats.Riparian).Select(uu => uu.Stand_ID).Distinct().ToList<int>();
			var acres = (from p in parcel.ParcelRiparians
						   where (from s in standids select s).Contains(p.STD_ID)
						   && (p.StandStatID == Convert.ToInt32(StandStats.Operable) || 
								p.StandStatID == Convert.ToInt32(StandStats.Riparian))
						   select p).Sum(uu => uu.Acres);
			var valperacre = TotalPresentValue / acres;
			Label_TotalAcresForested.Text = acres.ToString("N1");
			Label_TotalValueForestedPerAcre.Text = valperacre.ToString("C0");
		}
		private void SetValuePerEntireParcel(Label Label_TotalAcresEntireParcel, Label Label_TotalalueEntireParcelPerAcre)
		{
			var acres = parcel.Acres;
			var valperacre = TotalPresentValue / acres;
			Label_TotalAcresEntireParcel.Text = acres.ToString("N1");
			Label_TotalalueEntireParcelPerAcre.Text = valperacre.ToString("C0");
		}
		private void SetValueBareLandValue(Label Label_TotalBareLand, Label Label_BareLandValue, Label Label_TotalValuePareLandPerAcre)
		{
			var econVariables = new EconVariables(portfolio, rpaPortfolio);
			var tots = 0M;
			foreach(var standid in parcel.ParcelRiparians.Select(uu=>uu.STD_ID).Distinct())
			{
				var sev = new SEV(portfolio, rpaPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, parcel, standid, econVariables);
				var opt = sev.GetSEVRotationOptimum();
				tots += opt.RotationOptimum;
			}
			var acres = parcel.Acres;
			var valperacre = tots / acres;
			Label_TotalBareLand.Text = acres.ToString("N1");
			Label_BareLandValue.Text = tots.ToString("C0");
			Label_TotalValuePareLandPerAcre.Text = valperacre.ToString("C0");
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

			var rg = new RotationGenerator(portfolio, rpaPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, parcel, standid, EconVariables);
			decimal overallMax;
			var OuterList = rg.RunGenerator(out overallMax);
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

					Label_RotationLength_SEV.Text = rg.SEV.GetSEVRotationOptimum().Year.ToString();
					Label_SEV.Text = gc.SEV.RotationOptimum.ToString("C0");
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

		protected void Repeater_HarvestReport_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var standid = (int)e.Item.DataItem;
			var Label_HarvestReportStandIDNumber = e.Item.FindControl("Label_HarvestReportStandIDNumber") as Label;
			var harvestReport = new HarvestReport(portfolio, rpaPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, parcel, standid, EconVariables);

			var Repeater_HarvestReport_Species = e.Item.FindControl("Repeater_HarvestReport_Species") as Repeater;

			var Label_Year1 = e.Item.FindControl("Label_Year1") as Label;
			var Label_Year2 = e.Item.FindControl("Label_Year2") as Label;
			var Label_Year3 = e.Item.FindControl("Label_Year3") as Label;
			
			var Label_Year5 = e.Item.FindControl("Label_Year5") as Label;
			

			Label_HarvestReportStandIDNumber.Text = harvestReport.StandID.ToString();

			Label_Year1.Text = harvestReport.R1Year.ToString();
			Label_Year2.Text = harvestReport.R1Year.ToString();
			Label_Year3.Text = harvestReport.R2Year.ToString();
			Label_Year5.Text = harvestReport.R3Year.ToString();
			
			Repeater_HarvestReport_Species.DataSource = harvestReport.HarvestReportLogReportSpecies.Where(uu=>uu.HarvestReportItems.Any(cc=>cc.HasValue)).OrderBy(uu => uu.Specy.CommonName);
			Repeater_HarvestReport_Species.DataBind();
		}
		protected void Repeater_HarvestReport_Species_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var harvestReportLogReportSpecies = (HarvestReportLogReportSpecies)e.Item.DataItem;
			var Label_LogReportSpecies = e.Item.FindControl("Label_LogReportSpecies") as Label;
			Label_LogReportSpecies.Text = harvestReportLogReportSpecies.Specy.CommonName;

			var Repeater_HarvestReport_Sorts = e.Item.FindControl("Repeater_HarvestReport_Sorts") as Repeater;
			Repeater_HarvestReport_Sorts.DataSource = harvestReportLogReportSpecies.HarvestReportItems.Where(uu=>uu.HasValue == true).OrderBy(uu => uu.TimberMarket.OrderID);
			Repeater_HarvestReport_Sorts.DataBind();

		}
		protected void Repeater_HarvestReport_Sorts_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var harvestReportItem = (HarvestReportItem)e.Item.DataItem;
			var Label_Sort = e.Item.FindControl("Label_Sort") as Label;

			var Label_Volume_R1 = e.Item.FindControl("Label_Volume_R1") as Label;
			var Label_SortValue_R1 = e.Item.FindControl("Label_SortValue_R1") as Label;
			var Label_ValuePerMBF_R1 = e.Item.FindControl("Label_ValuePerMBF_R1") as Label;

			var Label_Volume_R2 = e.Item.FindControl("Label_Volume_R2") as Label;

			var Label_Volume_R3 = e.Item.FindControl("Label_Volume_R3") as Label;

			Label_Sort.Text = harvestReportItem.TimberMarket.Market;

			if (harvestReportItem.ValueAtHarvest_R1 > 0)
			{
				Label_Volume_R1.Text = harvestReportItem.ValueAtHarvest_R1.ToString("N0");
			}
			if (harvestReportItem.ValueSort_R1 > 0)
			{
				Label_SortValue_R1.Text = harvestReportItem.ValueSort_R1.ToString("C0");
			}
			if (harvestReportItem.ValueMBF_R1 > 0)
			{
				Label_ValuePerMBF_R1.Text = harvestReportItem.ValueMBF_R1.ToString("C0");
			}

			if (harvestReportItem.ValueAtHarvest_R2 > 0)
			{
				Label_Volume_R2.Text = harvestReportItem.ValueAtHarvest_R2.ToString("N0");
			}
			if (harvestReportItem.ValueAtHarvest_R3 > 0)
			{
				Label_Volume_R3.Text = harvestReportItem.ValueAtHarvest_R3.ToString("N0");
			}
		}
	}
}