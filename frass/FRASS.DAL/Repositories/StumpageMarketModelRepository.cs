using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;
using FRASS.Interfaces;
using System.Data.Linq;

namespace FRASS.DAL.Repositories
{
	internal class StumpageMarketModelRepository
	{
		private FRASSDataContext db;
		private StumpageMarketModelRepository()
		{
			db = new FRASSDataContext();
		}
		public static StumpageMarketModelRepository GetInstance()
		{
			return new StumpageMarketModelRepository();
		}

		public List<StumpageGroup> GetStumpageGroups()
		{
			return (from d in db.StumpageGroups select d).ToList<StumpageGroup>();
		}
		public List<StumpageGroupQualityCode> GetStumpageGroupQualityCodes()
		{
			return (from d in db.StumpageGroupQualityCodes select d).ToList<StumpageGroupQualityCode>();
		}
		public List<StumpagePrice> GetStumpagePrices()
		{
			return (from p in db.StumpagePrices select p).ToList<StumpagePrice>();
		}
		public void DeleteStumpagePrice(StumpagePrice stumpagePrice)
		{
			var p = from sp in db.StumpagePrices where sp.StumpagePricesID == stumpagePrice.StumpagePricesID select sp;
			db.StumpagePrices.DeleteAllOnSubmit(p);
			db.SubmitChanges();
		}
		public StumpagePrice GetStumpagePrice(int stumpagePricesID)
		{
			var price = (from p in db.StumpagePrices where p.StumpagePricesID == stumpagePricesID select p).FirstOrDefault();
			return price;
		}
		public StumpageGroup GetStumpageGroup(int stumpageGroupID)
		{
			var s = (from g in db.StumpageGroups where g.StumpageGroupID == stumpageGroupID select g).FirstOrDefault();
			return s;
		}

		public List<StumpageModelPortfolio> GetStumpageModelPortfolios(User user)
		{
			List<StumpageModelPortfolio> list;
			list = (from m in db.StumpageModelPortfolios where m.UserID == user.UserID select m).ToList<StumpageModelPortfolio>();
			var portfolios = (from m in db.StumpageModelPortfolioShares where m.UserID == user.UserID select m.StumpageModelPortfolio).ToList<StumpageModelPortfolio>();
			list.AddRange(portfolios);
			return list;
		}

		public StumpageModelPortfolio GetStumpageModelPortfolio(Int32 stumpageModelPortfolioID)
		{
			var model = (from m in db.StumpageModelPortfolios where m.StumpageModelPortfolioID == stumpageModelPortfolioID select m).FirstOrDefault();
			return model;
		}
		public StumpageModelPortfolio AddNewStumpageModelPortfolio(StumpageModelPortfolio stumpageModelPortfolio)
		{
			db.StumpageModelPortfolios.InsertOnSubmit(stumpageModelPortfolio);
			db.SubmitChanges();
			return stumpageModelPortfolio;
		}
		public StumpageModelPortfolio UpdateStumpageModelPortfolio(StumpageModelPortfolio stumpageModelPortfolio)
		{
			db.SubmitChanges();
			return stumpageModelPortfolio;
		}

		public void DeleteStumpageModelPortfolio(StumpageModelPortfolio stumpageModelPortfolio)
		{
			var shares = from s in db.StumpageModelPortfolioShares where s.StumpageModelPortfolioID == stumpageModelPortfolio.StumpageModelPortfolioID select s;
			db.StumpageModelPortfolioShares.DeleteAllOnSubmit(shares);

			var costs = from cost in db.StumpageModelPortfolioCosts where cost.StumpageModelPortfolioID == stumpageModelPortfolio.StumpageModelPortfolioID select cost;
			db.StumpageModelPortfolioCosts.DeleteAllOnSubmit(costs);

			var inflation = from infl in db.StumpageModelPortfolioInflationDetails where infl.StumpageModelPortfolioID == stumpageModelPortfolio.StumpageModelPortfolioID select infl;
			db.StumpageModelPortfolioInflationDetails.DeleteAllOnSubmit(inflation);

			var rpa = from r in db.StumpageModelPortfolioRPADatas where r.StumpageModelPortfolioID == stumpageModelPortfolio.StumpageModelPortfolioID select r;
			db.StumpageModelPortfolioRPADatas.DeleteAllOnSubmit(rpa);

			var val = from v in db.StumpageModelPortfolioValues where v.StumpageModelPortfolioID == stumpageModelPortfolio.StumpageModelPortfolioID select v;
			db.StumpageModelPortfolioValues.DeleteAllOnSubmit(val);

			var reports = from r in db.Reports where r.StumpageModelPortfolioID == stumpageModelPortfolio.StumpageModelPortfolioID select r;
			db.Reports.DeleteAllOnSubmit(reports);

			var portfolio = from cp in db.StumpageModelPortfolios where cp.StumpageModelPortfolioID == stumpageModelPortfolio.StumpageModelPortfolioID select cp;
			db.StumpageModelPortfolios.DeleteAllOnSubmit(portfolio);
			db.SubmitChanges();
		}
		public void AddStumpagePrice(StumpagePrice price)
		{
			db.StumpagePrices.InsertOnSubmit(price);
			db.SubmitChanges();
		}
		public void UpdateStumpagePrice(StumpagePrice price)
		{
			db.SubmitChanges();
		}

		public List<StumpageModelPortfolioShare> GetStumpageModelPortfolioShares(int stumpageModelPortfolioShareID)
		{
			var s = from m in db.StumpageModelPortfolioShares where m.StumpageModelPortfolioShareID == stumpageModelPortfolioShareID select m;
			return s.ToList<StumpageModelPortfolioShare>();
		}
		public List<User> AddStumpageModelPortfolioShares(StumpageModelPortfolio stumpageMarketModelPortfolio, List<StumpageModelPortfolioShare> shares)
		{
			var pshares = stumpageMarketModelPortfolio.StumpageModelPortfolioShares;
			var deletes = from p in pshares where !(from s in shares select s.UserID).Contains(p.UserID) select p;
			var inserts = from s in shares where !(from p in pshares select p.UserID).Contains(s.UserID) select s;
			db.StumpageModelPortfolioShares.DeleteAllOnSubmit(deletes);
			db.StumpageModelPortfolioShares.InsertAllOnSubmit(inserts);
			db.SubmitChanges();
			var users = (from u in db.Users where (from i in inserts select i.UserID).Contains(u.UserID) select u).ToList<User>();
			return users;
		}

		public List<StumpageModelPortfolioMultiplier> GetStumpageModelPortfolioMultipliers()
		{
			var m = from pm in db.StumpageModelPortfolioMultipliers select pm;
			return m.ToList<StumpageModelPortfolioMultiplier>();
		}
	}
}
