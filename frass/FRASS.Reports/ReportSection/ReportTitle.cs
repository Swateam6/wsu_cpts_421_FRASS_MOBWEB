using Aspose.Pdf.Generator;
using System.Collections.Generic;

namespace FRASS.Reports.ReportSection
{
	internal class ReportTitle
	{
		private ReportUtilities ReportUtilities;
		private Table TitleTable;
		private string Title;
		private List<string> TitleList;
		public ReportTitle(Table table, string title)
		{
			ReportUtilities = new ReportUtilities();
			TitleTable = ReportUtilities.AppendNewOneColumnTable(table);
			Title = title;
		}
		public ReportTitle(Table table, List<string> title)
		{
			ReportUtilities = new ReportUtilities();
			TitleTable = ReportUtilities.AppendNewOneColumnTable(table);
			TitleList = title;
		}
		public void SetHeaderRow()
		{
			if (TitleList == null)
			{
				var row = TitleTable.Rows.Add();
				var cell = ReportUtilities.AddLabelCell(row, Title, AlignmentType.Left);
				cell.BackgroundColor = ReportUtilities.Color_Gray;
				row.BackgroundColor = ReportUtilities.Color_Gray;
			}
			else
			{
				var row = TitleTable.Rows.Add();
				var cell = ReportUtilities.AddLabelCell(row, TitleList, AlignmentType.Left);
				cell.BackgroundColor = ReportUtilities.Color_Gray;
				row.BackgroundColor = ReportUtilities.Color_Gray;
			}
		}
	}
}