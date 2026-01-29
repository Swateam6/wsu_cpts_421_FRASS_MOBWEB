using System.Collections.Generic;
using System.Data.Linq;
using FRASS.BLL.Mail;
using FRASS.DAL;
using FRASS.DAL.DataManager;
using FRASS.Reports;
using FRASS.Interfaces;
using System.Configuration;
using System.Threading.Tasks;
using System;

namespace FRASS.Services.ReportService
{
	public class ReportService : IReportService
	{
		public void GenerateFullParcelReportDeliveredMarketModel(int UserID, int ParcelID, int PortfolioID, int RPAPortfolioID)
		{
			Task.Factory.StartNew(() =>
				SaveDeliveredMarketModelReport(UserID, ParcelID, PortfolioID, RPAPortfolioID)
			);
		}

		public void GenerateFullParcelReportStumpageMarketModel(int UserID, int ParcelID, int PortfolioID)
		{
			Task.Factory.StartNew(() =>
				SaveStumpageMarketModelReport(UserID, ParcelID, PortfolioID)
			);
		}

		private void SaveDeliveredMarketModelReport(int UserID, int ParcelID, int PortfolioID, int RPAPortfolioID)
		{
			var dbUser = UserDataManager.GetInstance();
			var user = dbUser.GetUser(UserID);
			try
			{
				var dbReport = ReportDataManager.GetInstance();

				var baseFilePath = ConfigurationManager.AppSettings.Get("ImagePath").ToString();
				var dbParcel = ParcelDataManager.GetInstance();

				var dbPortfolio = DeliveredLogMarketModelDataManager.GetInstance();
				var dbRPA = RPAPortfolioDataManager.GetInstance();
				var portfolio = dbPortfolio.GetMarketModelPortfolio(PortfolioID);

				var rpaPortfolio = dbRPA.GetRPAPortfolio(RPAPortfolioID);

				var parcel = dbParcel.GetParcel(ParcelID);
				var fullParcelReport = new FullParcelReport(user, parcel, dbParcel.GetCurrentParcelTimberSummary(ParcelID, System.DateTime.Now.Year), dbParcel.GetReportTimberStatistics(ParcelID, System.DateTime.Now.Year), baseFilePath, portfolio, rpaPortfolio);

				var report = new Report();
				report.DateCreated = System.DateTime.Now;
				report.MarketModelPortfolioID = portfolio.MarketModelPortfolioID;
				report.ParcelID = parcel.ParcelID;
				report.Title = "Full Parcel Report: " + parcel.ParcelNumber + ": " + portfolio.PortfolioName + ": " + rpaPortfolio.PortfolioName;
				report.ReportTypeID = DatabaseIDs.ReportTypes.FullParcelReport;
				report.UserID = user.UserID;
				var savedReport = dbReport.InsertReportWithoutPDF(report);

				var stream = fullParcelReport.GetReport();
				var final = dbReport.AddReport(savedReport, stream.ToArray());
				SiteDataManager.GetInstance().AddLog(DatabaseIDs.LogTypes.ReportGeneration, UserID, "Report Generation Success: " + report.Title + ": " + user.FirstName + " " + user.LastName);
				Task.Factory.StartNew(() =>
					SendEmail(user, savedReport.Title)
				);
			}
			catch (Exception ex)
			{

				SiteDataManager.GetInstance().AddLog(DatabaseIDs.LogTypes.ReportGeneration, UserID, "Report Generation Error:" + ex.Message + "<br/><br/>" + ex.StackTrace);
				var adminUser =
				Task.Factory.StartNew(() =>
					MailMan.SendErrorEmail(ex.ToString())
				);
			}
		}
		private void SaveStumpageMarketModelReport(int UserID, int ParcelID, int PortfolioID)
		{
			var dbReport = ReportDataManager.GetInstance();

			var baseFilePath = ConfigurationManager.AppSettings.Get("ImagePath").ToString();
			var dbParcel = ParcelDataManager.GetInstance();
			var dbUser = UserDataManager.GetInstance();
			var dbPortfolio = StumpageMarketModelDataManager.GetInstance();
			var portfolio = dbPortfolio.GetStumpageModelPortfolio(PortfolioID);
			var user = dbUser.GetUser(UserID);
			var parcel = dbParcel.GetParcel(ParcelID);
			var fullParcelReport = new FullParcelReport(user, parcel, dbParcel.GetCurrentParcelTimberSummary(ParcelID, System.DateTime.Now.Year), dbParcel.GetReportTimberStatistics(ParcelID, System.DateTime.Now.Year), baseFilePath, portfolio);

			var report = new Report();
			report.DateCreated = System.DateTime.Now;
			report.StumpageModelPortfolioID = portfolio.StumpageModelPortfolioID;
			report.ParcelID = parcel.ParcelID;
			report.Title = "Full Parcel Report: " + parcel.ParcelNumber + ": " + portfolio.PortfolioName;
			report.UserID = user.UserID;
			report.ReportTypeID = DatabaseIDs.ReportTypes.FullParcelReport;
			var savedReport = dbReport.InsertReportWithoutPDF(report);

			var stream = fullParcelReport.GetReport();
			dbReport.AddReport(savedReport, stream.ToArray());
			Task.Factory.StartNew(() =>
				SendEmail(user, savedReport.Title)
			);
		}

		private void SendEmail(User user, string title)
		{
			var m = new MailMan();
			m.SendReportCompleted(user, title);
		}
	}
}
