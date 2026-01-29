using System.Collections.Generic;
using Aspose.Pdf.Generator;

namespace FRASS.Reports.ReportSection
{
	internal class ReportFinalPage
	{
		private Table FinalPage;

		public ReportFinalPage(Table table)
		{
			var reportUtilities = new ReportUtilities(12);
			var cell = table.Rows.Add().Cells.Add();
			cell.ColumnsSpan = 1;
			FinalPage = reportUtilities.GetNewVariableColumnTable(cell, "25% 75%");
			cell.Paragraphs.Add(FinalPage);
		}

		public void SetFinalPage()
		{
			SetHeader();
			SetTextParagraph();
			SetUpTable();
		}
		private void SetHeader()
		{
			var reportUtilities = new ReportUtilities(10);
			var row = FinalPage.Rows.Add();
			var list = new List<string>();
			list.Add("Forest Resource Analysis System Software: Data Sources");
			list.Add(" ");
			reportUtilities.AddLabelCell(row, list, AlignmentType.Center, 2);
		}

		private void SetTextParagraph()
		{
			var reportUtilities = new ReportUtilities(10);
			var row = FinalPage.Rows.Add();
			var list = new List<string>();
			list.Add("Input data for this reporting system has brought together information from several sources, all arranged to evaluate the parcel's optimal financial value. Parcels include timber stands, are accessed by road networks, and are crossed by rivers. All of these attributes are compiled into a holistic representation of the parcel's characteristics. Values are derived from these characteristics using standard economic analysis practices.");
			reportUtilities.AddLabelCellNonBold(row, list, AlignmentType.Left, 2);
			row.Border = new BorderInfo((int)BorderSide.Bottom, .5f, reportUtilities.Color_Gray);
		}

		private void SetUpTable()
		{
			var reportUtilities = new ReportUtilities(8);
			var row = FinalPage.Rows.Add();
			reportUtilities.AddLabelCell(row, "Data", AlignmentType.Left);
			reportUtilities.AddLabelCell(row, "Source", AlignmentType.Left);
			row.Border = new BorderInfo((int)BorderSide.Bottom, .5f, reportUtilities.Color_Gray);

			AddRows(reportUtilities, "Parcel Data and Identification", "These used the PLSS framework for Townships, Ranges, and Sections, but all of the parcel lines were created by Geospatial Resource Analysis, for demonstration purposes. They are NOT real parcels for this property.");
			AddRows(reportUtilities, "Parcel Size and Location", "Determined in GIS with property lines projected in NAD83UTM10N");
			AddRows(reportUtilities, "Soil Survey Data", "USDA Natural Resource Conservation Service (NRCS), analyzed through the NRCS Soil Data Viewer in GIS.");
			AddRows(reportUtilities, "Threatened, Endangered, and Sensitive Species Habitat", "Washington GAP Analysis completed by USGS, 1997. Cassidy, K. M., C. E. Grue, M. R. Smith, and K. M. Dvornich, eds. 1997. Washington State Gap Analysis - Final Report. Washington Cooperative Fish and Wildlife Research Unit, University of Washington, Seattle, Volumes 1-5.");
			AddRows(reportUtilities, "Digital Elevation Data", "U.S. Geological Survey (USGS), and distributed by the EROS Data Center Road Network Washington State Department of Natural Resources (WaDNR). Geospatial Resource Analysis completed a Road Network solution to the Pack Forest area.");
			AddRows(reportUtilities, "Road Network", "Washington State Department of Natural Resources (WaDNR). Geospatial Resource Analysis (a Division of Forest Econometrics) completed a Road Network solution to the Pack Forest area.");
			AddRows(reportUtilities, "Forest Mensuration Data", "Original site characteristics (stand inventories) were identified in sample data provided in the Landscape Management System (LMS) ver. 3.1. All stands simulated a clearcut then were regenerated to the age consistent with current status using the Organon growth model in LMS. Each timber stand was projected in 5 year intervals for 200 years in each projection (current stand). Regeneration on each stand was accomplished with a uniform Site Index reforestation profile for each identified site index rating (by Pack Forest). Log merchandising for each stand was completed in R! using tree bole taper by species and crown height estimated in LMS software.");
			AddRows(reportUtilities, "Parcel Maps", "Data from listed sources, mapped by Geospatial Resource Analysis.");
			AddRows(reportUtilities, "Timber Stand Photographs", "William E. Schlosser, Geospatial Resource Analysis.");
			AddRows(reportUtilities, "Global Economic Parameters (inflation and discount rates)", "Bureau of Labor Statistics (BLS 1900-current, updated monthly)");
			AddRows(reportUtilities, "Reforestation Costs", "Estimated default values");
			AddRows(reportUtilities, "Road Use and Construction Costs", "Estimated default values");	
			AddRows(reportUtilities, "Delivered Log Market Real Price Appreciation", "Historic Delivered Log Market Data combined with Bureau of Labor Statistics data, calculated by Users");
			AddRows(reportUtilities, "Logging Cost Real Price Appreciation", "Bureau of Labor Statistics 'Receipts for contract logging of timber owned by others'");
			AddRows(reportUtilities, "Delivered Log Market Data", "Washington DNR and RISI (Log Lines market reporting), delivered log market data is updated monthly to the FRASS database.");
			AddRows(reportUtilities, "Logging Costs", "User estimated and entered");
			AddRows(reportUtilities, "Hauling Costs", "User estimated and entered");
			AddRows(reportUtilities, "Overhead & Administration", "User estimated and entered");
			AddRows(reportUtilities, "Profit and Risk", "User estimated and entered");
			AddRows(reportUtilities, "Inflation Rate", "User identified rate of inflation from BLS records, used to project future values from current ");
			AddRows(reportUtilities, "Discount Rate", "User identified discount rate from BLS records, used to discount future values to current values, used in combination with Inflation rate.");
			AddRows(reportUtilities, "Real Price Appreciation and Longevity", "RPA – User identified rate applied to specific log sorts for rates of increase greater than, or less than, the rate of inflation for a period of time (Longevity) when the Appreciation rate will peak, or when the Devaluation rate will trough. The effects of the RPA will continue for a period at least twice as long as Longevity to exhaust the RPA influence.");

			var rowSpacer = FinalPage.Rows.Add();
			reportUtilities.AddLabelCellNonBold(rowSpacer, " ", AlignmentType.Left);
			reportUtilities.AddLabelCellNonBold(rowSpacer, " ", AlignmentType.Left);

			var rowFooter = FinalPage.Rows.Add();
			var list = new List<string>();
			list.Add("Last Page of Report");
			list.Add("Remainder Intentionally Left Blank");
			reportUtilities.AddLabelCell(rowFooter, list, AlignmentType.Center, 2);
		}

		private void AddRows(ReportUtilities reportUtilities, string text1, string text2)
		{
			var row = FinalPage.Rows.Add();
			reportUtilities.AddLabelCellNonBold(row, text1, AlignmentType.Left);
			reportUtilities.AddLabelCellNonBold(row, text2, AlignmentType.Left);
			row.Border = new BorderInfo((int)BorderSide.Bottom, .5f, reportUtilities.Color_Gray);
		}
	}
}
