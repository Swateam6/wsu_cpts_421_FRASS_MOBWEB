using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
	public class StumpageHarvestReport
	{
		public int R1Year { get; set; }
		public int R2Year { get; set; }
		public int R3Year { get; set; }
		public List<HarvestStumpageGroup> HarvestStumpageGroups { get; set; }
		public List<StumpageGroup> StumpageGroups { get; set; }
		public StumpageHarvestReport(StumpageModelPortfolio portfolio, StumpageR1 stumpageR1, StumpageR2 stumpageR2, StumpageSEV stumpagesev, List<StumpageGroup> stumpageGroups, List<GrowthCuts> growthCuts, decimal overallMax, int minYear, EconVariables econVariables, Int32 haulZoneID, decimal acres)
		{
			StumpageGroups = new List<StumpageGroup>();
			LoadYears(stumpageR1, stumpageR2, stumpagesev, growthCuts, overallMax, minYear);
			var tempHarvestStumpageGroups = new List<HarvestStumpageGroup>();
			foreach (var sg in stumpageGroups)
			{
				var hsgR1 = new HarvestStumpageGroup();
				var groupVolume = 0M;
				var value = 0M;
				StumpageGroupIDs sgID = (StumpageGroupIDs)sg.StumpageGroupID;
				hsgR1.QualityCodeNumber = stumpageR1.GetQualityCodeForYearAndGroup(sgID, R1Year, out groupVolume);
				hsgR1.ValueAtHarvest_R1 = groupVolume * acres;
				hsgR1.ValueType = 1;
				stumpageR1.GetQualityCodePriceForYearAndGroup(sgID, R1Year, out value);



				hsgR1.ValueMBF_R1 = value;
				hsgR1.StumpageGroup = sg;

				var dif = R1Year - minYear;
				var inflations = portfolio.StumpageModelPortfolioInflationDetails;
				var todaysValue = portfolio.StumpageModelPortfolioValues.Where(uu => uu.StumpageGroupID == sg.StumpageGroupID && uu.StumpageGroupQualityCode.QualityCodeNumber == hsgR1.QualityCodeNumber).FirstOrDefault();
				var details = portfolio.StumpageModelPortfolioRPADatas.Where(uu => uu.StumpageGroupID == sg.StumpageGroupID && uu.StumpageGroupQualityCode.QualityCodeNumber == hsgR1.QualityCodeNumber).FirstOrDefault();
				var currentvalue = 0M;
				var rpa = 0M;
				var oNa = todaysValue.OverheadAndAdmin.Value;
				if (haulZoneID == 3)
				{
					currentvalue = todaysValue.Haul3.Value;
					rpa = details.Haul3.Value;
				}
				else if (haulZoneID == 4)
				{
					currentvalue = todaysValue.Haul4.Value;
					rpa = details.Haul3.Value;
				}
				else if (haulZoneID == 4)
				{
					currentvalue = todaysValue.Haul5.Value;
					rpa = details.Haul5.Value;
				}

				//var ir = inflations.InflationRate;
				//var longevity = details.Longevity;
				//var f28 = longevity * .7M;
				//var g27 = 1 - (dif / f28);
				//var g26 = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + ir), Convert.ToDouble(dif)));
				//var f27 = Convert.ToDecimal(Math.Pow(2, Convert.ToDouble(g27)));
				//var f26 = 1 + rpa * dif * f27;
				//var r1val = currentvalue * f26 * g26;

				var pr = Convert.ToDecimal(todaysValue.ProfitAndRisk.Value) / 100M;
				var futurerpa = TimberSortValue.GetFutureRPA(rpa, dif, details.Longevity.Value);
				var adjustedValue = (currentvalue - oNa) * (1 - pr);
				var rpaInflation = TimberSortValue.GetInflationRPA(futurerpa, econVariables.RateOfInflation, dif);
				hsgR1.ValueMBF_R1 = rpaInflation * adjustedValue;

				if (R1Year != minYear)
				{
					//hsgR1.ValueMBF_R1
				}

				hsgR1.ValueSort_R1 = hsgR1.ValueAtHarvest_R1 / 1000 * hsgR1.ValueMBF_R1;

				var hsgR2 = new HarvestStumpageGroup();
				var r2Volume = 0M;
				hsgR2.ValueType = 2;
				hsgR2.QualityCodeNumber = stumpageR2.GetQualityCodeForYearAndGroup(sgID, R2Year - R1Year, out r2Volume);
				hsgR2.ValueAtHarvest_R2 = r2Volume;
				hsgR2.StumpageGroup = sg;

				var hsgR3 = new HarvestStumpageGroup();
				var r3Volume = 0M;
				hsgR3.ValueType = 3;
				hsgR3.QualityCodeNumber = stumpagesev.GetQualityCodeForYearAndGroup(sgID, R3Year - R2Year, out r3Volume);
				hsgR3.ValueAtHarvest_R3 = r3Volume;
				hsgR3.StumpageGroup = sg;

				if (groupVolume > 0)
				{
					tempHarvestStumpageGroups.Add(hsgR1);
				}
				if (r2Volume > 0)
				{
					tempHarvestStumpageGroups.Add(hsgR2);
				}
				if (r3Volume > 0)
				{
					tempHarvestStumpageGroups.Add(hsgR3);
				}
				if (groupVolume > 0 || r2Volume > 0 || r3Volume > 0)
				{
					StumpageGroups.Add(sg);
				}
			}

			HarvestStumpageGroups = new List<HarvestStumpageGroup>();
			foreach (var hsg in tempHarvestStumpageGroups)
			{
				var temp = (from h in HarvestStumpageGroups where h.StumpageGroup.StumpageGroupID == hsg.StumpageGroup.StumpageGroupID && h.QualityCodeNumber == hsg.QualityCodeNumber select h).FirstOrDefault();
				if (temp == null)
				{
					HarvestStumpageGroups.Add(hsg);
				}
				else
				{
					if (hsg.ValueType == 1)
					{
						temp.ValueAtHarvest_R1 = hsg.ValueAtHarvest_R1;
						temp.ValueMBF_R1 = hsg.ValueMBF_R1;
						temp.ValueSort_R1 = hsg.ValueSort_R1;
					}
					else if (hsg.ValueType == 2)
					{
						temp.ValueAtHarvest_R2 = hsg.ValueAtHarvest_R2;
					}
					else if (hsg.ValueType == 3)
					{
						temp.ValueAtHarvest_R3 = hsg.ValueAtHarvest_R3;
					}
				}
			}
		}

		private void LoadYears(StumpageR1 r1, StumpageR2 r2, StumpageSEV sev, List<GrowthCuts> growthCuts, decimal overallMax, int minYear)
		{
			var o = growthCuts.Where(uu => uu.MaxValue == overallMax).FirstOrDefault();
			R1Year = o.HarvestYear;
			if (o != null)
			{
				var gc = o.Cuts.Where(uu => uu.Total.RotationOptimum == overallMax).FirstOrDefault();
				if (gc != null)
				{
					R2Year = R1Year + gc.Year;
					decimal maxSEV = 0M;
					R3Year = R2Year + sev.GetMaxSEV(out maxSEV);
				}
			}
		}
	}
}
