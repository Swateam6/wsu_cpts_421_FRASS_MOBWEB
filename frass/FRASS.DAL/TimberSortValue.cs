using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FRASS.DAL
{
	public class TimberSortValue
	{
		public TimberMarket TimberMarket { get; set; }
		public LogMarketReportSpecy LogMarketReportSpecy { get; set; }
		public Int32 Value { get; set; }
		public Int32 Year { get; set; }

		public TimberSortValue(TimberMarket timberMarket, LogMarketReportSpecy logMarketReportSpecy, Int32 value, Int32 year)
		{
			Value = value;
			Year = year;
			TimberMarket = timberMarket;
			LogMarketReportSpecy = logMarketReportSpecy;
		}

		public static RPAPortfolioDetail GetRealPriceDetails(RPAPortfolio rpaPortfolio, LogMarketReportSpecy logMarketReportSpecy, TimberMarket timbermarket)
		{
			return rpaPortfolio.RPAPortfolioDetails.Where(uu => uu.TimberMarket.TimberMarketID == timbermarket.TimberMarketID && uu.LogMarketReportSpeciesID == logMarketReportSpecy.LogMarketReportSpeciesID).FirstOrDefault();
		}

		public static MarketModelPortfolioDeliveredLogModelDetail GetDeliveredLogPrices(MarketModelPortfolio marketModelPortfolio, LogMarketReportSpecy logMarketReportSpecy, TimberMarket timbermarket)
		{
			return marketModelPortfolio.MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.TimberMarketID == timbermarket.TimberMarketID && uu.LogMarketReportSpeciesID == logMarketReportSpecy.LogMarketReportSpeciesID).FirstOrDefault();
		}
		public static List<TimberSortValue> LoadYears(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, TimberMarket tm, LogMarketReportSpecy species, int years, List<MarketModelData> marketModelData)
		{
			var ppiToday = marketModelData.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault().Value;

			var inflationRate = portfolio.MarketModelPortfolioInflationDetails.InflationRate;
			var timberSortValues = new List<TimberSortValue>();
			var delivered = TimberSortValue.GetDeliveredLogPrices(portfolio, species, tm);
			if (delivered != null)
			{
				decimal price = 0;
				decimal pnr = 0;
				decimal adjustedValue = 0;
				var detail = rpaPortfolio.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == species.LogMarketReportSpeciesID && uu.TimberMarketID == tm.TimberMarketID).FirstOrDefault();
				decimal rpa = detail.RPA;
				decimal longevity = detail.Longevity;

				if (delivered.DeliveredLogPrice.HasValue)
				{
					price = delivered.DeliveredLogPrice.Value;
				}
				if (delivered.ProfitAndRisk.HasValue)
				{
					pnr = Convert.ToDecimal(delivered.ProfitAndRisk.Value) / 100M;
				}
				var details = GetRealPriceDetails(rpaPortfolio, species, tm);
				rpa = details.RPA;
				longevity = details.Longevity;
				
				adjustedValue = price;				
				var sv2 = new TimberSortValue(tm, species, Convert.ToInt32(adjustedValue), 0);
				timberSortValues.Add(sv2);

				var realValue = decimal.Zero;
				if (rpaPortfolio.RPAPortfolioID == -1)
				{
					realValue = adjustedValue;
				}
				else
				{
					var ppiNominalDate = marketModelData.Where(uu => uu.Year == details.BeginningDate.Year && uu.Period == details.BeginningDate.Month).FirstOrDefault().Value;
					realValue = GetRealValueFromTodaysPPI(details.BeginningRealValue, ppiNominalDate, ppiToday);
				}

				var t = DateTime.Now.Year - detail.BeginningDate.Year;
				for (var n = 5; n <= years; n += 5)
				{
					t = t + 5;
					var rpaVal = GetFutureRPA(rpa, t, longevity);
					var rpaInflation = GetInflationRPA(rpaVal, inflationRate, n);
					var value = Convert.ToInt32(rpaInflation * realValue);
					var sortValue = new TimberSortValue(tm, species, value, n);
					timberSortValues.Add(sortValue);
					t = t + 5;
				}
			}
			return timberSortValues;
		}
		private static decimal GetRealValueFromTodaysPPI(decimal nominalValue, decimal ppiNominalDate, decimal ppiToday)
		{
			return nominalValue * (ppiToday / ppiNominalDate);
		}
		public static decimal GetFutureRPA(decimal rpa, int numberOfYears, decimal longevity)
		{
			decimal rpa1 = 0M;


			var exponent = 1 - (numberOfYears / (longevity * .69315M));
			var power = Math.Pow(Convert.ToDouble(2M), Convert.ToDouble(exponent));
			rpa1 = 1 + (rpa * numberOfYears * Convert.ToDecimal(power));

			return rpa1;
		}
		public static decimal GetInflationRPA(decimal rpa, decimal inflation, int numberOfYears)
		{
			var power = Math.Pow(Convert.ToDouble(1 + inflation), Convert.ToDouble(numberOfYears));
			var val = rpa * Convert.ToDecimal(power);
			return val;
		}
	}
}
