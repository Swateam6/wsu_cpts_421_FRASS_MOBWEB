using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
	public class HarvestReport
	{
		public int StandID { get; set; }
		public int R1Year {get; set;}
		public int R2Year {get; set;}
		public int R3Year {get; set;}
		public List<HarvestReportLogReportSpecies> HarvestReportLogReportSpecies { get; set; }

		TimberDataManager dbTimberDataManager;
		StandDataManager dbStandDataManager;

		public HarvestReport(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<RPAPortfolioDetail> rPAPortfolioDetails, List<MarketModelPortfolioDeliveredLogModelDetail> marketModelPortfolioDeliveredLogModelDetails, Parcel parcel, int standID, EconVariables econVariables)
		{
			dbTimberDataManager = TimberDataManager.GetInstance();
			dbStandDataManager = StandDataManager.GetInstance();

			List<IStandData> standDataCurrent = dbStandDataManager.GetStandDataCurrent(standID, parcel.ParcelID);
			List<IStandData> standDataFuture = dbStandDataManager.GetStandDataFuture(standID, parcel.ParcelID);
			List<TimberMarket> timberMarkets = dbTimberDataManager.GetTimberMarkets();
			List<TimberGrade> timberGrades = dbTimberDataManager.GetTimberGrades();
			List<LogMarketReportSpeciesMarket> logMarketReportSpeciesMarket = dbStandDataManager.GetLogMarketReportSpeciesMarkets();
			HarvestReportLogReportSpecies = new List<HarvestReportLogReportSpecies>();
			var rotationGenerator = new RotationGenerator(portfolio, rpaPortfolio, rPAPortfolioDetails, marketModelPortfolioDeliveredLogModelDetails, parcel, standID, econVariables);
			rotationGenerator.LoadR1();
			rotationGenerator.LoadSEV();
			StandID = standID;
			var minyear = dbStandDataManager.GetMinYear();
			var years = dbStandDataManager.GetCurrentStandSortYears();
			var cutYears = 0;
			decimal overallMax;
			var r2SEV = rotationGenerator.GetR2(0);
			var OuterList = rotationGenerator.RunGenerator(out overallMax);
			var o = OuterList.Where(uu => uu.MaxValue == overallMax).FirstOrDefault();
			var acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standID && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Sum(uu => uu.Acres);
			if (o != null)
			{
				var r2Offset = o.HarvestYear - minyear;
				
				var gc = o.Cuts.Where(uu => uu.Total.RotationOptimum == overallMax).FirstOrDefault();
				if (gc != null)
				{
					R1Year = o.HarvestYear;
					var ct = 0;
					foreach (var y in years.OrderBy(uu => uu))
					{
						if (y == R1Year)
						{
							break;
						}
						ct = ct + 5;
					}
					cutYears = gc.Year + ct;
					R2Year = R1Year +  gc.Year;
					R3Year = o.HarvestYear + gc.Year + rotationGenerator.SEV.GetSEVRotationOptimum().Year;
					var k = R3Year - minyear;
					var species = dbTimberDataManager.GetSpecies();
					foreach (var s in species.OrderBy(uu => uu.CommonName))
					{
						HarvestReportLogReportSpecies.Add(new HarvestReportLogReportSpecies(portfolio, rpaPortfolio, rotationGenerator, s, timberGrades, timberMarkets, logMarketReportSpeciesMarket, standDataCurrent, standDataFuture,cutYears, minyear, R1Year, r2SEV, gc.Year, acres, econVariables.RateOfInflation));
					}	
				}
			}

					
		}

		
	}

}