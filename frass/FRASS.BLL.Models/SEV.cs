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
	public class SEV
	{
		public decimal Acres;
		public decimal AccessFee;
		public decimal MaintenanceFee;

		private TimberDataManager dbTimberDataManager;
		private StandDataManager dbStandDataManager;

		public EconVariables EconVariables;
		public List<SEVItem> SEVItems;
		public decimal TotalVolume;
		public MarketModelPortfolio Portfolio;
		public RPAPortfolio RPAPortfolio;
		public Parcel Parcel;
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

		private int StandID;
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
		private List<RPAPortfolioDetail> RPAPortfolioDetails;
		private List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails;

		private decimal? _ppiToday;
		private decimal ppiToday
		{
			get { 
				if (!_ppiToday.HasValue)
				{
					_ppiToday = MarketModelDataList.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault().Value; 
				}
				return _ppiToday.Value;
			}
		}

		public SEV(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<RPAPortfolioDetail> rPAPortfolioDetails, List<MarketModelPortfolioDeliveredLogModelDetail> marketModelPortfolioDeliveredLogModelDetails, Parcel parcel, int standid, EconVariables econVariables)
		{
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbStandDataManager = StandDataManager.GetInstance();
			AccessFee = portfolio.MarketModelPortfolioCosts.AccessFeeTimber;
			MaintenanceFee = portfolio.MarketModelPortfolioCosts.MaintenanceFeeTimberHaul;
			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Parcel = parcel;
			EconVariables = econVariables;
			Acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standid && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.Acres).DefaultIfEmpty(0M).FirstOrDefault();
			RPAPortfolioDetails = rPAPortfolioDetails;
			MarketModelPortfolioDeliveredLogModelDetails = marketModelPortfolioDeliveredLogModelDetails;
			StandID = standid;
			SEVItems = GetSEVItems();
		}

		private List<SEVItem> GetSEVItems()
		{
			var key = "SEVItems_" + StandID.ToString() + "_" + Parcel.ParcelID.ToString();
			List<SEVItem> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = new List<SEVItem>();
				var sd = dbStandDataManager.GetStandDataFuture(StandID, Parcel.ParcelID);
				foreach (var standdata in sd)
				{
					var details2 = RPAPortfolioDetails.Where(uu => uu.TimberMarketID == standdata.TimberMarketID && uu.LogMarketReportSpeciesID == standdata.LogMarketReportSpeciesID).FirstOrDefault();
					if (details2 != null)
					{
						var ppiNominalDate = MarketModelDataList.Where(uu => uu.Year == details2.BeginningDate.Year && uu.Period == details2.BeginningDate.Month).FirstOrDefault().Value;
						var realValue = RPAREAL.GetRealValueFromTodaysPPI(details2.BeginningRealValue, ppiNominalDate, ppiToday);
						if (RPAPortfolio.RPAPortfolioID == -1)
						{
							realValue = Convert.ToDecimal(MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.TimberMarketID == standdata.TimberMarketID && uu.LogMarketReportSpeciesID == standdata.LogMarketReportSpeciesID).FirstOrDefault().DeliveredLogPrice);
						}
						var totalVolume = (from t in sd where t.ReportYear == standdata.ReportYear select t).Sum(uu => uu.Board_SN * uu.PctBrd);
						var volume = standdata.Board_SN * standdata.PctBrd;
						var item = new SEVItem(portfolio: Portfolio,
							rpaPortfolio: RPAPortfolio,
							logMarketReportSpeciesID: standdata.LogMarketReportSpeciesID,
							timberMarketID: standdata.TimberMarketID,
							timberGradeID: standdata.TimberGradeID,
							volume: volume,
							marketModelPortfolioDeliveredLogModelDetails: MarketModelPortfolioDeliveredLogModelDetails,
							year: standdata.ReportYear,
							orderid: standdata.OrderID,
							acres: standdata.Acres,
							totalVolume: totalVolume,
							accessFee: GetFutureAccessFee(standdata.ReportYear),
							maintenanceFee: GetFutureMaintenanceFee(standdata.ReportYear),
							realValue: realValue);
						list.Add(item);
					}
				}
				CacheHelper.Add(list, key);
			}
			return list;
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
		
		public decimal GetMaxValue(int year, int logMarketReportSpeciesID, int timberMarketID, decimal ppiToday)
		{
			var orderid = (from tm in TimberMarkets where tm.TimberMarketID == timberMarketID select tm.OrderID).FirstOrDefault();
			var items = (from lm in dbStandDataManager.GetLogMarketReportSpeciesMarkets() where lm.LogMarketReportSpeciesID == logMarketReportSpeciesID && lm.TimberMarket.OrderID >= orderid select lm).ToList();

			var details2 = RPAPortfolio.RPAPortfolioDetails.Where(uu => uu.TimberMarketID == timberMarketID && uu.LogMarketReportSpeciesID == logMarketReportSpeciesID).FirstOrDefault();
			var ppiNominalDate = MarketModelDataList.Where(uu => uu.Year == details2.BeginningDate.Year && uu.Period == details2.BeginningDate.Month).FirstOrDefault().Value;

			var realValue = RPAREAL.GetRealValueFromTodaysPPI(details2.BeginningRealValue, ppiNominalDate, ppiToday);
			if (RPAPortfolio.RPAPortfolioID == -1)
			{
				realValue = Convert.ToDecimal(MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.TimberMarketID == timberMarketID && uu.LogMarketReportSpeciesID == logMarketReportSpeciesID).FirstOrDefault().DeliveredLogPrice);
			}

			var value = 0M;
			foreach (var item in items)
			{
				var val = SEVValuesFuture.Value(Portfolio, RPAPortfolio, MarketModelPortfolioDeliveredLogModelDetails, year, logMarketReportSpeciesID, item.TimberMarketID, realValue);
				if (val > value)
				{
					value = val;
				}
			}
			return value;
		}
		public decimal GetTotalFutureValue(int year)
		{
			var thisYearSEVItems = SEVItems.Where(uu => uu.Year == year).ToList<SEVItem>();
			var totals = 0M;
			foreach (var item in thisYearSEVItems)
			{
				totals = totals + item.GetNetFutureSEV(GetMaxValue(year, item.LogMarketReportSpeciesID, item.TimberMarketID, ppiToday));
			}
			totals = (totals - GetFutureReforestation(year)) * Acres;
			return totals;
		}
		public decimal GetCurrentSEVValue(int year)
		{
			var val = GetTotalFutureValue(year);
			var val2 = (1M + EconVariables.RateOfInflation) * (1 + EconVariables.RealDiscount);
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(val2), year));
			val = val / (power - 1);
			return val;
		}
		public Optimal GetSEVRotationOptimum()
		{
			var lastValue = -100000M;
			var lastYear = 0;
			for (var year = 5; year <= 200; year += 5)
			{
				var value = GetCurrentSEVValue(year);
				if (value > lastValue)
				{
					lastValue = value;
					lastYear = year;
				}
			}
			return new Optimal() { RotationOptimum = lastValue, Year = lastYear };
		}

		public decimal SEVYearDiscount(int year)
		{
			var value = GetSEVRotationOptimum().RotationOptimum;
			var inflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + EconVariables.RateOfInflation), year)) * value;
			var discount = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + EconVariables.RateOfInflation) * (1 + EconVariables.RealDiscount)), year));
			return inflation / discount;
		}

		private decimal GetFutureAccessFee(int year)
		{
			var val1 = AccessFee * GetTotalMileage() / 1000M;
			var val2 = 1M + EconVariables.RateOfInflation;
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(val2), year));
			return val1 * power;
		}
		private decimal GetFutureMaintenanceFee(int year)
		{
			var val1 = MaintenanceFee * GetTotalMileage() / 1000M;
			var val2 = 1M + EconVariables.RateOfInflation;
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(val2), year));
			return val1 * power;
		}
		private decimal GetFutureReforestation(int year)
		{
			var rc = EconVariables.ReforestionCosts;
			var val1 = 1M + EconVariables.RateOfInflation;
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(val1), year));
			return rc * power;
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
	}
	public class SEVItem
	{
		public int LogMarketReportSpeciesID { get; set; }
		public int TimberMarketID { get; set; }
		public int TimberGradeID { get; set; }
		public decimal Volume;
		public int Year;
		public int OrderID;
		public MarketModelPortfolio MarketModelPortfolio { get; private set; }
		public List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails { get; private set; }
		public RPAPortfolio RPAPortfolio { get; private set; }
		public decimal AccessFee;
		public decimal MaintenanceFee;
		public decimal Acres;
		public decimal TotalVolume { get; set; }
		private readonly decimal RealValue;

		public SEVItem(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<MarketModelPortfolioDeliveredLogModelDetail> marketModelPortfolioDeliveredLogModelDetails, int logMarketReportSpeciesID, int timberMarketID, int timberGradeID, decimal volume, int year, int orderid, decimal acres, decimal totalVolume, decimal accessFee, decimal maintenanceFee, decimal realValue)
		{
			TotalVolume = totalVolume;
			Acres = acres;
			AccessFee = accessFee;
			MaintenanceFee = maintenanceFee;
			MarketModelPortfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Volume = volume;
			LogMarketReportSpeciesID = logMarketReportSpeciesID;
			TimberMarketID = timberMarketID;
			TimberGradeID = timberGradeID;
			Year = year;
			OrderID = orderid;
			RealValue = realValue;
			MarketModelPortfolioDeliveredLogModelDetails = marketModelPortfolioDeliveredLogModelDetails;
		}

		public decimal GetValuesFuture()
		{
			return SEVValuesFuture.Value(MarketModelPortfolio, RPAPortfolio, MarketModelPortfolioDeliveredLogModelDetails, Year, LogMarketReportSpeciesID, TimberMarketID, RealValue);
		}
		public decimal GetCostsFuture()
		{
			return SEVCostsFuture.Value(MarketModelPortfolio, RPAPortfolio, MarketModelPortfolioDeliveredLogModelDetails, Year, LogMarketReportSpeciesID, TimberMarketID);
		}
		public decimal GetNetFutureSEV(decimal maxValue)
		{
			return Volume * (maxValue - (GetCostsFuture() / 1000) - AccessFee - MaintenanceFee);
		}		
	}

	public class SEVValuesFuture
	{
		public static decimal Value(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<MarketModelPortfolioDeliveredLogModelDetail> marketModelPortfolioDeliveredLogModelDetails, int year, int logMarketReportSpeciesID, int timberMarketID, decimal realValue)
		{
			if (portfolio == null)
			{
				return 0M;
			}
			var details = marketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).Select(uu => uu).FirstOrDefault();
			if (details == null)
			{
				return 0M;
			}
			
			var marketValue = realValue / 1000M;
			var profitAndRisk = Convert.ToDecimal(details.ProfitAndRisk.Value) / 100M;
			var inflationRate = portfolio.MarketModelPortfolioInflationDetails.InflationRate;

			var val = marketValue * (1 - profitAndRisk);
			if (year > 0)
			{
				var val1 = 1M + inflationRate;
				var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(val1), year));
				val = val * power;
			}
			return val;
		}
	}
	public class SEVCostsFuture
	{
		public static decimal Value(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<MarketModelPortfolioDeliveredLogModelDetail> marketModelPortfolioDeliveredLogModelDetails,  int year, int logMarketReportSpeciesID, int timberMarketID)
		{
			var econVariables = new EconVariables(portfolio, rpaPortfolio);

			var details = marketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).Select(uu => uu).FirstOrDefault();
			var value = 0M;
			if (details != null)
			{
				var LoggingCosts = details.LoggingCosts.Value;
				var HaulingCosts = details.HaulingCosts.Value;
				var OverheadAndAdmin = details.OverheadAndAdmin.Value;
				value = Convert.ToDecimal(LoggingCosts + HaulingCosts + OverheadAndAdmin);
			}

			if (year > 0)
			{
				var val1 = 1M + econVariables.RateOfInflation;
				var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(val1), year));
				value = value * power;
			}

			return value;
		}
	}

	public class Optimal
	{
		public decimal RotationOptimum { get; set; }
		public int Year {get;set;}
	}
}