using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;
using FRASS.Reports;
using Telerik.Web.UI;

namespace FRASS.Parcels
{
	public partial class Report : System.Web.UI.Page
	{
		Int32 parcelID;
		User thisUser;

		decimal TotalRiparianAcres = 0M;
		decimal TotalOperableAcres = 0M;
		decimal TotalAcresOnParcel = 0M;
		decimal TotaBF = 0M;
		decimal TotalBFCurrentParcel = 0M;

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
				LoadModels();
			}
		}
		protected void Page_Init(object sender, EventArgs e)
		{
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			dbParcelDataManager = ParcelDataManager.GetInstance();
			dbRPAPortfolioDataManager = RPAPortfolioDataManager.GetInstance();
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
				Server.Transfer("/welcome.aspx");
			}
		}
		protected void Button_ExportPDF_Click(object sender, EventArgs e)
		{
			Response.Redirect("/PDFs/Report.aspx?ParcelID=" + parcelID.ToString(), true);
		}
		protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
		{
			List<Parcel> parcels = new List<Parcel>();
			parcels.Add(dbParcelDataManager.GetParcel(parcelID));
			RadGrid1.DataSource = parcels;
		}
		protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
		{
			if (e.Item is GridDataItem)
			{

				Parcel parcel = (Parcel)e.Item.DataItem;
				StringBuilder txt = new StringBuilder();
				Label Label_User = e.Item.FindControl("Label_User") as Label;
				Label Label_ParcelNum = e.Item.FindControl("Label_ParcelNum") as Label;
				Label Label_Acres = e.Item.FindControl("Label_Acres") as Label;
				Label Label_OwnerCategory = e.Item.FindControl("Label_OwnerCategory") as Label;
				Label Label_AllotmentNumber = e.Item.FindControl("Label_AllotmentNumber") as Label;
				Label Label_Owner = e.Item.FindControl("Label_Owner") as Label;
				Label Label_Township = e.Item.FindControl("Label_Township") as Label;
				Label Label_Range = e.Item.FindControl("Label_Range") as Label;
				Label Label_Section = e.Item.FindControl("Label_Section") as Label;
				Label Label_LegalDesc = e.Item.FindControl("Label_LegalDesc") as Label;
				Label Label_County = e.Item.FindControl("Label_County") as Label;
				Label Label_CountyAssessorLandValue = e.Item.FindControl("Label_CountyAssessorLandValue") as Label;
				Label Label_CountyAssessorBuildingValue = e.Item.FindControl("Label_CountyAssessorBuildingValue") as Label;

				Label_User.Text = thisUser.FirstName + " " + thisUser.LastName;
				Label_ParcelNum.Text = parcel.ParcelNumber;
				Label_Acres.Text = String.Format("{0:0.0}", parcel.Acres);
				//var ot = parcel.ParcelAllotments.Select(uu => uu.OwnerType).FirstOrDefault();
				//Label_OwnerCategory.Text = ot.OwnerType1.ToString();

				StringBuilder ownerStringBuilder = new StringBuilder();


				var owner = new List<string>();
				foreach (var parcelOwner in parcel.ParcelOwners.Select(uu => uu.Owner).Distinct())
				{
					var str = parcelOwner.Name + "<br/>";
					str = str + parcelOwner.Address.Replace("<br />","") + "<br/>";
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

				foreach (var an in parcel.ParcelAllotments.Select(uu => uu.AllotmentNumber).Distinct())
				{
					allotnumbers.Append(an + "<br/>");
				}
				foreach (var t in parcel.ParcelAllotments.Select(uu => uu.Town).Distinct())
				{
					townshipSB.Append(t + "<br/>");
				}
				foreach (var r in parcel.ParcelAllotments.Select(uu => uu.Range).Distinct())
				{
					rangeSB.Append(r + "<br/>");
				}
				foreach (var s in parcel.ParcelAllotments.Select(uu => uu.Section).Distinct())
				{
					sectionSB.Append(s + "<br/>");
				}

				//Label_AllotmentNumber.Text = allotnumbers.ToString();

				foreach (var pc in parcel.ParcelCounties.Select(uu => uu.County.County1).Distinct())
				{
					countySB.Append(pc + "<br/>");
				}

				Label_Township.Text = townshipSB.ToString();
				Label_Range.Text = rangeSB.ToString();
				Label_Section.Text = sectionSB.ToString();

				Label_County.Text = countySB.ToString();
				
				foreach (var pl in parcel.ParcelLegals.OrderBy(uu => uu.Legal))
				{
					Label_LegalDesc.Text += pl.Legal + "<br/>";
				}
				
				var Repeater_DerivedDataOnParcel = e.Item.FindControl("Repeater_DerivedDataOnParcel") as Repeater;
				List<MUKeyParcel> mukeyparcels = new List<MUKeyParcel>();
				mukeyparcels.AddRange(parcel.MUKeyParcels);

				Repeater_DerivedDataOnParcel.DataSource = mukeyparcels;
				Repeater_DerivedDataOnParcel.DataBind();

				var Repeater_SoilSurveyMUKEY = e.Item.FindControl("Repeater_SoilSurveyMUKEY") as Repeater;
				Repeater_SoilSurveyMUKEY.DataSource = mukeyparcels;
				Repeater_SoilSurveyMUKEY.DataBind();

				var Repeater_SoilSurveyAcres = e.Item.FindControl("Repeater_SoilSurveyAcres") as Repeater;
				Repeater_SoilSurveyAcres.DataSource = mukeyparcels;
				Repeater_SoilSurveyAcres.DataBind();

				var Repeater_FireDamageSusceptibility = e.Item.FindControl("Repeater_FireDamageSusceptibility") as Repeater;
				Repeater_FireDamageSusceptibility.DataSource = mukeyparcels;
				Repeater_FireDamageSusceptibility.DataBind();

				var Repeater_FireDamagePotential = e.Item.FindControl("Repeater_FireDamagePotential") as Repeater;
				Repeater_FireDamagePotential.DataSource = mukeyparcels;
				Repeater_FireDamagePotential.DataBind();

				var Repeater_SoilRuttingHazard = e.Item.FindControl("Repeater_SoilRuttingHazard") as Repeater;
				Repeater_SoilRuttingHazard.DataSource = mukeyparcels;
				Repeater_SoilRuttingHazard.DataBind();

				var Repeater_SuitabilityForRoadsNaturalSurface = e.Item.FindControl("Repeater_SuitabilityForRoadsNaturalSurface") as Repeater;
				Repeater_SuitabilityForRoadsNaturalSurface.DataSource = mukeyparcels;
				Repeater_SuitabilityForRoadsNaturalSurface.DataBind();

				var Repeater_SuitabilityForLogLandings = e.Item.FindControl("Repeater_SuitabilityForLogLandings") as Repeater;
				Repeater_SuitabilityForLogLandings.DataSource = mukeyparcels;
				Repeater_SuitabilityForLogLandings.DataBind();

				var Repeater_Construction = e.Item.FindControl("Repeater_Construction") as Repeater;
				Repeater_Construction.DataSource = mukeyparcels;
				Repeater_Construction.DataBind();

				var Repeater_Harvest = e.Item.FindControl("Repeater_Harvest") as Repeater;
				Repeater_Harvest.DataSource = mukeyparcels;
				Repeater_Harvest.DataBind();

				var Repeater_SitePrepSurface = e.Item.FindControl("Repeater_SitePrepSurface") as Repeater;
				Repeater_SitePrepSurface.DataSource = mukeyparcels;
				Repeater_SitePrepSurface.DataBind();

				var Repeater_SitePrepDeep = e.Item.FindControl("Repeater_SitePrepDeep") as Repeater;
				Repeater_SitePrepDeep.DataSource = mukeyparcels;
				Repeater_SitePrepDeep.DataBind();

				var Repeater_Hand = e.Item.FindControl("Repeater_Hand") as Repeater;
				Repeater_Hand.DataSource = mukeyparcels;
				Repeater_Hand.DataBind();

				var Repeater_Mech = e.Item.FindControl("Repeater_Mech") as Repeater;
				Repeater_Mech.DataSource = mukeyparcels;
				Repeater_Mech.DataBind();

				var Repeater_SeedlingMortality = e.Item.FindControl("Repeater_SeedlingMortality") as Repeater;
				Repeater_SeedlingMortality.DataSource = mukeyparcels;
				Repeater_SeedlingMortality.DataBind();

				var Repeater_SIDougFir = e.Item.FindControl("Repeater_SIDougFir") as Repeater;
				Repeater_SIDougFir.DataSource = mukeyparcels;
				Repeater_SIDougFir.DataBind();

				var Repeater_SIWesternRedAlder = e.Item.FindControl("Repeater_SIWesternRedAlder") as Repeater;
				Repeater_SIWesternRedAlder.DataSource = mukeyparcels;
				Repeater_SIWesternRedAlder.DataBind();

				var Label_MarbledMurrelet = e.Item.FindControl("Label_MarbledMurrelet") as Label;
				var Label_NorthernSpotedOwl = e.Item.FindControl("Label_NorthernSpotedOwl") as Label;
				var Label_Eagle = e.Item.FindControl("Label_Eagle") as Label;

				var pmtxt = new StringBuilder();
				foreach (ParcelMurrelett pm in parcel.ParcelMurreletts)
				{
					if (pm.MM_Suita != string.Empty)
					{
						pmtxt.Append(pm.Acres.ToString("N2") + " " + pm.MM_Suita + "<br/>");
					}
				}
				Label_MarbledMurrelet.Text = pmtxt.ToString();
				Label_NorthernSpotedOwl.Text = parcel.Acres.ToString("N2") + " Unsuitable Habitat";
				Label_Eagle.Text = parcel.Acres.ToString("N2") + " Unsuitable Habitat";

				var Label_MinElevationMeters = e.Item.FindControl("Label_MinElevationMeters") as Label;
				var Label_MaxElevationMeters = e.Item.FindControl("Label_MaxElevationMeters") as Label;
				var Label_MeanElevationMeters = e.Item.FindControl("Label_MeanElevationMeters") as Label;

				var Label_MinElevationFeet = e.Item.FindControl("Label_MinElevationFeet") as Label;
				var Label_MaxElevationFeet = e.Item.FindControl("Label_MaxElevationFeet") as Label;
				var Label_MeanElevationFeet = e.Item.FindControl("Label_MeanElevationFeet") as Label;

				var Label_MinSlope = e.Item.FindControl("Label_MinSlope") as Label;
				var Label_MaxSlope = e.Item.FindControl("Label_MaxSlope") as Label;
				var Label_MeanSlope = e.Item.FindControl("Label_MeanSlope") as Label;

				if (parcel.ParcelStats != null)
				{
					Label_MinElevationFeet.Text = parcel.ParcelStats.ElevationMinFt.ToString("N0");
					Label_MaxElevationFeet.Text = parcel.ParcelStats.ElevationMaxFt.ToString("N0");
					Label_MeanElevationFeet.Text = parcel.ParcelStats.ElevationMeanFt.ToString("N0");
					Label_MinSlope.Text = parcel.ParcelStats.SlopeMin.ToString("N0");
					Label_MaxSlope.Text = parcel.ParcelStats.SlopeMax.ToString("N0");
					Label_MeanSlope.Text = parcel.ParcelStats.SlopeMean.ToString("N0");
				}

				var Label_ExistingSurfaceLengthMeters = e.Item.FindControl("Label_ExistingSurfaceLengthMeters") as Label;
				var Label_ExistingSurfaceLengthFeets = e.Item.FindControl("Label_ExistingSurfaceLengthFeets") as Label;
				var Label_ExistingSurfaceLengthMiles = e.Item.FindControl("Label_ExistingSurfaceLengthMiles") as Label;

				var Label_ExistingMainHaulLengthMeters = e.Item.FindControl("Label_ExistingMainHaulLengthMeters") as Label;
				var Label_ExistingMainHaulLengthFeet = e.Item.FindControl("Label_ExistingMainHaulLengthFeet") as Label;
				var Label_ExistingMainHaulLengthMiles = e.Item.FindControl("Label_ExistingMainHaulLengthMiles") as Label;

				var Label_ExistingPavedLengthMeters = e.Item.FindControl("Label_ExistingPavedLengthMeters") as Label;
				var Label_ExistingPavedLengthFeet = e.Item.FindControl("Label_ExistingPavedLengthFeet") as Label;
				var Label_ExistingPavedLengthMiles = e.Item.FindControl("Label_ExistingPavedLengthMiles") as Label;

				if (parcel.ParcelRoadUseLengths != null)
				{
					if (parcel.ParcelRoadUseLengths.SurfaceLength.HasValue)
					{
						Label_ExistingSurfaceLengthMeters.Text = parcel.ParcelRoadUseLengths.SurfaceLength.Value.ToString("N0");
						Label_ExistingSurfaceLengthFeets.Text = (parcel.ParcelRoadUseLengths.SurfaceLength.Value * 3.280839895013123M).ToString("N0");
						Label_ExistingSurfaceLengthMiles.Text = ((parcel.ParcelRoadUseLengths.SurfaceLength.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadUseLengths.MainHaulLength.HasValue)
					{
						Label_ExistingMainHaulLengthMeters.Text = parcel.ParcelRoadUseLengths.MainHaulLength.Value.ToString("N0");
						Label_ExistingMainHaulLengthFeet.Text = (parcel.ParcelRoadUseLengths.MainHaulLength.Value * 3.280839895013123M).ToString("N0");
						Label_ExistingMainHaulLengthMiles.Text = ((parcel.ParcelRoadUseLengths.MainHaulLength.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadUseLengths.PavedLength.HasValue)
					{
						Label_ExistingPavedLengthMeters.Text = parcel.ParcelRoadUseLengths.PavedLength.Value.ToString("N0");
						Label_ExistingPavedLengthFeet.Text = (parcel.ParcelRoadUseLengths.PavedLength.Value * 3.280839895013123M).ToString("N0");
						Label_ExistingPavedLengthMiles.Text = ((parcel.ParcelRoadUseLengths.PavedLength.Value / 1000) * 0.621371192237334M).ToString("N1");
					}

				}
				var Label_DistToNearest_Meters = e.Item.FindControl("Label_DistToNearest_Meters") as Label;
				var Label_DistToNearest_Feet = e.Item.FindControl("Label_DistToNearest_Feet") as Label;
				var Label_DistToNearest_Miles = e.Item.FindControl("Label_DistToNearest_Miles") as Label;
				var Label_NearRoadUse = e.Item.FindControl("Label_NearRoadUse") as Label;

				var Label_ToSurface_Meters = e.Item.FindControl("Label_ToSurface_Meters") as Label;
				var Label_ToSurface_Feet = e.Item.FindControl("Label_ToSurface_Feet") as Label;
				var Label_ToSurface_Miles = e.Item.FindControl("Label_ToSurface_Miles") as Label;

				var Label_MainHaul_Meters = e.Item.FindControl("Label_MainHaul_Meters") as Label;
				var Label_MainHaul_Feet = e.Item.FindControl("Label_MainHaul_Feet") as Label;
				var Label_MainHaul_Miles = e.Item.FindControl("Label_MainHaul_Miles") as Label;

				if (parcel.ParcelRoadDistances != null)
				{

					Label_NearRoadUse.Text = parcel.ParcelRoadDistances.NearRoadUseFromParcelBoundary;

					if (parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
					{
						Label_DistToNearest_Meters.Text = parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value.ToString("N0");
						Label_DistToNearest_Feet.Text = (parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value * 3.280839895013123M).ToString("N0");
						Label_DistToNearest_Miles.Text = ((parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadDistances.MainHaulToPaved.HasValue)
					{
						Label_ToSurface_Meters.Text = parcel.ParcelRoadDistances.MainHaulToPaved.Value.ToString("N0");
						Label_ToSurface_Feet.Text = (parcel.ParcelRoadDistances.MainHaulToPaved.Value * 3.280839895013123M).ToString("N0");
						Label_ToSurface_Miles.Text = ((parcel.ParcelRoadDistances.MainHaulToPaved.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
					if (parcel.ParcelRoadDistances.ToMainHaul.HasValue)
					{
						Label_MainHaul_Meters.Text = parcel.ParcelRoadDistances.ToMainHaul.Value.ToString("N0");
						Label_MainHaul_Feet.Text = (parcel.ParcelRoadDistances.ToMainHaul.Value * 3.280839895013123M).ToString("N0");
						Label_MainHaul_Miles.Text = ((parcel.ParcelRoadDistances.ToMainHaul.Value / 1000) * 0.621371192237334M).ToString("N1");
					}
				}

				if (parcel.ParcelHaulZones.Any())
				{
					var Label_HaulZoneNumber = e.Item.FindControl("Label_HaulZoneNumber") as Label;
					Label_HaulZoneNumber.Text = parcel.ParcelHaulZones.FirstOrDefault().HaulZoneID.ToString();
				}

				var Repeater_TimberStandStatistics = e.Item.FindControl("Repeater_TimberStandStatistics") as Repeater;
				Repeater_TimberStandStatistics.DataSource = dbParcelDataManager.GetReportParcelTimberStatistics(parcel.ParcelID, DateTime.Now.Year).OrderBy(uu => uu.Stand_ID);
				Repeater_TimberStandStatistics.DataBind();

				var Label_TotalRiparianAcres = e.Item.FindControl("Label_TotalRiparianAcres") as Label;
				var Label_TotalOperableAcres = e.Item.FindControl("Label_TotalOperableAcres") as Label;
				var Label_TotalAcresOnParcel = e.Item.FindControl("Label_TotalAcresOnParcel") as Label;
				var Label_TotalBF = e.Item.FindControl("Label_TotalBF") as Label;


				Label_TotalRiparianAcres.Text = TotalRiparianAcres.ToString("N2");
				Label_TotalOperableAcres.Text = TotalOperableAcres.ToString("N2");
				Label_TotalAcresOnParcel.Text = TotalAcresOnParcel.ToString("N2");
				Label_TotalBF.Text = TotaBF.ToString("N0");

				var Repeater_CurrentParcelTimberSummary = e.Item.FindControl("Repeater_CurrentParcelTimberSummary") as Repeater;
				Repeater_CurrentParcelTimberSummary.DataSource = dbParcelDataManager.GetCurrentParcelTimberSummary(parcel.ParcelID, DateTime.Now.Year).OrderBy(uu => uu.CommonName);
				Repeater_CurrentParcelTimberSummary.DataBind();

				var Label_TotalBFCurrentParcel = e.Item.FindControl("Label_TotalBFCurrentParcel") as Label;
				Label_TotalBFCurrentParcel.Text = TotalBFCurrentParcel.ToString("N0");

				var stands = parcel.ParcelRiparians.Select(uu => uu).ToList<ParcelRiparian>();
				var Repeater_Imagery = e.Item.FindControl("Repeater_Imagery") as Repeater;

				var filePath = ConfigurationManager.AppSettings.Get("ImagePath").ToString();
				var maps = filePath + "Maps\\";
				var photos = filePath + "Photos\\";
				Repeater_Imagery.DataSource = DisplayImage.GetMapImagesByFullPaths(parcel, stands, maps, photos);
				Repeater_Imagery.DataBind();
			}
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

			var rpaPortfolios = dbRPAPortfolioDataManager.GetRPAPortfolios(thisUser);
			if (rpaPortfolios.Count() > 0)
			{
				DropDownList_RPA.Visible = true;
				DropDownList_RPA.Items.Clear();
				DropDownList_RPA.Items.Add(new ListItem("--- RPA Portfolios ---", "0"));
				DropDownList_RPA.Items.Add(new ListItem("No RPA Portfolio", "-1"));
				foreach (var model in rpaPortfolios.OrderBy(uu => uu.PortfolioName))
				{
					DropDownList_RPA.Items.Add(new ListItem(model.PortfolioName, model.ModelID.ToString()));
				}
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
		}
		protected void Button_ApplyMarketModel_Click(object sender, EventArgs e)
		{
			string id = DropDownList_MarketModels.SelectedValue;
			string rpaID = DropDownList_RPA.SelectedValue;
			if (id != "0" && rpaID != "0")
			{
				Response.Redirect("/Parcels/ReportApplied.aspx?ParcelID=" + parcelID.ToString() + "&MarketModelPortfolioID=" + id + "&RPAPortfolioID=" + rpaID, true);
			}
		}
		protected void Button_ApplyStumpageModels_Click(object sender, EventArgs e)
		{
			string id = DropDownList_StumpageModels.SelectedValue;
			if (id != "0")
			{
				Response.Redirect("/Parcels/StumpageApplied.aspx?ParcelID=" + parcelID.ToString() + "&StumpageModelPortfolioID=" + id, true);
			}
		}

		protected void Repeater_ZoningItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var parcelZoning = e.Item.DataItem as ParcelZoning;
			var Label_Zoning = e.Item.FindControl("Label_Zoning") as Label;
			var Label_ZoningAcres = e.Item.FindControl("Label_ZoningAcres") as Label;
			Label_Zoning.Text = parcelZoning.ParcelZoningType.Zoning;
			Label_ZoningAcres.Text = parcelZoning.Acres.ToString("N1");
		}
		protected void Repeater_DerivedDataOnParcelItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_MUKey = e.Item.FindControl("Label_MUKey") as Label;
			var Label_MUSYM = e.Item.FindControl("Label_MUSYM") as Label;
			var Label_MUKeyAcres = e.Item.FindControl("Label_MUKeyAcres") as Label;
			var Label_MUKeyDescription = e.Item.FindControl("Label_MUKeyDescription") as Label;
			Label_MUKey.Text = mukey.MUKey.MUKey1.ToString();
			Label_MUSYM.Text = mukey.MUKey.MUSym.ToString();
			Label_MUKeyAcres.Text = mukey.Acres.ToString("N1");
			Label_MUKeyDescription.Text = mukey.MUKey.Description;
		}
		protected void Repeater_SoilSurveyMUKEY_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_MUKEY = e.Item.FindControl("Label_MUKEY") as Label;
			Label_MUKEY.Text = mukey.MUKey.MUKey1.ToString() + "<br/>" + mukey.MUKey.MUSym.ToString();
		}
		protected void Repeater_SoilSurveyAcres_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_Acres = e.Item.FindControl("Label_Acres") as Label;
			Label_Acres.Text = mukey.Acres.ToString("N1");
		}

		protected void Repeater_FireDamageSusceptibility_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_FireDamageSusceptibility = e.Item.FindControl("Label_FireDamageSusceptibility") as Label;
			Label_FireDamageSusceptibility.Text = mukey.MUKey.FireDamageSusceptibility;
		}
		protected void Repeater_FireDamagePotential_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_FireDamagePotential = e.Item.FindControl("Label_FireDamagePotential") as Label;
			Label_FireDamagePotential.Text = mukey.MUKey.FireDamagePotential;
		}
		protected void Repeater_SoilRuttingHazard_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_SoilRuttingHazard = e.Item.FindControl("Label_SoilRuttingHazard") as Label;
			Label_SoilRuttingHazard.Text = mukey.MUKey.SoilRuttingHazard;
		}
		protected void Repeater_SuitabilityForRoadsNaturalSurface_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_SuitabilityForRoadsNaturalSurface = e.Item.FindControl("Label_SuitabilityForRoadsNaturalSurface") as Label;
			Label_SuitabilityForRoadsNaturalSurface.Text = mukey.MUKey.SuitabilityForRoadsNaturalSurface;
		}
		protected void Repeater_SuitabilityForLogLandings_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_SuitabilityForLogLandings = e.Item.FindControl("Label_SuitabilityForLogLandings") as Label;
			Label_SuitabilityForLogLandings.Text = mukey.MUKey.SuitabilityForLogLandings;
		}
		protected void Repeater_Construction_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_Construction = e.Item.FindControl("Label_Construction") as Label;
			Label_Construction.Text = mukey.MUKey.ConstructionLimitationsForHaulRoadsAndLogLandings;
		}
		protected void Repeater_Harvest_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_Harvest = e.Item.FindControl("Label_Harvest") as Label;
			Label_Harvest.Text = mukey.MUKey.HarvestEquipmentOperability;
		}
		protected void Repeater_SitePrepSurface_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_Surface = e.Item.FindControl("Label_Surface") as Label;
			Label_Surface.Text = mukey.MUKey.MechanicalSitePreparationSurface;
		}
		protected void Repeater_SitePrepDeep_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_Deep = e.Item.FindControl("Label_Deep") as Label;
			Label_Deep.Text = mukey.MUKey.MechanicalSitePreparationDeep;
		}
		protected void Repeater_Hand_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_Hand = e.Item.FindControl("Label_Hand") as Label;
			Label_Hand.Text = mukey.MUKey.SuitabilityForHandPlanting;
		}
		protected void Repeater_Mech_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_Mech = e.Item.FindControl("Label_Mech") as Label;
			Label_Mech.Text = mukey.MUKey.SuitabilityForMechanicalPlanting;
		}
		protected void Repeater_SeedlingMortality_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_SeedlingMortality = e.Item.FindControl("Label_SeedlingMortality") as Label;
			Label_SeedlingMortality.Text = mukey.MUKey.PotentialForSeedlingMortality;
		}
		protected void Repeater_SIDougFir_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_SI = e.Item.FindControl("Label_SI") as Label;
			Label_SI.Text = "&ndash;&ndash;";
			if (mukey.MUKey.TreeSiteIndexDouglasFir.HasValue)
			{
				Label_SI.Text = mukey.MUKey.TreeSiteIndexDouglasFir.Value.ToString();
			}
		}
		protected void Repeater_SIWesternRedAlder_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var mukey = e.Item.DataItem as MUKeyParcel;
			var Label_SI = e.Item.FindControl("Label_SI") as Label;
			Label_SI.Text = "&ndash;&ndash;";
			if (mukey.MUKey.TreeSiteIndexWesternRedAlder.HasValue)
			{
				Label_SI.Text = mukey.MUKey.TreeSiteIndexWesternRedAlder.Value.ToString();
			}
		}
			
		protected void Repeater_EndangeredSpeciesItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Label_Acres = e.Item.FindControl("Label_Acres") as Label;
		}
		protected void Repeater_TimberStandStatisticsItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var pr = (IReportParcelTimberStatistics)e.Item.DataItem;

			var Label_StandID = e.Item.FindControl("Label_StandID") as Label;
			var Label_VegLabel = e.Item.FindControl("Label_VegLabel") as Label;
			var Label_SI = e.Item.FindControl("Label_SI") as Label;
			var Label_AcresNotOperable = e.Item.FindControl("Label_AcresNotOperable") as Label;
			var Label_AcresOperable = e.Item.FindControl("Label_AcresOperable") as Label;
			var Label_TotalAcres = e.Item.FindControl("Label_TotalAcres") as Label;
			var Label_BFAcrePerStand = e.Item.FindControl("Label_BFAcrePerStand") as Label;
			var Label_TotalBFOnStand = e.Item.FindControl("Label_TotalBFOnStand") as Label;

			Label_StandID.Text = pr.Stand_ID.ToString();
			Label_VegLabel.Text = pr.Veg_Label;
			Label_SI.Text = pr.Site_Index.ToString();
			Label_AcresNotOperable.Text = pr.Riparian_Zone_Acres.ToString("N2");
			Label_AcresOperable.Text = pr.Operable_Land_Acres.ToString("N2");
			Label_BFAcrePerStand.Text = pr.BFAcre_PerStand.ToString("N0");
			Label_TotalAcres.Text = pr.Total_Acres.ToString("N2");
			Label_TotalBFOnStand.Text = pr.TotalBF_PerStand.ToString("N0");

			TotalRiparianAcres = TotalRiparianAcres + pr.Riparian_Zone_Acres;
			TotalOperableAcres = TotalOperableAcres + pr.Operable_Land_Acres;
			TotalAcresOnParcel = TotalAcresOnParcel + pr.Total_Acres;
			TotaBF = TotaBF + pr.TotalBF_PerStand;
		}

		protected void Repeater_CurrentParcelTimberSummary_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var result = (GetCurrentParcelTimberSummaryResult)e.Item.DataItem;
			var Label_Species = e.Item.FindControl("Label_Species") as Label;
			var Label_TotalBF = e.Item.FindControl("Label_TotalBF") as Label;

			Label_Species.Text = result.CommonName;
			Label_TotalBF.Text = result.TotalBF.Value.ToString("N0");
			TotalBFCurrentParcel = TotalBFCurrentParcel + result.TotalBF.Value;
		}

		protected void Repeater_Imagery_ItemBound(object sender, RepeaterItemEventArgs e)
		{
			var map = e.Item.DataItem as DisplayImage;
			var Literal_Map = e.Item.FindControl("Literal_Map") as Literal;
			Literal_Map.Text = "<a class=\"group1 cboxElement\" href=\"" + map.PathToRoot + "\"  style=\"color: blue\">" + map.FileName + "</a>";

		}

		private Double MAI_cuacyr(Int32 si)
		{
			double val = 2.628 * si;
			val = val - 49.8;
			return val;
		}
		private Double MAI_bfacyr(Double mai_cauyr)
		{
			Double val = mai_cauyr * 0.012;
			return val;
		}
		private Int32 Stumpage(Int32 delivered, Int32 LogNHaul)
		{
			return delivered - LogNHaul;
		}
		private Int32 MI(Int32 si, Int32 years, Int32 stumpage)
		{
			Double mi = ((2.628 * si) - 49.8) * (0.012 * years / 100) * stumpage;
			return Convert.ToInt32(mi);
		}
		private Int32 V_O(Int32 years, Int32 mi, Double discountRate)
		{
			Double val = (years * mi) / (Math.Pow((1 + discountRate), years)) - 1;
			return Convert.ToInt32(val);
		}
		private Int32 Volume(Int32 si, Int32 years)
		{
			Double val = ((2.628 * si) - 49.8) * years;
			return Convert.ToInt32(val);
		}



	}
}
