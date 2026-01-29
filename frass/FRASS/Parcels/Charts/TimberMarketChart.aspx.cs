using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Formulas;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Charting;

namespace FRASS.WebUI.Parcels.Charts
{
	public partial class TimberMarketChart : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				LoadChart();
			}
		}


		private List<Color> GetColors()
		{
			var list = new List<Color>();
			list.Add(Color.FromArgb(51, 102, 153));
			list.Add(Color.FromArgb(255, 102, 0));
			list.Add(Color.FromArgb(47, 136, 134));
			list.Add(Color.FromArgb(187, 102, 51));
			list.Add(Color.FromArgb(43, 170, 115));
			list.Add(Color.FromArgb(49, 119, 143));
			list.Add(Color.FromArgb(28, 119, 209));
			list.Add(Color.FromArgb(141, 187, 232));
			list.Add(Color.FromArgb(45, 153, 124));
			return list;
		}

		private void LoadChart()
		{
			RadChart1.Clear();
			var marketModelPortfolioID = Convert.ToInt32(Request.QueryString["MarketModelPortfolioID"].ToString());
			var logMarketReportSpeciesID = Convert.ToInt32(Request.QueryString["LogMarketReportSpeciesID"].ToString());
			var rpaPortfolioID = Convert.ToInt32(Request.QueryString["RPAPortfolioID"].ToString());
			var timberMarketID = Convert.ToInt32(Request.QueryString["TimberMarketID"].ToString());

			DeliveredLogMarketModelDataManager db = DeliveredLogMarketModelDataManager.GetInstance();
			ParcelDataManager dbParcel = ParcelDataManager.GetInstance();
			RPAPortfolioDataManager dbRPA = RPAPortfolioDataManager.GetInstance();
			TimberDataManager dbTimber = TimberDataManager.GetInstance();
			var portfolio = db.GetMarketModelPortfolio(marketModelPortfolioID);
			var rpaPortfolio = dbRPA.GetRPAPortfolio(rpaPortfolioID);
			var logMarketReportSpecy = dbParcel.GetLogMarketReportSpecy(logMarketReportSpeciesID);
			var marketModelData = db.GetMarketModelData().Where(uu=>uu.MarketModelTypeID == 3);

			var tms = portfolio.MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID).Select(uu => uu.TimberMarket).Distinct();
			
			var timberMarket = tms.Where(uu=>uu.TimberMarketID == timberMarketID).FirstOrDefault();
			if (timberMarket != null)
			{
				var details = rpaPortfolio.RPAPortfolioDetails.Where(uu => uu.TimberMarketID == timberMarketID && uu.LogMarketReportSpeciesID == logMarketReportSpeciesID).FirstOrDefault();
				var allLogPrices = dbTimber.GetHistoricLogPrices(logMarketReportSpeciesID, timberMarketID);
				var startYear = details.BeginningDate.Year;
				var endYear = details.EndingDate.Year;
				var startMonth = details.BeginningDate.Month;
				var endMonth = details.EndingDate.Month;
				var filterYears = allLogPrices.Where(uu => uu.Year >= startYear && uu.Year <= endYear);
				var filterStartMonth = filterYears.Where(uu => (uu.Year > startYear || (uu.Year == startYear && uu.Month >= startMonth)));
				var logPrices = filterStartMonth.Where(uu => (uu.Year < endYear || (uu.Year == endYear && uu.Month <= endMonth)));

				var chartSeriesCollectionReal = new ChartSeriesItemsCollection();
				var chartSeriesCollectionNominal = new ChartSeriesItemsCollection();
				var firstElement = logPrices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month).FirstOrDefault();
				var lastElement = logPrices.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Month).FirstOrDefault();
				var minValue = Convert.ToDouble(firstElement.Year);
				var maxValue = Convert.ToDouble(lastElement.Year + 1);
				
				foreach (var price in logPrices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month))
				{
					var item = new ChartSeriesItem();
					item.XValue = Convert.ToDouble(price.Year) + Convert.ToDouble(price.Month / 12M);
					item.YValue = Convert.ToDouble(price.Price);
					chartSeriesCollectionNominal.Add(item);

					var itemReal = new ChartSeriesItem();
					var rpaReal = new RPARealValue();
					
					var startPPI = marketModelData.Where(uu=>uu.Year == price.Year && uu.Period == price.Month).Select(uu=>uu.Value).FirstOrDefault();
					var endPPI = marketModelData.Where(uu=>uu.Year == lastElement.Year && uu.Period == lastElement.Month).Select(uu=>uu.Value).FirstOrDefault();
					var realPrice = rpaReal.GetRPARealValue(price.Price,
							new DateTime(price.Year, price.Month, 1),
							new DateTime(lastElement.Year, lastElement.Month, 1),
							startPPI,
							endPPI
						);
					itemReal.XValue = Convert.ToDouble(price.Year) + Convert.ToDouble(price.Month / 12M);
					itemReal.YValue = Convert.ToDouble(realPrice);
					chartSeriesCollectionReal.Add(itemReal);
				}

				var chartSeriesReal = new ChartSeries(timberMarket.Market + " Real");
				chartSeriesReal.Appearance.LineSeriesAppearance.Color = GetColors()[1];
				chartSeriesReal.Items.AddRange(chartSeriesCollectionReal);
				chartSeriesReal.Type = ChartSeriesType.Line;
				chartSeriesReal.Appearance.LabelAppearance.Visible = false;
				RadChart1.AddChartSeries(chartSeriesReal);

				var chartSeriesNominal = new ChartSeries(timberMarket.Market + " Nominal");
				chartSeriesNominal.Appearance.LineSeriesAppearance.Color = GetColors()[0];
				chartSeriesNominal.Items.AddRange(chartSeriesCollectionNominal);
				chartSeriesNominal.Type = ChartSeriesType.Line;
				chartSeriesNominal.Appearance.LabelAppearance.Visible = false;
				RadChart1.AddChartSeries(chartSeriesNominal);

				RadChart1.ChartTitle.TextBlock.Text = logMarketReportSpecy.LogMarketSpecies + " " + timberMarket.Market + " Nominal and Real Values (COMING SOON)"; // +rpaPortfolio.RPAPortfolioLoggingCostRealPriceAppreciations.EndingDate.ToShortDateString() + ")";
				RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font = new Font(RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Name, 14, RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Unit);
				RadChart1.Width = new Unit("800px");
				RadChart1.Height = new Unit("600px");
				RadChart1.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Date";
				RadChart1.PlotArea.XAxis.AxisLabel.Visible = true;
				RadChart1.PlotArea.YAxis.Appearance.CustomFormat = "$#,###";
				RadChart1.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Value per MBF";
				RadChart1.PlotArea.YAxis.AxisLabel.Visible = true;
				RadChart1.PlotArea.XAxis.AutoScale = false;
				RadChart1.PlotArea.XAxis.AddRange(minValue, maxValue, 2);

				RadChart1.DataBind();
			}
			
		}
	}
}