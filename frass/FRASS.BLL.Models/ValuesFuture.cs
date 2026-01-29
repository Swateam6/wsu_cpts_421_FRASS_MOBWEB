using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.BLL.Formulas;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
	public class ValuesFuture
	{
		public static decimal Value(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, int year, int logMarketReportSpeciesID, int timberMarketID, List<MarketModelData> marketModelDataList, RPARealValue rpaRealValue, decimal ppiToday)
		{
			if (portfolio == null)
			{
				return 0M;
			}
			var details = portfolio.MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).Select(uu => uu).FirstOrDefault();
			var details2 = rpaPortfolio.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).Select(uu => uu).FirstOrDefault();
			if (details == null || details2 == null)
			{
				return 0M;
			}
			var marketValue = Convert.ToDecimal(details.DeliveredLogPrice.Value) / 1000M;
			var profitAndRisk = Convert.ToDecimal(details.ProfitAndRisk.Value) / 100M;

			var costs = Convert.ToDecimal(details.OverheadAndAdmin.Value) / 1000M + Convert.ToDecimal(details.HaulingCosts.Value) / 1000M + Convert.ToDecimal(details.LoggingCosts.Value) / 1000M;
			var t = System.DateTime.Now.Year - details2.BeginningDate.Year + year;
			var val = marketValue * (1 - profitAndRisk) - costs;
			if (year > 0)
			{
				var ppiNominalDate = marketModelDataList.Where(uu => uu.Year == details2.BeginningDate.Year && uu.Period == details2.BeginningDate.Month).FirstOrDefault().Value;
				var realValue = rpaRealValue.GetRealValueFromTodaysPPI(details2.BeginningRealValue, ppiNominalDate, ppiToday) / 1000m;
				if (rpaPortfolio.RPAPortfolioID == -1)
				{
					realValue = marketValue;
				}
				var longevityTerm = details2.Longevity;
				var rpa = details2.RPA;
				var inflationRate = portfolio.MarketModelPortfolioInflationDetails.InflationRate;

				

				var rpaVal = GetFutureRPA(rpa, t, longevityTerm);
				var rpaInflation = GetInflationRPA(inflationRate, year);
				
				var rateOfInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + inflationRate),year));

				val = realValue * rpaVal * rpaInflation - costs * rateOfInflation;
			}
			return val;
		}

		private static decimal GetFutureRPA(decimal rpa, int t, decimal longevity)
		{
			decimal rpa1 = 0M;
			var exponent = 1 - (t / (longevity * .69315M));
			var power = Math.Pow(Convert.ToDouble(2M), Convert.ToDouble(exponent));
			rpa1 = 1 + (rpa * t * Convert.ToDecimal(power));

			return rpa1;
		}
		private static decimal GetInflationRPA(decimal inflation, int n)
		{
			var power = Math.Pow(Convert.ToDouble(1 + inflation), Convert.ToDouble(n));
			var val =Convert.ToDecimal(power);
			return val;
		}
	}
}
