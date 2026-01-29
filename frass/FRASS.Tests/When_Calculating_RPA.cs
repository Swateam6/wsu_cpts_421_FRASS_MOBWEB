using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRASS.BLL.Formulas;
using MbUnit.Framework;

namespace FRASS.Tests
{
	[TestFixture]
	public class When_Calculating_RPA
	{
		[Test]
		[Row(1989, 1, 2013, 3, 110.5, 204.1, .025715)]
		[Row(1989, 12, 2013, 3, 113, 204.1, .025755)]
		[Row(2001, 9, 2013, 3, 133.3, 204.1, .037739)]
		public void GetTheFValue(int startDateYear, int startDateMonth, int endingDateYear, int endingDateMonth, decimal startPPI, decimal endPPI, decimal expectedValue)
		{
			var f = new f();
			var startDate = new DateTime(startDateYear, startDateMonth, 1);
			var endDate = new DateTime(endingDateYear, endingDateMonth, 1);
			var result = f.GetF(startDate, endDate, startPPI, endPPI);
			Assert.AreApproximatelyEqual(expectedValue, result, .001M);
		}

		[Test]
		[Row(1989,12,2012,12,23)]
		[Row(1989, 1, 2013, 3, 24.16666)]
		[Row(1992, 3, 2013, 3, 21)]
		public void GetDeltaNM(int startDateYear, int startDateMonth, int endingDateYear, int endingDateMonth, decimal expectedValue)
		{
			var deltaNM = new DeltaNM();
			var result = deltaNM.GetDeltaNM(startDateYear, startDateMonth, endingDateYear, endingDateMonth);
			Assert.AreApproximatelyEqual(expectedValue, result, .01M);
		}

		[Test]
		[Row(286, 1989, 1, 2013, 3, 110.5, 204.1, 528.26)]
		[Row(361, 1989, 12, 2013, 3, 113, 204.1, 652.04)]
		[Row(482, 2001, 9, 2013, 3, 133.3, 204.1, 738.01)]
		public void GetRPARealValue(decimal price, int startDateYear, int startDateMonth, int endingDateYear, int endingDateMonth, decimal startPPI, decimal endPPI, decimal expectedValue)
		{
			var rPARealValue = new RPARealValue();
			var startDate = new DateTime(startDateYear, startDateMonth, 1);
			var endDate = new DateTime(endingDateYear, endingDateMonth, 1);
			var result = rPARealValue.GetRPARealValue(price, startDate, endDate, startPPI, endPPI);
			Assert.AreApproximatelyEqual(expectedValue, result, .01M);
		}

		[Test]
		[Row(739.43,420.92,3,-.1712297)]
		public void GetRPA(decimal startPrice, decimal endPrice, decimal longevity, decimal results)
		{
			var rPARealValue = new RPARealValue();
			var result = rPARealValue.GetRPA(startPrice, endPrice, longevity);
			Assert.AreApproximatelyEqual(results, result, .01M);
		}

		[Test]
		[Row(739.43, -.1712297,3,9.25,632.14)]
		public void GetRPAValueAtYear(decimal initialPrice, decimal rpa, decimal longevity, decimal endLongevity, decimal expectedResults)
		{
			var rPARealValue = new RPARealValue();
			var results = rPARealValue.GetRPAValueAtYear(initialPrice, rpa, longevity, endLongevity);
			Assert.AreApproximatelyEqual(expectedResults, results, .01M);
		}

		[Test]
		[Row(632.14, .0345, 676.51)]
		public void FutureValue(decimal rpaValue, decimal inflation, decimal expectedResults)
		{
			var rPARealValue = new RPARealValue();
 			var results = rPARealValue.FutureValue(rpaValue, inflation);
			Assert.AreApproximatelyEqual(expectedResults, results, .01M);
		}
	}
}
