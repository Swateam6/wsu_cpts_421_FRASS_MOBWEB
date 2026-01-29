using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelExistingRoads
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table CharacteristicsTable;

		public ParcelExistingRoads(Table table, Parcel parcel)
		{
			ReportUtilities = new ReportUtilities();
			CharacteristicsTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
		}
		public void SetParcelCharacteristicsRows()
		{
			SetSectionHeader();
			SetSurfaceRoads();
			SetMainHaulRoads();
			SetPavedRoads();
		}

		private void SetSectionHeader()
		{
			var row = CharacteristicsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Existing Roads on Parcel", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;

			var row2 = CharacteristicsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row2, "Road Type", AlignmentType.Left, 2);
			ReportUtilities.AddLabelCell(row2, "Feet", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, "Miles", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, " ", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, " ", AlignmentType.Left);
			row2.Border = ReportUtilities.RowBottomBorderInfo;
		}

		private void SetSurfaceRoads()
		{
			var row = CharacteristicsTable.Rows.Add();
			var miles = "0";
			var feet = "0";
			if (Parcel.ParcelRoadUseLengths != null)
			{
				if (Parcel.ParcelRoadUseLengths.SurfaceLength.HasValue)
				{
					feet = (Parcel.ParcelRoadUseLengths.SurfaceLength.Value * 3.280839895013123M).ToString("N0");
					miles = ((Parcel.ParcelRoadUseLengths.SurfaceLength.Value / 1000) * 0.621371192237334M).ToString("N1");

				}
			}

			ReportUtilities.AddLabelCell(row, "Surface Roads:", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, feet, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, miles, AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetMainHaulRoads()
		{
			var row = CharacteristicsTable.Rows.Add();
			var miles = "0";
			var feet = "0";
			if (Parcel.ParcelRoadUseLengths != null)
			{
				if (Parcel.ParcelRoadUseLengths.MainHaulLength.HasValue)
				{
					feet = (Parcel.ParcelRoadUseLengths.MainHaulLength.Value * 3.280839895013123M).ToString("N0");
					miles = ((Parcel.ParcelRoadUseLengths.MainHaulLength.Value / 1000) * 0.621371192237334M).ToString("N1");
				}
			}
			ReportUtilities.AddLabelCell(row, "Main Haul Roads:", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, feet, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, miles, AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetPavedRoads()
		{
			var row = CharacteristicsTable.Rows.Add();
			var miles = "0";
			var feet = "0";
			if (Parcel.ParcelRoadUseLengths != null)
			{
				if (Parcel.ParcelRoadUseLengths.PavedLength.HasValue)
				{
					feet = (Parcel.ParcelRoadUseLengths.PavedLength.Value * 3.280839895013123M).ToString("N0");
					miles = ((Parcel.ParcelRoadUseLengths.PavedLength.Value / 1000) * 0.621371192237334M).ToString("N1");
				}
			}
			ReportUtilities.AddLabelCell(row, "Paved Haul Roads:", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, feet, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, miles, AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
	}
}