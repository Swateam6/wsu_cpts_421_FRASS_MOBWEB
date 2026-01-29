using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FRASS.BLL.Models;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using Telerik.Charting;
using System.Drawing;
using System.Collections.Generic;

namespace FRASS.WebUI.Parcels.Charts
{
	public partial class R2Chart : System.Web.UI.Page
	{
		Int32 parcelID;
		Int32 portfolioID;
		Int32 rpaPortfolioID;
		MarketModelPortfolio portfolio;
		RPAPortfolio rpaPortfolio;
		List<RPAPortfolioDetail> RPAPortfolioDetails;
		List<MarketModelPortfolioDeliveredLogModelDetail> MarketModelPortfolioDeliveredLogModelDetails;
		Parcel parcel;
		Int32 standID;
		R2 r2;
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
			portfolio = DeliveredLogMarketModelDataManager.GetInstance().GetMarketModelPortfolio(portfolioID);
			rpaPortfolio = RPAPortfolioDataManager.GetInstance().GetRPAPortfolio(rpaPortfolioID);
			RPAPortfolioDetails = rpaPortfolio.RPAPortfolioDetails.ToList<RPAPortfolioDetail>();
			MarketModelPortfolioDeliveredLogModelDetails = portfolio.MarketModelPortfolioDeliveredLogModelDetails.ToList<MarketModelPortfolioDeliveredLogModelDetail>();
			parcel = ParcelDataManager.GetInstance().GetParcel(parcelID);
			EconVariables econVariables = new EconVariables(portfolio, rpaPortfolio);
			var rg = new RotationGenerator(portfolio, rpaPortfolio, RPAPortfolioDetails, MarketModelPortfolioDeliveredLogModelDetails, parcel, standID, econVariables);
			var offset = Convert.ToInt32(DropDownList_YearOffset.SelectedValue);
			r2 = rg.GetR2(offset);
			var yearToRun = Convert.ToInt32(DropDownList_Years.SelectedValue);
			var chartSeriesCollection = new ChartSeriesItemsCollection();
			var lastValue = -100000M;
			var lastYear = 0;
			var years = StandDataManager.GetInstance().GetCurrentStandSortYears();
			var minYear = years.Min();
			for (var year = 5; year <= yearToRun; year += 5)
			{
				var answer = r2.GetNPV(year);
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
			var chartSeries = new ChartSeries("R2 Rotation of " + lastValue.ToString("C0") + " at year " + lastYear.ToString());
			chartSeries.Items.AddRange(chartSeriesCollection);
			chartSeries.Type = ChartSeriesType.Line;
			chartSeries.Appearance.LabelAppearance.Visible = false;
			chartSeries.Appearance.LineSeriesAppearance.Color = GetColors()[0];
			RadChart1.AddChartSeries(chartSeries);

			RadChart1.ChartTitle.TextBlock.Text = "R2 For StandID: " + standID.ToString() + " Over " + yearToRun.ToString() + " with " + offset.ToString() + " years Offset";
			RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font = new Font(RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Name, 14, RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Unit);
			RadChart1.Width = new Unit("800px");
			RadChart1.Height = new Unit("600px");
			RadChart1.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Year";
			RadChart1.PlotArea.XAxis.AxisLabel.Visible = true;
			RadChart1.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Net Present Value of Rotation 2";
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
			DropDownList_YearOffset.Items.Clear();
			DropDownList_YearOffset.Items.Add(new ListItem("@0", "0"));
			for (var year = 5; year <= 200; year += 5)
			{
				DropDownList_YearOffset.Items.Add(new ListItem("@" + year.ToString(), year.ToString()));
			}
		}

		protected void DropDownList_Years_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadChart();
		}
	}
}