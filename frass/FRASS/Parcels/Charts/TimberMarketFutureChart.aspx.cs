using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Formulas;
using FRASS.DAL.DataManager;
using Telerik.Charting;

namespace FRASS.WebUI.Parcels.Charts
{
	public partial class TimberMarketFutureChart : System.Web.UI.Page
	{
		private DeltaNM deltaNM;
		private RPARealValue rpaReal;
		private decimal MAXPrice = 0M;
		
		protected void Page_Load(object sender, EventArgs e)
		{
			deltaNM = new DeltaNM();
			rpaReal = new RPARealValue();
			if (!Page.IsPostBack)
			{
				LoadChart();
			}
		}
		
		private void LoadChart()
		{
			RadChart1.Clear();
			var logMarketReportSpeciesID = Convert.ToInt32(Request.QueryString["LogMarketReportSpeciesID"].ToString());
			var timberMarketID = Convert.ToInt32(Request.QueryString["TimberMarketID"].ToString());
			var startDate = Convert.ToDateTime(Request.QueryString["StartDate"]);
			var endDate = Convert.ToDateTime(Request.QueryString["EndDate"]);
			var startPrice = Convert.ToDecimal(Request.QueryString["StartPrice"]);
			var endPrice = Convert.ToDecimal(Request.QueryString["EndPrice"]);

			DeliveredLogMarketModelDataManager db = DeliveredLogMarketModelDataManager.GetInstance();
			TimberDataManager dbTimber = TimberDataManager.GetInstance();
			var marketModelData = db.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).ToList();
			var tms = dbTimber.GetTimberMarkets();
			var timberMarket = tms.Where(uu => uu.TimberMarketID == timberMarketID).FirstOrDefault();
			var logMarketReportSpecies = dbTimber.GetLogMarketReportSpecies().Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID).FirstOrDefault();
			if (timberMarket != null)
			{
				var allLogPrices = dbTimber.GetHistoricLogPrices(logMarketReportSpeciesID, timberMarketID);
				var endPPI = marketModelData.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault();
				var realResults = LoadRealChart(allLogPrices, endDate, marketModelData, timberMarket, startDate, endPPI);
				LoadRPAChart(allLogPrices, startDate, endDate, marketModelData, timberMarket, startPrice, endPrice, endPPI);
				
				RadChart1.ChartTitle.TextBlock.Text = logMarketReportSpecies.LogMarketSpecies + " " + timberMarket.Market + " Real " + endPPI.Year.ToString() + endPPI.Period.ToString("D2") + " with RPA Forecast";
				RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font = new Font(RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Name, 14, RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Unit);
				RadChart1.Width = new Unit("800px");
				RadChart1.Height = new Unit("600px");
				RadChart1.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Date";
				RadChart1.PlotArea.XAxis.AxisLabel.Visible = true;
				RadChart1.PlotArea.XAxis.AutoScale = false;
				RadChart1.PlotArea.XAxis.AddRange(realResults.MinValue, realResults.MaxValue, 2);
				RadChart1.PlotArea.XAxis.Appearance.LabelAppearance.RotationAngle = -45;
				RadChart1.PlotArea.XAxis.Appearance.MajorGridLines.Visible = true;
				RadChart1.PlotArea.YAxis.Appearance.MinorGridLines.Visible = true;
				
				RadChart1.PlotArea.YAxis.Appearance.CustomFormat = "$#,###";
				RadChart1.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Value per MBF";
				RadChart1.PlotArea.YAxis.AxisLabel.Visible = true;
				RadChart1.PlotArea.YAxis.Appearance.MinorGridLines.Visible = false;
				RadChart1.PlotArea.YAxis.Appearance.MajorGridLines.Width = 2;
				RadChart1.PlotArea.YAxis.Appearance.MajorGridLines.PenStyle = System.Drawing.Drawing2D.DashStyle.Solid;
				RadChart1.PlotArea.YAxis.AddRange(0, Convert.ToDouble(MAXPrice + 100), 100);
				RadChart1.PlotArea.YAxis.AutoScale = false;

				RadChart1.DataBind();
			}
		}

		private RealResults LoadRealChart(IQueryable<DAL.HistoricLogPrice> allLogPrices, DateTime endDate, List<DAL.MarketModelData> marketModelData, DAL.TimberMarket timberMarket, DateTime startDate, DAL.MarketModelData endPPI)
		{
			List<DAL.HistoricLogPrice> prices = allLogPrices.ToList();
			var startYear = startDate.Year;
			var startMonth = startDate.Month;

			var firstElement = prices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month).FirstOrDefault();

			var endYear = endPPI.Year;
			var endMonth = endPPI.Period;
			
			var chartSeriesCollectionReal = new ChartSeriesItemsCollection();

			var minValue = Convert.ToDouble(firstElement.Year);
			var maxValue = Convert.ToDouble(endYear + 25);

			foreach (var price in prices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month))
			{
				var item = new ChartSeriesItem();
				item.XValue = Convert.ToDouble(price.Year) + Convert.ToDouble(price.Month / 12M);
				item.YValue = Convert.ToDouble(price.Price);

				var itemReal = new ChartSeriesItem();

				var startPPI = marketModelData.Where(uu => uu.Year == price.Year && uu.Period == price.Month).Select(uu => uu.Value).FirstOrDefault();
				if (startPPI != 0)
				{
					var realPrice = rpaReal.GetRPARealValue(price.Price,
							new DateTime(price.Year, price.Month, 1),
							new DateTime(endYear, endMonth, 1),
							startPPI,
							endPPI.Value
						);

					itemReal.XValue = Convert.ToDouble(price.Year) + Convert.ToDouble(price.Month / 12M);
					itemReal.YValue = Convert.ToDouble(realPrice);
					if (realPrice > MAXPrice)
					{
						MAXPrice = realPrice;
					}
					chartSeriesCollectionReal.Add(itemReal);
				}
			}

			var chartSeriesReal = new ChartSeries(timberMarket.Market + " Real");
			chartSeriesReal.Appearance.LineSeriesAppearance.Color = GetColors()[1];
			chartSeriesReal.Items.AddRange(chartSeriesCollectionReal);
			chartSeriesReal.Type = ChartSeriesType.Line;
			chartSeriesReal.Appearance.LabelAppearance.Visible = false;
			RadChart1.AddChartSeries(chartSeriesReal);

			return new RealResults(minValue, maxValue);
		}

		private void LoadRPAChart(IQueryable<DAL.HistoricLogPrice> allLogPrices, DateTime startDate, DateTime endDate, List<DAL.MarketModelData> marketModelData, DAL.TimberMarket timberMarket, decimal startPrice, decimal endPrice, DAL.MarketModelData endPPI)
		{
			var filterYears = allLogPrices.Where(uu => uu.Year >= startDate.Year && uu.Year <= endDate.Year);
			var filterStartMonth = filterYears.Where(uu => (uu.Year > startDate.Year || (uu.Year == startDate.Year && uu.Month >= startDate.Month)));
			var logPrices = filterStartMonth.Where(uu => (uu.Year < endDate.Year || (uu.Year == endDate.Year && uu.Month <= endDate.Month)));
			
			var chartSeriesCollectionFuture = new ChartSeriesItemsCollection();

			var startYear = startDate.Year;
			var startMonth = startDate.Month;

			var firstElement = new DAL.HistoricLogPrice()
			{
				Year = startYear,
				Month = startMonth,
				Price = startPrice
			};

			var endYear = endPPI.Year;
			var endMonth = endPPI.Period;

			

			var longevity = deltaNM.GetDeltaNM(startYear, startMonth, endDate.Year, endDate.Month);
			var rpa = rpaReal.GetRPA(startPrice, endPrice, longevity);

			var startElement = new DateTime(firstElement.Year, firstElement.Month, 1);
			var currentElement = new DateTime(endYear, endMonth, 1);
			var lastFutureElement = new DateTime(DateTime.Now.Year + 25, DateTime.Now.Month, 1);

			var startPPI = marketModelData.Where(uu => uu.Year == startYear && uu.Period == startMonth).Select(uu => uu.Value).FirstOrDefault();
			var realPrice = rpaReal.GetRPARealValue(startPrice,
					new DateTime(startYear, startMonth, 1),
					new DateTime(endYear, endMonth, 1),
					startPPI,
					endPPI.Value
				);
			var ct = 0;
			while (startElement <= lastFutureElement)
			{
				ct++;
				var itemFuture = new ChartSeriesItem();
				var endLongevity = deltaNM.GetDeltaNM(startDate.Year, startDate.Month, startElement.Year, startElement.Month);
				
				var futureRPA = rpaReal.GetRPAValueAtYear(startPrice, rpa, longevity, endLongevity);
				
				itemFuture.XValue = Convert.ToDouble(startElement.Year) + Convert.ToDouble(startElement.Month / 12M);
				itemFuture.YValue = Convert.ToDouble(futureRPA);

				if (futureRPA > MAXPrice)
				{
					MAXPrice = futureRPA;
				}
				chartSeriesCollectionFuture.Add(itemFuture);

				startElement = startElement.AddMonths(1);
			}

			var chartSeriesFuture = new ChartSeries(timberMarket.Market + " RPA");
			chartSeriesFuture.Appearance.LineSeriesAppearance.Color = GetColors()[2];
			chartSeriesFuture.Items.AddRange(chartSeriesCollectionFuture);
			chartSeriesFuture.Type = ChartSeriesType.Line;
			chartSeriesFuture.Appearance.LabelAppearance.Visible = false;
			RadChart1.AddChartSeries(chartSeriesFuture);
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
		
		private class RealResults
		{
			public RealResults(Double minValue, Double maxValue)
			{
				MinValue = minValue;
				MaxValue = maxValue;
			}
			public Double MinValue;
			public Double MaxValue;
		}
	}
}