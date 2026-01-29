using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;

namespace FRASS.DAL.DataManager
{
	public class TimberDataManager
	{
		TimberRepository db;
		private TimberDataManager()
		{
			db = TimberRepository.GetInstance();
		}
		public static TimberDataManager GetInstance()
		{
			return new TimberDataManager();
		}

		public void UpdateSpecies(Specy species)
		{
			db.UpdateSpecies(species);
		}
		public void AddNewSpecies(Specy species)
		{
			db.AddNewSpecies(species);
		}
		public Specy GetSpecies(Int32 speciesID)
		{
			return db.GetSpecies(speciesID);
		}

		public List<Specy> GetSpecies()
		{
			var key = "AllSpecies";
			var list = new List<Specy>();
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetSpecies();
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public List<LogMarketReportSpecy> GetLogMarketReportSpecies()
		{
			var key = "LogMarketReportSpecies";
			var list = new List<LogMarketReportSpecy>();
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetLogMarketReportSpecies();
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public LogMarketReportSpecy GetLogMarketReportSpecies(Int32 logMarketReportSpeciesID)
		{
			var key = "TimberGrades_" + logMarketReportSpeciesID.ToString();
			LogMarketReportSpecy log;
			if (!CacheHelper.Get(key, out log))
			{
				log = db.GetLogMarketReportSpecies(logMarketReportSpeciesID);
				CacheHelper.Add(log, key);
			}
			return log;
		}
		public List<TimberGrade> GetTimberGrades()
		{
			var key = "TimberGrades";
			var list = new List<TimberGrade>();
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetTimberGrades();
			}
			CacheHelper.Add(list, key);
			return list;
		}
		public List<TimberMarket> GetTimberMarkets()
		{
			var key = "TimberMarkets";
			var list = new List<TimberMarket>();
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetTimberMarkets();
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public List<v_HistoricLogPrice> GetHistoricLogPrice()
		{
			var key = "v_HistoricLogPrices";
			var list = new List<v_HistoricLogPrice>();
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetHistoricLogPrice();
				CacheHelper.Add(list, key);

			}
			return list;
		}
		public void AddHistoricLogPrices(List<HistoricLogPrice> prices)
		{
			db.AddHistoricLogPrices(prices);
			CacheHelper.Clear("v_HistoricLogPrices");
		}
		public void AddHistoricLogPrice(IHistoricPrice price)
		{
			db.AddHistoricLogPrice(price);
			CacheHelper.Clear("v_HistoricLogPrices");
		}
		public void DeleteHistoricLogPrice(IQueryable<HistoricLogPrice> prices)
		{
			db.DeleteHistoricLogPrice(prices.ToList<HistoricLogPrice>());
			CacheHelper.Clear("v_HistoricLogPrices");
		}
		public void EditHistoricLogPrices(IHistoricPrice prices)
		{
			db.EditHistoricLogPrices(prices);
			CacheHelper.Clear("v_HistoricLogPrices");
		}
		public IQueryable<HistoricLogPrice> GetHistoricLogPrice(Int32 year, Int32 month, Int32 LogMarketReportSpeciesID)
		{
			return db.GetHistoricLogPrice(year, month, LogMarketReportSpeciesID);
		}

		public IQueryable<HistoricLogPrice> GetHistoricLogPrices(Int32 LogMarketReportSpeciesID, Int32 TimberMarketID)
		{
			return db.GetHistoricLogPrices(LogMarketReportSpeciesID, TimberMarketID);
		}
	}
}
