using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelQINZoning
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table ZoningTable;
		public ParcelQINZoning(Table table, Parcel parcel)
		{
			ReportUtilities = new ReportUtilities();
			ZoningTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
		}
		public void SetParcelZoningRows()
		{
			SetSectionHeader();
			SetZoning();
		}

		public void SetParcelZoningValueRows(string baseFilePath)
		{
			SetSectionHeader();
			SetZoningValueRows(baseFilePath);
		}

		private void SetSectionHeader()
		{
			var row = ZoningTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Zoning", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}

		private void SetZoning()
		{
			foreach(var zone in Parcel.ParcelZonings)
			{
				var row = ZoningTable.Rows.Add();
				ReportUtilities.AddLabelCell(row, zone.ParcelZoningType.Zoning, AlignmentType.Right, 3);
				ReportUtilities.AddHighlightCell(row, zone.Acres.ToString("N1"), AlignmentType.Left, 3);
				row.Border = ReportUtilities.RowBottomBorderInfo;
			}
		}
		private void SetZoningValueRows(string baseFilePath)
		{
			var filePath = "";
			var txt = "";
			foreach (var zone in Parcel.ParcelZonings)
			{
				if (zone.ParcelZoningType.Zoning == "Wilderness" || zone.ParcelZoningType.Zoning == "Commercial" || zone.ParcelZoningType.Zoning == "Residential")
				{
					filePath = baseFilePath + "/images/32_stop.png";
					txt = "This parcel is located in a Zoning category that is not consistent with commercial timber production.";

				}
				else if (zone.ParcelZoningType.Zoning == "Industrial")
				{
					filePath = baseFilePath + "/images/32_warn.png";
					txt = "This parcel's use for commercial timber production may require a special use permit.";
				}
				else
				{
					filePath = baseFilePath + "/images/32_ok.png";
					txt = "This parcel's use for commercial timber production is consistent with the Forestry Zoning Category.";
				}


				var row = ZoningTable.Rows.Add();
				ReportUtilities.AddLabelCell(row, zone.ParcelZoningType.Zoning, AlignmentType.Right);
				ReportUtilities.AddHighlightCell(row, zone.Acres.ToString("N1"), AlignmentType.Left);
				ReportUtilities.AddImageCell(row, filePath, AlignmentType.Right, ImageFileType.Png);
				ReportUtilities.AddHighlightCell(row, txt, AlignmentType.Left, 3);
				row.Border = ReportUtilities.RowBottomBorderInfo;
			}
		}
	}
}