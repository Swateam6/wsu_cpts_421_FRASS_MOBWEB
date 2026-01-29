using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;

namespace FRASS.BLL.Models
{
	public class EconVariables
	{
		public decimal RateOfInflation { get; private set; }
		public decimal RealDiscount { get; private set; }
		public int ReforestionCosts { get; private set; }
		public decimal AccessFee { get; private set; }
		public decimal MaintenanceFee { get; private set; }
		public int NewRoad { get; private set; }

		private readonly RPAPortfolio RPA;

		public EconVariables(MarketModelPortfolio portfolio, RPAPortfolio rpa)
		{
			RPA = rpa;
			RateOfInflation = portfolio.MarketModelPortfolioInflationDetails.InflationRate;
			RealDiscount = portfolio.MarketModelPortfolioInflationDetails.LandownerDiscountRate;
			ReforestionCosts = portfolio.MarketModelPortfolioCosts.ReforestationCost;
			AccessFee = portfolio.MarketModelPortfolioCosts.AccessFeeTimber;
			MaintenanceFee = portfolio.MarketModelPortfolioCosts.MaintenanceFeeTimberHaul;
			NewRoad = portfolio.MarketModelPortfolioCosts.RoadConstructionCosts;
		}

		public EconVariables(StumpageModelPortfolio portfolio)
		{
			RateOfInflation = portfolio.StumpageModelPortfolioInflationDetails.InflationRate;
			RealDiscount = portfolio.StumpageModelPortfolioInflationDetails.LandownerDiscountRate;
			ReforestionCosts = portfolio.StumpageModelPortfolioCosts.ReforestationCost;
			AccessFee = portfolio.StumpageModelPortfolioCosts.AccessFeeTimber;
			MaintenanceFee = portfolio.StumpageModelPortfolioCosts.MaintenanceFeeTimberHaul;
			NewRoad = portfolio.StumpageModelPortfolioCosts.RoadConstructionCosts;
		}

		public decimal GetRPA(LogMarketReportSpecy logMarketReportSpecy, TimberMarket timberMarket)
		{
			return GetRPA(logMarketReportSpecy.LogMarketReportSpeciesID, timberMarket.TimberMarketID);
		}

		public decimal GetRPA(int logMarketReportSpeciesID, int timberMarketID)
		{
			var detail =  RPA.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).FirstOrDefault();
			if (detail != null)
			{
				return detail.RPA;	 
			}
			return decimal.Zero;
		}

		public decimal GetLongevity(LogMarketReportSpecy logMarketReportSpecy, TimberMarket timberMarket)
		{
			return GetLongevity(logMarketReportSpecy.LogMarketReportSpeciesID, timberMarket.TimberMarketID);
		}

		public decimal GetLongevity(int logMarketReportSpeciesID, int timberMarketID)
		{
			var detail = RPA.RPAPortfolioDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID && uu.TimberMarketID == timberMarketID).FirstOrDefault();
			if (detail != null)
			{
				return detail.Longevity;
			}
			return decimal.Zero;
		}

	}
}