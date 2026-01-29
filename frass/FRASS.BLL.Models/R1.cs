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
	[Serializable]
	public class R1
	{
		public decimal Acres;
		public EconVariables EconVariables;
		public List<R1Item> R1Items;
		public MarketModelPortfolio Portfolio;
		public RPAPortfolio RPAPortfolio;
		public Parcel Parcel;

		TimberDataManager dbTimberDataManager;
		StandDataManager dbStandDataManager;

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
			get { return _mmd ?? (_mmd = dbDeliveredLogMarketModel.GetMarketModelData().Where(uu=>uu.MarketModelTypeID == 3).ToList<MarketModelData>()); }
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

		public R1(MarketModelPortfolio portfolio,RPAPortfolio rpaPortfolio, Parcel parcel, int standid, EconVariables econVariables)
		{
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbStandDataManager = StandDataManager.GetInstance();

			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Parcel = parcel;
			EconVariables = econVariables;
			R1Items = new List<R1Item>();
			var sd = dbStandDataManager.GetStandDataCurrent(standid, parcel.ParcelID);//.Where(uu=>uu.TimberMarketID == 2 && uu.LogMarketReportSpeciesID == 1 && uu.TimberGradeID == 41);
			Acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.Acres).DefaultIfEmpty(0M).FirstOrDefault();
			
			var years = dbStandDataManager.GetCurrentStandSortYears();
			foreach (var standdata in sd)
			{
				var volume = standdata.Board_SN * standdata.PctBrd;
				var index = years.FindIndex(uu=>uu == standdata.ReportYear) * 5;
				var item = new R1Item(portfolio, rpaPortfolio, standdata.LogMarketReportSpeciesID, standdata.TimberMarketID, standdata.TimberGradeID, volume, standdata.ReportYear, index, standdata.OrderID, standdata.Acres);
				R1Items.Add(item);
			}

		}

		public Decimal GetAccessFeePerAcre(int year, decimal totalVolume)
		{
			var value = 0M;
			var powerInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation)), year));
			value = EconVariables.AccessFee * totalVolume * GetTotalMileage() / 1000M * powerInflation;
			return value;
		}
		public Decimal GetMaintenanceFeePerAcre(int year, decimal totalVolume)
		{
			var value = 0M;
			var powerInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation)), year));
			value = EconVariables.MaintenanceFee * totalVolume * GetTotalMileage() / 1000M * powerInflation;
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
		public decimal GetValueCosts(int calendarYear, int year)
		{
			var thisYearR1Items = R1Items.Where(uu => uu.Year == calendarYear).Distinct();
			var totals = 0M;
			foreach (var r1Item in thisYearR1Items)
			{
				var maxValue = GetMaxValue(year, r1Item.LogMarketReportSpeciesID, r1Item.TimberMarketID);
				totals = totals + r1Item.Summary(maxValue);
			}
			return totals;
		}
		public decimal GetAdditionalCosts(int calendarYear, int year)
		{
			var thisYearR2Items = R1Items.Where(uu => uu.Year == calendarYear);
			var totalVolume = thisYearR2Items.Sum(uu => uu.Volume);
			var accessFee = GetAccessFeePerAcre(year, totalVolume);
			var maintenanceFee = GetMaintenanceFeePerAcre(year, totalVolume);
			var reforestationFee = GetReforestationFeePerAcre(year);
			return accessFee + maintenanceFee + reforestationFee;
		}
		public decimal GetPeriodicNetRevenue(int calendarYear, int year)
		{
			var val = GetValueCosts(calendarYear, year);
			var costs = GetAdditionalCosts(calendarYear, year);
			return val - costs;
		}
		public decimal GetAcreFV(int calendarYear, int year)
		{
			return GetPeriodicNetRevenue(calendarYear, year) * Acres;
		}
		public decimal GetNPV(int calendarYear, int year)
		{
			var total = GetAcreFV(calendarYear, year);
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation) * (1 + EconVariables.RealDiscount)), year));
			var npv = total / power;
			return npv;
		}
		public Optimal GetFVMax()
		{
			var lastValue = -1000000M;
			var lastYear = 0;
			var currentStandYears = dbStandDataManager.GetCurrentStandSortYears();
			var firstYear = currentStandYears.Min(uu => uu);
			foreach (var year in currentStandYears.OrderBy(uu => uu))
			{
				var val = GetNPV(year, year - firstYear);
				if (val > lastValue)
				{
					lastValue = val;
					lastYear = year;
				}
			}
			lastYear = lastYear - firstYear;
			return new Optimal() { RotationOptimum = lastValue, Year = lastYear };
		}
		public Optimal GetFirstPositive()
		{
			var currentStandYears = dbStandDataManager.GetCurrentStandSortYears();
			var firstYear = currentStandYears.Min(uu => uu);
			foreach (var year in currentStandYears.OrderBy(uu => uu))
			{
				var val = GetNPV(year, year - firstYear);
				if (val > 0)
				{
					return new Optimal() { RotationOptimum = val, Year = year};
				}
			}
			return null;
		}
	}

	public class R1Item
	{
		public int LogMarketReportSpeciesID { get; set; }
		public int TimberMarketID { get; set; }
		public int TimberGradeID { get; set; }
		public decimal Volume;
		public int Year;
		public int YearNum;
		public int OrderID;
		public MarketModelPortfolio MarketModelPortfolio { get; private set; }
		public RPAPortfolio RPAPortfolio { get; private set; }
		public decimal AccessFee;
		public decimal MaintenanceFee;
		public decimal Acres;
		
		//private decimal GetCostsFuture()
		//{
		//	return CostsFuture.Value(MarketModelPortfolio,RPAPortfolio, YearNum, LogMarketReportSpeciesID, TimberMarketID);
		//}
		public R1Item(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, int logMarketReportSpeciesID, int timberMarketID, int timberGradeID, decimal volume, int year, int yearNum, int orderid, decimal acres)
		{
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
			YearNum = yearNum;
			OrderID = orderid;
		}

		//private decimal SortCosts()
		//{
		//	var costs = GetCostsFuture();
		//	var value = Volume * costs;
		//	return value;
		//}
		private decimal SortValues(decimal maxValue)
		{
			var value = maxValue * Volume;
			return value;
		}
		public decimal Summary(decimal maxValue)
		{
			var value = SortValues(maxValue);
			return value;
		}
	}
}