using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.BLL.Models;
using FRASS.Interfaces;

namespace FRASS.Reports.ReportSection
{
	internal class Allotees
	{
		private decimal TotalPresentValue;
		private ReportUtilities ReportUtilities;
		private Parcel Parcel;
		private List<ParcelAllotmentShare> Shares;
		private Int32 pageSize = 30;

		public Allotees(Parcel parcel, List<ParcelAllotmentShare> shares, decimal totalPresentValue)
		{
			ReportUtilities = new ReportUtilities();
			Parcel = parcel;
			Shares = shares;
			TotalPresentValue = totalPresentValue;
		}

		public List<Section> GetAllotteePages()
		{
			var pages = new List<Section>();
			Int32 pageCT = ((Shares.Count() - 1) / pageSize) + 1;
			for(var ct = 0; ct < pageCT; ct++)
			{
				var section = new Section();
				var table = ReportUtilities.GetNewTable(7);
				SetAllotteesRow(table, ct);
				section.Paragraphs.Add(table);
				section.PageInfo.Margin = ReportUtilities.SectionMargin;
				pages.Add(section);
			}
			return pages;
		}

		private void SetAllotteesRow(Table table, int pageCT)
		{
			SetHeader(table);
			SetLowerTableHeader(table);
			SetAllotteeRows(table, pageCT);
		}

		protected void SetHeader(Table table)
		{
			var row = table.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Allottee Owner Names and Shares (1999)", AlignmentType.Center, 7);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected void SetLowerTableHeader(Table table)
		{
			var header1 = new List<string>();
			header1.Add("Allottee");
			header1.Add("Number");
			var header2 = new List<string>();
			header2.Add("Undivided Interest");
			header2.Add("Share");
			var header3 = new List<string>();
			header3.Add("Prorated");
			header3.Add("Value");
			var header4 = new List<string>();
			header4.Add("Fractionation");
			header4.Add("Scalar");
			var header5= new List<string>();
			header5.Add("Value Adjusted for");
			header5.Add("Fractionation");
			var row = table.Rows.Add();
			ReportUtilities.AddLabelCellGray(row, "Last Name", AlignmentType.Center);
			ReportUtilities.AddLabelCellGray(row, "First Name", AlignmentType.Center);
			ReportUtilities.AddLabelCellGray(row, header1, AlignmentType.Center);
			ReportUtilities.AddLabelCellGray(row, header2, AlignmentType.Center);
			ReportUtilities.AddLabelCellGray(row, header3, AlignmentType.Center);
			ReportUtilities.AddLabelCellGray(row, header4, AlignmentType.Center);
			ReportUtilities.AddLabelCellGray(row, header5, AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;

		}
		private void SetAllotteeRows(Table table, int pageCT)
		{
			foreach (var share in Shares.Skip(pageCT * pageSize).Take(pageSize))
			{
				SetAllotteeRow(table, share);
			}
		}

		private void SetAllotteeRow(Table table, ParcelAllotmentShare allottee)
		{
			var LastName = "";
			var FirstName = "";
			var AllotteeNumber = "";
			var UndivdedInteresetShare = "";
			var ProratedValue = "";
			var FractionationScalar = "";
			var ValueAdjustedForFractionation = "";
			FirstName = allottee.ParcelAllottee.FirstName;
			LastName = allottee.ParcelAllottee.LastName;
			AllotteeNumber = allottee.ParcelAllottee.AllotteeNumber.ToString();
			var share = allottee.Share;
			
			UndivdedInteresetShare = share.ToString("N5");
			var proratedValue = TotalPresentValue * share;
			ProratedValue = proratedValue.ToString("C2");
			var fractionalShare = GetFractionalShare(share);
			FractionationScalar = fractionalShare.ToString("N1");
			ValueAdjustedForFractionation = (proratedValue * fractionalShare).ToString("C2");

			var row = table.Rows.Add();
			ReportUtilities.AddLabelCell(row, LastName, AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, FirstName, AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, AllotteeNumber, AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, UndivdedInteresetShare, AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, ProratedValue, AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, FractionationScalar, AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, ValueAdjustedForFractionation, AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		private decimal GetFractionalShare(decimal share)
		{
			var f = 0M;
			if (share > .9M)
			{
				f = 1M;
			}
			else if (share > .8M)
			{
				f = .9M;
			}
			else if (share > .7M)
			{
				f = .85M;
			}
			else if (share > .6M)
			{
				f = .8M;
			}
			else if (share > .5M)
			{
				f = .7M;
			}
			else if (share > .4M)
			{
				f = .6M;
			}
			else if (share > .3M)
			{
				f = .5M;
			}
			else if (share > .2M)
			{
				f = .3M;
			}
			else if (share > .1M)
			{
				f = .1M;
			}
			return f;
		}
	}
}
