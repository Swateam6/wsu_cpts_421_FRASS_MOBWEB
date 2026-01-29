using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;

namespace FRASS.DAL.Repositories
{
	internal class SiteRepository
	{
		private FRASSDataContext db;
		private SiteRepository()
		{
			db = new FRASSDataContext();
		}
		public static SiteRepository GetInstance()
		{
			return new SiteRepository();
		}

		public void UpdateSiteSettings(SiteSetting siteSetting)
		{
			var s = (from ss in db.SiteSettings where ss.SiteSettingID == siteSetting.SiteSettingID select ss).FirstOrDefault();
			s.Value = siteSetting.Value;
			db.SubmitChanges();
		}
		public SiteSetting GetSiteSetting(Int32 siteSettingID)
		{
			var s = (from ss in db.SiteSettings where ss.SiteSettingID == siteSettingID select ss).FirstOrDefault();
			return s;
		}

		public void AddLog(Int32 LogTypeID, Int32? UserID, string Description)
		{
			var log = new Log();
			log.DateCreated = System.DateTime.Now;
			log.Description = Description;
			log.LogTypeID = LogTypeID;
			log.UserID = UserID;
			db.Logs.InsertOnSubmit(log);
			db.SubmitChanges();
		}

		public IEnumerable<Log> GetLogs()
		{
			return from l in db.Logs select l;
		}
	}
}
