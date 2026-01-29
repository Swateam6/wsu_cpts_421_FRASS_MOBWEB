using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FRASS.DAL;
using FRASS.DAL.Repositories;
using FRASS.Interfaces;

namespace FRASS.DAL.DataManager
{
	public class OwnerDataManager
	{
		OwnerRepository db;
		private OwnerDataManager()
		{
			db = OwnerRepository.GetInstance();
		}
		public static OwnerDataManager GetInstance()
		{
			return new OwnerDataManager();
		}

		public Owner GetOwner(Int32 ownerid)
		{
			return db.GetOwner(ownerid);
		}
		public void UpdateOwner(Owner owner)
		{
			db.UpdateOwner(owner);
		}
		public void AddNewOwner(Owner owner)
		{
			db.AddNewOwner(owner);
		}

		public List<IAllottee> GetAllottees()
		{
			var key = "GetAllottees";
			List<IAllottee> list;
			if (!CacheHelper.Get(key, out list))
			{
				list = db.GetAllottees();
				CacheHelper.Add(list, key);
			}
			return list;
		}

		public IEnumerable<IParcelOwnerInfo> GetParcelOwnerInfo()
		{
			return db.GetParcelOwnerInfo();
		}


	}
}
