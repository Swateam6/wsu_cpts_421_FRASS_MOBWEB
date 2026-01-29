using System.Linq;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using System.Collections.Generic;
using System;
using FRASS.DAL;
using FRASS.BLL.Formulas;

namespace FRASS.Reports.ReportSection
{
	internal class MarketModelSortSettings
	{
		private ReportUtilities ReportUtilities;
		private Table MarketModelSettingsHeaderTable;
		private MarketModelPortfolio MarketModelPortfolio;
		private List<MarketModelData> MarketModelDataList;
		private RPAPortfolio RPAPortfolio;
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
					_ppiToday = MarketModelDataList.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault().Value;
				}
				return _ppiToday.Value;
			}
		}

		public MarketModelSortSettings(Table table, MarketModelPortfolio marketModelPortfolio, RPAPortfolio rpaPortfolio, List<MarketModelData> marketModelDataList)
		{
			ReportUtilities = new ReportUtilities(7);
			var cell = table.Rows.Add().Cells.Add();
			MarketModelSettingsHeaderTable = ReportUtilities.GetNewVariableColumnTable(cell,"12% 10% 7% 8% 8% 11% 7% 7% 15% 15%");
			MarketModelPortfolio = marketModelPortfolio;
			RPAPortfolio = rpaPortfolio;
			MarketModelDataList = marketModelDataList;
			cell.Paragraphs.Add(MarketModelSettingsHeaderTable);
		}
		public void SetMarketModelSortSettingsRow()
		{
			SetHeaderRow();
			SetSortRows();
		}
		private void SetHeaderRow()
		{
			var row = MarketModelSettingsHeaderTable.Rows.Add();
			ReportUtilities.AddLabelCellGray(row, "Sort", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Current Market Value", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "RPA", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Longevity Term", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Profit & Risk", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Overhead & Administration", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Logging Cost", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Hauling Cost", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Beginning Date", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Turning Point", AlignmentType.Left);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
		private void SetSortRows()
		{
			var details = MarketModelPortfolio.MarketModelPortfolioDeliveredLogModelDetails.Select(uu => uu.LogMarketReportSpecy).Distinct();
			
			foreach (var detail in details.OrderBy(uu => uu.LogMarketSpecies))
			{
				var row = MarketModelSettingsHeaderTable.Rows.Add();
				ReportUtilities.AddHighlightCell(row, detail.LogMarketSpecies, AlignmentType.Left, 10);
				ReportUtilities.AddHighlightCell(row, " ", AlignmentType.Left);
				var timbermarket = RPAPortfolio.RPAPortfolioDetails.Where(uu=>uu.LogMarketReportSpeciesID == detail.LogMarketReportSpeciesID).Select(uu => uu.TimberMarket).OrderBy(uu => uu.OrderID);
				var timberMarkets = timbermarket.ToList<TimberMarket>();
				SetTimberMarketRows(detail, timberMarkets);
			}
		}

		private void SetTimberMarketRows(LogMarketReportSpecy specy, List<TimberMarket> timberMarkets)
		{
			foreach(var timberMarket in timberMarkets)
			{
				var row = MarketModelSettingsHeaderTable.Rows.Add();
				var delivered = TimberSortValue.GetDeliveredLogPrices(MarketModelPortfolio, specy, timberMarket);
				decimal price = 0;
				decimal pnr = 0;
				decimal adjustedValue = 0;
				decimal rpa = 0;
				decimal longevity = 0;
				int ona = 0;
				int loggingcosts = 0;
				int haulcosts = 0;

				if (delivered.DeliveredLogPrice.HasValue)
				{
					price = delivered.DeliveredLogPrice.Value;
				}

				if (delivered.ProfitAndRisk.HasValue)
				{
					pnr = Convert.ToDecimal(delivered.ProfitAndRisk.Value) / 100M;
				}
				if (delivered.OverheadAndAdmin.HasValue)
				{
					ona = delivered.OverheadAndAdmin.Value;
				}
				if (delivered.LoggingCosts.HasValue)
				{
					loggingcosts = delivered.LoggingCosts.Value;
				}
				if (delivered.HaulingCosts.HasValue)
				{
					haulcosts = delivered.HaulingCosts.Value;
				}

				var detail = RPAPortfolio.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == specy.LogMarketReportSpeciesID && uu.TimberMarketID == timberMarket.TimberMarketID).FirstOrDefault();
				var ppiNominalDate = MarketModelDataList.Where(uu => uu.Year == detail.BeginningDate.Year && uu.Period == detail.BeginningDate.Month).FirstOrDefault().Value;
				var realValue = RPAREAL.GetRealValueFromTodaysPPI(detail.BeginningRealValue, ppiNominalDate, ppiToday);

				rpa = detail.RPA;
				longevity = detail.Longevity;

				adjustedValue = price * (1 - pnr);

				var inflationRate = MarketModelPortfolio.MarketModelPortfolioInflationDetails.InflationRate;
			
				ReportUtilities.AddLabelCellNonBold(row, timberMarket.Market, AlignmentType.Left);
				ReportUtilities.AddLabelCellNonBold(row, price.ToString("C0"), AlignmentType.Left);
				ReportUtilities.AddLabelCellNonBold(row, rpa.ToString("N4"), AlignmentType.Left);
				ReportUtilities.AddLabelCellNonBold(row, longevity.ToString("N2"), AlignmentType.Left);
				ReportUtilities.AddLabelCellNonBold(row, pnr.ToString("N3"), AlignmentType.Left);
				ReportUtilities.AddLabelCellNonBold(row, ona.ToString("C0"), AlignmentType.Left);
				ReportUtilities.AddLabelCellNonBold(row, loggingcosts.ToString("C0"), AlignmentType.Left);
				ReportUtilities.AddLabelCellNonBold(row, haulcosts.ToString("C0"), AlignmentType.Left);
				if (RPAPortfolio.RPAPortfolioID == -1)
				{
					ReportUtilities.AddLabelCellNonBold(row, string.Empty, AlignmentType.Left);
					ReportUtilities.AddLabelCellNonBold(row, string.Empty, AlignmentType.Left);
				}
				else
				{
					ReportUtilities.AddLabelCellNonBold(row, detail.BeginningDate.ToShortDateString(), AlignmentType.Left);
					ReportUtilities.AddLabelCellNonBold(row, detail.EndingDate.ToShortDateString(), AlignmentType.Left);
				}
			}
		}
	}
}