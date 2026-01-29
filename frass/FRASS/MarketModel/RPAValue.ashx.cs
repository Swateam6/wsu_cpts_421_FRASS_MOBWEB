using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.BLL.Formulas;
using FRASS.DAL.DataManager;

namespace FRASS.WebUI.MarketModel
{
	/// <summary>
	/// Summary description for RPACost
	/// </summary>
	public class RPAValue : IHttpHandler
	{


		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			context.Response.Write(GetRPAValue(context));
		}

		public string GetRPAValue(HttpContext context)
		{
			var dbTimberDataManager = TimberDataManager.GetInstance();
			var dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			var timberMarkets = dbTimberDataManager.GetTimberMarkets();
			var marketModelData = dbDeliveredLogMarketModelDataManager.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3);

			var rpaData = new RPAData();
			var deltaNM = new DeltaNM();

			var rpa = new RPARealValue();
			var startDate = Convert.ToDateTime(context.Request.Form["startDate"]);
			var endingDate = Convert.ToDateTime(context.Request.Form["endingDate"]);
			var logMarketReportSpeciesID = Convert.ToInt32(context.Request.Form["logMarketReportSpeciesID"]);
			var timberMarketID = Convert.ToInt32(context.Request.Form["timberMarketID"]);

			decimal realPrice1 = decimal.Zero;
			decimal realPrice2 = decimal.Zero;
			var rpaREAL = new RPARealValue();

			if (context.Request.QueryString["WithPrice"].ToString() == "1")
			{
				Decimal.TryParse(context.Request.Form["startingPrice"], out realPrice1);
				Decimal.TryParse(context.Request.Form["endingPrice"], out realPrice2);
			}
			if (realPrice1 == decimal.Zero || realPrice2 == decimal.Zero)
			{
				var startPrice = 0M;
				var endPrice = 0M;

				var allLogPrices = dbTimberDataManager.GetHistoricLogPrices(logMarketReportSpeciesID, timberMarketID);
				var logPrice = allLogPrices.Where(uu => uu.Year == startDate.Year && uu.Month == startDate.Month).FirstOrDefault();
				if (logPrice != null)
				{
					startPrice = logPrice.Price;
				}
				var logPrice2 = allLogPrices.Where(uu => uu.Year == endingDate.Year && uu.Month == endingDate.Month).FirstOrDefault();
				if (logPrice2 != null)
				{
					endPrice = logPrice2.Price;
				}

				var startingPPI = marketModelData.Where(uu => uu.MarketModelTypeID == 3 && uu.Year == startDate.Year && uu.Period == startDate.Month).FirstOrDefault();
				var endingPPI = marketModelData.Where(uu => uu.MarketModelTypeID == 3 && uu.Year == endingDate.Year && uu.Period == endingDate.Month).FirstOrDefault();
				var currentPPI = marketModelData.Where(uu => uu.MarketModelTypeID == 3).OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault();

				if (logPrice == null)
				{
					realPrice1 = decimal.Zero;
				}
				else
				{
					realPrice1 = rpaREAL.GetRPARealValue(startPrice,
								new DateTime(startDate.Year, startDate.Month, 1),
								new DateTime(endingDate.Year, endingDate.Month, 1),
								startingPPI.Value,
								currentPPI.Value
							);
				}

				if (logPrice2 == null)
				{
					realPrice2 = decimal.Zero;
				}
				else
				{
					realPrice2 = rpaREAL.GetRPARealValue(endPrice,
									new DateTime(startDate.Year, startDate.Month, 1),
									new DateTime(endingDate.Year, endingDate.Month, 1),
									endingPPI.Value,
									currentPPI.Value
								);
				}
			}
			



			var longevity = deltaNM.GetDeltaNM(startDate.Year, startDate.Month, endingDate.Year, endingDate.Month);
			rpaData.Longevity = longevity.ToString("N2");

			if (realPrice1 == decimal.Zero || realPrice2 == decimal.Zero)
			{
				rpaData.RPA = "----";
			}
			else
			{
				rpaData.RPA = rpaREAL.GetRPA(realPrice1, realPrice2, longevity).ToString("N4");
			}
			
			rpaData.BeginningRealValue = realPrice1.ToString("F2");
			rpaData.EndingRealValue = realPrice2.ToString("F2");

			return JSONHelper.Serialize(rpaData);
		}

		private class RPAData
		{
			public string BeginningRealValue { get; set; }
			public string EndingRealValue { get; set; }
			public string RPA { get; set; }
			public string Longevity { get; set; }
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}