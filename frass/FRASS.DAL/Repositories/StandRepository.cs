using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;
using FRASS.Interfaces;
using System.Data.Linq;

namespace FRASS.DAL.Repositories
{
	internal class StandRepository
	{
		private FRASSDataContext db;
		private StandRepository()
		{
			db = new FRASSDataContext();
		}
		public static StandRepository GetInstance()
		{
			return new StandRepository();
		}

		public List<IStandData> GetStandDataCurrent(int standid, int parcelid)
		{
			var data = from pr in db.ParcelRiparians
					   join tcs in db.TimberCurrentStands on pr.STD_ID equals tcs.STD_ID
					   join tcss in db.TimberCurrentStandSorts on tcs.TimberCurrentStandID equals tcss.TimberCurrentStandID
					   join tg in db.TimberGrades on tcss.TimberGradeID equals tg.TimberGradeID
					   join tm in db.TimberMarkets on tg.TimberMarketID equals tm.TimberMarketID
					   join s in db.Species on tg.SpeciesID equals s.SpeciesID
					   join lss in db.LogMarketReportSpeciesSpecies on s.SpeciesID equals lss.SpeciesID
					   join ls in db.LogMarketReportSpecies on lss.LogMarketReportSpeciesID equals ls.LogMarketReportSpeciesID
					   where pr.STD_ID == standid && pr.ParcelID == parcelid && pr.StandStatID  == Convert.ToInt32(StandStats.Operable)
					   select new StandData()
					   {
						   ReportYear = tcss.ReportYear,
						   LogMarketReportSpeciesID = ls.LogMarketReportSpeciesID,
						   LogMarketSpecies = ls.LogMarketSpecies,
						   TimberMarketID = tm.TimberMarketID,
						   Market = tm.Market,
						   SpeciesID = s.SpeciesID,
						   Abbreviation = s.Abbreviation,
						   SortCode = tg.SortCode,
						   TimberGradeID = tg.TimberGradeID,
						   Board_SN = tcss.Board_SN,
						   PctBrd = tcss.PctBrd,
						   Acres = pr.Acres,
						   OrderID = tm.OrderID.Value
					   };
			return data.ToList<IStandData>();
		}

		public List<IStandData> GetStandDataFuture(int standid, int parcelid)
		{
			var data = from pr in db.ParcelRiparians
					   join tcs in db.TimberCurrentStands on pr.STD_ID equals tcs.STD_ID
					   join tfs in db.TimberFutureStands on tcs.Yield_Index equals tfs.Yield_Index
					   join tfss in db.TimberFutureStandSorts on tfs.TimberFutureStandID equals tfss.TimberFutureStandID
					   join tg in db.TimberGrades on tfss.TimberGradeID equals tg.TimberGradeID
					   join tm in db.TimberMarkets on tg.TimberMarketID equals tm.TimberMarketID
					   join s in db.Species on tg.SpeciesID equals s.SpeciesID
					   join lss in db.LogMarketReportSpeciesSpecies on s.SpeciesID equals lss.SpeciesID
					   join ls in db.LogMarketReportSpecies on lss.LogMarketReportSpeciesID equals ls.LogMarketReportSpeciesID
					   where pr.STD_ID == standid && pr.ParcelID == parcelid && pr.StandStatID == Convert.ToInt32(StandStats.Operable)
                       select new StandData()
					   {
						   ReportYear = tfs.ReportYears,
						   LogMarketReportSpeciesID = ls.LogMarketReportSpeciesID,
						   LogMarketSpecies = ls.LogMarketSpecies,
						   TimberMarketID = tm.TimberMarketID,
						   Market = tm.Market,
						   SpeciesID = s.SpeciesID,
						   Abbreviation = s.Abbreviation,
						   SortCode = tg.SortCode,
						   TimberGradeID = tg.TimberGradeID,
						   Board_SN = tfss.Board_SN,
						   PctBrd = tfss.PctBrd,
						   Acres = pr.Acres,
						   OrderID = tm.OrderID.Value
					   };
			return data.ToList<IStandData>();
		}

		public List<LogMarketReportSpeciesMarket> GetLogMarketReportSpeciesMarkets()
		{
			return (from lm in db.LogMarketReportSpeciesMarkets select lm).ToList();
		}

		public List<int> GetCurrentStandSortYears()
		{
			return (from tcss in db.TimberCurrentStandSorts select tcss.ReportYear).Distinct().OrderBy(uu => uu).ToList();
		}

		public int GetMinYear()
		{
			return (from tcss in db.TimberCurrentStandSorts select tcss.ReportYear).Min();
		}

		private class StandData : IStandData
		{
			public int ReportYear { get; set; }
			public int LogMarketReportSpeciesID { get; set; }
			public string LogMarketSpecies { get; set; }
			public int TimberMarketID { get; set; }
			public string Market { get; set; }
			public int OrderID { get; set; }
			public int SpeciesID { get; set; }
			public string Abbreviation { get; set; }
			public string SortCode { get; set; }
			public int TimberGradeID { get; set; }
			public decimal Board_SN { get; set; }
			public decimal PctBrd { get; set; }
			public decimal Acres { get; set; }
		}
	}
}
