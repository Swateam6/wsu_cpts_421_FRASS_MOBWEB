using System.Collections.Generic;
using Aspose.Pdf.Generator;
using System.Text;

namespace FRASS.Reports.ReportSection
{
	internal class ReportTOC
	{
		private Table TOCPage;
		public ReportTOC(Table table)
		{
			var reportUtilities = new ReportUtilities();
			TOCPage = reportUtilities.AppendNewTableNoBorder(table,1);
		}

		public void SetReportTOC()
		{
			var reportUtilities = new ReportUtilities(10);
			var row = TOCPage.Rows.Add();
			var list = new List<string>();
			list.Add("Forest Resource Analysis System Software: Report Table of Contents");
			list.Add(" ");
			reportUtilities.AddLabelCell(row, list, AlignmentType.Left);

			var sb = new StringBuilder();
			sb.Append("<ul>");
			sb.Append("<li style='font-size: 12px;'>Cover Page with summary</li>");
			sb.Append("<li style='font-size: 12px;'>Area Locator Map (1 page)</li>");
			sb.Append("<li style='font-size: 12px;'>Parcel status report - current conditions (2 to 3 pages)</li>");
			sb.Append("<li style='font-size: 12px;'>Market Portfolio Settings (1 page)</li>");
			sb.Append("<li style='font-size: 12px;'>Parcel Status report - for Timber Production as the Highest and Best Use (1 Page)</li>");
			sb.Append("<li style='font-size: 12px;'>Market Value Report (Parcel value and Bare Land Value)</li>");
			sb.Append("<li style='font-size: 12px;'>Harvest Reports by Timber Stand (1 to many pages)</li>");
			sb.Append("<li style='font-size: 12px;'>Parcel Maps (17 to 20 pages)</li>");
			sb.Append("<li style='font-size: 12px;'>Timber Stand representative photographs based on Vegetation Labels (1 to many pages)</li>");
			sb.Append("<li style='font-size: 12px;'>Forest Resource Analysis System Software: Data Sources (1 page & Last Page of Report)</li>");
			sb.Append("</ul>");

			var textBody = new Text(sb.ToString());
			textBody.TextInfo = reportUtilities.TextInfoRight;
			textBody.TextInfo.FontSize = 8;
			textBody.TextInfo.FontName = "Arial";
			textBody.IsHtmlTagSupported = true;

			var rowTOC = TOCPage.Rows.Add();
			var cell = reportUtilities.AddLabelCell(rowTOC, "", AlignmentType.Left);
			cell.Paragraphs.Add(textBody);
		}
	}
}
