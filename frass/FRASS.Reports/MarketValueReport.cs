using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Pdf.Generator;
using System.IO;
using System.Text;
using FRASS.Reports.ReportSection;
using FRASS.Reports;
using FRASS.DAL;
using System.Data.Linq;
using FRASS.BLL.Models;
using FRASS.Interfaces;

namespace FRASS.Reports
{
	public class MarketValueReport
	{
		User User;
		public Parcel Parcel;
		ReportUtilities ReportUtilities;
		ISingleResult<GetCurrentParcelTimberSummaryResult> CurrentParcelTimberSummaryResult;
		string BaseFilePath;
		public MarketModelPortfolio Portfolio;
		public RPAPortfolio RPAPortfolio;
		private List<IReportParcelTimberStatistic> TimberStats;


		public MarketValueReport(User user, Parcel parcel, MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, ISingleResult<GetCurrentParcelTimberSummaryResult> currentParcelTimberSummaryResult, List<IReportParcelTimberStatistic> timberStats, string baseFilePath)
		{
			User = user;
			Parcel = parcel;
			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			CurrentParcelTimberSummaryResult = currentParcelTimberSummaryResult;
			BaseFilePath = baseFilePath;
			TimberStats = timberStats;
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
			ReportUtilities = new ReportUtilities();
			var sections = new List<Section>();
			sections.Add(GetPage1());
			sections.Add(GetPage2());
			sections.AddRange(GetHarvestReports());
			return sections;
		}
		public List<Section> GetMarketValueReportSections()
		{
			return GetReportSections();
		}
		private Section GetPage1()
		{

			var section = new Section();
			var table = ReportUtilities.GetNewTable();
			var title = new List<string>();
			title.Add("Forest Resource Analysis System Software Reporting System:");
			title.Add("Delivered Log Market Model - " + Portfolio.PortfolioName);
			var reportTitle = new ReportTitle(table, title);
			var valueReportHeader = new ParcelInfo(table, User, Parcel);
			var valueQINZoning = new ParcelQINZoning(table, Parcel);
			var tes = new ParcelTES(table, Parcel);
			var parcelExistingRoads = new ParcelExistingRoads(table, Parcel);
			var parcelTravelDistance = new ParcelTravelDistance(table, Parcel);
			
			
			reportTitle.SetHeaderRow();
			valueReportHeader.SetParcelInfoRowsForValueReport(BaseFilePath);
			//valueQINZoning.SetParcelZoningValueRows(BaseFilePath);
			tes.SetParcelTESValueRows(BaseFilePath);
			parcelExistingRoads.SetParcelCharacteristicsRows();
			parcelTravelDistance.SetParcelTravelDistanceRows();
			
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}

		private Section GetPage2()
		{
			var section = new Section();
			var table = ReportUtilities.GetNewTable();
			var parcelHarvestVolumes = new MarketModelHarvestVolumes(table, Parcel, Portfolio, RPAPortfolio, TimberStats);

			parcelHarvestVolumes.SetHarvestVolumesRows();

			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}

		private List<Section> GetHarvestReports()
		{
			var rPAPortfolioDetails = RPAPortfolio.RPAPortfolioDetails.ToList<RPAPortfolioDetail>();
			var marketModelPortfolioDeliveredLogModelDetails = Portfolio.MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
			var HarvestReport = new MarketModelStandHarvestReport(Parcel, Portfolio, RPAPortfolio, rPAPortfolioDetails, marketModelPortfolioDeliveredLogModelDetails);
			return HarvestReport.GetHarvestReportPages();
		}
	}
}
