using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Formulas;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Charting;

namespace FRASS.WebUI.Parcels.Charts
{
	public partial class TimberMarketRPAChart : System.Web.UI.Page
	{
		private DeltaNM deltaNM;
		private RPARealValue rpaReal;
		private decimal MAXPrice = 0M;
		private decimal RealPrice = 0M;

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

			DeliveredLogMarketModelDataManager db = DeliveredLogMarketModelDataManager.GetInstance();
			TimberDataManager dbTimber = TimberDataManager.GetInstance();
			var marketModelData = db.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).ToList();
			var tms = dbTimber.GetTimberMarkets();
			var timberMarket = tms.Where(uu => uu.TimberMarketID == timberMarketID).FirstOrDefault();
			var logMarketReportSpecies = dbTimber.GetLogMarketReportSpecies().Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID).FirstOrDefault();
			
			if (timberMarket != null)
			{
				var allLogPrices = dbTimber.GetHistoricLogPrices(logMarketReportSpeciesID, timberMarketID);
				var realResults = LoadRealChart(allLogPrices, marketModelData, timberMarket);
				var lastElement = allLogPrices.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Month).FirstOrDefault();

				LoadNominalChart(allLogPrices, timberMarket);

				var endPPI = marketModelData.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Period).FirstOrDefault();

				RadChart1.ChartTitle.TextBlock.Text = logMarketReportSpecies.LogMarketSpecies + " " + timberMarket.Market + " Nominal and Real Values (" + endPPI.Period.ToString() + "/1/" + endPPI.Year.ToString() + ")";
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
				RadChart1.PlotArea.YAxis.AxisLabel.Visible = true;
				RadChart1.PlotArea.YAxis.Appearance.MinorGridLines.Visible = false;
				RadChart1.PlotArea.YAxis.Appearance.MajorGridLines.Width = 2;
				RadChart1.PlotArea.YAxis.Appearance.MajorGridLines.PenStyle = System.Drawing.Drawing2D.DashStyle.Solid;
				RadChart1.PlotArea.YAxis.AddRange(0, Convert.ToDouble(MAXPrice + 100), 100);
				RadChart1.PlotArea.YAxis.AutoScale = false;

				RadChart1.DataBind();
			}

		}

		private void LoadNominalChart(IQueryable<DAL.HistoricLogPrice> allLogPrices, TimberMarket timberMarket)
		{
			var lastElementInPrices = allLogPrices.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Month).FirstOrDefault();

			var chartSeriesCollectionReal = new ChartSeriesItemsCollection();
			var chartSeriesCollectionNominal = new ChartSeriesItemsCollection();
			var firstElement = allLogPrices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month).FirstOrDefault();
			var lastElement = allLogPrices.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Month).FirstOrDefault();

			var startYear = firstElement.Year;
			var endYear = lastElement.Year;
			var startMonth = firstElement.Month;
			var endMonth = lastElement.Month;


			var minValue = Convert.ToDouble(firstElement.Year);
			var maxValue = Convert.ToDouble(lastElement.Year + 1);

			foreach (var price in allLogPrices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month))
			{
				var item = new ChartSeriesItem();
				item.XValue = Convert.ToDouble(price.Year) + Convert.ToDouble(price.Month / 12M);
				item.YValue = Convert.ToDouble(price.Price);
				chartSeriesCollectionNominal.Add(item);
			}

			var chartSeriesNominal = new ChartSeries(timberMarket.Market + " Nominal");
			chartSeriesNominal.Appearance.LineSeriesAppearance.Color = GetColors()[0];
			chartSeriesNominal.Items.AddRange(chartSeriesCollectionNominal);
			chartSeriesNominal.Type = ChartSeriesType.Line;
			chartSeriesNominal.Appearance.LabelAppearance.Visible = false;
			RadChart1.AddChartSeries(chartSeriesNominal);
		}

		private RealResults LoadRealChart(IQueryable<DAL.HistoricLogPrice> allLogPrices, List<DAL.MarketModelData> marketModelData, DAL.TimberMarket timberMarket)
		{	


			var firstElement = allLogPrices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month).FirstOrDefault();
			var lastElement = allLogPrices.OrderByDescending(uu => uu.Year).ThenByDescending(uu => uu.Month).FirstOrDefault();

			var endYear = lastElement.Year;
			var endMonth = lastElement.Month;


			var startYear = firstElement.Year;
			var startMonth = firstElement.Month;

			var minValue = Convert.ToDouble(startYear);
			var maxValue = Convert.ToDouble(endYear);
			var chartSeriesCollectionReal = new ChartSeriesItemsCollection();

			var endPPI = marketModelData.OrderByDescending(uu=>uu.Year).ThenByDescending(uu=>uu.Period).FirstOrDefault();

			foreach (var price in allLogPrices.OrderBy(uu => uu.Year).ThenBy(uu => uu.Month))
			{
				var item = new ChartSeriesItem();
				item.XValue = Convert.ToDouble(price.Year) + Convert.ToDouble(price.Month / 12M);
				item.YValue = Convert.ToDouble(price.Price);

				var itemReal = new ChartSeriesItem();

				var realPrice = decimal.Zero;
				var startPPI = marketModelData.Where(uu => uu.Year == price.Year && uu.Period == price.Month).Select(uu => uu.Value).FirstOrDefault();

				if (price.Year == lastElement.Year && price.Month == lastElement.Month)
				{
					realPrice = price.Price;
				}
				else
				{
					realPrice = rpaReal.GetRPARealValue(price.Price,
						new DateTime(price.Year, price.Month, 1),
						new DateTime(lastElement.Year, lastElement.Month, 1),
						startPPI,
						endPPI.Value
					);
				}
				
				if (price.Year == startYear && price.Month == startMonth)
				{
					RealPrice = realPrice;
				}

				itemReal.XValue = Convert.ToDouble(price.Year) + Convert.ToDouble(price.Month / 12M);
				itemReal.YValue = Convert.ToDouble(realPrice);
				if (realPrice > MAXPrice)
				{
					MAXPrice = realPrice;
				}
				
				chartSeriesCollectionReal.Add(itemReal);
			}

			var chartSeriesReal = new ChartSeries(timberMarket.Market + " Real");
			chartSeriesReal.Appearance.LineSeriesAppearance.Color = GetColors()[1];
			chartSeriesReal.Items.AddRange(chartSeriesCollectionReal);
			chartSeriesReal.Type = ChartSeriesType.Line;
			chartSeriesReal.Appearance.LabelAppearance.Visible = false;
			RadChart1.AddChartSeries(chartSeriesReal);

			return new RealResults(minValue, maxValue);
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
	}
}