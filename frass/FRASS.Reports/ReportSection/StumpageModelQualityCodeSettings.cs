using System.Linq;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using System.Collections.Generic;
using System;
using FRASS.DAL;
namespace FRASS.Reports.ReportSection
{
	internal class StumpageModelQualityCodeSettings
	{
		private ReportUtilities ReportUtilities;
		private Table StumpageModelQualityCodeSettingsTable;
		private StumpageModelPortfolio StumpageModelPortfolio;
		public StumpageModelQualityCodeSettings(Table table, StumpageModelPortfolio stumpageModelPortfolio)
		{
			ReportUtilities = new ReportUtilities(7);
			var cell = table.Rows.Add().Cells.Add();
			StumpageModelQualityCodeSettingsTable = ReportUtilities.GetNewVariableColumnTable(cell, "10% 10% 10% 10% 10% 10% 10% 10% 10% 5% 5%");
			StumpageModelPortfolio = stumpageModelPortfolio;
			cell.Paragraphs.Add(StumpageModelQualityCodeSettingsTable);
		}
		public void SetStumpageModelQualityCodeSettingsRow(List<StumpageGroup> stumpageGroups)
		{
			SetHeaderRow();
			SetQualityCodeRows(stumpageGroups);
		}

		private void SetHeaderRow()
		{
			var rpa3 = new List<string>();
			rpa3.Add("Haul Zone 3");
			rpa3.Add("RPA");

			var rpa4 = new List<string>();
			rpa4.Add("Haul Zone 4");
			rpa4.Add("RPA");

			var rpa5 = new List<string>();
			rpa5.Add("Haul Zone 5");
			rpa5.Add("RPA");

			var price3 = new List<string>();
			price3.Add("Haul Zone 3");
			price3.Add("Price");

			var price4 = new List<string>();
			price4.Add("Haul Zone 4");
			price4.Add("Price");

			var price5 = new List<string>();
			price5.Add("Haul Zone 5");
			price5.Add("Price");


			var row = StumpageModelQualityCodeSettingsTable.Rows.Add();
			ReportUtilities.AddLabelCellGray(row, "Species", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Quality Code", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, rpa3, AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, rpa4, AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, rpa5, AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "Longevity", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, price3, AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, price4, AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, price5, AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "O & A", AlignmentType.Left);
			ReportUtilities.AddLabelCellGray(row, "P&R", AlignmentType.Left);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
		private void SetQualityCodeRows(List<StumpageGroup> stumpageGroups)
		{
			foreach (var stumpageGroup in stumpageGroups.OrderBy(uu => uu.StumpageGroupName))
			{
				SetSpecies(stumpageGroup);
				foreach (var code in stumpageGroup.StumpageGroupQualityCodes.OrderBy(uu => uu.QualityCodeNumber))
				{
					SetQualityCodeRow(code);
				}
			}
		}

		private void SetSpecies(StumpageGroup stumpageGroup)
		{
			var row = StumpageModelQualityCodeSettingsTable.Rows.Add();
			ReportUtilities.AddHighlightCellBold(row, stumpageGroup.StumpageGroupName, AlignmentType.Left, 11);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
		private void SetQualityCodeRow(StumpageGroupQualityCode code)
		{
			var rpas = (from p in StumpageModelPortfolio.StumpageModelPortfolioRPADatas
						where p.StumpageGroupQualityCodeID == code.StumpageGroupQualityCodeID
						select p).FirstOrDefault();

			var rpa3 = "--";
			var rpa4 = "--";
			var rpa5 = "--";
			var price3 = "--";
			var price4 = "--";
			var price5 = "--";
			var longevity = "--";
			var ona = "--";
			var pnr = "--";

			if (rpas != null)
			{
				rpa3 = rpas.Haul3.HasValue ? rpas.Haul3.Value.ToString("N4") : "--";
				rpa4 = rpas.Haul4.HasValue ? rpas.Haul4.Value.ToString("N4") : "--";
				rpa5 = rpas.Haul5.HasValue ? rpas.Haul5.Value.ToString("N4") : "--";
				longevity = rpas.Longevity.HasValue ? rpas.Longevity.Value.ToString("") : "--";
			}
			var prices = (from p in StumpageModelPortfolio.StumpageModelPortfolioValues
						  where p.StumpageGroupQualityCodeID == code.StumpageGroupQualityCodeID
						  select p).FirstOrDefault();
			if (prices != null)
			{
				price3 = prices.Haul3.HasValue ? prices.Haul3.Value.ToString("C0") : "--";
				price4 = prices.Haul4.HasValue ? prices.Haul4.Value.ToString("C0") : "--";
				price5 = prices.Haul5.HasValue ? prices.Haul5.Value.ToString("C0") : "--";
				ona = prices.OverheadAndAdmin.HasValue ? prices.OverheadAndAdmin.Value.ToString("C0") : "--";
				pnr = prices.ProfitAndRisk.HasValue ? prices.ProfitAndRisk.Value.ToString("N0") + "%" : "--";
			}

			var row = StumpageModelQualityCodeSettingsTable.Rows.Add();
			ReportUtilities.AddLabelCellNonBold(row,"", AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, code.QualityCodeNumber.ToString(), AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, rpa3, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, rpa4, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, rpa5, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, longevity, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, price3, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, price4, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, price5, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, ona, AlignmentType.Left);
			ReportUtilities.AddLabelCellNonBold(row, pnr, AlignmentType.Left);
			row.BackgroundColor = ReportUtilities.Color_Gray;
		}
	}
}
