using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.BLL.Formulas;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Interfaces;

namespace FRASS.BLL.Models
{
	public class R2
	{
		public decimal Acres;
		public EconVariables EconVariables;
		public List<R2Item> R2Items;
		public MarketModelPortfolio Portfolio;
		public RPAPortfolio RPAPortfolio;
		public Parcel Parcel;
		private TimberDataManager _dbTimberDataManager;
		private TimberDataManager dbTimberDataManager
		{
			get
			{
				if (_dbTimberDataManager == null)
				{
					_dbTimberDataManager = TimberDataManager.GetInstance();
				}
				return _dbTimberDataManager;
			}
		}
		private StandDataManager _dbStandDataManager;
		private StandDataManager dbStandDataManager
		{
			get
			{
				if (_dbStandDataManager == null)
				{
					_dbStandDataManager = StandDataManager.GetInstance();
				}
				return _dbStandDataManager;
			}
		}

		private DeliveredLogMarketModelDataManager _dbDeliveredLogMarketModel;
		private DeliveredLogMarketModelDataManager dbDeliveredLogMarketModel
		{
		get
			{
				if (_dbDeliveredLogMarketModel == null)
				{
					_dbDeliveredLogMarketModel = DeliveredLogMarketModelDataManager.GetInstance();
				}
				return _dbDeliveredLogMarketModel;
			}
		}

		private RPARealValue _rpaREAL;
		private RPARealValue RPAREAL
		{
			get { return _rpaREAL ?? (_rpaREAL = new RPARealValue()); }
		}
		private List<MarketModelData> _mmd;
		private List<MarketModelData> MarketModelDataList
		{
			get { return _mmd ?? (_mmd = dbDeliveredLogMarketModel.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).ToList<MarketModelData>()); }
		}
		private decimal? _ppiToday;
		private decimal ppiToday
		{
			get
			{
				if (!_ppiToday.HasValue)
				{
					_ppiToday = MarketModelDataList.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault().Value;
				}
				return _ppiToday.Value;
			}
		}

		public R2(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio,  Parcel parcel, int standid, EconVariables econVariables)
		{
			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Parcel = parcel;
			EconVariables = econVariables;
			R2Items = new List<R2Item>();
			var sd = dbStandDataManager.GetStandDataFuture(standid, parcel.ParcelID);
			Acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.Acres).DefaultIfEmpty(0M).FirstOrDefault();
			foreach (var standdata in sd)
			{
				var volume = standdata.Board_SN * standdata.PctBrd;
				var item = new R2Item(portfolio, rpaPortfolio, standdata.LogMarketReportSpeciesID, standdata.TimberMarketID, standdata.TimberGradeID, volume, standdata.ReportYear, standdata.OrderID, standdata.Acres);
				R2Items.Add(item);
			}
		}
		public R2(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, Parcel parcel, int standid, List<R2Item> r2Items, EconVariables econVariables)
		{
			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Parcel = parcel;
			EconVariables = econVariables;
			R2Items =r2Items;
			Acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.Acres).DefaultIfEmpty(0M).FirstOrDefault();
		}
		public Decimal GetAccessFeePerAcre(int year, decimal totalVolume)
		{
			var value = 0M;
			var powerInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation)), year));
			var miles = GetTotalMileage();
			value = EconVariables.AccessFee * totalVolume * miles / 1000M * powerInflation;
			return value;
		}
		public Decimal GetMaintenanceFeePerAcre(int year, decimal totalVolume)
		{
			var value = 0M;
			var powerInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation)), year));
			var miles = GetTotalMileage();
			value = EconVariables.MaintenanceFee * totalVolume * miles / 1000M * powerInflation;
			return value;
		}
		public Decimal GetReforestationFeePerAcre(int year)
		{
			var value = 0M;
			var powerInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation)), year));
			value = Convert.ToDecimal(EconVariables.ReforestionCosts) * powerInflation;
			return value;
		}

		private List<TimberMarket> _TimberMarkets;
		private List<TimberMarket> TimberMarkets
		{
			get
			{
				if (_TimberMarkets == null)
				{
					_TimberMarkets = dbTimberDataManager.GetTimberMarkets().ToList();
				}
				return _TimberMarkets;
			}
		}
		public decimal GetMaxValue(int year, int logMarketReportSpeciesID, int timberMarketID)
		{
			var orderid = (from tm in TimberMarkets where tm.TimberMarketID == timberMarketID select tm.OrderID).FirstOrDefault();
			var items = (from lm in dbStandDataManager.GetLogMarketReportSpeciesMarkets() where lm.LogMarketReportSpeciesID == logMarketReportSpeciesID && lm.TimberMarket.OrderID >= orderid select lm).ToList();

			var value = 0M;
			foreach (var item in items)
			{
				var val = ValuesFuture.Value(Portfolio, RPAPortfolio, year, logMarketReportSpeciesID, item.TimberMarketID, MarketModelDataList, RPAREAL, ppiToday);
				if (val > value)
				{
					value = val;
				}
			}
			return value;
		}
		public decimal GetValueCosts(int year)
		{
			var thisYearR2Items = R2Items.Where(uu => uu.Year == year);
			var totals = 0M;
			foreach (var r2Item in thisYearR2Items)
			{
				var maxValue = GetMaxValue(r2Item.Year, r2Item.LogMarketReportSpeciesID, r2Item.TimberMarketID);
				totals = totals + r2Item.Summary(maxValue);
			}
			return totals;
		}
		public decimal GetAdditionalCosts(int year)
		{
			var totalVolume = R2Items.Where(uu => uu.Year == year).Sum(uu=>uu.Volume);
			var totals = GetAccessFeePerAcre(year, totalVolume) + GetMaintenanceFeePerAcre(year, totalVolume) + GetReforestationFeePerAcre(year);
			return totals;
		}

		private decimal GetTotalMileage()
		{
			var miles1 = 0M;
			var miles2 = 0M;
			var miles3 = 0M;

			if (Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
			{
				miles1 = Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value;
				if (miles1 > 0)
				{
					miles1 = (miles1 / 1000) * 0.621371192237334M;
				}
			}
			if (Parcel.ParcelRoadDistances.ToMainHaul.HasValue)
			{
				miles2 = Parcel.ParcelRoadDistances.ToMainHaul.Value;
				if (miles2 > 0)
				{
					miles2 = (miles2 / 1000) * 0.621371192237334M;
				}
			}
			if (Parcel.ParcelRoadDistances.MainHaulToPaved.HasValue)
			{
				miles3 = Parcel.ParcelRoadDistances.MainHaulToPaved.Value;
				if (miles3 > 0)
				{
					miles3 = (miles3 / 1000) * 0.621371192237334M;
				}
			}
			return miles1 + miles2 + miles3;
		}

		public decimal GetAcreFV(int year)
		{
			var totals = 0M;
			var thisYearR2Items = R2Items.Where(uu => uu.Year == year);
			foreach (var r2Item in thisYearR2Items)
			{
				var maxValue = GetMaxValue(r2Item.Year, r2Item.LogMarketReportSpeciesID, r2Item.TimberMarketID);
				totals = totals + r2Item.Summary(maxValue);
			}
			
			var costs = GetAdditionalCosts(year);

			var answer = (totals - costs) * Acres;
			return answer;
		}
		public decimal GetNPV(int reportYear)
		{
			var total = GetAcreFV(reportYear);
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation) * (1 + EconVariables.RealDiscount)), reportYear));
			var npv = total / power;
			return npv;
		}
		public Optimal GetFVMax()
		{
			var lastValue = -1000000M;
			var lastYear = 0;
			for (var year = 5; year <= 200; year += 5)
			{
				var val = GetNPV(year);
				if (val > lastValue)
				{
					lastValue = val;
					lastYear = year;
				}
			}

			return new Optimal() { RotationOptimum = lastValue, Year = lastYear };
		}
		public Optimal GetFirstPositive()
		{
			for (var year = 5; year <= 400; year += 5)
			{
				var val = GetNPV(year);
				if (val > 0)
				{
					return new Optimal() { RotationOptimum = val, Year = year };
				}
			}
			return null;
		}
	}

	public class R2Item
	{
		public int LogMarketReportSpeciesID { get; set; }
		public int TimberMarketID { get; set; }
		public int TimberGradeID { get; set; }
		public decimal Volume;
		public int Year;
		public int OrderID;
		public MarketModelPortfolio MarketModelPortfolio { get; private set; }
		public RPAPortfolio RPAPortfolio { get; private set; }
		public decimal AccessFee;
		public decimal MaintenanceFee;
		public decimal Acres;
		
		public decimal GetCostsFuture()
		{
			return CostsFuture.Value(MarketModelPortfolio, RPAPortfolio,  Year, LogMarketReportSpeciesID, TimberMarketID);
		}
		public R2Item(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, int logMarketReportSpeciesID, int timberMarketID, int timberGradeID, decimal volume, int year, int orderid, decimal acres)
		{
			var details = portfolio.MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).Select(uu => uu).FirstOrDefault();
			Acres = acres;
			AccessFee = portfolio.MarketModelPortfolioCosts.AccessFeeTimber;
			MaintenanceFee = portfolio.MarketModelPortfolioCosts.MaintenanceFeeTimberHaul;
			MarketModelPortfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Volume = volume;
			LogMarketReportSpeciesID = logMarketReportSpeciesID;
			TimberMarketID = timberMarketID;
			TimberGradeID = timberGradeID;
			Year = year;
			OrderID = orderid;
		}

		private decimal SortCosts()
		{
			var costs = GetCostsFuture();
			var value = Volume * costs;
			return value;
		}
		private decimal SortValues(decimal maxValue)
		{
			var value = maxValue * Volume;
			return value;
		}
		public decimal Summary(decimal maxValue)
		{
			var value = SortValues(maxValue) - SortCosts();
			return value;
		}		
	}
}