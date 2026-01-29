using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelTravelDistance
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table TravelDistanceTable;

		public ParcelTravelDistance(Table table, Parcel parcel)
		{
			ReportUtilities = new ReportUtilities();
			TravelDistanceTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
		}

		public void SetParcelTravelDistanceRows()
		{
			SetSectionHeader();
			SetStraightLineDistance();
			SetSurfaceRoads();
			SetMainHaulRoads();
		}

		private void SetSectionHeader()
		{
			var row = TravelDistanceTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Distance of Travel to Reach Paved Haul Road", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;

			var row2 = TravelDistanceTable.Rows.Add();
			ReportUtilities.AddLabelCell(row2, "Road Type", AlignmentType.Left, 2);
			ReportUtilities.AddLabelCell(row2, "Meters", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, "Feet", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, "Miles", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row2, "Nearest Road is", AlignmentType.Left);
			row2.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetStraightLineDistance()
		{
			var row = TravelDistanceTable.Rows.Add();
			var meters = "0";
			var feet = "0";
			var miles = "0";
			if (Parcel.ParcelRoadDistances != null)
			{
				if (Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
				{
					meters = Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value.ToString("N0");
					feet = (Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value * 3.280839895013123M).ToString("N0");
					miles = ((Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value / 1000) * 0.621371192237334M).ToString("N1");
				}
			}

			ReportUtilities.AddLabelCell(row, "If Parcel has no road, then straight-line distance to nearest road is (Construction Required):", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, meters, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, feet, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, miles, AlignmentType.Left);
			var road = Parcel.ParcelRoadDistances.NearRoadUseFromParcelBoundary == null ? "" : Parcel.ParcelRoadDistances.NearRoadUseFromParcelBoundary;
			ReportUtilities.AddHighlightCell(row, road, AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetSurfaceRoads()
		{
			var row = TravelDistanceTable.Rows.Add();
			var meters = "0";
			var feet = "0";
			var miles = "0";
			if (Parcel.ParcelRoadDistances != null)
			{
				if (Parcel.ParcelRoadDistances.MainHaulToPaved.HasValue)
				{
					meters = Parcel.ParcelRoadDistances.MainHaulToPaved.Value.ToString("N0");
					feet = (Parcel.ParcelRoadDistances.MainHaulToPaved.Value * 3.280839895013123M).ToString("N0");
					miles = ((Parcel.ParcelRoadDistances.MainHaulToPaved.Value / 1000) * 0.621371192237334M).ToString("N1");
				}
			}
			ReportUtilities.AddLabelCell(row, "Travel distance along Surface Roads:", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, meters, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, feet, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, miles, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, " ", AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetMainHaulRoads()
		{
			var row = TravelDistanceTable.Rows.Add();
			var meters = "0";
			var feet = "0";
			var miles = "0";
			if (Parcel.ParcelRoadDistances != null)
			{
				if (Parcel.ParcelRoadDistances.ToMainHaul.HasValue)
				{
					meters = Parcel.ParcelRoadDistances.ToMainHaul.Value.ToString("N0");
					feet = (Parcel.ParcelRoadDistances.ToMainHaul.Value * 3.280839895013123M).ToString("N0");
					miles = ((Parcel.ParcelRoadDistances.ToMainHaul.Value / 1000) * 0.621371192237334M).ToString("N1");
				}
			}
			ReportUtilities.AddLabelCell(row, "Travel distance along Main Haul Roads", AlignmentType.Left, 2);
			ReportUtilities.AddHighlightCell(row, meters, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, feet, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, miles, AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, " ", AlignmentType.Left);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
	}
}