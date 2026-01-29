using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Charting;

namespace FRASS.WebUI.Parcels.Charts
{
	public partial class SortValuesChart : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				LoadYears();
				LoadChart();
			}
		}


		private List<Color> GetColors()
		{
			var list = new List<Color>();
			list.Add(Color.FromArgb(51,102,153));
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
			var year = Convert.ToInt32(DropDownList_Years.SelectedValue);
			var marketModelPortfolioID = Convert.ToInt32(Request.QueryString["MarketModelPortfolioID"].ToString());
			var logMarketReportSpeciesID = Convert.ToInt32(Request.QueryString["LogMarketReportSpeciesID"].ToString());
			var rpaPortfolioID = Convert.ToInt32(Request.QueryString["RPAPortfolioID"].ToString());

			DeliveredLogMarketModelDataManager db = DeliveredLogMarketModelDataManager.GetInstance();
			ParcelDataManager dbParcel = ParcelDataManager.GetInstance();
			RPAPortfolioDataManager dbRPA = RPAPortfolioDataManager.GetInstance();
			var portfolio = db.GetMarketModelPortfolio(marketModelPortfolioID);
			var rpaPortfolio = dbRPA.GetRPAPortfolio(rpaPortfolioID);
			var logMarketReportSpecy = dbParcel.GetLogMarketReportSpecy(logMarketReportSpeciesID);

			var tms = portfolio.MarketModelPortfolioDeliveredLogModelDetails.Where(uu => uu.LogMarketReportSpeciesID == logMarketReportSpeciesID).Select(uu => uu.TimberMarket).Distinct();
			var ict = 0;

			var marketModelData = db.GetMarketModelData().Where(uu => uu.MarketModelTypeID == 3).ToList<MarketModelData>();
			
			foreach (var tm in tms.OrderBy(uu => uu.OrderID))
			{
				var values = TimberSortValue.LoadYears(portfolio, rpaPortfolio, tm, logMarketReportSpecy, year, marketModelData);
				var chartSeriesCollection = new ChartSeriesItemsCollection();
				foreach (var val in values.OrderBy(uu => uu.Year))
				{
					var item = new ChartSeriesItem();
					item.XValue = val.Year;
					item.YValue = val.Value;
					chartSeriesCollection.Add(item);
				}
				var chartSeries = new ChartSeries(tm.Market);
				chartSeries.Appearance.LineSeriesAppearance.Color = GetColors()[ict];
				chartSeries.Items.AddRange(chartSeriesCollection);
				chartSeries.Type = ChartSeriesType.Line;
				chartSeries.Appearance.LabelAppearance.Visible = false;
				RadChart1.AddChartSeries(chartSeries);
				ict++;
			}
			RadChart1.ChartTitle.TextBlock.Text = logMarketReportSpecy.LogMarketSpecies + " Sort Values with RPA and Inflation Over " + year.ToString() + " Years";
			RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font = new Font(RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Name,14, RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Unit);
			RadChart1.Width = new Unit("800px");
			RadChart1.Height = new Unit("600px");
			RadChart1.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Year";
			RadChart1.PlotArea.XAxis.AxisLabel.Visible = true;
			RadChart1.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Value per MBF";
			RadChart1.PlotArea.YAxis.AxisLabel.Visible = true;
			RadChart1.PlotArea.YAxis.Appearance.CustomFormat = "$#,###";
			RadChart1.DataBind();
		}

		

		private void LoadYears()
		{
			for (var i = 200; i >= 5; i -= 5)
			{
				DropDownList_Years.Items.Add(new ListItem(i.ToString(), i.ToString()));
			}
		}

		protected void DropDownList_Years_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadChart();
		}
	}
}