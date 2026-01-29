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
	public partial class R1Chart : System.Web.UI.Page
	{
		Int32 parcelID;
		Int32 portfolioID;
		Int32 rpaPortfolioID;
		MarketModelPortfolio portfolio;
		RPAPortfolio rpaPortfolio;
		Parcel parcel;
		Int32 standID;
		R1 r1;
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
			var dbM = DeliveredLogMarketModelDataManager.GetInstance();
			var dbP = ParcelDataManager.GetInstance();
			var dbS = StandDataManager.GetInstance();

			RadChart1.Clear();
			portfolio = dbM.GetMarketModelPortfolio(portfolioID);
			rpaPortfolio = RPAPortfolioDataManager.GetInstance().GetRPAPortfolio(rpaPortfolioID);
			parcel = dbP.GetParcel(parcelID);
			EconVariables econVariables = new EconVariables(portfolio, rpaPortfolio);
			r1 = new R1(portfolio, rpaPortfolio, parcel, standID, econVariables);
			var yearToRun = Convert.ToInt32(DropDownList_Years.SelectedValue);
			var chartSeriesCollection = new ChartSeriesItemsCollection();
			var lastValue = -100000M;
			var lastYear = 0;
			var years = dbS.GetCurrentStandSortYears();
			var minYear = years.Min();
			foreach(var year in years.OrderBy(uu=>uu))
			{
				if (year >= yearToRun)
				{
					var answer = r1.GetNPV(year, year - minYear);
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
			}
			var chartSeries = new ChartSeries("R1 Rotation of " + lastValue.ToString("C0") + " at year " + lastYear.ToString());
			chartSeries.Items.AddRange(chartSeriesCollection);
			chartSeries.Type = ChartSeriesType.Line;
			chartSeries.Appearance.LabelAppearance.Visible = false;
			chartSeries.Appearance.LineSeriesAppearance.Color = GetColors()[0];
			RadChart1.AddChartSeries(chartSeries);

			RadChart1.ChartTitle.TextBlock.Text = "R1 For StandID: " + standID.ToString() + " From " + yearToRun.ToString() + " - " + years.Max().ToString();
			RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font = new Font(RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Name, 14, RadChart1.ChartTitle.TextBlock.Appearance.TextProperties.Font.Unit);
			RadChart1.Width = new Unit("800px");
			RadChart1.Height = new Unit("600px");
			RadChart1.PlotArea.XAxis.MinValue = minYear;
			RadChart1.PlotArea.XAxis.MaxValue = years.Max();
			RadChart1.PlotArea.XAxis.IsZeroBased = false;
			RadChart1.PlotArea.XAxis.AxisLabel.TextBlock.Text = "Year";
			RadChart1.PlotArea.XAxis.AxisLabel.Visible = true;
			RadChart1.PlotArea.YAxis.Appearance.CustomFormat = "$#,###";
			RadChart1.PlotArea.YAxis.AxisLabel.TextBlock.Text = "Net Present Value of Rotation 1";
			RadChart1.PlotArea.YAxis.AxisLabel.Visible = true;
			RadChart1.DataBind();
		}

		private void LoadYears()
		{
			for (var i = 0; i <= 200; i += 5)
			{
				var ic = System.DateTime.Now.Year + i;
				DropDownList_Years.Items.Add(new ListItem(ic.ToString(), ic.ToString()));
			}
		}

		protected void DropDownList_Years_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadChart();
		}
	}
}