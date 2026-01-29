using System.Collections.Generic;
using System.Linq;
using FRASS.DAL.Context;
using System;

namespace FRASS.DAL.Repositories
{
	internal class ReportRepository
	{
		private FRASSDataContext db;
		private ReportRepository()
		{
			db = new FRASSDataContext();
			db.CommandTimeout = 0;
		}
		public static ReportRepository GetInstance()
		{
			return new ReportRepository();
		}

		public Report InsertReportWithoutPDF(Report report)
		{
			var r = new Report();
			r.Completed = false;
			r.DateCreated = report.DateCreated;
			r.MarketModelPortfolioID = report.MarketModelPortfolioID;
			r.ParcelID = report.ParcelID;
			r.ReportTypeID = report.ReportTypeID;
			r.StumpageModelPortfolioID = report.StumpageModelPortfolioID;
			r.Title = report.Title;
			r.UserID = report.UserID;
			db.Reports.InsertOnSubmit(r);
			db.SubmitChanges();
			return r;
		}
		public Report UpdateReport(Report report)
		{
			var r = GetReport(report.ReportID);
			r.Completed = false;
			r.DateCreated = report.DateCreated;
			r.MarketModelPortfolioID = report.MarketModelPortfolioID;
			r.ParcelID = report.ParcelID;
			r.ReportTypeID = report.ReportTypeID;
			r.StumpageModelPortfolioID = report.StumpageModelPortfolioID;
			r.Title = report.Title;
			r.UserID = report.UserID;
			db.SubmitChanges();
			return r;
		}
		public Report AddReport(Report report, byte[] pdf)
		{
			var r = GetReport(report.ReportID);
			r.PDF = pdf;
			r.Completed = true;
			r.DateCreated = System.DateTime.Now;
			db.SubmitChanges();
			return r;
		}
		public Report GetReport(int reportID)
		{
			var report = from r in db.Reports
						 where r.ReportID == reportID
						 select r;
			return report.FirstOrDefault();
		}
		public List<Report> GetReports(User user)
		{
			var report = from r in db.Reports
						 where r.UserID == user.UserID
						 select r;
			return report.ToList<Report>();

		}
		public void ClearOldReports()
		{
			var expiringDays = FRASS.Interfaces.DatabaseIDs.SiteSettings.ReportExpirationDays;

			var days = (from d in db.SiteSettings
						where d.SiteSettingID == expiringDays
						select d.Value).FirstOrDefault();

			if (days != null)
			{
				var dayToDelete = Convert.ToInt32(days);
				var reportsToDelete = from r in db.Reports
									  where (r.DateCreated.HasValue && r.DateCreated.Value <= System.DateTime.Now.AddDays(-dayToDelete)) || (r.DateCreated <= System.DateTime.Now.AddDays(-1) && r.Completed == false)
									  select r;
				db.Reports.DeleteAllOnSubmit(reportsToDelete);
				db.SubmitChanges();
			}
		}
		public void DeleteReport(Int32 reportID)
		{
			var report = from r in db.Reports where r.ReportID == reportID select r;
			db.Reports.DeleteAllOnSubmit(report);
			db.SubmitChanges();
		}
	}
}
