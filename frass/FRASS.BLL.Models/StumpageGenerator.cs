using System;
using System.Collections.Generic;
using System.Linq;
using FRASS.DAL;
using FRASS.Interfaces;
using FRASS.BLL.Models.Services;

namespace FRASS.BLL.Models
{
	public class StumpageGenerator
	{
		public StumpageR1 StumpageR1 { get; set; }
		public StumpageR2 StumpageR2 { get; set; }
		public StumpageSEV StumpageSEV { get; set; }
		public List<Int32> CalendarYears { get; set; }
		public List<Int32> ReportYears { get; set; }
		private List<StumpageGroup> StumpageGroups {get;set;}
		private Int32 StandID { get; set; }
		private Int32 MinYear { get; set; }
		private StumpageModelPortfolio Portfolio { get; set; }
		private Int32 HaulzoneID { get; set; }
		private EconVariables EconVariables { get; set; }
		private decimal Acres { get; set; }
		private bool UseDynamicTimberProcessing { get; set; }
		private TimberProcessingService TimberService { get; set; }

		public StumpageGenerator(Parcel parcel, Int32 standID, StumpageModelPortfolio portfolio, List<StumpageGroup> stumpageGroups, List<IStandData> currentStandData, List<IStandData> futureStandData, List<TimberMarket> timberMarkets, Int32 haulZoneID, Int32 minYear, EconVariables econVariables, List<StumpageGroupQualityCode> stumpageGroupQualityCodes, List<Int32> calendarYears, List<Int32> reportYears)
			: this(parcel, standID, portfolio, stumpageGroups, currentStandData, futureStandData, timberMarkets, haulZoneID, minYear, econVariables, stumpageGroupQualityCodes, calendarYears, reportYears, false)
		{
		}

		public StumpageGenerator(Parcel parcel, Int32 standID, StumpageModelPortfolio portfolio, List<StumpageGroup> stumpageGroups, List<IStandData> currentStandData, List<IStandData> futureStandData, List<TimberMarket> timberMarkets, Int32 haulZoneID, Int32 minYear, EconVariables econVariables, List<StumpageGroupQualityCode> stumpageGroupQualityCodes, List<Int32> calendarYears, List<Int32> reportYears, bool useDynamicTimberProcessing)
		{
			UseDynamicTimberProcessing = useDynamicTimberProcessing;
			if (UseDynamicTimberProcessing)
			{
				TimberService = new TimberProcessingService();
			}

			StumpageR1 = new StumpageR1(parcel, standID, portfolio, stumpageGroups, currentStandData, timberMarkets, haulZoneID, minYear, econVariables, stumpageGroupQualityCodes, calendarYears);
			StumpageR2 = new StumpageR2(parcel, standID, portfolio, stumpageGroups, futureStandData, timberMarkets, haulZoneID, minYear, econVariables, stumpageGroupQualityCodes, reportYears);
			StumpageSEV = new StumpageSEV(parcel, standID, portfolio, stumpageGroups, futureStandData, timberMarkets, haulZoneID, minYear, econVariables, stumpageGroupQualityCodes, reportYears);
			CalendarYears = calendarYears;
			ReportYears = reportYears;
			StumpageGroups = stumpageGroups;
			StandID = standID;
			MinYear = minYear;
			Portfolio = portfolio;
			HaulzoneID = haulZoneID;
			EconVariables = econVariables;
			Acres = parcel.ParcelRiparians.Where(uu => uu.STD_ID == standID && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).Select(uu => uu.Acres).Sum();
		}

		public int FirstR1Year()
		{
			var firstYear = CalendarYears.Min();
			foreach (var year in CalendarYears.OrderBy(uu => uu))
			{
				var r1 = StumpageR1.GetNetFV(year);
				if (r1 > 0)
				{
					firstYear = year;
					break;
				}
			}
			return firstYear;
		}

		public List<GrowthCuts> GetGrowthCuts(out decimal overallMax)
		{
			var gcs = new List<GrowthCuts>();
			var minYear = CalendarYears.Min(uu => uu);
			overallMax = 0M;
			var countPastMaxYear = 45;

			var firstYear = FirstR1Year();
			foreach (var year in CalendarYears.OrderBy(uu => uu))
			{
				if (year >= firstYear)
				{
					countPastMaxYear = countPastMaxYear - 5;
					var growthCuts = new GrowthCuts();
					growthCuts.Title = "This Harvests R1 in " + year.ToString();
					growthCuts.Cuts = new List<GrowthCut>();
					growthCuts.MaxValue = 0M;
					growthCuts.HarvestYear = year;
					var countPastMax = 200;

					foreach (var reportYear in ReportYears.OrderBy(uu => uu))
					{
						countPastMax = countPastMax - 5;
						var gc = new GrowthCut();
						var r2 = StumpageR2.GetNPV(reportYear, year - minYear);
						if (r2 > 0)
						{
							var r1 = StumpageR1.GetNPV(year);
							var sev = StumpageSEV.GetSEVRedux(reportYear + (year - minYear));
							gc.R1 = new Optimal() { RotationOptimum = r1, Year = year };
							gc.R2 = new Optimal() { RotationOptimum = r2, Year = (year + reportYear) };
							gc.SEV = new Optimal() { RotationOptimum = sev, Year = year };
							gc.Total = new Optimal() { RotationOptimum = r1 + r2 + sev, Year = year };
							if (gc.Total.RotationOptimum > growthCuts.MaxValue)
							{
								growthCuts.MaxValue = gc.Total.RotationOptimum;
								countPastMax = 200;
							}
							gc.Year = reportYear;
							growthCuts.Cuts.Add(gc);
							if (countPastMax == 0)
							{
								break;
							}
						}
					}

					if (growthCuts.Cuts.Count() == 0)
					{
						growthCuts.Title = "Rotation values cannot be calculated for this stand in " + year.ToString();
					}

					if (growthCuts.MaxValue > overallMax)
					{
						countPastMaxYear = 45;
						overallMax = growthCuts.MaxValue;
					}
					gcs.Add(growthCuts);
					if (countPastMaxYear == 0)
					{
						break;
					}
				}
			}


			return gcs;
		}
	
		public StumpageHarvestReport GetStumpageHarvestReport()
		{
			decimal overallMax;
			var gc = GetGrowthCuts(out overallMax);
			return new StumpageHarvestReport(Portfolio, StumpageR1, StumpageR2, StumpageSEV, StumpageGroups, gc, overallMax, MinYear, EconVariables, HaulzoneID, Acres);

		}

		/// <summary>
		/// Gets dynamic timber volumes using FRASS.Timber algorithms
		/// </summary>
		/// <param name="currentStandData">Current stand data</param>
		/// <returns>Dictionary of volumes by grade if dynamic processing is enabled, null otherwise</returns>
		public Dictionary<string, decimal> GetDynamicTimberVolumes(List<IStandData> currentStandData)
		{
			if (!UseDynamicTimberProcessing || TimberService == null)
			{
				return null;
			}

			try
			{
				return TimberService.ProcessStandFromStandData(StandID, 0, currentStandData); // parcelId not needed for this calculation
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error calculating dynamic timber volumes: {ex.Message}");
				return null;
			}
		}
	}
}
