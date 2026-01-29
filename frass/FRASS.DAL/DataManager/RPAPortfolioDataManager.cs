using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;

namespace FRASS.DAL.DataManager
{
	public class RPAPortfolioDataManager
	{
		RPAPortfolioRepository db;
		private RPAPortfolioDataManager()
		{
			db = RPAPortfolioRepository.GetInstance();
		}
		public static RPAPortfolioDataManager GetInstance()
		{
			return new RPAPortfolioDataManager();
		}
		public RPAPortfolio AddNewRPAPortfolio(RPAPortfolio rpaPortfolio)
		{
			db.AddNewRPAPortfolio(rpaPortfolio);
			CacheHelper.Clear("GetRPAPortfolios_" + rpaPortfolio.UserID.ToString());
			return rpaPortfolio;
		}

		public RPAPortfolio UpdateRPAPortfolio(RPAPortfolio rpaPortfolio)
		{
			if (rpaPortfolio.RPAPortfolioID == -1)
			{
				return rpaPortfolio;
			}
			db.UpdateRPAPortfolio(rpaPortfolio);
			CacheHelper.Clear("GetRPAPortfolios_" + rpaPortfolio.UserID.ToString());
			return rpaPortfolio;
		}
		public void DeleteRPAPortfolio(RPAPortfolio rpaPortfolio)
		{
			if (rpaPortfolio.RPAPortfolioID != -1)
			{
				CacheHelper.Clear("GetRPAPortfolios_" + rpaPortfolio.UserID.ToString());
				db.DeleteRPAPortfolio(rpaPortfolio);
			}
		}
		public RPAPortfolio GetRPAPortfolio(Int32 rpaPortfolioID)
		{
			if (rpaPortfolioID == -1)
			{
				return GetNoRPAPortfolio();
			}
			return db.GetRPAPortfolio(rpaPortfolioID);
		}
		private RPAPortfolio GetNoRPAPortfolio()
		{
			var p = db.GetNoRPAPortfolio();
			p.PortfolioName = "No RPA Portfolio";
			foreach(var d in p.RPAPortfolioDetails)
			{
				d.RPA = decimal.Zero;
				d.Longevity = 1m;
				d.RPAPortfolioID = -1;
			}
			p.RPAPortfolioID = -1;
			return p;
		}
		public List<IModelShare> GetRPAPortfolios(User user)
		{
			var key = "GetRPAPortfolios_" + user.UserID.ToString();
			List<IModelShare> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetRPAPortfolios(user);
				CacheHelper.Add(list, key);
			}
			return list;
		}

		public void CopyModel(int portfolioID, int userID)
		{
			var p = GetRPAPortfolio(portfolioID);
			var m = new RPAPortfolio();
			m.CreatedByUserID = p.UserID;
			m.UserID = userID;
			m.LastEdited = DateTime.Now;
			m.PortfolioName = p.PortfolioName;
			db.AddNewRPAPortfolio(m);
			foreach (var pd in p.RPAPortfolioDetails)
			{
				var d = new RPAPortfolioDetail();
				d.BeginningDate = pd.BeginningDate;
				d.BeginningRealValue = pd.BeginningRealValue;
				d.EndingDate = pd.EndingDate;
				d.EndingRealValue = pd.EndingRealValue;
				d.LogMarketReportSpeciesID = pd.LogMarketReportSpeciesID;
				d.Longevity = pd.Longevity;
				d.RPA = pd.RPA;
				d.RPAPortfolioID = m.RPAPortfolioID;
				d.TimberMarketID = pd.TimberMarketID;
				m.RPAPortfolioDetails.Add(d);
			}
			db.UpdateRPAPortfolio(m);
		}
	}
}
