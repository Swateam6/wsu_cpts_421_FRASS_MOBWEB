using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;


namespace FRASS.Reports.ReportSection
{
	internal class StumpageModelSettingsHeader
	{
		private ReportUtilities ReportUtilities;
		private Table StumpageModelSettingsHeaderTable;
		private StumpageModelPortfolio StumpageModelPortfolio;
		private Parcel Parcel;
		public StumpageModelSettingsHeader(Table table, StumpageModelPortfolio stumpageModelPortfolio, Parcel parcel)
		{
			ReportUtilities = new ReportUtilities(7);
			Parcel = parcel;

			var cell = table.Rows.Add().Cells.Add();
			StumpageModelSettingsHeaderTable = ReportUtilities.GetNewVariableColumnTable(cell, "13% 12% 12% 12% 13% 12% 13% 13%");
			StumpageModelPortfolio = stumpageModelPortfolio;
			cell.Paragraphs.Add(StumpageModelSettingsHeaderTable);
		}
		public void SetStumpageModelSettingsHeaderRow()
		{
			SetHeaderRow();
			SetHeaderDataRow();
		}
		private void SetHeaderRow()
		{
			var row = StumpageModelSettingsHeaderTable.Rows.Add();
			ReportUtilities.AddLabelCellGray(row, "Market Model Name", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Rate of Inflation", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Landowner Discount Rate", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Reforestation Cost", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Access Fee (Timber)", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Maintenance Fee", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "New Logging Road Construction", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Parcel Number", AlignmentType.Left);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
		private void SetHeaderDataRow()
		{
			var row = StumpageModelSettingsHeaderTable.Rows.Add();
			ReportUtilities.AddHighlightCell(row, StumpageModelPortfolio.PortfolioName, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, (100M * StumpageModelPortfolio.StumpageModelPortfolioInflationDetails.InflationRate).ToString("N2") + "%", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, (100M * StumpageModelPortfolio.StumpageModelPortfolioInflationDetails.LandownerDiscountRate).ToString("N2") + "%", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, StumpageModelPortfolio.StumpageModelPortfolioCosts.ReforestationCost.ToString("C0") + "/Acre", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, StumpageModelPortfolio.StumpageModelPortfolioCosts.AccessFeeTimber.ToString("C2") + "/MBF/Mile", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, StumpageModelPortfolio.StumpageModelPortfolioCosts.MaintenanceFeeTimberHaul.ToString("C2") + "/MBF/Mile", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, StumpageModelPortfolio.StumpageModelPortfolioCosts.RoadConstructionCosts.ToString("C0") + "/Mile", AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, Parcel.ParcelNumber, AlignmentType.Left);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
	}
}
