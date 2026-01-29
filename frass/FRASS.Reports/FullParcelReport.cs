using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using Aspose.Pdf.Generator;
using FRASS.DAL;
using FRASS.Interfaces;
using FRASS.Reports.ReportSection;

namespace FRASS.Reports
{
	public class FullParcelReport
	{
		User User;
		public Parcel Parcel;
		ReportUtilities ReportUtilities;
		ISingleResult<GetCurrentParcelTimberSummaryResult> CurrentParcelTimberSummaryResult;
		string BaseFilePath;
		MarketModelPortfolio MarketModelPortfolio;
		RPAPortfolio RPAPortfolio;
		StumpageModelPortfolio StumpageModelPortfolio;
		List<IReportParcelTimberStatistic> Timberstats;


		public FullParcelReport(User user, Parcel parcel, ISingleResult<GetCurrentParcelTimberSummaryResult> currentParcelTimberSummaryResult, List<IReportParcelTimberStatistic> timberstats, string baseFilePath, MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio)
		{
			User = user;
			Parcel = parcel;
			BaseFilePath = baseFilePath;
			CurrentParcelTimberSummaryResult = currentParcelTimberSummaryResult;
			MarketModelPortfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Timberstats = timberstats;
		}
		public FullParcelReport(User user, Parcel parcel, ISingleResult<GetCurrentParcelTimberSummaryResult> currentParcelTimberSummaryResult, List<IReportParcelTimberStatistic> timberstats, string baseFilePath, StumpageModelPortfolio portfolio)
		{
			User = user;
			Parcel = parcel;
			BaseFilePath = baseFilePath;
			CurrentParcelTimberSummaryResult = currentParcelTimberSummaryResult;
			StumpageModelPortfolio = portfolio;
			Timberstats = timberstats;
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
			var sections = new List<Section>();
			if (MarketModelPortfolio != null)
			{
				sections.Add(GetCoverPage(MarketModelPortfolio,RPAPortfolio, Timberstats));
			}
			else
			{
				sections.Add(GetCoverPage(StumpageModelPortfolio, Timberstats));
			}
			sections.Add(GetTOCPage());
			var pi = new ParcelIndexMaps(Parcel, BaseFilePath);
			sections.AddRange(pi.GetImageSections());
			sections.AddRange(GetReportSections());
			sections.Add(GetFinalPage());
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
			var parcelReport = new ParcelReport(User, Parcel, CurrentParcelTimberSummaryResult, BaseFilePath);
			sections.AddRange(parcelReport.GetReportSectionsNoImages());

			if (MarketModelPortfolio != null)
			{
				var marketModelSettingsReport = new MarketModelSettingsReport(User, MarketModelPortfolio.MarketModelPortfolioID, RPAPortfolio.RPAPortfolioID);
				sections.AddRange(marketModelSettingsReport.GetMarketModelSettingsReportSections());

				foreach (var pa in Parcel.ParcelAllotments)
				{
					if (pa.OwnerType.OwnerType1 == "AiT")
					{
						var allotmentReport = new AllotmentReport(User, Parcel, MarketModelPortfolio, RPAPortfolio, Timberstats);
						sections.AddRange(allotmentReport.GetAllotmentReportSections());
					}
				}
				var marketValueReport = new MarketValueReport(User, Parcel, MarketModelPortfolio, RPAPortfolio, CurrentParcelTimberSummaryResult, Timberstats, BaseFilePath);
				sections.AddRange(marketValueReport.GetMarketValueReportSections());
			}
			else
			{
				var stumpageModelSettingsReport = new StumpageModelSettingsReport(User, StumpageModelPortfolio.StumpageModelPortfolioID, Parcel.ParcelID);
				sections.AddRange(stumpageModelSettingsReport.GetStumpageModelPortfolioReportSections());

				foreach (var pa in Parcel.ParcelAllotments)
				{
					if (pa.OwnerType.OwnerType1 == "AiT")
					{
						var allotmentReport = new AllotmentReport(User, Parcel, StumpageModelPortfolio, Timberstats);
						sections.AddRange(allotmentReport.GetAllotmentReportSections());
					}
				}
				var stumpageValueReport = new StumpageMarketValueReport(User, Parcel, StumpageModelPortfolio, CurrentParcelTimberSummaryResult, Timberstats, BaseFilePath);
				sections.AddRange(stumpageValueReport.GetStumpageMarketValueReportSections());

			}

			var maps = BaseFilePath + "Maps\\";
			var photos = BaseFilePath + "Photos\\";
			var pi = new ParcelImages(Parcel, maps, photos);
			sections.AddRange(pi.GetImageSections());
			return sections;
		}

		private Section GetCoverPage(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			ReportUtilities = new ReportUtilities();
			var section = new Section();
			var table = ReportUtilities.GetNewBlankTable();

			var reportCover = new ReportCover(table, User, Parcel, BaseFilePath);
			reportCover.SetCoverPage(portfolio, rpaPortfolio, timberStats);
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}
		private Section GetCoverPage(StumpageModelPortfolio portfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			ReportUtilities = new ReportUtilities();
			var section = new Section();
			var table = ReportUtilities.GetNewBlankTable();

			var reportCover = new ReportCover(table, User, Parcel, BaseFilePath);
			reportCover.SetCoverPage(portfolio, timberStats);
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}
		private Section GetTOCPage()
		{
			ReportUtilities = new ReportUtilities();
			var section = new Section();
			var table = ReportUtilities.GetNewBlankTable();
			var toc = new ReportTOC(table);
			toc.SetReportTOC();
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}
		private Section GetFinalPage()
		{
			ReportUtilities = new ReportUtilities();
			var section = new Section();
			var table = ReportUtilities.GetNewBlankTable();

			var reportCover = new ReportFinalPage(table);
			reportCover.SetFinalPage();
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}
	}
}
