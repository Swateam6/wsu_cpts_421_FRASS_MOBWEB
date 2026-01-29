using System.Collections.Generic;
using System.Linq;
using FRASS.Interfaces;
using FRASS.DAL;
using System;
using FRASS.BLL.Prices;

namespace FRASS.BLL.Models
{
	public abstract class Stumpage
	{
		protected List<IStandData> StandData;
		protected List<StumpageGroup> StumpageGroups;
		protected List<StumpageGroupQualityCode> StumpageGroupQualityCodes;
		protected List<TimberMarket> TimberMarkets;
		protected StumpageModelPortfolio Portfolio;
		protected Int32 HaulZoneID;
		protected Int32 MinYear;
		protected EconVariables EconVariables;
		protected Parcel Parcel;
		protected Int32 StandID;
		protected List<Int32> Years;

		public Stumpage(Parcel parcel, Int32 standID, StumpageModelPortfolio portfolio, List<StumpageGroup> stumpageGroups, List<IStandData> standData, List<TimberMarket> timberMarkets, Int32 haulZoneID, Int32 minYear, EconVariables econVariables, List<StumpageGroupQualityCode> stumpageGroupQualityCodes, List<Int32> years)
		{
			StandData = standData;
			StumpageGroups = stumpageGroups;
			TimberMarkets = timberMarkets;
			HaulZoneID = haulZoneID;
			Portfolio = portfolio;
			MinYear = minYear;
			EconVariables = econVariables;
			Parcel = parcel;
			StandID = standID;
			StumpageGroupQualityCodes = stumpageGroupQualityCodes;
			Years = years;
		}
		protected decimal GetHaulZonePrice(StumpageModelPortfolioValue stumpageGroupValue)
		{
			if (HaulZoneID == 3)
			{
				if (stumpageGroupValue.Haul3.HasValue)
				{
					return stumpageGroupValue.Haul3.Value;
				}
			}
			else if (HaulZoneID == 4)
			{
				if (stumpageGroupValue.Haul4.HasValue)
				{
					return stumpageGroupValue.Haul4.Value;
				}
			}
			else if (HaulZoneID == 5)
			{
				if (stumpageGroupValue.Haul5.HasValue)
				{
					return stumpageGroupValue.Haul5.Value;
				}
			}
			return 0M;
		}
		protected decimal GetPR(StumpageModelPortfolioValue stumpageGroupValue)
		{
			if (stumpageGroupValue.ProfitAndRisk.HasValue)
			{
				return stumpageGroupValue.ProfitAndRisk.Value / 100M;
			}
			return 0M;
		}
		protected decimal GetONA(StumpageModelPortfolioValue stumpageGroupValue)
		{
			if (stumpageGroupValue.OverheadAndAdmin.HasValue)
			{
				return stumpageGroupValue.OverheadAndAdmin.Value;
			}
			return 0M;
		}
		protected decimal GetRPA(StumpageModelPortfolioRPAData stumpageRPA)
		{
			if (HaulZoneID == 3)
			{
				if (stumpageRPA.Haul3.HasValue)
				{
					return stumpageRPA.Haul3.Value;
				}
			}
			else if (HaulZoneID == 4)
			{
				if (stumpageRPA.Haul4.HasValue)
				{
					return stumpageRPA.Haul4.Value;
				}
			}
			else if (HaulZoneID == 5)
			{
				if (stumpageRPA.Haul5.HasValue)
				{
					return stumpageRPA.Haul5.Value;
				}
			}
			return 0M;
		}
		protected decimal GetLongevity(StumpageModelPortfolioRPAData stumpageRPA)
		{
			if (stumpageRPA.Longevity.HasValue)
			{
				return stumpageRPA.Longevity.Value;
			}
			return 0M;
		}
		public decimal OperableAcres()
		{
			var pr = Parcel.ParcelRiparians.Where(uu => uu.STD_ID == StandID && uu.StandStatID == Convert.ToInt32(StandStats.Operable)).FirstOrDefault();
			if (pr != null)
			{
				return pr.Acres;
			}
			return 0M;
		}
		protected virtual decimal GetBeginningPrice(StumpageGroupIDs stumpageGroup, StumpageGroupQualityCode stumpageGroupQualityCode)
		{
			var stumpageGroupValue = (from port in Portfolio.StumpageModelPortfolioValues where port.StumpageGroupID == Convert.ToInt32(stumpageGroup) && port.StumpageGroupQualityCodeID == stumpageGroupQualityCode.StumpageGroupQualityCodeID select port).FirstOrDefault();
			if (stumpageGroupValue != null)
			{
				return GetHaulZonePrice(stumpageGroupValue);
			}
			return 0M;
		}
		public decimal TotalVolumeHarvested(Int32 reportYear)
		{
			var standDataForYear = StandData.Where(uu => uu.ReportYear == reportYear).ToList<IStandData>();
			List<IStandData> data = new List<IStandData>();
			foreach (var d in standDataForYear)
			{
				var isThere = (from dd in data where dd.ReportYear == d.ReportYear && dd.TimberGradeID == d.TimberGradeID select dd).Any();
				if (!isThere)
				{
					data.Add(d);
				}
			}

			return data.Sum(uu => uu.Board_SN * uu.PctBrd);
		}
		public decimal GetStandT1Vol(Int32 reportYear)
		{
			var acres = OperableAcres();
			var groupVolume = TotalVolumeHarvested(reportYear);
			return (acres * groupVolume) / 1000M;
		}
		protected virtual decimal HaulingMaint(Int32 reportYear)
		{
			var dateDiff = reportYear;
			var rateOfInflation = EconVariables.RateOfInflation;
			var mFee = EconVariables.MaintenanceFee;
			var aFee = EconVariables.AccessFee;
			var haulRoads = 0M;
			var surfaceRoads = 0M;
			var newRoad = 0M;

			if (Parcel.ParcelRoadDistances.MainHaulToPaved.HasValue)
			{
				haulRoads = ((Parcel.ParcelRoadDistances.MainHaulToPaved.Value / 1000) * 0.621371192237334M);
			}
			if (Parcel.ParcelRoadDistances.ToMainHaul.HasValue)
			{
				surfaceRoads = ((Parcel.ParcelRoadDistances.ToMainHaul.Value / 1000) * 0.621371192237334M);
			}
			if (Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
			{
				newRoad = ((Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value / 1000) * 0.621371192237334M);
			}
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(dateDiff)));
			var standT1Vol = GetStandT1Vol(reportYear);
			var leftAddition = aFee + mFee;
			var roads = newRoad + surfaceRoads + haulRoads;
			var answer = ((leftAddition) * (roads) * standT1Vol) * (power);
			return answer;
		}
		protected decimal NewRoad(Int32 reportYear)
		{
			var rateOfInflation = EconVariables.RateOfInflation;
			var roadConstruction = EconVariables.NewRoad;
			var dateDiff = reportYear;
			var newRoad = 0M;
			if (Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
			{
				newRoad = ((Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value / 1000) * 0.621371192237334M);
			}
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(dateDiff)));
			var answer = roadConstruction * newRoad * power;
			return answer;
		}
		protected decimal Reforestation(Int32 reportYear)
		{
			var reforestation = EconVariables.ReforestionCosts;
			var rateOfInflation = EconVariables.RateOfInflation;
			var dateDiff = reportYear;
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(dateDiff)));
			var answer = reforestation * OperableAcres() * power;
			return answer;
		}
		public virtual decimal TotalCosts(Int32 reportYear)
		{
			var haul = HaulingMaint(reportYear);
			var newRoad = NewRoad(reportYear);
			var reforest = Reforestation(reportYear);

			return haul + newRoad + reforest;
		}
		public Int32 GetQualityCodeForYearAndGroup(StumpageGroupIDs stumpageGroup, Int32 reportYear, out Decimal groupVolume)
		{
			var standDataForYear = StandData.Where(uu => uu.ReportYear == reportYear).ToList<IStandData>();
			List<IStandData> data = new List<IStandData>();
			foreach (var d in standDataForYear)
			{
				var isThere = (from dd in data where dd.ReportYear == d.ReportYear && dd.TimberGradeID == d.TimberGradeID select dd).Any();
				if (!isThere)
				{
					data.Add(d);
				}
			}
			var sgroup = (from s in StumpageGroups where s.StumpageGroupID == Convert.ToInt32(stumpageGroup) select s).FirstOrDefault();
			var sg = new StumpageGrader(sgroup, data, TimberMarkets);
			groupVolume = sg.GetTotalVolume();
			return sg.GetQualityCode();
		}	
	}

	public class StumpageR1 : Stumpage
	{
		public StumpageR1(Parcel parcel, Int32 standID, StumpageModelPortfolio portfolio, List<StumpageGroup> stumpageGroups, List<IStandData> standData, List<TimberMarket> timberMarkets, Int32 haulZoneID, Int32 minYear, EconVariables econVariables, List<StumpageGroupQualityCode> stumpageGroupQualityCodes, List<Int32> years)
			: base(parcel, standID, portfolio, stumpageGroups, standData, timberMarkets, haulZoneID, minYear, econVariables, stumpageGroupQualityCodes, years)
		{ }

		public decimal GetPrice(StumpageGroupIDs stumpageGroup, Int32 calendarYear, StumpageGroupQualityCode stumpageGroupQualityCode)
		{
			var initialPrice = GetBeginningPrice(stumpageGroup, stumpageGroupQualityCode);
			var rpas = (from rpa in Portfolio.StumpageModelPortfolioRPADatas where rpa.StumpageGroupID == Convert.ToInt32(stumpageGroup) && rpa.StumpageGroupQualityCodeID == stumpageGroupQualityCode.StumpageGroupQualityCodeID select rpa).FirstOrDefault();
			var vals = (from rpa in Portfolio.StumpageModelPortfolioValues where rpa.StumpageGroupID == Convert.ToInt32(stumpageGroup) && rpa.StumpageGroupQualityCodeID == stumpageGroupQualityCode.StumpageGroupQualityCodeID select rpa).FirstOrDefault();
			if (rpas != null)
			{
				var rpa = GetRPA(rpas);
				var longevity = GetLongevity(rpas);
				var dateDiff = calendarYear - MinYear;
				var rateOfInflation = EconVariables.RateOfInflation;
				var pNr = GetPR(vals);
				var oNa = GetONA(vals);
				if (calendarYear == MinYear)
				{
					return R1R2Price.CalculateAdjustedPrice(initialPrice, pNr, oNa);
				}
				return R1R2Price.CalculateFuturePrice(initialPrice, pNr, oNa, rpa, rateOfInflation, dateDiff, 0, longevity);
			}

			return 0M;

		}

		protected override decimal HaulingMaint(Int32 calendarYear)
		{
			var dateDiff = calendarYear - MinYear;
			var rateOfInflation = EconVariables.RateOfInflation;
			var mFee = EconVariables.MaintenanceFee;
			var aFee = EconVariables.AccessFee;

			var haulRoads = 0M;
			var surfaceRoads = 0M;
			var newRoad = 0M;

			if (Parcel.ParcelRoadDistances.MainHaulToPaved.HasValue)
			{
				haulRoads = ((Parcel.ParcelRoadDistances.MainHaulToPaved.Value / 1000) * 0.621371192237334M);
			}
			if (Parcel.ParcelRoadDistances.ToMainHaul.HasValue)
			{
				surfaceRoads = ((Parcel.ParcelRoadDistances.ToMainHaul.Value / 1000) * 0.621371192237334M);
			}
			if (Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.HasValue)
			{
				newRoad = ((Parcel.ParcelRoadDistances.DistanceToNearestFromParcelBoundary.Value / 1000) * 0.621371192237334M);
			}
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(dateDiff)));
			var standT1Vol = GetStandT1Vol(calendarYear);
			var leftAddition = aFee + mFee;
			var roads = newRoad + surfaceRoads + haulRoads;
			var answer = ((leftAddition) * (roads) * standT1Vol) * (power);
			return answer;
		}
		public override decimal TotalCosts(Int32 calendarYear)
		{
			var reportYear = calendarYear - MinYear;
			var haul = HaulingMaint(calendarYear);
			var newRoad = NewRoad(reportYear);
			var reforest = Reforestation(reportYear);

			return haul + newRoad + reforest;
		}

		public decimal GetStandValue(Int32 calendarYear)
		{
			var totalValue = 0M;
			for (var ict = 1; ict <= 5; ict++)
			{
				var val = 0M;
				GetQualityCodePriceForYearAndGroup((StumpageGroupIDs)ict, calendarYear, out val);
				totalValue = totalValue + val;
			}

			return totalValue * OperableAcres();
		}
		public decimal GetNetFV(Int32 calendarYear)
		{
			var sv = GetStandValue(calendarYear);
			var costs = TotalCosts(calendarYear);
			return sv - costs;
		}
		public decimal GetNPV(Int32 calendarYear)
		{
			var netFV = GetNetFV(calendarYear);
			var dateDiff = calendarYear - MinYear;
			var rateOfInflation = EconVariables.RateOfInflation;
			var dr = EconVariables.RealDiscount;
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation + dr), Convert.ToDouble(dateDiff)));
			var answer = netFV / power;
			return answer;
		}

		public Int32 GetMaxNPV(out decimal MaxNPV)
		{
			Int32 year = 0;
			MaxNPV = -9999M;

			foreach (var y in Years)
			{
				var val = GetNPV(y);
				if (val > MaxNPV)
				{
					MaxNPV = val;
					year = y;
				}
			}
			return year;
		}

		public Int32 GetQualityCodePriceForYearAndGroup(StumpageGroupIDs stumpageGroup, Int32 calendarYear, out Decimal groupPrice)
		{
			var vol = 0M;
			var qc = GetQualityCodeForYearAndGroup(stumpageGroup, calendarYear, out vol);
			var sgqc = StumpageGroupQualityCodes.Where(uu => uu.StumpageGroupID == Convert.ToInt32(stumpageGroup) && uu.QualityCodeNumber == qc).FirstOrDefault();
			var price = GetPrice(stumpageGroup, calendarYear, sgqc);
			groupPrice = vol * price / 1000M;
			return qc;
		}

	}

	public class StumpageR2 : Stumpage
	{
		public StumpageR2(Parcel parcel, Int32 standID, StumpageModelPortfolio portfolio, List<StumpageGroup> stumpageGroups, List<IStandData> standData, List<TimberMarket> timberMarkets, Int32 haulZoneID, Int32 minYear, EconVariables econVariables, List<StumpageGroupQualityCode> stumpageGroupQualityCodes, List<Int32> years)
			: base(parcel, standID, portfolio, stumpageGroups, standData, timberMarkets, haulZoneID, minYear, econVariables, stumpageGroupQualityCodes, years)
		{ }

		public decimal GetPrice(StumpageGroupIDs stumpageGroup, Int32 reportYear, Int32 offset, StumpageGroupQualityCode stumpageGroupQualityCode)
		{
			var initialPrice = GetBeginningPrice(stumpageGroup, stumpageGroupQualityCode);
			var rpas = (from rpa in Portfolio.StumpageModelPortfolioRPADatas where rpa.StumpageGroupID == Convert.ToInt32(stumpageGroup) && rpa.StumpageGroupQualityCodeID == stumpageGroupQualityCode.StumpageGroupQualityCodeID select rpa).FirstOrDefault();
			var vals = (from rpa in Portfolio.StumpageModelPortfolioValues where rpa.StumpageGroupID == Convert.ToInt32(stumpageGroup) && rpa.StumpageGroupQualityCodeID == stumpageGroupQualityCode.StumpageGroupQualityCodeID select rpa).FirstOrDefault();
			if (rpas != null)
			{
				var rpa = GetRPA(rpas);
				var longevity = GetLongevity(rpas);
				var dateDiff = reportYear + offset;
				var rateOfInflation = EconVariables.RateOfInflation;
				var pNr = GetPR(vals);
				var oNa = GetONA(vals);
				if (reportYear == 0)
				{
					return R1R2Price.CalculateAdjustedPrice(initialPrice, pNr, oNa);
				}
				return R1R2Price.CalculateFuturePrice(initialPrice, pNr, oNa, rpa, rateOfInflation, reportYear, offset, longevity);
			}
			return 0M;

		}
		public decimal GetStandValue(Int32 reportYear, Int32 offset)
		{
			var totalValue = 0M;
			for (var ict = 1; ict <= 5; ict++)
			{
				var val = 0M;
				GetQualityCodePriceForYearAndGroup((StumpageGroupIDs)ict, reportYear, offset, out val);
				totalValue = totalValue + val;
			}

			return totalValue * OperableAcres();
		}
		public decimal GetNetFV(Int32 reportYear, Int32 offset)
		{
			var sv = GetStandValue(reportYear, offset);
			var costs = TotalCosts(reportYear);
			var dateDiff = reportYear + offset;
			var rateOfInflation = EconVariables.RateOfInflation;
			if (offset > 0)
			{
				var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(dateDiff)));
				return sv - (costs * power);
			}
			return sv - costs;
		}
		public decimal GetNPV(Int32 reportYear, Int32 offset)
		{
			var netFV = GetNetFV(reportYear, offset);
			var dateDiff = reportYear + offset;
			var rateOfInflation = EconVariables.RateOfInflation;
			var dr = EconVariables.RealDiscount;
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation + dr), Convert.ToDouble(dateDiff)));
			var answer = netFV / power;
			return answer;
		}
		public Int32 GetMaxNPV(Int32 offset, out decimal MaxNPV)
		{
			Int32 year = 0;
			MaxNPV = -9999M;

			foreach (var y in Years)
			{
				var val = GetNPV(y, offset);
				if (val > MaxNPV)
				{
					MaxNPV = val;
					year = y;
				}
			}
			return year;
		}

		public Int32 GetQualityCodePriceForYearAndGroup(StumpageGroupIDs stumpageGroup, Int32 reportYear, Int32 offset, out Decimal groupPrice)
		{
			var vol = 0M;
			var qc = GetQualityCodeForYearAndGroup(stumpageGroup, reportYear, out vol);
			var sgqc = StumpageGroupQualityCodes.Where(uu => uu.StumpageGroupID == Convert.ToInt32(stumpageGroup) && uu.QualityCodeNumber == qc).FirstOrDefault();
			var price = GetPrice(stumpageGroup, reportYear, offset, sgqc);
			groupPrice = vol * price / 1000M;
			return qc;
		}
	}

	public class StumpageSEV : Stumpage
	{
		public StumpageSEV(Parcel parcel, Int32 standID, StumpageModelPortfolio portfolio, List<StumpageGroup> stumpageGroups, List<IStandData> standData, List<TimberMarket> timberMarkets, Int32 haulZoneID, Int32 minYear, EconVariables econVariables, List<StumpageGroupQualityCode> stumpageGroupQualityCodes, List<Int32> years)
			: base(parcel, standID, portfolio, stumpageGroups, standData, timberMarkets, haulZoneID, minYear, econVariables, stumpageGroupQualityCodes, years)
		{ }
		protected override decimal GetBeginningPrice(StumpageGroupIDs stumpageGroup, StumpageGroupQualityCode stumpageGroupQualityCode)
		{
			var stumpageGroupValue = (from port in Portfolio.StumpageModelPortfolioValues where port.StumpageGroupID == Convert.ToInt32(stumpageGroup) && port.StumpageGroupQualityCodeID == stumpageGroupQualityCode.StumpageGroupQualityCodeID select port).FirstOrDefault();
			if (stumpageGroupValue != null)
			{
				var price = GetHaulZonePrice(stumpageGroupValue);
				var pr = GetPR(stumpageGroupValue) / 100M;
				var ona = GetONA(stumpageGroupValue);

				return (price * (1 - pr)) - ona;
			}
			return 0M;
		}

		public decimal GetPrice(StumpageGroupIDs stumpageGroup, Int32 reportYear, StumpageGroupQualityCode stumpageGroupQualityCode)
		{
			var initialPrice = GetBeginningPrice(stumpageGroup, stumpageGroupQualityCode);
			if (reportYear == 0)
			{
				return initialPrice;
			}

			var rateOfInflation = EconVariables.RateOfInflation;
			var rightPower = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(reportYear)));
			var answer = initialPrice * rightPower;
			return answer;
		}

		public decimal GetStandValue(Int32 reportYear)
		{
			var totalValue = 0M;
			for (var ict = 1; ict <= 5; ict++)
			{
				var val = 0M;
				GetQualityCodePriceForYearAndGroup((StumpageGroupIDs)ict, reportYear, out val);
				totalValue = totalValue + val;
			}

			return totalValue * OperableAcres();
		}
		public decimal GetNetFV(Int32 reportYear)
		{
			var sv = GetStandValue(reportYear);
			var costs = TotalCosts(reportYear);
			return sv - costs;
		}
		public decimal GetSEV(Int32 reportYear)
		{
			var netFV = GetNetFV(reportYear);
			var dateDiff = reportYear;
			var rateOfInflation = EconVariables.RateOfInflation;
			var dr = EconVariables.RealDiscount;
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation + dr), Convert.ToDouble(dateDiff)));
			var answer = netFV / (power - 1);
			return answer;
		}
		public decimal GetSEVRedux(Int32 reportYear)
		{
			var sev = 0M;
			var sevYear = GetMaxSEV(out sev);
			var rateOfInflation = EconVariables.RateOfInflation;
			var dr = EconVariables.RealDiscount;
			var leftpower = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(reportYear)));
			var rightpower = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation + dr), Convert.ToDouble(reportYear)));
			var answer = sev * (leftpower / rightpower);
			return answer;
		}
		public Int32 GetMaxSEV(out decimal MaxSEV)
		{
			Int32 year = 0;
			MaxSEV = -9999M;

			foreach (var y in Years)
			{
				var val = GetSEV(y);
				if (val > MaxSEV)
				{
					MaxSEV = val;
					year = y;
				}
			}
			return year;
		}

		public Int32 GetQualityCodePriceForYearAndGroup(StumpageGroupIDs stumpageGroup, Int32 reportYear, out Decimal groupPrice)
		{
			var vol = 0M;
			var qc = GetQualityCodeForYearAndGroup(stumpageGroup, reportYear, out vol);
			var sgqc = StumpageGroupQualityCodes.Where(uu => uu.StumpageGroupID == Convert.ToInt32(stumpageGroup) && uu.QualityCodeNumber == qc).FirstOrDefault();
			var price = GetPrice(stumpageGroup, reportYear, sgqc);
			groupPrice = vol * price / 1000M;
			return qc;
		}
	}

	public enum StumpageGroupIDs
	{
		DF = 1,
		WW = 2,
		WR = 3,
		RA = 4,
		BM = 5
	}
}
