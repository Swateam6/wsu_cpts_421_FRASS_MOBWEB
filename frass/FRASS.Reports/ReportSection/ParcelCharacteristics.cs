using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelCharacteristics
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table CharacteristicsTable;

		public ParcelCharacteristics(Table table, Parcel parcel)
		{
			ReportUtilities = new ReportUtilities();
			CharacteristicsTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
		}
		public void SetParcelCharacteristicsRows()
		{
			SetSectionHeader();
			SetElevationFeet();
			SetSlope();
		}

		private void SetSectionHeader()
		{
			var row = CharacteristicsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Physical Site Characteristics", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;

			var row2 = CharacteristicsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row2, " ", AlignmentType.Left, 2);
			ReportUtilities.AddLabelCell(row2, "Min", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, "Max", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, "Mean", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, " ", AlignmentType.Left);
			row2.Border = ReportUtilities.RowBottomBorderInfoLight;
		}

		private void SetElevationFeet()
		{
			var row = CharacteristicsTable.Rows.Add();
			var min = Parcel.ParcelStats.ElevationMinFt.ToString("N0");
			var max = Parcel.ParcelStats.ElevationMaxFt.ToString("N0");
			var mean = Parcel.ParcelStats.ElevationMeanFt.ToString("N0");
			ReportUtilities.AddLabelCell(row, "Elevation (feet)", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, min, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, max, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, mean, AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetSlope()
		{
			var row = CharacteristicsTable.Rows.Add();
			var min = Parcel.ParcelStats.SlopeMin.ToString("N0");
			var max = Parcel.ParcelStats.SlopeMax.ToString("N0");
			var mean = Parcel.ParcelStats.SlopeMean.ToString("N0");
			ReportUtilities.AddLabelCell(row, "Slope (degrees)", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, min, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, max, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, mean, AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
	}
}