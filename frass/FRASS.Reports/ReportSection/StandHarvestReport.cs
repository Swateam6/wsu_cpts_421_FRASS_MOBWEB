using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;
using FRASS.BLL.Models;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;

namespace FRASS.Reports.ReportSection
{
	internal class MarketModelStandHarvestReport : StandHarvestReportBase
	{
		private MarketModelPortfolio Portfolio;
		private RPAPortfolio RPAPortfolio;
		private List<RPAPortfolioDetail> RPAPortfolioDetails;
		private List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails;

		public MarketModelStandHarvestReport(Parcel parcel, MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<RPAPortfolioDetail> rPAPortfolioDetails, List<MarketModelPortfolioDeliveredLogModelDetail> marketModelPortfolioDeliveredLogModelDetails)
			:base(parcel)
		{
			
			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			EconVariables = new EconVariables(Portfolio, RPAPortfolio);
			RPAPortfolioDetails = rPAPortfolioDetails;
			MarketModelPortfolioDeliveredLogModelDetails = marketModelPortfolioDeliveredLogModelDetails;
		}

		protected override void SetYearHeaders(Table table, int standid)
		{
			var harvestReport = new HarvestReport(Portfolio, RPAPortfolio, RPAPortfolioDetails,MarketModelPortfolioDeliveredLogModelDetails, Parcel, standid, EconVariables);

			var r1year = harvestReport.R1Year.ToString();
			var r2year = harvestReport.R2Year.ToString();
			var r3year = harvestReport.R3Year.ToString();

			var row = table.Rows.Add();
			ReportUtilities.AddLabelCellNonBold(row, "Species & Sort", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, "Volume at Harvest", AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, "Value/MBF in " + r1year.ToString(), AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, "Sort Value in " + r1year.ToString(), AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, "Volume at Harvest (" + r2year.ToString() + ")", AlignmentType.Center,2);
			ReportUtilities.AddLabelCellNonBold(row, "Volume at First Entry (" + r3year.ToString() + ")", AlignmentType.Center,2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
			var species = harvestReport.HarvestReportLogReportSpecies.Where(uu => uu.HarvestReportItems.Any(cc => cc.HasValue)).OrderBy(uu => uu.Specy.CommonName);
			foreach (var specy in species)
			{
				SetSpeciesHeader(table, specy);
			}
		}
		protected void SetSpeciesHeader(Table table, HarvestReportLogReportSpecies specy)
		{
			var row = table.Rows.Add();
			ReportUtilities.AddHighlightCellBold(row, specy.Specy.CommonName, AlignmentType.Left, 9);
			row.Border = ReportUtilities.RowBottomBorderInfo;
			var items = specy.HarvestReportItems.Where(uu => uu.HasValue == true).OrderBy(uu => uu.TimberMarket.OrderID);
			foreach (var item in items)
			{
				SetHarvestReportItem(table, item);
			}
		}
		protected void SetHarvestReportItem(Table table, HarvestReportItem harvestReportItem)
		{

			var col1 = "";
			var col2 = "";
			var col3 = "";
			var col4 = "";
			var col5 = "";

			if (harvestReportItem.ValueAtHarvest_R1 > 0)
			{
				col1 = harvestReportItem.ValueAtHarvest_R1.ToString("N0");
			}
			
			if (harvestReportItem.ValueMBF_R1 > 0)
			{
				col2 = harvestReportItem.ValueMBF_R1.ToString("C0");
			}
			if (harvestReportItem.ValueSort_R1 > 0)
			{
				col3 = harvestReportItem.ValueSort_R1.ToString("C0");
			}

			if (harvestReportItem.ValueAtHarvest_R2 > 0)
			{
				col4 = harvestReportItem.ValueAtHarvest_R2.ToString("N0");
			}
			if (harvestReportItem.ValueAtHarvest_R3 > 0)
			{
				col5 = harvestReportItem.ValueAtHarvest_R3.ToString("N0");
			}
			var row = table.Rows.Add();
			ReportUtilities.AddLabelCellNonBold(row, harvestReportItem.TimberMarket.Market, AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, col1, AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, col2, AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, col3, AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, col4, AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, col5, AlignmentType.Center, 2);
		}
	}

	internal class StumpageModelStandHarvestReport : StandHarvestReportBase
	{
		private StumpageModelPortfolio Portfolio;
		private StandDataManager dbStandDataManager;
		private StumpageMarketModelDataManager dbStumpageMarketModelDataManager;
		private TimberDataManager dbTimberDataManager;
		private List<StumpageGroup> StumpageGroups;
		private List<TimberMarket> TimberMarkets;
		private List<StumpageGroupQualityCode> StumpageGroupQualityCodes;
		private List<Int32> CalendarYears;
		private List<Int32> ReportYears;
		private int HaulZoneID;
		private int minYear;

		public StumpageModelStandHarvestReport(Parcel parcel, StumpageModelPortfolio portfolio)
			:base(parcel)
		{
			Portfolio = portfolio;
			EconVariables = new EconVariables(Portfolio);
			dbStandDataManager = StandDataManager.GetInstance();
			dbStumpageMarketModelDataManager = StumpageMarketModelDataManager.GetInstance();
			dbTimberDataManager = TimberDataManager.GetInstance();
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
		protected override void SetYearHeaders(Table table, int standid)
		{
			List<IStandData> CurrentStandData = dbStandDataManager.GetStandDataCurrent(standid, Parcel.ParcelID);
			List<IStandData> FutureStandData = dbStandDataManager.GetStandDataFuture(standid, Parcel.ParcelID);
			var sg = new StumpageGenerator(Parcel, standid, Portfolio, StumpageGroups, CurrentStandData, FutureStandData, TimberMarkets, HaulZoneID, minYear, EconVariables, StumpageGroupQualityCodes, CalendarYears, ReportYears);
			var harvestReport = sg.GetStumpageHarvestReport();

			var r1year = harvestReport.R1Year.ToString();
			var r2year = harvestReport.R2Year.ToString();
			var r3year = harvestReport.R3Year.ToString();

			var row = table.Rows.Add();
			ReportUtilities.AddLabelCellNonBold(row, "Species & Sort", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, "Volume at Harvest", AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, "Value/MBF in " + r1year.ToString(), AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, "Sort Value in " + r1year.ToString(), AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, "Volume at Harvest (" + r2year.ToString() + ")", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, "Volume at First Entry (" + r3year.ToString() + ")", AlignmentType.Center, 2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
			var species = harvestReport.StumpageGroups.OrderBy(uu => uu.StumpageGroupName);
			foreach (var specy in species)
			{
				SetSpeciesHeader(table, specy, harvestReport);
			}
		}
		protected void SetSpeciesHeader(Table table, StumpageGroup specy, StumpageHarvestReport harvestReport)
		{
			var row = table.Rows.Add();
			ReportUtilities.AddHighlightCellBold(row, specy.StumpageGroupName, AlignmentType.Left, 9);
			row.Border = ReportUtilities.RowBottomBorderInfo;
			var items = harvestReport.HarvestStumpageGroups.Where(uu => uu.StumpageGroup.StumpageGroupID == specy.StumpageGroupID);
			foreach (var item in items)
			{
				SetHarvestReportItem(table, item);
			}
		}
		protected void SetHarvestReportItem(Table table, HarvestStumpageGroup harvestReportItem)
		{
			var col1 = "";
			var col2 = "";
			var col3 = "";
			var col4 = "";
			var col5 = "";

			if (harvestReportItem.ValueAtHarvest_R1 > 0)
			{
				col1 = harvestReportItem.ValueAtHarvest_R1.ToString("N0");
			}
			if (harvestReportItem.ValueSort_R1 > 0)
			{
				col2 = harvestReportItem.ValueSort_R1.ToString("C0");
			}
			if (harvestReportItem.ValueMBF_R1 > 0)
			{
				col3 = harvestReportItem.ValueMBF_R1.ToString("C0");
			}

			if (harvestReportItem.ValueAtHarvest_R2 > 0)
			{
				col4 = harvestReportItem.ValueAtHarvest_R2.ToString("N0");
			}
			if (harvestReportItem.ValueAtHarvest_R3 > 0)
			{
				col5 = harvestReportItem.ValueAtHarvest_R3.ToString("N0");
			}
			var row = table.Rows.Add();
			ReportUtilities.AddLabelCellNonBold(row, harvestReportItem.QualityCodeNumber.ToString(), AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, col1, AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, col2, AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, col3, AlignmentType.Center);
			ReportUtilities.AddLabelCellNonBold(row, col4, AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, col5, AlignmentType.Center, 2);
		}
	}
}
