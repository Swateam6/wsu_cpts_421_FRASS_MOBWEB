using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FRASS.WebUI.WWW.DAL
{
	public static class Database
	{
		public static string Kamiak
		{
			get
			{
				return ConfigurationManager.ConnectionStrings["Kamiak"].ToString();
			}
		}
	}
}