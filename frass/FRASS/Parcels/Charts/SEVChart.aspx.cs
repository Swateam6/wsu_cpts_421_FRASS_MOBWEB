using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Models;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Charting;

namespace FRASS.WebUI.Parcels.Charts
{
	public partial class SEVChart : System.Web.UI.Page
	{
		Int32 parcelID;
		Int32 portfolioID;
		Int32 rpaPortfolioID;
		MarketModelPortfolio portfolio;
		RPAPortfolio rpaPortfolio;
		Parcel parcel;
		Int32 standID;
		SEV sev;
		DeliveredLogMarketModelDataManager dbDeliveredLogMarketModelDataManager;
		ParcelDataManager dbParcelDataManager;
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

		protected void Page_Init(object sender, EventArgs e)
		{
			dbDeliveredLogMarketModelDataManager = DeliveredLogMarketModelDataManager.GetInstance();
			dbParcelDataManager = ParcelDataManager.GetInstance();
			Int32 tmpInt;
			if (Int32.TryParse(Request.QueryString["ParcelID"].ToString(), out tmpInt))
			{
				parcelID = tmpInt;
			}
			if (Int32.TryParse(Request.QueryString["MarketModelPortfolioID"].ToString(), out tmpInt))
			{
				portfolioID = tmpInt;
			}
			if (Int32.TryParse(Request.QueryString["RPAPortfolioID"].ToString(), out tmpInt))
			{
				rpaPortfolioID = tmpInt;
			}
			if (Int32.TryParse(Request.QueryString["StandID"].ToString(), out tmpInt))
			{
				standID = tmpInt;
			}
		}
		private void LoadChart()
		{
			RadChart1.Clear();
			portfolio = dbDeliveredLogMarketModelDataManager.GetMarketModelPortfolio(portfolioID);
			rpaPortfolio = RPAPortfolioDataManager.GetInstance().GetRPAPortfolio(rpaPortfolioID);
			var rPAPortfolioDetails = rpaPortfolio.RPAPortfolioDetails.ToList<RPAPortfolioDetail>();
			var marketModelPortfolioDeliveredLogModelDetails = portfolio.MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
			parcel = dbParcelDataManager.GetParcel(parcelID);
			EconVariables econVariables = new EconVariables(portfolio, rpaPortfolio);
			sev = new SEV(portfolio, rpaPortfolio, rPAPortfolioDetails, marketModelPortfolioDeliveredLogModelDetails, parcel, standID, econVariables);
			var yearToRun = Convert.ToInt32(DropDownList_Years.SelectedValue);
			var chartSeriesCollection = new ChartSeriesItemsCollection();
			var lastValue = -100000M;
			var lastYear = 0;
			for (var year = 5; year <= yearToRun; year += 5)
			{
				var answer = sev.GetCurrentSEVValue(year);
				var item = new ChartSeriesItem();
				item.XValue = year;
				item.YValue = Convert.ToDouble(answer);
				chartSeriesCollection.Add(item);
				if (answer > lastValue)
				{
					lastValue = answer;
					lastYear = year;
				}
			}
			var chartSeries = new ChartSeries("SEV Rotation of " + lastValue.ToString("C0") + " at year " + lastYear.ToString());
			chartSeries.Items.AddRange(chartSeriesCollection);
			chartSeries.Type = ChartSeriesType.Line;
			chartSeries.Appearance.LabelAppearance.Visible = false;
			chartSeries.Appearance.LineSeriesAppearance.Color = GetColors()[0];
			RadChart1.AddChartSeries(chartSeries);

			RadChart1.ChartTitle.TextBlock.Text = "SEV For StandID: " + standID.ToString() + " Over " + yearToRun.ToString() + " Years";
			RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font = new Font(RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Name, 14, RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Unit);
			RadChart1.Width = new Unit("800px");
			RadChart1.Height = new Unit("600px");
			RadChart1.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Year";
			RadChart1.PlotArea.XAxis.AxisLabel.Visible = true;
			RadChart1.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Soil Expectation Value";
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