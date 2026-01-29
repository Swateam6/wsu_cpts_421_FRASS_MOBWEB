using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;
using System.Data.Linq;

namespace FRASS.DAL.DataManager
{
	public class ReportDataManager
	{
		private ReportRepository db;
		private ReportDataManager()
		{
			db = ReportRepository.GetInstance();
		}
		public static ReportDataManager GetInstance()
		{
			return new ReportDataManager();
		}

		public Report InsertReportWithoutPDF(Report report)
		{
			return db.InsertReportWithoutPDF(report);
		}
		public Report UpdateReport(Report report)
		{
			return db.UpdateReport(report);
		}
		public Report AddReport(Report report, byte[] pdf)
		{
			return db.AddReport(report, pdf);
		}
		public Report GetReport(int reportID)
		{
			return db.GetReport(reportID);
		}
		public List<Report> GetReports(User user)
		{
			return db.GetReports(user);

		}
		public void ClearOldReports()
		{
			db.ClearOldReports();
		}
		public void DeleteReport(Int32 reportID)
		{
			db.DeleteReport(reportID);
		}
	}
}
