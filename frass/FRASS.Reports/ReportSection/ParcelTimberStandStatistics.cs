using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelTimberStandStatistics
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table ParentTable;
		private Table TimberStandStatisticsTable;
		private ParcelDataManager dbParcelDataManager;

		public ParcelTimberStandStatistics(Table table, Parcel parcel)
		{
			dbParcelDataManager = ParcelDataManager.GetInstance();
			ReportUtilities = new ReportUtilities();
			ParentTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
		}
		public void SetTimberStandStatisticsRows()
		{
			SetChildTable();
			SetSectionHeader();
			SetDataRows();
		}
		private void SetChildTable()
		{
			var row = ParentTable.Rows.Add();
			var cell = row.Cells.Add();
			cell.ColumnsSpan = 6;
			TimberStandStatisticsTable = ReportUtilities.GetNewVariableColumnTable(cell, 8);
			cell.Paragraphs.Add(TimberStandStatisticsTable);
		}
		private void SetSectionHeader()
		{
			var row = TimberStandStatisticsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Timber Stand Statistics (current)", AlignmentType.Center, 8);
			row.Border = ReportUtilities.RowBottomBorderInfo;

			var row2 = TimberStandStatisticsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row2, "Stand ID Number", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, "Vegetation Label", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, "Site Index", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, "Riparian Zone Non-Operable Acres", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, "Operable Commercial Timber Land Acres", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, "BF/Acre per Stand", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, "Total Forested Acres on Parcel", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, "Total BF on Each Stand", AlignmentType.Center);
			row2.Border = ReportUtilities.RowBottomBorderInfo;
		}

		private void SetDataRows()
		{
			var stands = dbParcelDataManager.GetReportParcelTimberStatistics(Parcel.ParcelID, System.DateTime.Now.Year).OrderBy(uu => uu.Stand_ID);
			decimal TotalRiparianAcres = 0M;
			decimal TotalOperableAcres = 0M;
			decimal TotalAcresOnParcel = 0M;
			decimal TotaBF = 0M;
			foreach (var stand in stands)
			{
				TotalRiparianAcres = TotalRiparianAcres + stand.Riparian_Zone_Acres;
				TotalOperableAcres = TotalOperableAcres + stand.Operable_Land_Acres;
				TotalAcresOnParcel = TotalAcresOnParcel + stand.Total_Acres;
				TotaBF = TotaBF + stand.TotalBF_PerStand;
				SetDataRow(stand);
			}

			var row = TimberStandStatisticsTable.Rows.Add();
			ReportUtilities.AddHighlightCellBold(row, "Totals:", AlignmentType.Left,3);
			ReportUtilities.AddHighlightCellBold(row, TotalRiparianAcres.ToString("N2"), AlignmentType.Right);
			ReportUtilities.AddHighlightCellBold(row, TotalOperableAcres.ToString("N2"), AlignmentType.Right);
			ReportUtilities.AddHighlightCellBold(row, " ", AlignmentType.Right);
			ReportUtilities.AddHighlightCellBold(row, TotalAcresOnParcel.ToString("N2"), AlignmentType.Right);
			ReportUtilities.AddHighlightCellBold(row, TotaBF.ToString("N0"), AlignmentType.Right);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetDataRow(IReportParcelTimberStatistics data)
		{
			var row = TimberStandStatisticsTable.Rows.Add();
			ReportUtilities.AddHighlightCell(row, data.Stand_ID.ToString(), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, data.Veg_Label, AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, data.Site_Index.ToString(), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, data.Riparian_Zone_Acres.ToString("N2"), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, data.Operable_Land_Acres.ToString("N2"), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, data.BFAcre_PerStand.ToString("N0"), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, data.Total_Acres.ToString("N2"), AlignmentType.Right);
			ReportUtilities.AddHighlightCell(row, data.TotalBF_PerStand.ToString("N0"), AlignmentType.Right);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
	}
}