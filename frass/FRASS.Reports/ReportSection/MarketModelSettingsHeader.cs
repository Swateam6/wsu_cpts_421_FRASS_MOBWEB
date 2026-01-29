using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;


namespace FRASS.Reports.ReportSection
{
	internal class MarketModelSettingsHeader
	{
		private ReportUtilities ReportUtilities;
		private Table MarketModelSettingsHeaderTable;
		private MarketModelPortfolio MarketModelPortfolio;
		private RPAPortfolio RPAPortfolio;
		public MarketModelSettingsHeader(Table table, MarketModelPortfolio marketModelPortfolio, RPAPortfolio rpaPortfolio)
		{
			ReportUtilities = new ReportUtilities(fontSize: 7);
			var cell = table.Rows.Add().Cells.Add();
			MarketModelSettingsHeaderTable = ReportUtilities.GetNewVariableColumnTable(cell,"13% 13% 12% 13% 12% 12% 12% 13%");
			MarketModelPortfolio = marketModelPortfolio;
			RPAPortfolio = rpaPortfolio;
			cell.Paragraphs.Add(MarketModelSettingsHeaderTable);
		}
		public void SetMarketModelSettingsHeaderRow()
		{
			SetHeaderRow();
			SetHeaderDataRow();
		}
		private void SetHeaderRow()
		{
			var row = MarketModelSettingsHeaderTable.Rows.Add();
			ReportUtilities.AddLabelCellGray(row, "Market Model Name", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "RPA Portfolio Name", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Rate of Inflation", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Landowner Discount Rate", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Reforestation Cost", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Access Fee (Timber)", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Maintenance Fee", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "New Logging Road Construction", AlignmentType.Left);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
		private void SetHeaderDataRow()
		{
			var row = MarketModelSettingsHeaderTable.Rows.Add();
			ReportUtilities.AddHighlightCell(row, MarketModelPortfolio.PortfolioName, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, RPAPortfolio.PortfolioName, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, (100M * MarketModelPortfolio.MarketModelPortfolioInflationDetails.InflationRate).ToString("N2") + "%", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, (100M * MarketModelPortfolio.MarketModelPortfolioInflationDetails.LandownerDiscountRate).ToString("N2") + "%", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, MarketModelPortfolio.MarketModelPortfolioCosts.ReforestationCost.ToString("C0") + "/Acre", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, MarketModelPortfolio.MarketModelPortfolioCosts.AccessFeeTimber.ToString("C2") + "/MBF/Mile", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, MarketModelPortfolio.MarketModelPortfolioCosts.MaintenanceFeeTimberHaul.ToString("C2") + "/MBF/Mile", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, MarketModelPortfolio.MarketModelPortfolioCosts.RoadConstructionCosts.ToString("C0") + "/Mile", AlignmentType.Left);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
	}
}