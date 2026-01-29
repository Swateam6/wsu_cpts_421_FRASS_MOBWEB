using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;

namespace FRASS.BLL.Models
{
	public class RotationGenerator
	{
		private MarketModelPortfolio Portfolio;
		private RPAPortfolio RPAPortfolio;
		private List<RPAPortfolioDetail> RPAPortfolioDetails;
		private List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails;
		private Parcel Parcel;
		private int StandID;
		private List<IStandData> StandData;
		public R1 R1 {get; private set;}
		public SEV SEV { get; private set; }
		private EconVariables EconVariables;

		private StandDataManager dbStandDataManager;

		public RotationGenerator(MarketModelPortfolio portfolio, RPAPortfolio rpaPortfolio, List<RPAPortfolioDetail> rPAPortfolioDetails, List<MarketModelPortfolioDeliveredLogModelDetail> marketModelPortfolioDeliveredLogModelDetails, Parcel parcel, int standid, EconVariables econVariables)
		{
			Portfolio = portfolio;
			RPAPortfolio = rpaPortfolio;
			Parcel = parcel;
			StandID = standid;
			dbStandDataManager = StandDataManager.GetInstance();
			StandData = dbStandDataManager.GetStandDataFuture(standid, parcel.ParcelID).ToList<IStandData>();
			EconVariables = econVariables;
			RPAPortfolioDetails = rPAPortfolioDetails;
			MarketModelPortfolioDeliveredLogModelDetails = marketModelPortfolioDeliveredLogModelDetails;
		}

		public R1 LoadR1()
		{
			if (R1 == null)
			{
				R1 = new R1(Portfolio, RPAPortfolio, Parcel, StandID, EconVariables);
			}
			return R1;
		}
		public SEV LoadSEV()
		{
			if (SEV == null)
			{
				SEV = new SEV(Portfolio, RPAPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, Parcel, StandID, EconVariables);
			}
			return SEV;
		}
		public R2 GetR2(int offset)
		{
			var R2Items = new List<R2Item>();
			foreach (var standdata in StandData)
			{
				var reportYear = standdata.ReportYear;// -offset;
				var totalVolume = (from t in StandData where t.ReportYear == reportYear select t).Sum(uu => uu.Board_SN * uu.PctBrd);
				var offSetVolume = (from t in StandData where t.ReportYear == reportYear && t.LogMarketReportSpeciesID == standdata.LogMarketReportSpeciesID && t.SpeciesID == standdata.SpeciesID && t.TimberGradeID == standdata.TimberGradeID && t.TimberMarketID == standdata.TimberMarketID select t).FirstOrDefault();
				decimal volume = 0M;
				if (offSetVolume != null)
				{
					volume = offSetVolume.Board_SN * offSetVolume.PctBrd;
				}
				var item = new R2Item(Portfolio, RPAPortfolio, standdata.LogMarketReportSpeciesID, standdata.TimberMarketID, standdata.TimberGradeID, volume, standdata.ReportYear + offset, standdata.OrderID, standdata.Acres);
				R2Items.Add(item);
			}
			return new R2(Portfolio, RPAPortfolio, Parcel, StandID, R2Items, EconVariables);
		}
		public decimal GetSEVMax(decimal sev, int year)
		{
			var powerInflation = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + SEV.EconVariables.RateOfInflation)), year));
			var power2 = Convert.ToDecimal(Math.Pow(Convert.ToDouble((1 + SEV.EconVariables.RateOfInflation + SEV.EconVariables.RealDiscount)),year));
			return (sev*(powerInflation))/(power2);
		}

		public GrowthCut GetGrowthCut(R2 r2, Optimal initial, int yearNum, decimal r1_val, int offset)
		{
			var sev_val = SEV.SEVYearDiscount(yearNum + 5);
			var values = new List<Optimal>();
			var r2val = r2.GetNPV(yearNum);
			var total = r1_val + r2val + sev_val;
			var gc = new GrowthCut();
		    gc.Total = new Optimal() { RotationOptimum = total, Year = 0 };
			gc.R1 = new Optimal() { RotationOptimum = r1_val, Year = 0 };
			gc.R2 = new Optimal() { RotationOptimum = r2val, Year = yearNum };
			gc.SEV = new Optimal() { RotationOptimum = sev_val, Year = 0 };
			gc.Year = offset;
			return gc;
		}
		public GrowthCuts GetRotation(int calendarYear, int r1_offset,int offset)
		{
			var title = "This harvests R1 in " + calendarYear.ToString();
			var r2 = GetR2(offset);

			var r2_initial = r2.GetFirstPositive();
			if (r2_initial == null)
			{
				var gc = new GrowthCuts();
				gc.Title = "Rotation values cannot be calculated for this stand in " + calendarYear.ToString();
				gc.Cuts = new List<GrowthCut>();
				gc.MaxValue = 0;
				gc.HarvestYear = calendarYear;

				return gc;
			}
			else
			{
				var r2_optimal = r2.GetFVMax();
				var cuts = new List<GrowthCut>();
				var lastYear = r2_optimal.Year + 250;
				var r1_val = R1.GetNPV(calendarYear, r1_offset);
				var endYear = r2_initial.Year + 200;
				for (var year = r2_initial.Year; year < endYear; year += 5)
				{
					if (year < lastYear)
					{
						var r2_offset = year - offset;
						var growthCut = GetGrowthCut(r2, r2_initial, year, r1_val, r2_offset);
						if (growthCut.R2.RotationOptimum > 0)
						{
							cuts.Add(growthCut);
						}
					}
				}

				var maxValue = -99999M;
				foreach (var cut in cuts)
				{
					if (cut.Total.RotationOptimum > maxValue)
					{
						maxValue = cut.Total.RotationOptimum;
					}
				}

				var gc = new GrowthCuts();
				gc.Title = title;
				gc.Cuts = cuts;
				gc.MaxValue = maxValue;
				gc.HarvestYear = calendarYear;

				return gc;
			}
		}
		public List<GrowthCuts> RunGenerator(out decimal overallMax)
		{
			var key = "RunGenerator_" + Portfolio.MarketModelPortfolioID.ToString() + "_" + Parcel.ParcelID.ToString() + "_" + StandID.ToString() + "_" + RPAPortfolio.RPAPortfolioID.ToString();
			var key2 = "OverallMax_" + Portfolio.MarketModelPortfolioID.ToString() + "_" + Parcel.ParcelID.ToString() + "_" + StandID.ToString() + "_" + RPAPortfolio.RPAPortfolioID.ToString();
			List<GrowthCuts> OuterList;
			if (!CacheHelper.Get(key, out OuterList))
			{
				LoadR1();
				LoadSEV();
				overallMax = 0M;
				OuterList = new List<GrowthCuts>();
				var r1_initial = R1.GetFirstPositive();
				if (r1_initial != null)
				{
					var currentStandYears = dbStandDataManager.GetCurrentStandSortYears();
					var startYear = currentStandYears.Min(uu => uu);
					var minYear = startYear;
					if (r1_initial.Year > minYear)
					{
						startYear = r1_initial.Year;
					}


					var offsets = new List<int>();
					int ct = 0;

					var startLoop = -5;
					var loopStop = R1.GetFVMax().Year + 20;
					foreach (var year in currentStandYears.OrderBy(uu => uu))
					{
						if (year >= startYear)
						{
							GrowthCuts gc = GetRotation(year, year - minYear, ct);

							if (overallMax < gc.MaxValue)
							{
								overallMax = gc.MaxValue;
								startLoop = 0;
							}
							OuterList.Add(gc);
							if (startLoop >= 0)
							{
								startLoop = startLoop + 5;
							}
							if (startLoop == loopStop)
							{
								break;
							}
						}
						ct = ct + 5;
					}
				}
				CacheHelper.Add(OuterList, key);
				CacheHelper.Add(overallMax, key2);
			}
			else
			{
				CacheHelper.Get(key2, out overallMax);
				LoadSEV();
			}
			return OuterList;
		}
	}

	public class GrowthCut
	{
		public Optimal Total { get; set; }
		public Optimal R1 { get; set; }
		public Optimal R2 { get; set; }
		public Optimal SEV { get; set; }
		public int Year { get; set; }
	}

	public class GrowthCuts
	{
		public string Title { get; set; }
		public List<GrowthCut> Cuts { get; set; }
		public decimal MaxValue { get; set; }
		public int HarvestYear { get; set; }
	}
}