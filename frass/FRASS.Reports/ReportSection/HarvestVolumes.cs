using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Pdf.Generator;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.BLL.Models;
using FRASS.Interfaces;

namespace FRASS.Reports.ReportSection
{
	internal class MarketModelHarvestVolumes : HarvestVolumesBase
	{
		private MarketModelPortfolio Portfolio;
		private RPAPortfolio RPAPortfolio;
		private List<RPAPortfolioDetail> RPAPortfolioDetails;
		private List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails;

		public MarketModelHarvestVolumes(Table table, Parcel parcel, MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<IReportParcelTimberStatistic> timberStats)
			: base(table, parcel, timberStats)
		{
			Parcel = parcel;
			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			RPAPortfolioDetails = rpaPortfolio.RPAPortfolioDetails.ToList<RPAPortfolioDetail>();
			MarketModelPortfolioDeliveredLogModelDetails = portfolio.MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
			EconVariables = new EconVariables(portfolio, rpaPortfolio);
			TimberStats = timberStats;
		}

		protected override void SetUpStandValue(int standid)
		{
			var acres = Parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Sum(uu => uu.Acres);
			var econ = new EconVariables(Portfolio, RPAPortfolio);
			var rg = new RotationGenerator(Portfolio, RPAPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, Parcel, standid, econ);
			decimal overallMax;
			var OuterList = rg.RunGenerator(out overallMax);
			var o = OuterList.Where(uu => uu.MaxValue == overallMax).FirstOrDefault();
			var harvestYear = "--";
			var npv = "--";
			var r2length = "--";
			var r2npv = "--";
			var r3length = "--";
			var r3npv = "--";
			var tpv = "--";
			var tpvPerAcre = "--";
			if (o != null)
			{
				var gc = o.Cuts.Where(uu => uu.Total.RotationOptimum == overallMax).FirstOrDefault();
				if (gc != null)
				{
					harvestYear = o.HarvestYear.ToString();
					npv = gc.R1.RotationOptimum.ToString("C0");


					r2length = gc.Year.ToString();
					r2npv = gc.R2.RotationOptimum.ToString("C0");

					r3length = rg.SEV.GetSEVRotationOptimum().Year.ToString();
					r3npv = gc.SEV.RotationOptimum.ToString("C0");
				}
				if (EarliestHarvestDate > o.HarvestYear)
				{
					EarliestHarvestDate = o.HarvestYear;
				}
				if (EarliestHarvestDate < minYear)
				{
					EarliestHarvestDate = minYear;
				}


				if (acres > 0)
				{
					var valPerAcre = overallMax / acres;
					tpvPerAcre = valPerAcre.ToString("C0");
				}
				tpv = overallMax.ToString("C0");
				TotalPresentValue = TotalPresentValue + overallMax;
			}
		}
		protected override void SetStandValue(int standid)
		{

			var acres = Parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Sum(uu => uu.Acres);
			var econ = new EconVariables(Portfolio, RPAPortfolio);
			var rg = new RotationGenerator(Portfolio, RPAPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, Parcel, standid, econ);
			decimal overallMax;
			var OuterList = rg.RunGenerator(out overallMax);
			var o = OuterList.Where(uu => uu.MaxValue == overallMax).FirstOrDefault();
			var harvestYear = "--";
			var npv = "--";
			var r2length = "--";
			var r2npv = "--";
			var r3length = "--";
			var r3npv = "--";
			var tpv = "--";
			var tpvPerAcre = "--";
			if (o != null)
			{
				var gc = o.Cuts.Where(uu => uu.Total.RotationOptimum == overallMax).FirstOrDefault();
				if (gc != null)
				{
					harvestYear = o.HarvestYear.ToString();
					npv = gc.R1.RotationOptimum.ToString("C0");


					r2length = gc.Year.ToString();
					r2npv = gc.R2.RotationOptimum.ToString("C0");

					r3length = rg.SEV.GetSEVRotationOptimum().Year.ToString();
					r3npv = gc.SEV.RotationOptimum.ToString("C0");
				}
				if (EarliestHarvestDate > o.HarvestYear)
				{
					EarliestHarvestDate = o.HarvestYear;
				}
				if (EarliestHarvestDate < minYear)
				{
					EarliestHarvestDate = minYear;
				}


				if (acres > 0)
				{
					var valPerAcre = overallMax / acres;
					tpvPerAcre = valPerAcre.ToString("C0");
				}
				tpv = overallMax.ToString("C0");

				var row = HarvestVolumesTable.Rows.Add();
				ReportUtilities.AddHighlightCell(row, standid.ToString(), AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, acres.ToString("N2"), AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, harvestYear, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, npv, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, r2length, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, r2npv, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, r3length, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, r3npv, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, tpv, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, tpvPerAcre, AlignmentType.Right);
				row.Border = ReportUtilities.RowBottomBorderInfo;

				TotalPresentValue = TotalPresentValue + overallMax;
			}
		}
		protected override void SetRoadBareLand()
		{
			var tots = 0M;
			foreach (var standid in Parcel.ParcelRiparians.Select(uu => uu.STD_ID).Distinct())
			{
				var sev = new SEV(Portfolio, RPAPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, Parcel, standid, EconVariables);
				var opt = sev.GetSEVRotationOptimum();
				tots += opt.RotationOptimum;
			}
			var acres = Parcel.Acres;
			var valperacre = tots / acres;

			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Bare Land Value (Entire Parcel):", AlignmentType.Right, 7);
			ReportUtilities.AddLabelCellNonBold(row, acres.ToString("N1") + " Acres", AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, tots.ToString("C0"), AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, valperacre.ToString("C0") + "/Acre", AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
	}

	internal class StumpageModelHarvestVolumes : HarvestVolumesBase
	{
		private StumpageModelPortfolio Portfolio;
		private StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		private TimberDataManager dbTimberDataManager;
		private List<StumpageGroup> StumpageGroups;
		private List<TimberMarket> TimberMarkets;
		private List<StumpageGroupQualityCode> StumpageGroupQualityCodes;

		private List<Int32> ReportYears;
		private int HaulZoneID;

		public StumpageModelHarvestVolumes(Table table, Parcel parcel, StumpageModelPortfolio portfolio, List<IReportParcelTimberStatistic> timberStats)
			: base(table, parcel, timberStats)
		{
			Parcel = parcel;
			Portfolio = portfolio;
			EconVariables = new EconVariables(portfolio);
			TimberStats = timberStats;

			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbTimberDataManager = TimberDataManager.GetInstance();

			StumpageGroups = dbStumpageMarketModelDataManager.GetStumpageGroups();
			TimberMarkets = dbTimberDataManager.GetTimberMarkets();

			StumpageGroupQualityCodes = dbStumpageMarketModelDataManager.GetStumpageGroupQualityCodes();

			ReportYears = new List<int>();
			HaulZoneID = parcel.ParcelHaulZones.FirstOrDefault().HaulZoneID;
			for (var year = 5; year <= 200; year += 5)
			{
				ReportYears.Add(year);
			}
		}
		protected override void SetUpStandValue(int standid)
		{
			List<IStandData> CurrentStandData = dbStandDataManager.GetStandDataCurrent(standid, Parcel.ParcelID);
			List<IStandData> FutureStandData = dbStandDataManager.GetStandDataFuture(standid, Parcel.ParcelID);
			var sg = new StumpageGenerator(Parcel, standid, Portfolio, StumpageGroups, CurrentStandData, FutureStandData, TimberMarkets, HaulZoneID, minYear, EconVariables, StumpageGroupQualityCodes, CalendarYears, ReportYears);

			decimal overallMax;
			var OuterList = sg.GetGrowthCuts(out overallMax);
			var o = OuterList.Where(uu => uu.MaxValue == overallMax).FirstOrDefault();
			var acres = Parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Sum(uu => uu.Acres);
			var harvestYear = "--";
			var npv = "--";
			var r2length = "--";
			var r2npv = "--";
			var r3length = "--";
			var r3npv = "--";
			var tpv = "--";
			var tpvPerAcre = "--";

			if (o != null)
			{
				var gc = o.Cuts.Where(uu => uu.Total.RotationOptimum == overallMax).FirstOrDefault();
				if (gc != null)
				{
					harvestYear = o.HarvestYear.ToString();
					npv = gc.R1.RotationOptimum.ToString("C0");


					r2length = gc.Year.ToString();
					r2npv = gc.R2.RotationOptimum.ToString("C0");
					decimal sevMax = 0M;
					r3length = sg.StumpageSEV.GetMaxSEV(out sevMax).ToString();
					r3npv = gc.SEV.RotationOptimum.ToString("C0");
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
				tpv = valPerAcre.ToString("C0");
			}

			tpvPerAcre = overallMax.ToString("C0");
			TotalPresentValue = TotalPresentValue + overallMax;
		}
		protected override void SetStandValue(int standid)
		{
			List<IStandData> CurrentStandData = dbStandDataManager.GetStandDataCurrent(standid, Parcel.ParcelID);
			List<IStandData> FutureStandData = dbStandDataManager.GetStandDataFuture(standid, Parcel.ParcelID);
			var sg = new StumpageGenerator(Parcel, standid, Portfolio, StumpageGroups, CurrentStandData, FutureStandData, TimberMarkets, HaulZoneID, minYear, EconVariables, StumpageGroupQualityCodes, CalendarYears, ReportYears);

			decimal overallMax;
			var OuterList = sg.GetGrowthCuts(out overallMax);
			var o = OuterList.Where(uu => uu.MaxValue == overallMax).FirstOrDefault();
			var acres = Parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Sum(uu => uu.Acres);
			var harvestYear = "--";
			var npv = "--";
			var r2length = "--";
			var r2npv = "--";
			var r3length = "--";
			var r3npv = "--";
			var tpv = "--";
			var tpvPerAcre = "--";

			if (o != null)
			{
				var gc = o.Cuts.Where(uu => uu.Total.RotationOptimum == overallMax).FirstOrDefault();
				if (gc != null)
				{
					harvestYear = o.HarvestYear.ToString();
					npv = gc.R1.RotationOptimum.ToString("C0");


					r2length = gc.Year.ToString();
					r2npv = gc.R2.RotationOptimum.ToString("C0");
					decimal sevMax = 0M;
					r3length = sg.StumpageSEV.GetMaxSEV(out sevMax).ToString();
					r3npv = gc.SEV.RotationOptimum.ToString("C0");
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
				tpv = valPerAcre.ToString("C0");
			}

			tpvPerAcre = overallMax.ToString("C0");

			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddHighlightCell(row, standid.ToString(), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, acres.ToString("N2"), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, harvestYear, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, npv, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, r2length, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, r2npv, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, r3length, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, r3npv, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, tpv, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, tpvPerAcre, AlignmentType.Right);
			row.Border = ReportUtilities.RowBottomBorderInfo;
			TotalPresentValue = TotalPresentValue + overallMax;
		}
		protected override void SetRoadBareLand()
		{
			var tots = 0M;
			foreach (var standid in Parcel.ParcelRiparians.Select(uu => uu.STD_ID).Distinct())
			{
				List<IStandData> CurrentStandData = dbStandDataManager.GetStandDataCurrent(standid, Parcel.ParcelID);
				List<IStandData> FutureStandData = dbStandDataManager.GetStandDataFuture(standid, Parcel.ParcelID);

				var sg = new StumpageGenerator(Parcel, standid, Portfolio, StumpageGroups, CurrentStandData, FutureStandData, TimberMarkets, HaulZoneID, minYear, EconVariables, StumpageGroupQualityCodes, CalendarYears, ReportYears);
				var maxSEV = 0M;
				var year = sg.StumpageSEV.GetMaxSEV(out maxSEV);
				tots += maxSEV;
			}
			var acres = Parcel.Acres;
			var valperacre = tots / acres;
			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Bare Land Value (Entire Parcel):", AlignmentType.Right, 7);
			ReportUtilities.AddLabelCellNonBold(row, acres.ToString("N1") + " Acres", AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, tots.ToString("C0"), AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, valperacre.ToString("C0") + "/Acre", AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
	}

}
