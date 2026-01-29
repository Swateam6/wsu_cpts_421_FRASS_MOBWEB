using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
	public class CostsFuture
	{
		public static decimal Value(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, int year, int logMarketReportSpeciesID, int timberMarketID)
		{
			var econVariables = new EconVariables(portfolio, rpaPortfolio);
			var details = portfolio.MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).Select(uu => uu).FirstOrDefault();
			var LoggingCosts = 0M;
			var HaulingCosts = 0M;
			var OverheadAndAdmin = 0M;
			if (details != null)
			{
				LoggingCosts = details.LoggingCosts.Value;
				HaulingCosts = details.HaulingCosts.Value;
				OverheadAndAdmin = details.OverheadAndAdmin.Value;
			}

			var value = 0M;
			var values2 = 0M;
			var longevity = econVariables.GetLongevity(logMarketReportSpeciesID, timberMarketID);
			if (longevity != 0)
			{
				var exponent = 1 - (year / (longevity * .7M));
				var power2 = Convert.ToDecimal(Math.Pow(2, Convert.ToDouble(exponent)));
				values2 = econVariables.GetRPA(logMarketReportSpeciesID, timberMarketID) * year * power2;
			}


			var powerInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + econVariables.RateOfInflation)), year));

			var logCosts = Convert.ToDecimal(LoggingCosts) / 1000M;
			var haulCosts = Convert.ToDecimal(HaulingCosts) / 1000M;
			var oaCosts = Convert.ToDecimal(OverheadAndAdmin) / 1000M;
			var costs = haulCosts + oaCosts;
			value = (logCosts * ((1 + values2)) * powerInflation) + (costs * powerInflation);
			return value;
		}
	}
}
