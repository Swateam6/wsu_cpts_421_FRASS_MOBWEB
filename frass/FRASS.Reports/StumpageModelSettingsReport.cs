using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aspose.Pdf.Generator;
using System.IO;
using System.Text;
using FRASS.Reports.ReportSection;
using FRASS.Reports;
using FRASS.DAL;
using FRASS.DAL.DataManager;

namespace FRASS.Reports
{
	public class StumpageModelSettingsReport
	{
		StumpageMarketModelDataManager db;
		User User;
		public StumpageModelPortfolio StumpageModelPortfolio;
		ReportUtilities ReportUtilities;
		Parcel Parcel;
		public StumpageModelSettingsReport(User user, int portfolioID, int parcelID)
		{
			db = StumpageMarketModelDataManager.GetInstance();
			var dbP = ParcelDataManager.GetInstance();
			User = user;
			StumpageModelPortfolio = db.GetStumpageModelPortfolio(portfolioID);
			Parcel = dbP.GetParcel(parcelID);
			ReportUtilities = new ReportUtilities(7);
		}

		public MemoryStream GetReport()
		{
			var stream = new MemoryStream();
			Pdf pdf = new Pdf();
			pdf.Author = User.FirstName + " " + User.LastName;
			pdf.Creator = User.FirstName + " " + User.LastName;

			Aspose.Pdf.License license = new Aspose.Pdf.License();
			license.SetLicense("FRASS.Reports.Aspose.Pdf.lic");
			license.Embedded = true;
			var sections = GetReportSections();
			foreach (var sec in sections)
			{
				pdf.Sections.Add(sec);
				sec.PageInfo.PageWidth = PageSize.LetterWidth;
				sec.PageInfo.PageHeight = PageSize.LetterHeight;
				var footer = new HeaderFooter(sec);
				sec.OddFooter = footer;
				sec.EvenFooter = footer;
				Text pager = new Text(footer, "$p");
				pager.TextInfo = ReportUtilities.TextInfoRight;
				footer.Paragraphs.Add(pager);
			}

			pdf.Save(stream);
			return stream;
		}

		private List<Section> GetReportSections()
		{
			var sections = new List<Section>();
			var section = new Section();
			var table = ReportUtilities.GetNewTable();

			var stumpageModelSettingsHeader = new StumpageModelSettingsHeader(table, StumpageModelPortfolio, Parcel);

			stumpageModelSettingsHeader.SetStumpageModelSettingsHeaderRow();
			section.Paragraphs.Add(table);

			section.Paragraphs.Add(ReportUtilities.GetBlankTable());

			var table2 = ReportUtilities.GetNewTable();
			var stumpageModelQualityCodeSettings = new StumpageModelQualityCodeSettings(table2, StumpageModelPortfolio);

			stumpageModelQualityCodeSettings.SetStumpageModelQualityCodeSettingsRow(db.GetStumpageGroups());

			section.Paragraphs.Add(table2);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			sections.Add(section);
			return sections;
		}
		public List<Section> GetStumpageModelPortfolioReportSections()
		{
			return GetReportSections();
		}
	}
}
