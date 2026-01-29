using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelReportHaulZone
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table HaulZoneTable;

		public ParcelReportHaulZone(Table table, Parcel parcel)
			{
				ReportUtilities = new ReportUtilities();
				HaulZoneTable = ReportUtilities.AppendNewSixColumnTable(table);
				Parcel = parcel;
			}

		public void SetParcelReportHaulZoneRows()
		{
			SetHaulZone();
		}
		private void SetSectionHeader()
		{
			var row = HaulZoneTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}

		private void SetHaulZone()
		{
			var row = HaulZoneTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Washington Department of Revenue Haul Zone", AlignmentType.Left, 3);
			ReportUtilities.AddHighlightCell(row, Parcel.ParcelHaulZones.FirstOrDefault().HaulZoneID.ToString(), AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Left, 2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
	}
}