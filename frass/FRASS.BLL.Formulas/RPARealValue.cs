using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRASS.BLL.Formulas
{
	public class RPARealValue
	{
		public RPARealValue() { }
		public decimal GetRPARealValue(decimal price, DateTime startDate, DateTime endDate, decimal startPPI, decimal endPPI)
		{
			var deltaNM = new DeltaNM();
			var F = new f();
			var fValue = F.GetF(startDate, endDate, startPPI, endPPI);
			var dNM = Convert.ToDouble(deltaNM.GetDeltaNM(startDate.Year, startDate.Month, endDate.Year, endDate.Month));
			var baseValue = Convert.ToDouble(1M + fValue);
			var rightSide = Convert.ToDecimal(Math.Pow(baseValue, dNM));
			return price * rightSide;
		}

		public decimal GetRPA(decimal startPrice, decimal endPrice, decimal longevity)
		{
			decimal fraction = 0;
			if (startPrice != 0)
			{
                fraction = endPrice / startPrice;
            }
			
			var power = 1M / longevity;
			return Convert.ToDecimal(Math.Pow(Convert.ToDouble(fraction), Convert.ToDouble(power))) - 1M;
		}

		public decimal GetRPAValueAtYear(decimal initialPrice, decimal rpa, decimal longevity, decimal endLongevity)
		{
			var naturalLogOf2 = Convert.ToDecimal(Math.Log(2)); //.69315M 
			var exponent = 1 - (endLongevity) / (longevity * naturalLogOf2);

			var power = Convert.ToDecimal(Math.Pow(2, Convert.ToDouble(exponent)));
			return initialPrice * (1 + (rpa * endLongevity * power));
		}

		public decimal FutureValue(decimal rpaValue, decimal inflation)
		{
			var power = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1M + inflation), 2));
			return rpaValue * power;
		}

		public decimal GetNominalValueFromReal(decimal realValue, decimal ppiNominalDate, decimal ppiToday)
		{
			return realValue * (ppiNominalDate / ppiToday);
		}

		public decimal GetRealValueFromTodaysPPI(decimal nominalValue, decimal ppiNominalDate, decimal ppiToday)
		{
			return nominalValue * (ppiToday / ppiNominalDate);
		}
	}
}
