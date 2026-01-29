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
	public class MarketModelSettingsReport
	{
		DeliveredLogMarketModelDataManager db;
		RPAPortfolioDataManager dbRPA;
		User User;
		public MarketModelPortfolio MarketModelPortfolio;
		public RPAPortfolio RPAPortfolio;
		ReportUtilities ReportUtilities;
		private List<MarketModelData> _mmd;
		private List<MarketModelData> MarketModelDataList
		{
			get { return _mmd ?? (_mmd = db.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).ToList<MarketModelData>()); }
		}
		public MarketModelSettingsReport(User user, int portfolioID, int rpaPortfolioID)
		{
			db = DeliveredLogMarketModelDataManager.GetInstance();
			dbRPA = RPAPortfolioDataManager.GetInstance();
			User = user;
			MarketModelPortfolio = db.GetMarketModelPortfolio(portfolioID);
			RPAPortfolio = dbRPA.GetRPAPortfolio(rpaPortfolioID);
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
				sec.OddFooter= footer;
				sec.EvenFooter =footer;
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

			var marketModelSettingsHeader = new MarketModelSettingsHeader(table, MarketModelPortfolio, RPAPortfolio);

			marketModelSettingsHeader.SetMarketModelSettingsHeaderRow();
			section.Paragraphs.Add(table);

			section.Paragraphs.Add(ReportUtilities.GetBlankTable());

			var table2 = ReportUtilities.GetNewTable();
			var marketModelSortSettings = new MarketModelSortSettings(table2, MarketModelPortfolio, RPAPortfolio, MarketModelDataList);
			marketModelSortSettings.SetMarketModelSortSettingsRow();

			section.Paragraphs.Add(table2);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			sections.Add(section);
			return sections;
		}
		public List<Section> GetMarketModelSettingsReportSections()
		{
			return GetReportSections();
		}


	}
}