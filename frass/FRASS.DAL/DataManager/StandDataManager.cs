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
	public class StandDataManager
	{
		StandRepository db;
		private StandDataManager()
		{
			db = StandRepository.GetInstance();
		}
		public static StandDataManager GetInstance()
		{
			return new StandDataManager();
		}

		public List<IStandData> GetStandDataCurrent(int standid, int parcelid)
		{
			var key = "StandDataCurrent_" + standid.ToString() + "_" + parcelid.ToString();
			var list = new List<IStandData>();
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetStandDataCurrent(standid, parcelid);
				CacheHelper.Add(list, key);
			}
			return list;
		}

		public List<IStandData> GetStandDataFuture(int standid, int parcelid)
		{
			var key = "StandDataFuture" + standid.ToString() + "_" + parcelid.ToString();
			var list = new List<IStandData>();
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetStandDataFuture(standid, parcelid);
				CacheHelper.Add(list, key);
			}
			return list;
		}

		public List<LogMarketReportSpeciesMarket> GetLogMarketReportSpeciesMarkets()
		{

			var key = "LogMarketReportSpeciesMarkets";
			List<LogMarketReportSpeciesMarket> vals;

			if (!CacheHelper.Get(key, out vals))
			{
				vals = db.GetLogMarketReportSpeciesMarkets();
				CacheHelper.Add(vals, key);
			}
			return vals;
		}

		public List<int> GetCurrentStandSortYears()
		{
			var key = "CurrentStandSortYears";
			List<int> years;

			if (!CacheHelper.Get(key, out years))
			{
				years = db.GetCurrentStandSortYears();
				CacheHelper.Add(years, key);
			}
			return years;
		}

		public int GetMinYear()
		{
			var key = "MinYear";
			int minYear;

			if (!CacheHelper.Get(key, out minYear))
			{
				minYear = db.GetMinYear();
				CacheHelper.Add(minYear, key);
			}
			return minYear;
		}
	}
}
