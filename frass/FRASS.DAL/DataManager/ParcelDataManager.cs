using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;
using System.Data.Linq;

namespace FRASS.DAL.DataManager
{
	public class ParcelDataManager
	{
		ParcelRepository db;
		private ParcelDataManager()
		{
			db = ParcelRepository.GetInstance();
		}
		public static ParcelDataManager GetInstance()
		{
			return new ParcelDataManager();
		}

		public Parcel GetParcel(Int32 parcelID)
		{
			return db.GetParcel(parcelID);
		}
		public ISingleResult<GetCurrentParcelTimberSummaryResult> GetCurrentParcelTimberSummary(Int32 parcelid, Int32 reportyear)
		{
			return db.GetCurrentParcelTimberSummary(parcelid, reportyear);
		}
		public List<IReportParcelTimberStatistic> GetReportTimberStatistics(Int32 parcelID, Int32 reportyear)
		{
			var key = "ReportParcelTimberStatistic_" + parcelID.ToString() + "_" + reportyear;
			List<IReportParcelTimberStatistic> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetReportTimberStatistics(parcelID, reportyear);
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public List<IReportParcelTimberStatistics> GetReportParcelTimberStatistics(Int32 ParcelID, Int32 ReportYear)
		{
			var key = "ReportParcelTimberStatistics" + ParcelID.ToString() + "_" + ReportYear;
			List<IReportParcelTimberStatistics> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetReportParcelTimberStatistics(ParcelID, ReportYear);
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public LogMarketReportSpecy GetLogMarketReportSpecy(Int32 logMarketReportSpeciesID)
		{
			return db.GetLogMarketReportSpecy(logMarketReportSpeciesID);
		}
		public List<IParcelListingItem> GetParcelListingItems()
		{
			var key = "ParcelListingItem";
			List<IParcelListingItem> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetParcelListingItems();
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public List<ParcelAllotment> GetParcelAllotmentByAllotmentNumber(string allotmentNumber, int parcelID)
		{
			var key = "GetParcelAllotmentByAllotmentNumber" + allotmentNumber + "_" + parcelID.ToString();
			List<ParcelAllotment> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetParcelAllotmentByAllotmentNumber(allotmentNumber, parcelID);
				CacheHelper.Add(list, key);
			}

			return list;
		}
		public List<ParcelAllotmentShare> GetParcelAllotmentSharesByParcelID(int parcelID)
		{
			var key = "GetParcelAllotmentSharesByParcelID" + parcelID.ToString();
			List<ParcelAllotmentShare> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetParcelAllotmentSharesByParcelID(parcelID);
				CacheHelper.Add(list, key);
			}

			return list;
		}

		public int GetIndexNumber(decimal town, decimal range, decimal section)
		{
			var key = "GetTRSIndex_" + town.ToString() + "_" + range.ToString() + "_" + section;
			var i = 0;
			if (!CacheHelper.Get(key, out i))
			{
				i = db.GetIndexNumber(town, range, section);
				CacheHelper.Add(i, key);
			}
			return i;
		}
	}
}
