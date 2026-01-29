using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelSoils
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table SoilsTable;
		public ParcelSoils(Table table, Parcel parcel)
		{
			ReportUtilities = new ReportUtilities();
			SoilsTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
		}
		public void SetParcelSoilsRows()
		{
			SetSectionHeader();
			SetMUKeyDescriptions();
			SetSectionHeader2();
			SetMUKeySection();
		}
		private void SetSectionHeader()
		{
			var row = SoilsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Soils Derived Data on Parcel", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetSectionHeader2()
		{
			var row = SoilsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "MUKEY (Soil Survey)", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetMUKeyDescriptions()
		{
			var row = SoilsTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "MUKEY", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, "MUSYM", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, "Acres", AlignmentType.Left);
			ReportUtilities.AddLabelCell(row, "Description", AlignmentType.Left, 3);
			row.Border = ReportUtilities.RowBottomBorderInfoLight;
			foreach (var mu in Parcel.MUKeyParcels)
			{
				SETMUKeyDescription(mu);
			}
		}
		private void SETMUKeyDescription(MUKeyParcel mukeyParcel)
		{
			var row = SoilsTable.Rows.Add();
			ReportUtilities.AddHighlightCell(row, mukeyParcel.MUKey.MUKey1.ToString(), AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, mukeyParcel.MUKey.MUSym.ToString(), AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, mukeyParcel.Acres.ToString("N1"), AlignmentType.Left);
			ReportUtilities.AddHighlightCell(row, mukeyParcel.MUKey.Description, AlignmentType.Left, 3);
			row.Border = ReportUtilities.RowBottomBorderInfoLight;
		}

		private void SetMUKeySection()
		{
			var parcelMUKeys = Parcel.MUKeyParcels.OrderByDescending(uu => uu.MUKey.MUKey1).ToList<MUKeyParcel>();
			SetMUKeyRow1(parcelMUKeys);
			SetMUKeyRow2(parcelMUKeys);
			SetMUKeyRowAcres(parcelMUKeys);
			SetFireDamageSusceptibility(parcelMUKeys);
			SetPotentialForDamageByFire(parcelMUKeys);
			SetSoilRuttingHazard(parcelMUKeys);
			SetSuitabilityForRoadsNatural(parcelMUKeys);
			SetSuitabilityForLogLandings(parcelMUKeys);
			SetConstructionLimitationsForHaulRoadsAndLogLandings(parcelMUKeys);
			SetHarvestEquipmentOperability(parcelMUKeys);
			SetMechanicalSitePrepSurface(parcelMUKeys);
			SetMechanicalSitePrepDeep(parcelMUKeys);
			SetSuitabilityForHandPlanting(parcelMUKeys);
			SetSuitabilityForMechPlanting(parcelMUKeys);
			SetPotentialSeedlingMortality(parcelMUKeys);
			SetSiteIndex_DouglasFir(parcelMUKeys);
			SetSiteIndex_WesternRedAlder(parcelMUKeys);
		}

		private void SetMUKeyRow1(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "MUKEY", "", false);
			SetMUKeyRowData1(parcelMUKeys, table);
		}
		private void SetMUKeyRowData1(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCellBold(rowData, mu.MUKey.MUKey1.ToString(), AlignmentType.Left);
			}
		}

		private void SetMUKeyRow2(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "MUSYM", "", false);
			SetMUKeyRowData2(parcelMUKeys, table);
		}
		private void SetMUKeyRowData2(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCellBold(rowData, mu.MUKey.MUSym.ToString(), AlignmentType.Left);
			}
		}

		private void SetMUKeyRowAcres(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Acres", "", true);
			SetMUKeyRowAcresData(parcelMUKeys, table);
		}
		private void SetMUKeyRowAcresData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.Acres.ToString("N1"), AlignmentType.Left);
			}
		}

		private void SetFireDamageSusceptibility(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Fire Damage Susceptibility", "/Documents/Fire Damage Susceptibility.pdf", false);
			SetFireDamageSusceptibilityData(parcelMUKeys, table);
		}
		private void SetFireDamageSusceptibilityData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.FireDamageSusceptibility, AlignmentType.Left);
			}
		}
		private void SetPotentialForDamageByFire(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Potential for Damage by Fire", "/Documents/Potential for Damage by Fire.pdf", false);
			SetPotentialForDamageByFireData(parcelMUKeys, table);
		}
		private void SetPotentialForDamageByFireData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.FireDamagePotential, AlignmentType.Left);
			}
		}
		private void SetSoilRuttingHazard(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Soil Rutting Hazard", "/Documents/Soil Rutting Hazard.pdf", false);
			SetSoilRuttingHazardData(parcelMUKeys, table);
		}
		private void SetSoilRuttingHazardData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.SoilRuttingHazard, AlignmentType.Left);
			}
		}
		private void SetSuitabilityForRoadsNatural(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Suitability for Roads (Natural Surface) (WA)", "/Documents/Suitability for Roads (Natural Surface) (WA).pdf", false);
			SetSuitabilityForRoadsNaturalData(parcelMUKeys, table);
		}
		private void SetSuitabilityForRoadsNaturalData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.SuitabilityForRoadsNaturalSurface, AlignmentType.Left);
			}
		}
		private void SetSuitabilityForLogLandings(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Suitability for Log Landings (WA)", "/Documents/Suitability for Log Landings (WA).pdf", false);
			SetSuitabilityForLogLandingsData(parcelMUKeys, table);
		}
		private void SetSuitabilityForLogLandingsData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.SuitabilityForLogLandings, AlignmentType.Left);
			}
		}
		private void SetConstructionLimitationsForHaulRoadsAndLogLandings(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Construction Limitations for Haul Roads and Log Landings", "/Documents/Construction Limitations for Haul Roads and Log Landings.pdf", false);
			SetConstructionLimitationsForHaulRoadsAndLogLandingsData(parcelMUKeys, table);
		}
		private void SetConstructionLimitationsForHaulRoadsAndLogLandingsData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.ConstructionLimitationsForHaulRoadsAndLogLandings, AlignmentType.Left);
			}
		}
		private void SetHarvestEquipmentOperability(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Harvest Equipment Operability", "/Documents/Harvest Equipuipment Operability.pdf", false);
			SetHarvestEquipmentOperabilityData(parcelMUKeys, table);
		}
		private void SetHarvestEquipmentOperabilityData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.HarvestEquipmentOperability, AlignmentType.Left);
			}
		}
		private void SetMechanicalSitePrepSurface(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Mechanical Site Preparation (Surface)", "/Documents/Mechanical Site Preparation (Surface).pdf", false);
			SetMechanicalSitePrepSurfaceData(parcelMUKeys, table);
		}
		private void SetMechanicalSitePrepSurfaceData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.MechanicalSitePreparationSurface, AlignmentType.Left);
			}
		}
		private void SetMechanicalSitePrepDeep(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Mechanical Site Preparation (Deep)", "/Documents/Mechanical Site Preparation (Deep).pdf", false);
			SetMechanicalSitePrepDeepData(parcelMUKeys, table);
		}
		private void SetMechanicalSitePrepDeepData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.MechanicalSitePreparationDeep, AlignmentType.Left);
			}
		}
		private void SetSuitabilityForHandPlanting(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Suitability for Hand Planting", "/Documents/Suitability for Hand Planting.pdf", false);
			SetSuitabilityForHandPlantingData(parcelMUKeys, table);
		}
		private void SetSuitabilityForHandPlantingData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.SuitabilityForHandPlanting, AlignmentType.Left);
			}
		}
		private void SetSuitabilityForMechPlanting(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Suitability for Mechanical Planting", "/Documents/Suitability for Mechanical Planting.pdf", false);
			SetSuitabilityForMechPlantingData(parcelMUKeys, table);
		}
		private void SetSuitabilityForMechPlantingData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.SuitabilityForMechanicalPlanting, AlignmentType.Left);
			}
		}
		private void SetPotentialSeedlingMortality(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Potential for Seedling Mortality", "/Documents/Potential for Seedling Mortality.pdf", false);
			SetPotentialSeedlingMortalityData(parcelMUKeys, table);
		}
		private void SetPotentialSeedlingMortalityData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				ReportUtilities.AddHighlightCell(rowData, mu.MUKey.PotentialForSeedlingMortality, AlignmentType.Left);
			}
		}
		private void SetSiteIndex_DouglasFir(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Tree Site Index Douglas-fir", "/Documents/Site Index PSME.pdf", false);
			SetSiteIndex_DouglasFirData(parcelMUKeys, table);
		}
		private void SetSiteIndex_DouglasFirData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				if (mu.MUKey.TreeSiteIndexDouglasFir.HasValue)
				{
					ReportUtilities.AddHighlightCell(rowData, mu.MUKey.TreeSiteIndexDouglasFir.Value.ToString(), AlignmentType.Left);
				}
				else
				{
					ReportUtilities.AddHighlightCell(rowData, "--", AlignmentType.Left);
				}
			}
		}
		private void SetSiteIndex_WesternRedAlder(List<MUKeyParcel> parcelMUKeys)
		{
			var table = GetMUKeyTable(parcelMUKeys, "Tree Site Index Western Red Alder", "/Documents/Site Index ALRU.pdf", false);
			SetSiteIndex_WesternRedAlderData(parcelMUKeys, table);
		}
		private void SetSiteIndex_WesternRedAlderData(List<MUKeyParcel> parcelMUKeys, Table table)
		{
			var rowData = table.Rows.Add();
			foreach (var mu in parcelMUKeys)
			{
				if (mu.MUKey.TreeSiteIndexWesternRedAlder.HasValue)
				{
					ReportUtilities.AddHighlightCell(rowData, mu.MUKey.TreeSiteIndexWesternRedAlder.Value.ToString(), AlignmentType.Left);
				}
				else
				{
					ReportUtilities.AddHighlightCell(rowData, "--", AlignmentType.Left);
				}
			}
		}

		private Table GetMUKeyTable(List<MUKeyParcel> parcelMUKeys, string text, string url, bool highlightLabel)
		{
			var count = parcelMUKeys.Count();
			var row = SoilsTable.Rows.Add();
			if (highlightLabel)
			{
				ReportUtilities.AddHighlightCellBold(row, text, AlignmentType.Left, 2);
			}
			else
			{
				var cellText = ReportUtilities.AddLabelCell(row, text, AlignmentType.Left, 2);
				if (!string.IsNullOrEmpty(url))
				{
					cellText.Paragraphs.Clear();
					var t = new Text();
					cellText.Paragraphs.Add(t);
					var seg = t.Segments.Add(text);
					seg.Hyperlink = new Hyperlink();
					seg.Hyperlink.LinkType = HyperlinkType.Web;
					seg.Hyperlink.Url = "http://frass.forest-econometrics.com" + url;
					seg.TextInfo = ReportUtilities.LinkTextInfo;
				}
			}
			var cell = row.Cells.Add();
			cell.ColumnsSpan = 4;

			var mukeyTable = ReportUtilities.GetNewVariableColumnTable(cell, count);
			cell.Paragraphs.Add(mukeyTable);
			row.Border = ReportUtilities.RowBottomBorderInfoLight;
			return mukeyTable;
		}
	}
}