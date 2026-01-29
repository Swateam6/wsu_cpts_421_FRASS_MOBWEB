using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelCurrentTimberSummary
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table ParcelCurrentTimberSummaryTable;
		private decimal TotalBFCurrentParcel = 0;
		private ISingleResult<GetCurrentParcelTimberSummaryResult> CurrentParcelTimberSummaryResult;

		public ParcelCurrentTimberSummary(Table table, Parcel parcel, ISingleResult<GetCurrentParcelTimberSummaryResult> currentParcelTimberSummaryResult)
		{
			ReportUtilities = new ReportUtilities();
			ParcelCurrentTimberSummaryTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
			CurrentParcelTimberSummaryResult = currentParcelTimberSummaryResult;
		}
		public void SetParcelCurrentTimberSummaryRows()
		{
			SetSectionHeader();
			SetTimberTotals();
		}

		private void SetSectionHeader()
		{
			var row = ParcelCurrentTimberSummaryTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Current Parcel Timber Summary (Operable Acres Only)", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;

			var row2 = ParcelCurrentTimberSummaryTable.Rows.Add();
			ReportUtilities.AddLabelCell(row2, " ", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCell(row2, "Total BF", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row2, " ", AlignmentType.Center, 3);
			row2.Border = ReportUtilities.RowBottomBorderInfo;
		}

		private void SetTimberTotals()
		{
			var GetCurrentParcelTimberSummaryResults = CurrentParcelTimberSummaryResult.OrderBy(uu => uu.CommonName);
			foreach (var data in GetCurrentParcelTimberSummaryResults)
			{
				SetTimberTotal(data);
			}
			var row = ParcelCurrentTimberSummaryTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Totals", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCellBold(row, TotalBFCurrentParcel.ToString("N0"), AlignmentType.Right);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left, 3);
		}
		private void SetTimberTotal(GetCurrentParcelTimberSummaryResult data)
		{
			var row = ParcelCurrentTimberSummaryTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, data.CommonName, AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row,  data.TotalBF.Value.ToString("N0"), AlignmentType.Right);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left, 3);
			row.Border = ReportUtilities.RowBottomBorderInfo;
			TotalBFCurrentParcel = TotalBFCurrentParcel + data.TotalBF.Value;
		}
	}
}