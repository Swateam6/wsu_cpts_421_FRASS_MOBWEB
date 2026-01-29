using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;
using FRASS.BLL.Models;
using FRASS.Interfaces;

namespace FRASS.Reports.ReportSection
{
	internal abstract class StandHarvestReportBase
	{
		protected ReportUtilities ReportUtilities;
		protected Parcel Parcel;
		protected EconVariables EconVariables;

		protected StandHarvestReportBase(Parcel parcel)
		{
			ReportUtilities = new ReportUtilities();
			Parcel = parcel;
		}
		public List<Section> GetHarvestReportPages()
		{
			var pages = new List<Section>();
			var standids = Parcel.ParcelRiparians.Where(uu => uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.STD_ID).Distinct();
			foreach (var standid in standids)
			{
				var section = new Section();
				var table = ReportUtilities.GetNewTable(9);
				SetStandReport(table, standid);
				section.Paragraphs.Add(table);
				section.PageInfo.Margin = ReportUtilities.SectionMargin;
				pages.Add(section);
			}
			return pages;
		}
		protected void SetStandReport(Table table, int standid)
		{
			SetHeader(table);
			SetStandLabel(table, standid);
			SetMainTableHeader(table);
			SetYearHeaders(table, standid);
		}
		protected void SetHeader(Table table)
		{
			var row = table.Rows.Add();
			var list = new List<string>();
			list.Add("Harvest Reports");
			list.Add("Based on Operable Commercial Timber Land Acres");
			list.Add("Values Represent Future Values");
			ReportUtilities.AddLabelCell(row, list, AlignmentType.Center, 9);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected void SetStandLabel(Table table, int standid)
		{
			var row = table.Rows.Add();
			ReportUtilities.AddLabelCellGray(row, "Stand ID Number: " + standid.ToString(), AlignmentType.Left, 9);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected void SetMainTableHeader(Table table)
		{
			var row = table.Rows.Add();
			ReportUtilities.AddLabelCellGray(row, " ", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellGray(row, "Current Rotation", AlignmentType.Center, 3);
			ReportUtilities.AddLabelCellGray(row, "Next Rotation", AlignmentType.Center, 2);
			var list = new List<string>();
			list.Add("Third Rotation Into");
			list.Add("Perpetuity");
			ReportUtilities.AddLabelCellGray(row, list, AlignmentType.Center, 2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected abstract void SetYearHeaders(Table table, int standid);
	}
}
