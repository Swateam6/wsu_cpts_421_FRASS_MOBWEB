using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRASS.BLL.Prices
{
	public class R1R2Price
	{
		public static decimal CalculateAdjustedPrice(decimal modelPrice, decimal profitAndRisk, decimal overheadAndAdministration)
		{
			var price = modelPrice;
			var pNr = profitAndRisk;
			var oNa = overheadAndAdministration;
			var answer = price * (1 - pNr) - oNa;
			return answer;
		}

		public static decimal CalculateFutureRPA(decimal rpa, int reportYear, int offset, decimal longevity)
		{
			var denominator = longevity * .7M;
			var yearPart = reportYear + offset;
			var power2 = Convert.ToDecimal(Math.Pow(2, Convert.ToDouble(1 - yearPart / denominator)));
			var answer = 1 + (rpa * (yearPart) * power2);
			return answer;
		}

		public static decimal CalculateFutureInflation(decimal rateOfInflation, int reportYear, int offset)
		{
			var datePart = reportYear + offset;
			var answer = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + rateOfInflation), Convert.ToDouble(datePart)));
			return answer;
		}

		public static decimal CalculateFuturePrice(decimal modelPrice, decimal profitAndRisk, decimal overheadAndAdmin, decimal rpa, decimal rateOfInflation, int reportYear, int offset, decimal longevity)
		{
			var price = CalculateAdjustedPrice(modelPrice, profitAndRisk, overheadAndAdmin);
			var futureRpa = CalculateFutureRPA(rpa, reportYear, offset, longevity);
			var inflation = CalculateFutureInflation(rateOfInflation, reportYear, offset);
			var answer = price * futureRpa * inflation;
			return answer;
		}
	}
}
