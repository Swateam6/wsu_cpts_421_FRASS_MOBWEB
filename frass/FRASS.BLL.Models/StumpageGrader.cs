using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.Interfaces;

namespace FRASS.BLL.Models
{
	public class StumpageGrader
	{
		StumpageGroup StumpageGroup;
		IEnumerable<IStandData> StandData;
		List<TimberMarket> TimberMarkets;

		public StumpageGrader(StumpageGroup stumpageGroup, List<IStandData> standData, List<TimberMarket> timberMarkets)
		{
			StumpageGroup = stumpageGroup;
			var species = StumpageGroup.StumpageGroupSpecies.Select(uu => uu.Specy);
			StandData = from s in standData where (from sp in species select sp.SpeciesID).Contains(s.SpeciesID) select s;
			TimberMarkets = timberMarkets;
		}

		public Decimal GetTotalVolume()
		{
			var totalVolume = 0M;
			var boardsn = 0M;
			foreach (var d in StandData)
			{
				boardsn = d.Board_SN;
				totalVolume = totalVolume + d.PctBrd;
			}
			return totalVolume * boardsn;
		}

		public Int32 GetQualityCode()
		{
			var qualitycode = 1;
			
			if (StumpageGroup.StumpageGroupID == 1)
			{
				qualitycode = GetDF();
			}
			else if (StumpageGroup.StumpageGroupID == 2)
			{
				qualitycode = GetWW();
			}
			else if (StumpageGroup.StumpageGroupID == 4)
			{
				qualitycode = GetRA();
			}
			return qualitycode;
		}

		public Int32 GetDF()
		{
			var qualitycode = 4;

			var totalVolume = GetTotalVolume();

			if (totalVolume > 0)
			{ 

				var tm2Saw = (from tm in TimberMarkets where tm.TimberMarketID == 2 select tm).FirstOrDefault();
				var betterTM2Saw = from tm in TimberMarkets where tm.OrderID <= tm2Saw.OrderID select tm;
				var vol2Saw = from s in StandData where (from b in betterTM2Saw select b.TimberMarketID).Contains(s.TimberMarketID) select s;

				var Saw2Volume = 0M;
				foreach(var d in vol2Saw)
				{
					Saw2Volume = d.PctBrd * d.Board_SN + Saw2Volume;			
				}

				var smBetter = (from tm in TimberMarkets where tm.TimberMarketID == 11 select tm).FirstOrDefault();
				var betterSM = from tm in TimberMarkets where tm.OrderID <= smBetter.OrderID select tm;
				var volXport = from s in StandData where (from b in betterSM select b.TimberMarketID).Contains(s.TimberMarketID) select s;

				var XportVolume = 0M;
				foreach (var d in volXport)
				{
					XportVolume = d.PctBrd * d.Board_SN + XportVolume;
				}

				var saw2 = (Saw2Volume / totalVolume) * 100;
				var xport = (XportVolume / totalVolume) * 100;

				if (saw2 >= 50 && xport >= 15)
				{
					qualitycode = 1;
				}
				else if (saw2 >= 50 && xport < 15)
				{
					qualitycode = 2;
				}
				else if (saw2 >= 25)
				{
					qualitycode = 3;
				}
			}
			return qualitycode;
		}
		public Int32 GetWW()
		{
			var qualitycode = 4;
			var totalVolume = GetTotalVolume();
			if (totalVolume > 0)
			{
				var tm2Saw = (from tm in TimberMarkets where tm.TimberMarketID == 2 select tm).FirstOrDefault();
				var betterTM2Saw = from tm in TimberMarkets where tm.OrderID <= tm2Saw.OrderID select tm;
				var vol2Saw = from s in StandData where (from b in betterTM2Saw select b.TimberMarketID).Contains(s.TimberMarketID) select s;

				var Saw2Volume = 0M;
				foreach (var d in vol2Saw)
				{
					Saw2Volume = d.PctBrd * d.Board_SN + Saw2Volume;
				}

				var smBetter = (from tm in TimberMarkets where tm.TimberMarketID == 11 select tm).FirstOrDefault();
				var betterSM = from tm in TimberMarkets where tm.OrderID <= smBetter.OrderID select tm;
				var volXport = from s in StandData where (from b in betterSM select b.TimberMarketID).Contains(s.TimberMarketID) select s;

				var XportVolume = 0M;
				foreach (var d in volXport)
				{
					XportVolume = d.PctBrd * d.Board_SN + XportVolume;
				}

				var saw2 = (Saw2Volume / totalVolume) * 100;
				var xport = (XportVolume / totalVolume) * 100;

				if (saw2 >= 50 && xport >= 5)
				{
					qualitycode = 1;
				}
				else if (saw2 >= 50 && xport < 5)
				{
					qualitycode = 2;
				}
				else if (saw2 >= 25)
				{
					qualitycode = 3;
				}
			}
			return qualitycode;
		}
		public Int32 GetRA()
		{
			var qualitycode = 2;
			var totalVolume = GetTotalVolume();
			if (totalVolume > 0)
			{
				var tm3Saw = (from tm in TimberMarkets where tm.TimberMarketID == 3 select tm).FirstOrDefault();
				var betterTM3Saw = from tm in TimberMarkets where tm.OrderID <= tm3Saw.OrderID select tm;
				var vol3Saw = from s in StandData where (from b in betterTM3Saw select b.TimberMarketID).Contains(s.TimberMarketID) select s;

				var Saw3Volume = 0M;
				foreach (var d in vol3Saw)
				{
					Saw3Volume = d.PctBrd * d.Board_SN + Saw3Volume;
				}

				var saw3 = (Saw3Volume / totalVolume) * 100;

				if (saw3 >= 40)
				{
					qualitycode = 1;
				}
			}
			return qualitycode;
		}
	}
}