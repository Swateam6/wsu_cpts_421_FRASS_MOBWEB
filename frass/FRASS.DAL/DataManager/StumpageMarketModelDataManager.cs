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
	public class StumpageMarketModelDataManager
	{
		StumpageMarketModelRepository db;
		private StumpageMarketModelDataManager()
		{
			db = StumpageMarketModelRepository.GetInstance();
		}
		public static StumpageMarketModelDataManager GetInstance()
		{
			return new StumpageMarketModelDataManager();
		}

		public List<StumpageGroup> GetStumpageGroups()
		{
			var key = "GetStumpageGroups";
			List<StumpageGroup> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetStumpageGroups();
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public List<StumpageGroupQualityCode> GetStumpageGroupQualityCodes()
		{
			var key = "GetStumpageGroupQualityCodes";
			List<StumpageGroupQualityCode> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetStumpageGroupQualityCodes();
				CacheHelper.Add(list, key);
			}
			return list;
		}
		public List<StumpagePrice> GetStumpagePrices()
		{
			var key = "GetStumpagePrices";
			List<StumpagePrice> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetStumpagePrices();
				CacheHelper.Add(list, key);
			}

			return list;
		}
		public void DeleteStumpagePrice(StumpagePrice stumpagePrice)
		{
			db.DeleteStumpagePrice(stumpagePrice);
			CacheHelper.Clear("GetStumpagePrices");
		}
		public StumpagePrice GetStumpagePrice(int stumpagePricesID)
		{
			return db.GetStumpagePrice(stumpagePricesID);
		}
		public StumpageGroup GetStumpageGroup(int stumpageGroupID)
		{
			return db.GetStumpageGroup(stumpageGroupID);
		}

		public List<StumpageModelPortfolio> GetStumpageModelPortfolios(User user)
		{
			return db.GetStumpageModelPortfolios(user);
		}

		public StumpageModelPortfolio GetStumpageModelPortfolio(Int32 stumpageModelPortfolioID)
		{
			return db.GetStumpageModelPortfolio(stumpageModelPortfolioID);
		}
		public StumpageModelPortfolio AddNewStumpageModelPortfolio(StumpageModelPortfolio stumpageModelPortfolio)
		{
			return db.AddNewStumpageModelPortfolio(stumpageModelPortfolio);
		}
		public StumpageModelPortfolio UpdateStumpageModelPortfolio(StumpageModelPortfolio stumpageModelPortfolio)
		{
			return db.UpdateStumpageModelPortfolio(stumpageModelPortfolio);
		}

		public void DeleteStumpageModelPortfolio(StumpageModelPortfolio stumpageModelPortfolio)
		{
			db.DeleteStumpageModelPortfolio(stumpageModelPortfolio);
		}
		public void AddStumpagePrice(StumpagePrice price)
		{
			db.AddStumpagePrice(price);
			CacheHelper.Clear("GetStumpagePrices");
		}
		public void UpdateStumpagePrice(StumpagePrice price)
		{
			CacheHelper.Clear("GetStumpagePrices");
			db.UpdateStumpagePrice(price);
		}
		public List<StumpageModelPortfolioShare> GetStumpageModelPortfolioShares(int stumpageModelPortfolioShareID)
		{
			return db.GetStumpageModelPortfolioShares(stumpageModelPortfolioShareID);
		}
		public List<User> AddStumpageModelPortfolioShares(StumpageModelPortfolio stumpageMarketModelPortfolio, List<StumpageModelPortfolioShare> shares)
		{
			return db.AddStumpageModelPortfolioShares(stumpageMarketModelPortfolio, shares);
		}
		public List<StumpageModelPortfolioMultiplier> GetStumpageModelPortfolioMultipliers()
		{
			var key = "GetStumpageModelPortfolioMultipliers";
			List<StumpageModelPortfolioMultiplier> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetStumpageModelPortfolioMultipliers();
				CacheHelper.Add(list, key);
			}
			return list;
		}
	}
}
