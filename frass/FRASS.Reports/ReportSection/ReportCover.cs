using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.DAL;
using FRASS.Interfaces;

namespace FRASS.Reports.ReportSection
{
	internal class ReportCover
	{
		private Table CoverTable;
		private User User;
		private Parcel Parcel;
		private string ImagesPath;
		public ReportCover(Table table, User user, Parcel parcel, string imagesPath)
		{
			var reportUtilities = new ReportUtilities();
			CoverTable = reportUtilities.AppendNewTableNoBorder(table, 1);
			User = user;
			Parcel = parcel;
			ImagesPath = imagesPath;
		}

		public void SetCoverPage(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			SetHeader();
			SetInfo();
			SetHarvestVolumes(portfolio, rpaPortfolio, timberStats);
			SetCoverPageSection();
			SetBlankRows();
			SetFooter();
		}

		public void SetCoverPage(StumpageModelPortfolio portfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			SetHeader();
			SetInfo();
			SetHarvestVolumes(portfolio, timberStats);
			SetCoverPageSection();
			SetBlankRows();
			SetFooter();
		}

		private void SetHeader()
		{
			var reportUtilities = new ReportUtilities(10);
			var row = CoverTable.Rows.Add();
			var list = new List<string>();
			list.Add("Forest Resource Analysis System Software");
			list.Add(" ");
			reportUtilities.AddLabelCell(row, list, AlignmentType.Center);
			row.Border = new BorderInfo((int)BorderSide.Bottom, 2f, new Color(51, 102, 153));
		}
		private void SetInfo()
		{
			var parcelInfo = new ParcelInfo(CoverTable, User, Parcel, true, false);
			parcelInfo.SetParcelInfoRows();
		}

		private void SetHarvestVolumes(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			var parcelHarvestVolumes = new MarketModelHarvestVolumes(CoverTable, Parcel, portfolio, rpaPortfolio, timberStats);
			parcelHarvestVolumes.SetTotalRows();
			var marketModelSettingsHeader = new MarketModelSettingsHeader(CoverTable, portfolio, rpaPortfolio);
			marketModelSettingsHeader.SetMarketModelSettingsHeaderRow();
		}
		private void SetHarvestVolumes(StumpageModelPortfolio portfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			var parcelHarvestVolumes = new StumpageModelHarvestVolumes(CoverTable, Parcel, portfolio, timberStats);
			parcelHarvestVolumes.SetTotalRows();
			var stumpageModelSettingsHeader = new StumpageModelSettingsHeader(CoverTable, portfolio, Parcel);
			stumpageModelSettingsHeader.SetStumpageModelSettingsHeaderRow();
		}
		private void SetCoverPageSection()
		{
			var reportUtilities = new ReportUtilities(8);
			var row = CoverTable.Rows.Add();
			var str = "Values in this Report represent the Income Capitalization Approach to generating a parcel appraisal value, with Timber Production as the Highest and Best Use.";
			reportUtilities.AddLabelCellNonBold(row, str, AlignmentType.Left);
		}
		private void SetBlankRows()
		{
			var reportUtilities = new ReportUtilities(25);
			for (var ict = 0; ict <= 1; ict++)
			{
				var row = CoverTable.Rows.Add();
				reportUtilities.AddLabelCellNonBold(row, " ", AlignmentType.Left);
			}

		}
		private void SetFooter()
		{
			var reportUtilities = new ReportUtilities(10);
			var row = CoverTable.Rows.Add();
			var list = new List<String>();
			list.Add("FRASS Parcel Report Demonstration");
			reportUtilities.AddLabelCellNonBold(row,list,AlignmentType.Center);

			SetBlankRows();

			var row2 = CoverTable.Rows.Add();
			reportUtilities.AddImageCell(row2, ImagesPath + "/Forest_Econometrics.png", AlignmentType.Center, ImageFileType.Png);
			var text = "http://www.Forest-Econometrics.com";

			var row3 = CoverTable.Rows.Add();
			var cellText = reportUtilities.AddLabelCell(row3, text, AlignmentType.Center, 2);
			cellText.Paragraphs.Clear();
			var t = new Text();
			cellText.Paragraphs.Add(t);
			var seg = t.Segments.Add(text);
			seg.Hyperlink = new Hyperlink();
			seg.Hyperlink.LinkType = HyperlinkType.Web;
			seg.Hyperlink.Url = text;
			seg.TextInfo = reportUtilities.LinkTextInfo;
			
		}
	}
}
