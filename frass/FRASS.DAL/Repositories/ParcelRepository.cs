using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;
using FRASS.Interfaces;
using System.Data.Linq;

namespace FRASS.DAL.Repositories
{
	internal class ParcelRepository
	{
		private FRASSDataContext db;
		private ParcelRepository()
		{
			db = new FRASSDataContext();
		}
		public static ParcelRepository GetInstance()
		{
			return new ParcelRepository();
		}
		public Parcel GetParcel(Int32 parcelID)
		{
			var parcel = (from p in db.Parcels where p.ParcelID == parcelID select p).FirstOrDefault();
			return parcel;
		}

		public ISingleResult<GetCurrentParcelTimberSummaryResult> GetCurrentParcelTimberSummary(Int32 parcelid, Int32 reportyear)
		{
			return db.GetCurrentParcelTimberSummary(parcelid, reportyear);
		}

		public List<IReportParcelTimberStatistic> GetReportTimberStatistics(Int32 parcelID, Int32 reportyear)
		{
			List<IReportParcelTimberStatistic> list;
			var vals = (from tcs in db.TimberCurrentStands
						join pr in db.ParcelRiparians on tcs.STD_ID equals pr.STD_ID
						join tcss in db.TimberCurrentStandSorts on tcs.TimberCurrentStandID equals tcss.TimberCurrentStandID
						where pr.ParcelID == parcelID && tcss.ReportYear == reportyear
						select new ReportParcelTimberStatistic()
						{
							Site_Index = tcs.Yield_Index,
							Stand_ID = tcs.STD_ID,
							Veg_Label = tcs.Veg_Label,
							Acres = pr.Acres,
							StandStats = (StandStats)pr.StandStatID,
							Board_SN = tcss.Board_SN,
							pctbrd = tcss.PctBrd
						}).ToList();

			var noVals = (from pr in db.ParcelRiparians
						  where !(from v in vals select v.Stand_ID).Contains(pr.STD_ID) && pr.ParcelID == parcelID && pr.ParcelID == parcelID
						  select new ReportParcelTimberStatistic()
						  {
							  Site_Index = pr.TimberCurrentStand.Yield_Index,
							  Stand_ID = pr.STD_ID,
							  Veg_Label = pr.TimberCurrentStand.Veg_Label,
							  Acres = pr.Acres,
							  StandStats = (StandStats)pr.StandStatID,
							  Board_SN = 0,
							  pctbrd = 0
						  }).ToList();
			vals.AddRange(noVals);
			list = new List<IReportParcelTimberStatistic>();
			foreach (var v in vals)
			{
				var item = (from l in list
							where l.Acres == v.Acres && l.Board_SN == v.Board_SN && l.StandStats == v.StandStats && l.Site_Index == v.Site_Index && l.Stand_ID == v.Stand_ID && l.Veg_Label == v.Veg_Label && l.pctbrd == v.pctbrd
							select l).FirstOrDefault();
				if (item == null)
				{
					list.Add(v);
				}
			}
			return list;
		}

		public LogMarketReportSpecy GetLogMarketReportSpecy(Int32 logMarketReportSpeciesID)
		{
			return (from l in db.LogMarketReportSpecies where l.LogMarketReportSpeciesID == logMarketReportSpeciesID select l).FirstOrDefault();
		}

		public List<IParcelListingItem> GetParcelListingItems()
		{
			var list = (from p in db.Parcels
						join pa in db.ParcelAllotments on p.ParcelID equals pa.ParcelID
						join pc in db.ParcelCounties on p.ParcelID equals pc.ParcelID
						join c in db.Counties on pc.CountyID equals c.CountyID
						join ot in db.OwnerTypes on pa.OwnerTypeID equals ot.OwnerTypeID
						select new ParcelListingItem()
						{
							ParcelID = p.ParcelID,
							ParcelNumber = p.ParcelNumber,
							Township = pa.Town,
							Section = pa.Section,
							Range = pa.Range,
							Acre = p.Acres,
							Hectare = p.Hectares,
							County = c.County1,
							Allotment = pa.AllotmentNumber,
							OwnerStatus = ot.OwnerType1
						}
					).ToList<IParcelListingItem>();
			var list2 = new List<IParcelListingItem>();
			foreach (var l in list)
			{
				var isThere = (from ls in list2
							   where
								   ls.ParcelID == l.ParcelID &&
								   ls.ParcelNumber == l.ParcelNumber &&
								   ls.Township == l.Township &&
								   ls.Section == l.Section &&
								   ls.Range == l.Range &&
								   ls.Acre == l.Acre &&
								   ls.Hectare == l.Hectare &&
								   ls.County == l.County &&
								   ls.Allotment == l.Allotment &&
								   ls.OwnerStatus == l.OwnerStatus
							   select ls
								).Any();
				if (!isThere)
				{
					list2.Add(l);
				}
			}
			list = list2.OrderBy(uu => uu.ParcelNumber).ToList<IParcelListingItem>();
			return list;
		}

		public List<ParcelAllotment> GetParcelAllotmentByAllotmentNumber(string allotmentNumber, int parcelID)
		{
			return (from p in db.ParcelAllotments where p.AllotmentNumber.Equals(allotmentNumber) && p.ParcelID == parcelID select p).ToList<ParcelAllotment>();
		}
		public List<ParcelAllotmentShare> GetParcelAllotmentSharesByParcelID(int parcelID)
		{
			return (from p in db.ParcelAllotmentShares where p.ParcelAllotment.Parcel.ParcelID == parcelID select p).ToList<ParcelAllotmentShare>();
		}
		public int GetIndexNumber(decimal town, decimal range, decimal section)
		{
			var index = (from i in db.TRS_IndexNumbers where i.Township == town && i.Range == range && i.Section == section select i.IndexNumber).FirstOrDefault();
			return index;
		}
		public List<IReportParcelTimberStatistics> GetReportParcelTimberStatistics(Int32 ParcelID, Int32 ReportYear)
		{
			var stats = GetReportTimberStatistics(ParcelID, ReportYear);
			var items = GetListOfUniqueAreas(stats);

			foreach (var pr in stats)
			{
				var pItem = (from i in items
							 where i.Stand_ID == pr.Stand_ID && i.Veg_Label == pr.Veg_Label && i.Site_Index == pr.Site_Index
							 select i).FirstOrDefault();
				if (pItem != null)
				{
					if (pr.StandStats == StandStats.Operable)
					{
						var acre = from oa in pItem.operableAcres where oa == pr.Acres select oa;
						if (acre.Count() == 0)
						{
							pItem.operableAcres.Add(pr.Acres);
							pItem.Operable_Land_Acres = pItem.Operable_Land_Acres + pr.Acres;
							pItem.Total_Acres = pItem.Total_Acres + pr.Acres;
						}
						pItem.StandAmount = pItem.StandAmount + pr.Board_SN * pr.pctbrd * pr.Acres;
					}
					else if (pr.StandStats == StandStats.Riparian)
					{
						var acre = from noa in pItem.nonoperableAcres where noa == pr.Acres select noa;
						if (acre.Count() == 0)
						{
							pItem.Riparian_Zone_Acres = pItem.Riparian_Zone_Acres + pr.Acres;
							pItem.nonoperableAcres.Add(pr.Acres);							
							pItem.Total_Acres = pItem.Total_Acres + pr.Acres;
						}
						pItem.StandAmount = pItem.StandAmount + pr.Board_SN * pr.pctbrd * pr.Acres;
					}
				}
			}

			foreach (var report in items)
			{
				if (report.Total_Acres == 0)
				{
					report.BFAcre_PerStand = 0;
				}
				else
				{
					report.BFAcre_PerStand = report.StandAmount / report.Total_Acres;
				}
				report.TotalBF_PerStand = report.BFAcre_PerStand * report.Operable_Land_Acres;
			}
			return items;
		}
		private List<IReportParcelTimberStatistics> GetListOfUniqueAreas(List<IReportParcelTimberStatistic> stats)
		{
			var items = new List<IReportParcelTimberStatistics>();
			foreach (var pr in stats)
			{
				var item = new ReportParcelTimberStatistics();
				item.Stand_ID = pr.Stand_ID;
				item.Veg_Label = pr.Veg_Label;
				item.Site_Index = pr.Site_Index;
				item.operableAcres = new List<decimal>();
				item.nonoperableAcres = new List<decimal>();
				item.StandAmount = 0;
				item.Operable_Land_Acres = 0;
				item.Riparian_Zone_Acres = 0;
				item.Total_Acres = 0;
				item.TotalBF_PerStand = 0;

				var pItem = (from i in items
							 where i.Stand_ID == item.Stand_ID && i.Veg_Label == item.Veg_Label && i.Site_Index == item.Site_Index
							 select i).FirstOrDefault();
				if (pItem == null)
				{
					items.Add(item);
				}
			}
			return items;
		}

		private class ReportParcelTimberStatistic : IReportParcelTimberStatistic
		{
			public int Stand_ID { get; set; }
			public string Veg_Label { get; set; }
			public int Site_Index { get; set; }
			public decimal Board_SN { get; set; }
			public decimal Acres { get; set; }
			public StandStats StandStats { get; set; }
			public decimal pctbrd { get; set; }
		}

		private class ReportParcelTimberStatistics : IReportParcelTimberStatistics
		{
			public int Stand_ID { get; set; }
			public string Veg_Label { get; set; }
			public int Site_Index { get; set; }
			public decimal Riparian_Zone_Acres { get; set; }
			public decimal Operable_Land_Acres { get; set; }
			public decimal Total_Acres { get; set; }
			public decimal BFAcre_PerStand { get; set; }
			public decimal TotalBF_PerStand { get; set; }
			public decimal StandAmount { get; set; }
			public List<decimal> operableAcres { get; set; }
			public List<decimal> nonoperableAcres { get; set; }
		}

		private class ParcelListingItem : IParcelListingItem
		{
			public Int32 ParcelID { get; set; }
			public string ParcelNumber { get; set; }
			public decimal Acre { get; set; }
			public decimal Hectare { get; set; }
			public string Township { get; set; }
			public string Range { get; set; }
			public string Section { get; set; }
			public string County { get; set; }
			public string Allotment { get; set; }
			public string OwnerStatus { get; set; }
		}
	}
}
