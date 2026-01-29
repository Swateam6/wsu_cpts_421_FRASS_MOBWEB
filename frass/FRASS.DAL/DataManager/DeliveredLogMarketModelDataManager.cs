using System;
using System.Collections.Generic;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;

namespace FRASS.DAL.DataManager
{
	public class DeliveredLogMarketModelDataManager
	{
		DeliveredLogMarketModelRepository db;
		private DeliveredLogMarketModelDataManager()
		{
			db = DeliveredLogMarketModelRepository.GetInstance();
		}
		public static DeliveredLogMarketModelDataManager GetInstance()
		{
			return new DeliveredLogMarketModelDataManager();
		}

		public void TruncateMarketModelData()
		{
			db.TruncateMarketModelData();
			CacheHelper.Clear("GetMarketModelData");
		}

		public void InsertMarketModelData(List<MarketModelData> marketModelData)
		{
			db.InsertMarketModelData(marketModelData);
			CacheHelper.Clear("GetMarketModelData");
		}

		public List<MarketModelData> GetMarketModelData()
		{
			var key = "GetMarketModelData";
			List<MarketModelData> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetMarketModelData();
				CacheHelper.Add(list, key);
			}

			return list;
		}
		public List<IModelShare> GetMarketModelPortfolios(User user)
		{
			var key = "GetMarketModelPortfolios_" + user.UserID.ToString();
			List<IModelShare> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetMarketModelPortfolios(user);
				CacheHelper.Add(list, key);
			}
			return list;
		}

		public MarketModelPortfolio GetMarketModelPortfolio(Int32 MarketModelPortfolioID)
		{
			return db.GetMarketModelPortfolio(MarketModelPortfolioID);
		}
		public MarketModelPortfolio AddNewMarketModelPortfolio(MarketModelPortfolio marketModelPortfolio)
		{
			db.AddNewMarketModelPortfolio(marketModelPortfolio);
			CacheHelper.Clear("GetMarketModelPortfolios_" + marketModelPortfolio.UserID.ToString());
			return marketModelPortfolio;
		}

		public MarketModelPortfolio UpdateMarketModelPortfolio(MarketModelPortfolio marketModelPortfolio)
		{
			db.UpdateMarketModelPortfolio(marketModelPortfolio);
			CacheHelper.Clear("GetMarketModelPortfolios_" + marketModelPortfolio.UserID.ToString());
			return marketModelPortfolio;
		}

		public MarketModelData GetMarketModelDataByID(Int32 marketModelDataID)
		{
			return db.GetMarketModelDataByID(marketModelDataID);
		}

		public void UpdateCPIPPI(MarketModelData marketModelData)
		{
			CacheHelper.Clear("GetMarketModelData");
			db.UpdateCPIPPI(marketModelData);
		}
		public void AddNewMarketModelData(MarketModelData marketModelData)
		{
			CacheHelper.Clear("GetMarketModelData");
			db.AddNewMarketModelData(marketModelData);
		}

		public void DeleteMarketModelData(int marketModelDataID)
		{
			CacheHelper.Clear("GetMarketModelData");
			db.DeleteMarketModelData(marketModelDataID);
		}

		public void DeleteMarketModelPortfolio(MarketModelPortfolio marketModelPortfolio)
		{
			CacheHelper.Clear("GetMarketModelPortfolios_" + marketModelPortfolio.UserID.ToString());
			db.DeleteMarketModelPortfolio(marketModelPortfolio);
		}

		public void CopyModel(int portfolioID, int userID)
		{
			var p = GetMarketModelPortfolio(portfolioID);
			var m = new MarketModelPortfolio();
			m.CreatedByUserID = p.UserID;
			m.UserID = userID;
			m.LastEdited = p.LastEdited;
			m.PortfolioName = p.PortfolioName;
			
			db.AddNewMarketModelPortfolio(m);

			var c = new MarketModelPortfolioCost();
			c.AccessFeeRock = p.MarketModelPortfolioCosts.AccessFeeRock;
			c.AccessFeeTimber = p.MarketModelPortfolioCosts.AccessFeeTimber;
			c.MaintenanceFeeRockHaul = p.MarketModelPortfolioCosts.MaintenanceFeeRockHaul;
			c.MaintenanceFeeTimberHaul = p.MarketModelPortfolioCosts.MaintenanceFeeTimberHaul;
			c.MarketModelPortfolioID = m.MarketModelPortfolioID;
			c.ReforestationCost = p.MarketModelPortfolioCosts.ReforestationCost;
			c.RoadConstructionCosts = p.MarketModelPortfolioCosts.RoadConstructionCosts;
			m.MarketModelPortfolioCosts = c;

			foreach (var dm in p.MarketModelPortfolioDeliveredLogModelDetails)
			{
				var d = new MarketModelPortfolioDeliveredLogModelDetail();
				d.CurrentNetValue = dm.CurrentNetValue;
				d.DeliveredLogPrice = dm.DeliveredLogPrice;
				d.HaulingCosts = dm.HaulingCosts;
				d.LoggingCosts = dm.LoggingCosts;
				d.LogMarketReportSpeciesID = dm.LogMarketReportSpeciesID;
				d.MarketModelPortfolioID = m.MarketModelPortfolioID;
				d.Notes = dm.Notes;
				d.OverheadAndAdmin = dm.OverheadAndAdmin;
				d.ProfitAndRisk = dm.ProfitAndRisk;
				d.TimberMarketID = dm.TimberMarketID;
				m.MarketModelPortfolioDeliveredLogModelDetails.Add(d);
			}

			var id = new MarketModelPortfolioInflationDetail();
			id.BeginningYear = p.MarketModelPortfolioInflationDetails.BeginningYear;
			id.EndingYear = p.MarketModelPortfolioInflationDetails.EndingYear;
			id.InflationRate = p.MarketModelPortfolioInflationDetails.InflationRate;
			id.LandownerDiscountRate = p.MarketModelPortfolioInflationDetails.LandownerDiscountRate;
			id.MarketModelPortfolioID = p.MarketModelPortfolioID;
			m.MarketModelPortfolioInflationDetails = id;
			db.UpdateMarketModelPortfolio(m);


			CacheHelper.Clear("GetMarketModelPortfolios_" + userID.ToString());
		}
	}
}
