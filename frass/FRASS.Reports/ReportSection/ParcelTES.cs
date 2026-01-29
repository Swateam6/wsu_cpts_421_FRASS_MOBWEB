using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;

namespace FRASS.Reports.ReportSection
{
	internal class ParcelTES
	{
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private Table TESTable;

		private string warn = "Caution: Timber harvest timing may be restricted to times when the species is not nesting.";
		private string ok = "No restrictions apply";

		public ParcelTES(Table table, Parcel parcel)
		{
			ReportUtilities = new ReportUtilities();
			TESTable = ReportUtilities.AppendNewSixColumnTable(table);
			Parcel = parcel;
		}
		public void SetParcelTESRows()
		{
			SetSectionHeader();
			SetMurrelet(); 
			SetOwls();
			SetEagle();
		}

		public void SetParcelTESValueRows(string BaseFilePath)
		{
			SetHeader();
			SetMurreletImages(BaseFilePath); 
			SetOwlImages(BaseFilePath);
			SetEagleImages(BaseFilePath);
		}

		private void SetHeader()
		{
			var row = TESTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Threatened, Endagered & Sensitive Species Habitat", AlignmentType.Center, 6);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetSectionHeader()
		{
			SetHeader();
			var row = TESTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Species", AlignmentType.Center, 4);
			ReportUtilities.AddHighlightCellBold(row, "Acres", AlignmentType.Center, 2);
			row.Border = ReportUtilities.RowBottomBorderInfoLight;
		}
		private void SetOwlImages(string BaseFilePath)
		{
			var txt = ok;
			var filePath = BaseFilePath + "/images/32_ok.png";

			var owlp = Parcel.ParcelOwls.Sum(uu => uu.Acres);
			if (owlp > 0)
			{
				txt = warn;
				filePath = BaseFilePath + "/images/32_warn.png";
			}
			var row = TESTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Northern Spotted Owl, (Stix occidentalis caurina)", AlignmentType.Center, 3);
			ReportUtilities.AddImageCell(row, filePath, AlignmentType.Right, ImageFileType.Png);
			ReportUtilities.AddHighlightCell(row, txt, AlignmentType.Left,2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetMurreletImages(string BaseFilePath)
		{
			var txt = ok;
			var filePath = BaseFilePath + "/images/32_ok.png";
			var mp = Parcel.ParcelMurreletts.Select(uu => uu.MM_Suita);
			foreach (var m in mp)
			{
				if (m == "Core Habitat")
				{
					txt = warn;
					filePath = BaseFilePath + "/images/32_warn.png";
				}
			}
			var row = TESTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Marbled Murrelet, (Brachyramphus marmoratus)", AlignmentType.Center, 3);
			ReportUtilities.AddImageCell(row, filePath, AlignmentType.Right, ImageFileType.Png);
			ReportUtilities.AddHighlightCell(row, txt, AlignmentType.Left,2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetEagleImages(string BaseFilePath)
		{
			var txt = ok;
			var filePath = BaseFilePath + "/images/32_ok.png";
			var eaglep = Parcel.ParcelEagles.Select(uu => uu.Nest_Status);
			foreach (var e in eaglep)
			{
				if (e == "Occupied+Active" || e == "Unoccup'd+Active")
				{
					txt = warn;
					filePath = BaseFilePath + "/images/32_warn.png";
				}
			}
			var row = TESTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "American Bald Eagle, (Haliaeetus leucocephalus)", AlignmentType.Center, 3);
			ReportUtilities.AddImageCell(row, filePath, AlignmentType.Right, ImageFileType.Png);
			ReportUtilities.AddHighlightCell(row, txt, AlignmentType.Left,2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private void SetOwls()
		{
			var row = TESTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Northern Spotted Owl, (Strix occidentalis caurina)", AlignmentType.Left, 4);
			string txt = Parcel.Acres.ToString("N2") + " Unsuitable Habitat";
			ReportUtilities.AddHighlightCell(row, txt, AlignmentType.Left, 2);
			row.Border = ReportUtilities.RowBottomBorderInfoLight;
		}
		private void SetMurrelet()
		{
			var row = TESTable.Rows.Add();
			var owlp = Parcel.ParcelMurreletts.Sum(uu => uu.Acres);
			ReportUtilities.AddLabelCell(row, "Marbled Murrelet, (Brachyramphus marmoratus)", AlignmentType.Left, 4);
			var list = new List<string>();
			if (Parcel.ParcelMurreletts.Count > 0)
			{
				foreach (ParcelMurrelett pm in Parcel.ParcelMurreletts)
				{
					if (pm.MM_Suita != string.Empty)
					{
						list.Add(pm.Acres.ToString("N2") + " " + pm.MM_Suita);
					}
				}
			}
			else
			{
				list.Add(" ");
			}

			ReportUtilities.AddHighlightCell(row, list, AlignmentType.Left, 2);
			row.Border = ReportUtilities.RowBottomBorderInfoLight;
		}
		private void SetEagle()
		{
			var row = TESTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "American Bald Eagle, (Haliaeetus leucocephalus)", AlignmentType.Left, 4);
			string txt = Parcel.Acres.ToString("N2") + " Unsuitable Habitat";
			ReportUtilities.AddHighlightCell(row, txt, AlignmentType.Left, 2);
			row.Border = ReportUtilities.RowBottomBorderInfoLight;
		}
	}

}