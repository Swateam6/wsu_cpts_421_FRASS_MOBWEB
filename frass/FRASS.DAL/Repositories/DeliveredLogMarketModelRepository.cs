using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;
using FRASS.Interfaces;
using System.Data.Linq;

namespace FRASS.DAL.Repositories
{
	internal class DeliveredLogMarketModelRepository
	{
		private FRASSDataContext db;
		private DeliveredLogMarketModelRepository()
		{
			db = new FRASSDataContext();
		}
		public static DeliveredLogMarketModelRepository GetInstance()
		{
			return new DeliveredLogMarketModelRepository();
		}
		public List<MarketModelData> GetMarketModelData()
		{
			return (from cpippi in db.MarketModelDatas select cpippi).ToList<MarketModelData>();
		}
		public List<IModelShare> GetMarketModelPortfolios(User user)
		{
			var list = (from m in db.MarketModelPortfolios 
						join u in db.Users on m.UserID equals u.UserID
						join u2 in db.Users on m.CreatedByUserID equals u2.UserID
						where m.UserID == user.UserID select new ModelShare(
							m.MarketModelPortfolioID,
							m.PortfolioName,
							m.LastEdited,
							u.FirstName + " " + u.LastName,
							u2.FirstName + " " + u2.LastName)).ToList<IModelShare>();
			
			return list;
		}

		private class ModelShare : IModelShare
		{
			public ModelShare(int modelID, string name, DateTime lastEdited, string editor, string creator)
			{
				ModelID = modelID;
				PortfolioName = name;
				LastEdited = lastEdited;
				Creator = creator;
				Editor = editor;
			}

			public int ModelID { get; private set; }
			public string PortfolioName { get; private set; }
			public DateTime LastEdited { get; private set; }
			public string Creator { get; private set; }
			public string Editor { get; private set; }
		}

		public MarketModelPortfolio GetMarketModelPortfolio(Int32 MarketModelPortfolioID)
		{
			var model = (from m in db.MarketModelPortfolios where m.MarketModelPortfolioID == MarketModelPortfolioID select m).FirstOrDefault();
			return model;
		}
		public MarketModelPortfolio AddNewMarketModelPortfolio(MarketModelPortfolio marketModelPortfolio)
		{
			db.MarketModelPortfolios.InsertOnSubmit(marketModelPortfolio);
			db.SubmitChanges();
			return marketModelPortfolio;
		}

		public MarketModelPortfolio UpdateMarketModelPortfolio(MarketModelPortfolio marketModelPortfolio)
		{
			db.SubmitChanges();
			return marketModelPortfolio;
		}

		public MarketModelData GetMarketModelDataByID(Int32 marketModelDataID)
		{
			var c = (from cpippi in db.MarketModelDatas where cpippi.MarketModelDataID == marketModelDataID select cpippi).FirstOrDefault();
			return c;
		}

		public void UpdateCPIPPI(MarketModelData marketModelData)
		{
			var c = (from cp in db.MarketModelDatas where cp.MarketModelDataID == marketModelData.MarketModelDataID select cp).FirstOrDefault();
			if (c != null)
			{
				c.MarketModelTypeID = marketModelData.MarketModelTypeID;
				c.Year = marketModelData.Year;
				c.Period = marketModelData.Period;
				c.SeriesID = marketModelData.SeriesID;
				c.Value = marketModelData.Value;
				db.SubmitChanges();
			}
		}
		public void AddNewMarketModelData(MarketModelData marketModelData)
		{
			var cCheck = (from cp in db.MarketModelDatas where cp.Year == marketModelData.Year && cp.SeriesID == marketModelData.SeriesID && cp.MarketModelTypeID == marketModelData.MarketModelTypeID && cp.Period == marketModelData.Period select cp).FirstOrDefault();
			if (cCheck == null)
			{
				MarketModelData c = new MarketModelData();
				c.MarketModelTypeID = marketModelData.MarketModelTypeID;
				c.Year = marketModelData.Year;
				c.Period = marketModelData.Period;
				c.SeriesID = marketModelData.SeriesID;
				c.Value = marketModelData.Value;
				db.MarketModelDatas.InsertOnSubmit(c);
				db.SubmitChanges();
			}
		}

		public void DeleteMarketModelData(int marketModelDataID)
		{
			var c = from cp in db.MarketModelDatas where cp.MarketModelDataID == marketModelDataID select cp;
			db.MarketModelDatas.DeleteAllOnSubmit(c);
			db.SubmitChanges();
		}

		public void TruncateMarketModelData()
		{
			db.ExecuteCommand("Truncate Table MarketModelData");
		}

		public void InsertMarketModelData(List<MarketModelData> marketModelData)
		{
			foreach (var data in marketModelData.OrderBy(uu => uu.Year).ThenBy(uu => uu.Period))
			{
				MarketModelData c = new MarketModelData();
				c.MarketModelTypeID = data.MarketModelTypeID;
				c.Year = data.Year;
				c.Period = data.Period;
				c.SeriesID = data.SeriesID;
				c.Value = data.Value;
				db.MarketModelDatas.InsertOnSubmit(c);
			}
			db.SubmitChanges();
		}

		public void DeleteMarketModelPortfolio(MarketModelPortfolio marketModelPortfolio)
		{
			var cost = from c in db.MarketModelPortfolioCosts where c.MarketModelPortfolioID == marketModelPortfolio.MarketModelPortfolioID select c;
			db.MarketModelPortfolioCosts.DeleteAllOnSubmit(cost);

			var infl = from i in db.MarketModelPortfolioInflationDetails where i.MarketModelPortfolioID == marketModelPortfolio.MarketModelPortfolioID select i;
			db.MarketModelPortfolioInflationDetails.DeleteAllOnSubmit(infl);

			var log = from l in db.MarketModelPortfolioDeliveredLogModelDetails where l.MarketModelPortfolioID == marketModelPortfolio.MarketModelPortfolioID select l;
			db.MarketModelPortfolioDeliveredLogModelDetails.DeleteAllOnSubmit(log);

			var reports = from r in db.Reports where r.MarketModelPortfolioID == marketModelPortfolio.MarketModelPortfolioID select r;
			db.Reports.DeleteAllOnSubmit(reports);

			var portfolio = from cp in db.MarketModelPortfolios where cp.MarketModelPortfolioID == marketModelPortfolio.MarketModelPortfolioID select cp;
			db.MarketModelPortfolios.DeleteAllOnSubmit(portfolio);
			db.SubmitChanges();
		}
	}
}
