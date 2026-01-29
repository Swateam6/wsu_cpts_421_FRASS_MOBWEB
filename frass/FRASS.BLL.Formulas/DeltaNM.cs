using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRASS.BLL.Formulas
{
	public class DeltaNM
	{
		public DeltaNM() { }
		public decimal GetDeltaNM(int startDateYear, int startDateMonth, int endingDateYear, int endingDateMonth)
		{
			decimal startVal = Convert.ToDecimal(startDateYear) + (Convert.ToDecimal(startDateMonth) / 12M);
			decimal endVal = Convert.ToDecimal(endingDateYear) + (Convert.ToDecimal(endingDateMonth) / 12M);
			return endVal - startVal;
		}
	}
}
