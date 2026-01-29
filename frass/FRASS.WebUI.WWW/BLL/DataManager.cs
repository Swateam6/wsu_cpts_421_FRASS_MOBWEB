using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.WebUI.WWW.DAL;

namespace FRASS.WebUI.WWW.BLL
{
	public class DataManager
	{
		private Repository _db { get; set; }
		private Repository DB
		{
			get
			{
				return _db ?? (_db = Repository.GetInstance(SqlQuery.GetInstance(Database.Kamiak)));
			}
		}

		private DataManager()
		{

		}

		public static DataManager GetInstance()
		{
			return new DataManager();
		}

		public int SaveOrder(string name, string street, string city, string state, string zip, int countryID, string email, int numberOfCopies, bool isTaxExempt, ShippingTypes shippingOption)
		{
			return DB.SaveOrder(name, street, city, state, zip, countryID, email, numberOfCopies, isTaxExempt, shippingOption);
		}
	}
}