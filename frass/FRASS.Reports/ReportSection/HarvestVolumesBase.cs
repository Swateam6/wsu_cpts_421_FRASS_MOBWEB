using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Pdf.Generator;
using FRASS.Reports;
using FRASS.DAL;
using FRASS.BLL.Models;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;

namespace FRASS.Reports.ReportSection
{
	internal abstract class HarvestVolumesBase
	{
		protected Table HarvestVolumesTable;
		protected ReportUtilities ReportUtilities;
		protected Parcel Parcel;
		protected List<IReportParcelTimberStatistic> TimberStats;
		protected EconVariables EconVariables;
		protected int EarliestHarvestDate = 3000;
		protected int minYear = 0;
		protected decimal TotalPresentValue = 0M;
		protected StandDataManager dbStandDataManager;
		protected List<Int32> CalendarYears;

		protected HarvestVolumesBase(Table table, Parcel parcel, List<IReportParcelTimberStatistic> timberStats)
		{
			ReportUtilities = new ReportUtilities();
			HarvestVolumesTable = ReportUtilities.AppendNewTable(table, 10);
			Parcel = parcel;
			TimberStats = timberStats;
			dbStandDataManager = StandDataManager.GetInstance();
			CalendarYears = dbStandDataManager.GetCurrentStandSortYears();
			minYear = CalendarYears.Min(uu => uu);
		}
		protected abstract void SetUpStandValue(int standid);
		public void SetTotalRows()
		{
			SetUpStandValues();
			SetStandValuesTotals();
			SetRoadTotalRow();
			SetRoadValueForested();
			SetRoadValueEntire();
			SetRoadBareLand();
		}
		public decimal SetHarvestVolumesRows()
		{
			SetHeader();
			SetUpperTableHeader();
			SetLowerTableHeader();
			SetStandValues();
			SetStandValuesTotalRowHeaders();
			SetStandValuesTotals();
			SetRoadTotalRow();
			SetRoadValueForested();
			SetRoadValueEntire();
			SetRoadBareLand();
			return TotalPresentValue;
		}
		protected void SetHeader()
		{
			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Harvest Volumes & Value Summary", AlignmentType.Center, 10);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected void SetUpperTableHeader()
		{
			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Stand Info", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCell(row, "Current Rotation", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCell(row, "Next Rotation", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCell(row, "Third Rotation Into Perpetuity", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCell(row, "Total Present Value", AlignmentType.Center, 2);
		}
		protected void SetLowerTableHeader()
		{
			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Stand ID Number", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Operable Commercial Timber Land Acres", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Harvest Year", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Net Present Value", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Rotation Length (Years)", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Net Present Value", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Rotation Length", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Soil Expectation Value (Present Value)", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Stand", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, "Per Acre", AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;

		}
		protected void SetStandValues()
		{
			var list = Parcel.ParcelRiparians.Where(uu => uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.STD_ID).ToList<int>().Distinct();
			foreach (var l in list)
			{
				SetStandValue(l);
			}
		}
		protected void SetUpStandValues()
		{
			var list = Parcel.ParcelRiparians.Where(uu => uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.STD_ID).ToList<int>().Distinct();
			foreach (var l in list)
			{
				SetUpStandValue(l);
			}
		}
		protected abstract void SetStandValue(int standid);
		protected void SetStandValuesTotalRowHeaders()
		{
			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCellNonBold(row, " ", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, "Scheduled In", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, "Current Cost", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, "Future Cost", AlignmentType.Center, 2);
			ReportUtilities.AddLabelCellNonBold(row, "Discounted Road Cost", AlignmentType.Center, 2);
		}
		protected void SetStandValuesTotals()
		{
			var distance = Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary;

			var sched = "--";
			var cur = "--";
			var fut = "--";
			var rd = "--";

			if (distance.HasValue && distance.Value > 0)
			{
				distance = (distance / 1000) * 0.621371192237334M;
				var currentCosts = EconVariables.NewRoad * distance.Value;
				var years = EarliestHarvestDate - 2 - DateTime.Now.Year;
				var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + EconVariables.RateOfInflation), years));
				var futureCosts = currentCosts * power;
				var discount = futureCosts / (Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation) * (1 + EconVariables.RealDiscount)), years)));
				sched = (EarliestHarvestDate - 2).ToString();
				if (EarliestHarvestDate == minYear)
				{
					sched = (EarliestHarvestDate).ToString();
				}

				cur = currentCosts.ToString("C2");
				fut = futureCosts.ToString("C2");
				rd = (-1 * discount).ToString("C2");
				TotalPresentValue = TotalPresentValue - discount;
			}


			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "New Road Construction", AlignmentType.Left, 2);
			ReportUtilities.AddLabelCell(row, sched, AlignmentType.Right, 2);
			ReportUtilities.AddLabelCell(row, cur, AlignmentType.Right, 2);
			ReportUtilities.AddLabelCell(row, fut, AlignmentType.Right, 2);
			ReportUtilities.AddLabelCell(row, rd, AlignmentType.Right, 2);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected void SetRoadTotalRow()
		{
			var standids = TimberStats.Where(uu => uu.StandStats == StandStats.Operable).Select(uu => uu.Stand_ID).Distinct().ToList<int>();

			var opacres = (from p in Parcel.ParcelRiparians
						   where p.StandStatID == Convert.ToInt32(StandStats.Operable) && (from s in standids select s).Contains(p.STD_ID)
						   select p).Sum(uu => uu.Acres);
			var valperacre = 0M;
			if (opacres > 0)
			{
				valperacre = TotalPresentValue / opacres;
			}

			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Total Value based on Operable Commercial Timber Land Acres:", AlignmentType.Right, 7);
			ReportUtilities.AddLabelCellNonBold(row, opacres.ToString("N1") + " Acres", AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, TotalPresentValue.ToString("C0"), AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, valperacre.ToString("C0") + "/Acre", AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected void SetRoadValueForested()
		{
			var standids = TimberStats.Where(uu => uu.StandStats == StandStats.Operable || uu.StandStats == StandStats.ActChan || uu.StandStats == StandStats.Riparian).Select(uu => uu.Stand_ID).Distinct().ToList<int>();
			var acres = (from p in Parcel.ParcelRiparians
						 where (from s in standids select s).Contains(p.STD_ID)
						 && (p.StandStatID == Convert.ToInt32(StandStats.Operable) ||
							  p.StandStatID == Convert.ToInt32(StandStats.Riparian))
						 select p).Sum(uu => uu.Acres);
			var valperacre = TotalPresentValue / acres;

			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Value per Acres (Forested Acres):", AlignmentType.Right, 7);
			ReportUtilities.AddLabelCellNonBold(row, acres.ToString("N1") + " Acres", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, valperacre.ToString("C0") + "/Acre", AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected void SetRoadValueEntire()
		{
			var acres = Parcel.Acres;
			var valperacre = TotalPresentValue / acres;

			var row = HarvestVolumesTable.Rows.Add();
			ReportUtilities.AddLabelCell(row, "Value per Acres (Entire Parcel):", AlignmentType.Right, 7);
			ReportUtilities.AddLabelCellNonBold(row, acres.ToString("N1") + " Acres", AlignmentType.Center);
			ReportUtilities.AddLabelCell(row, " ", AlignmentType.Center);
			ReportUtilities.AddHighlightCell(row, valperacre.ToString("C0") + "/Acre", AlignmentType.Center);
			row.Border = ReportUtilities.RowBottomBorderInfo;
		}
		protected abstract void SetRoadBareLand();
	}
}
