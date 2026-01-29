using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;

namespace FRASS.DAL.DataManager
{
	public class SiteDataManager
	{
		SiteRepository db;
		private SiteDataManager()
		{
			db = SiteRepository.GetInstance();
		}
		public static SiteDataManager GetInstance()
		{
			return new SiteDataManager();
		}

		public void UpdateSiteSettings(SiteSetting siteSetting)
		{
			db.UpdateSiteSettings(siteSetting);
		}
		public SiteSetting GetSiteSetting(Int32 siteSettingID)
		{
			return db.GetSiteSetting(siteSettingID);
		}

		public void AddLog(Int32 LogTypeID, Int32? UserID, string Description)
		{
			db.AddLog(LogTypeID, UserID, Description);
		}

		public IEnumerable<Log> GetLogs()
		{
			return db.GetLogs();
		}
	}
}
