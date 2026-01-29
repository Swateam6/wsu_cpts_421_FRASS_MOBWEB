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
using FRASS.DAL.DataManager;

namespace FRASS.Reports
{
	public class AllotmentReport
	{
		User User;
		public Parcel Parcel;
		ReportUtilities ReportUtilities;
		MarketModelPortfolio MarketModelPortfolio;
		RPAPortfolio RPAPortfolio;
		StumpageModelPortfolio StumpageModelPortfolio;
		List<IReportParcelTimberStatistic> TimberStats;
		List<ParcelAllotmentShare> Shares;
		Decimal TotalValue;

		public AllotmentReport(User user, Parcel parcel, MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			User = user;
			Parcel = parcel;
			MarketModelPortfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			TimberStats = timberStats;
			Shares = ParcelDataManager.GetInstance().GetParcelAllotmentSharesByParcelID(Parcel.ParcelID);
		}
		public AllotmentReport(User user, Parcel parcel, StumpageModelPortfolio portfolio, List<IReportParcelTimberStatistic> timberStats)
		{
			User = user;
			Parcel = parcel;
			StumpageModelPortfolio = portfolio;
			TimberStats = timberStats;
			Shares = ParcelDataManager.GetInstance().GetParcelAllotmentSharesByParcelID(Parcel.ParcelID);
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
			if (MarketModelPortfolio != null)
			{
				sections.Add(GetDMMPage1());
			}
			else 
			{
				sections.Add(GetSMMPage1());
			}
			sections.AddRange(GetAllotteePages());
			return sections;
		}
		public List<Section> GetAllotmentReportSections()
		{
			return GetReportSections();
		}

		private Section GetDMMPage1()
		{
			var section = new Section();
			var table = ReportUtilities.GetNewTable();
			var title = new List<string>();
			title.Add("Allotment Value Allocation");
			title.Add("Based on BIA Data Circa 1999");
			title.Add("Delivered Log Market Model - " + MarketModelPortfolio.PortfolioName);
			var reportTitle = new ReportTitle(table, title);
			var parcelInfo = new ParcelInfo(table, User, Parcel);
			var parcelHarvestVolumes = new MarketModelHarvestVolumes(table, Parcel, MarketModelPortfolio, RPAPortfolio,  TimberStats);
			reportTitle.SetHeaderRow();
			parcelInfo.SetParcelInfoRows();
			TotalValue = parcelHarvestVolumes.SetHarvestVolumesRows();
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}
		private Section GetSMMPage1()
		{
			var section = new Section();
			var table = ReportUtilities.GetNewTable();
			var title = new List<string>();
			title.Add("Allotment Value Allocation");
			title.Add("Based on BIA Data Circa 1999");
			title.Add("Stumpage Market Model - " + StumpageModelPortfolio.PortfolioName);
			var reportTitle = new ReportTitle(table, title);
			var parcelInfo = new ParcelInfo(table, User, Parcel);
			var parcelHarvestVolumes = new StumpageModelHarvestVolumes(table, Parcel, StumpageModelPortfolio, TimberStats);
			reportTitle.SetHeaderRow();
			parcelInfo.SetParcelInfoRows();
			TotalValue = parcelHarvestVolumes.SetHarvestVolumesRows();
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}

		private List<Section> GetAllotteePages()
		{
			var allottees = new Allotees(Parcel, Shares, TotalValue);
			return allottees.GetAllotteePages();
		}
	}
}
