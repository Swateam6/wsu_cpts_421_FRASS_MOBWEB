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

namespace FRASS.Reports
{
	public class ParcelReport
	{
		User User;
		public Parcel Parcel;
		ReportUtilities ReportUtilities;
		ISingleResult<GetCurrentParcelTimberSummaryResult> CurrentParcelTimberSummaryResult;
		string BaseFilePath;
		private string Title = "Forest Resource Analysis System Software Reporting System";
		public ParcelReport(User user, Parcel parcel, ISingleResult<GetCurrentParcelTimberSummaryResult> currentParcelTimberSummaryResult, string baseFilePath)
		{
			User = user;
			Parcel = parcel;
			BaseFilePath = baseFilePath;
			CurrentParcelTimberSummaryResult = currentParcelTimberSummaryResult;
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
			ReportUtilities = new ReportUtilities();
			var sections = new List<Section>();
			sections.Add(GetPage1());
			sections.Add(GetPage2());
			var maps = BaseFilePath + "Maps\\";
			var photos = BaseFilePath + "Photos\\";
			var pi = new ParcelImages(Parcel, maps, photos);
			sections.AddRange(pi.GetImageSections());
			return sections;
		}

		public List<Section> GetReportSectionsNoImages()
		{
			ReportUtilities = new ReportUtilities();
			var sections = new List<Section>();
			sections.Add(GetPage1());
			sections.Add(GetPage2());
			return sections;
		}

		private Section GetPage1()
		{
			
			var section = new Section();
			var table = ReportUtilities.GetNewTable();
			

			var reportTitle = new ReportTitle(table, Title);
			var parcelInfo = new ParcelInfo(table, User, Parcel, true, false);
			var parcelZoning = new ParcelQINZoning(table, Parcel);
			var parcelSoils = new ParcelSoils(table, Parcel);
			var parcelTES = new ParcelTES(table, Parcel);

			reportTitle.SetHeaderRow();
			parcelInfo.SetParcelInfoRows();
			//parcelZoning.SetParcelZoningRows();
			parcelSoils.SetParcelSoilsRows();
			parcelTES.SetParcelTESRows();
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}
		private Section GetPage2()
		{
			var section = new Section();
			var table = ReportUtilities.GetNewTable();
			var parcelChar = new ParcelCharacteristics(table, Parcel);
			var parcelExistingRoads = new ParcelExistingRoads(table, Parcel);
			var parcelTravelDistance = new ParcelTravelDistance(table, Parcel);
			//var parcelHaul = new ParcelReportHaulZone(table, Parcel);
			var parcelTimberStandStatistics = new ParcelTimberStandStatistics(table, Parcel);
			var parcelCurrentTimberSummary = new ParcelCurrentTimberSummary(table, Parcel, CurrentParcelTimberSummaryResult);

			parcelChar.SetParcelCharacteristicsRows();
			parcelExistingRoads.SetParcelCharacteristicsRows();
			parcelTravelDistance.SetParcelTravelDistanceRows();
			//parcelHaul.SetParcelReportHaulZoneRows();
			parcelTimberStandStatistics.SetTimberStandStatisticsRows();
			parcelCurrentTimberSummary.SetParcelCurrentTimberSummaryRows();
			section.Paragraphs.Add(table);
			section.PageInfo.Margin = ReportUtilities.SectionMargin;
			return section;
		}

	}
}