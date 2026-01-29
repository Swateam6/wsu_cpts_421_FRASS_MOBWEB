using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRASS.Interfaces;

namespace FRASS.BLL.Formulas
{
	public class f
	{
		public f() {}
		public decimal GetF(DateTime startDate, DateTime endDate, decimal startPPI, decimal endPPI)
		{
			var baseValue = GetBase(startPPI, endPPI);
			var exponent = GetExponent(startDate, endDate);
			var raisedValue = Convert.ToDecimal(Math.Pow(baseValue, exponent));
			return raisedValue - 1M;
		}

		internal double GetBase(decimal startPPI, decimal endPPI)
		{
			return Convert.ToDouble(endPPI) / Convert.ToDouble(startPPI);
		}

		private double GetExponent(DateTime startDate, DateTime endDate)
		{
			var deltaNM = new DeltaNM();
			var dNM = deltaNM.GetDeltaNM(startDate.Year, startDate.Month, endDate.Year, endDate.Month);
			return Convert.ToDouble(1M) / Convert.ToDouble(dNM);
		}
	}
}
