using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;
using FRASS.DAL;

namespace FRASS.BLL.Models
{

	public class HarvestReportLogReportSpecies
	{
		public Specy Specy { get; set; }
		public List<HarvestReportItem> HarvestReportItems { get; set; }

		public HarvestReportLogReportSpecies(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, RotationGenerator rotationGenerator, Specy specy, List<TimberGrade> timberGrades, List<TimberMarket> timberMarkets, List<LogMarketReportSpeciesMarket> logMarketReportSpeciesMarket, List<IStandData> standDataCurrent, List<IStandData> standDataFuture, int cutYears, int currentYear, int reportYear, R2 r2SEV, int optimalYear, decimal acres, decimal rateOfInflation)
		{
			Specy = specy;
			HarvestReportItems = new List<HarvestReportItem>();
			var logMarketReportSpecies = specy.LogMarketReportSpeciesSpecies.FirstOrDefault();
				if (logMarketReportSpecies == null)
				{
					return;
				}
				var logMarketReportSpeciesID = logMarketReportSpecies.LogMarketReportSpeciesID;
			var tm = from l in logMarketReportSpeciesMarket where l.LogMarketReportSpeciesID == logMarketReportSpeciesID select l.TimberMarket;
			foreach (var t in tm)
			{
				var todaysValue = portfolio.MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == t.TimberMarketID).FirstOrDefault();
				if (todaysValue != null)
				{
					var pr = Convert.ToDecimal(todaysValue.ProfitAndRisk.Value) / 100M;
					var details = rpaPortfolio.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == t.TimberMarketID).FirstOrDefault();
					HarvestReportItems.Add(new HarvestReportItem(todaysValue.DeliveredLogPrice.Value, specy, rotationGenerator, timberGrades, t, standDataCurrent, standDataFuture, cutYears, currentYear, reportYear, r2SEV, optimalYear, acres, rateOfInflation, pr, details.RPA, details.Longevity));
				}
			}
		}
	}
}
