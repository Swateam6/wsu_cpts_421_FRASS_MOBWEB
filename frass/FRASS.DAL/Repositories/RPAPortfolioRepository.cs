using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;
using FRASS.Interfaces;

namespace FRASS.DAL.Repositories
{
	internal class RPAPortfolioRepository
	{
		private FRASSDataContext db;
		private RPAPortfolioRepository()
		{
			db = new FRASSDataContext();
		}
		public static RPAPortfolioRepository GetInstance()
		{
			return new RPAPortfolioRepository();
		}
		public RPAPortfolio AddNewRPAPortfolio(RPAPortfolio rpaPortfolio)
		{
			db.RPAPortfolios.InsertOnSubmit(rpaPortfolio);
			db.SubmitChanges();
			return rpaPortfolio;
		}
		public RPAPortfolio UpdateRPAPortfolio(RPAPortfolio rpaPortfolio)
		{
			db.SubmitChanges();
			return rpaPortfolio;
		}
		public void DeleteRPAPortfolio(RPAPortfolio rpaPortfolio)
		{
			var detail = from d in db.RPAPortfolioDetails where d.RPAPortfolioID == rpaPortfolio.RPAPortfolioID select d;
			db.RPAPortfolioDetails.DeleteAllOnSubmit(detail);

			var portfolio = from cp in db.RPAPortfolios where cp.RPAPortfolioID == rpaPortfolio.RPAPortfolioID select cp;
			db.RPAPortfolios.DeleteAllOnSubmit(portfolio);
			db.SubmitChanges();
		}
		public RPAPortfolio GetRPAPortfolio(Int32 rpaPortfolioID)
		{
			var model = (from m in db.RPAPortfolios where m.RPAPortfolioID == rpaPortfolioID select m).FirstOrDefault();
			return model;
		}

		public RPAPortfolio GetNoRPAPortfolio()
		{
			var model = (from m in db.RPAPortfolios select m).FirstOrDefault();
			return model;
		}

		public List<IModelShare> GetRPAPortfolios(User user)
		{
			var list = (from m in db.RPAPortfolios
						join u in db.Users on m.UserID equals u.UserID
						join u2 in db.Users on m.CreatedByUserID equals u2.UserID
						where m.UserID == user.UserID
						select new ModelShare(
							m.RPAPortfolioID,
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
	}
}
