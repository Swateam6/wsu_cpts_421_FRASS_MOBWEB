using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.Interfaces;
using FRASS.DAL.DataManager;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
	public class HarvestReportItem
	{
		public TimberMarket TimberMarket { get; set; }
		public bool HasValue { get; set; }
		public decimal ValueAtHarvest_R1 { get; set; }
		public decimal ValueMBF_R1 { get; set; }
		public decimal ValueSort_R1 { get; set; }
		public decimal ValueAtHarvest_R2 { get; set; }
		public decimal ValueAtHarvest_R3 { get; set; }

		public HarvestReportItem(decimal todaysValue, Specy specy, RotationGenerator rotationGenerator, List<TimberGrade> grades, TimberMarket timberMarket, List<IStandData> standDataCurrent, List<IStandData> standDataFuture, int cutYears, int currentYear, int reportYear, R2 r2SEV, int optimalYear, decimal acres, decimal rateOfInflation, decimal pr, decimal rpa, decimal longevity)
		{
			TimberMarket = timberMarket;
			HasValue = false;
			if (LoadR1(todaysValue, specy, rotationGenerator, grades, timberMarket, standDataCurrent, currentYear, reportYear, acres, rpa, longevity, pr, rateOfInflation))
			{
				HasValue = true;
			}
			decimal valueAtHarvest_R2;
			if (LoadR2(specy, grades, timberMarket, standDataFuture, out valueAtHarvest_R2, optimalYear, acres))
			{
				ValueAtHarvest_R2 = valueAtHarvest_R2;
				HasValue = true;
			}
			decimal valueAtHarvest_R3;
			if (LoadSEV(specy, grades, timberMarket, rotationGenerator.SEV, standDataFuture, out valueAtHarvest_R3, acres))
			{
				ValueAtHarvest_R3 = valueAtHarvest_R3;
				HasValue = true;
			}
		}

		private bool LoadR1(decimal todaysValue, Specy specy, RotationGenerator rotationGenerator, List<TimberGrade> grades, TimberMarket timberMarket, List<IStandData> standDataCurrent, int currentYear, int reportYear, decimal acres, decimal rpa, decimal longevity, decimal pr, decimal rateOfInflation)
		{
			var dirty = false;
			decimal vol = 0M;
			decimal sort = 0M;
			decimal mbf = 0M;

			var tgrades = (from t in grades where t.TimberMarketID == timberMarket.TimberMarketID && t.SpeciesID == specy.SpeciesID select t.TimberGradeID).Distinct();
			foreach (var grade in tgrades)
			{
				var dataCurrent = (standDataCurrent.Where(uu => uu.TimberGradeID == grade && uu.ReportYear == reportYear)).FirstOrDefault();
				if (dataCurrent != null)
				{
					dirty = true;
					var dif = reportYear - currentYear;
					var volume = dataCurrent.Board_SN * dataCurrent.PctBrd;
					volume = volume * acres;

					var futurerpa = TimberSortValue.GetFutureRPA(rpa, dif, longevity);
					var adjustedValue = todaysValue * (1 - pr);
					var rpaInflation = TimberSortValue.GetInflationRPA(futurerpa, rateOfInflation, dif);
					var valMBF = rpaInflation * adjustedValue;
					vol = vol + volume;
					mbf = valMBF;
					sort = sort + (volume * valMBF) / 1000M;
				}
			}
			ValueAtHarvest_R1 = vol;
			ValueSort_R1 = sort;
			ValueMBF_R1 = mbf;
			return dirty;
		}
		private bool LoadR2(Specy specy, List<TimberGrade> grades, TimberMarket timberMarket, List<IStandData> standDataFuture, out decimal valVolume, int optimalYear, decimal acres)
		{
			var dirty = false;
			valVolume = 0M;
			var tgrades = from t in grades where t.TimberMarketID == timberMarket.TimberMarketID && t.SpeciesID == specy.SpeciesID select t;
			foreach (var grade in tgrades)
			{
				var dataFuture = standDataFuture.Where(uu => uu.TimberGradeID == grade.TimberGradeID && uu.ReportYear == optimalYear).FirstOrDefault();

				if (dataFuture != null)
				{
					dirty = true;
					var volume = dataFuture.Board_SN * dataFuture.PctBrd;
					volume = volume * acres;
					valVolume = valVolume + volume;
				}
			}
			return dirty;
		}
		private bool LoadSEV(Specy specy, List<TimberGrade> grades, TimberMarket timberMarket, SEV sev, List<IStandData> standDataFuture, out decimal valVolume, decimal acres)
		{
			var dirty = false;
			valVolume = 0M;

			var opt = sev.GetSEVRotationOptimum();
			var tgrades = from t in grades where t.TimberMarketID == timberMarket.TimberMarketID && t.SpeciesID == specy.SpeciesID select t;
			foreach (var grade in tgrades)
			{

				var dataFuture = standDataFuture.Where(uu => uu.TimberGradeID == grade.TimberGradeID && uu.ReportYear == opt.Year).FirstOrDefault();
				if (dataFuture != null)
				{
					dirty = true;
					var volume = dataFuture.Board_SN * dataFuture.PctBrd;
					volume = volume * acres;
					valVolume = valVolume + volume;
				}
			}
			return dirty;
		}
	}
}
