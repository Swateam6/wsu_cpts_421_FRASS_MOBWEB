using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FRASS.DAL.Context;
using FRASS.Interfaces;
using System.Data.Linq;

namespace FRASS.DAL.Repositories
{
	internal class OwnerRepository
	{
		private FRASSDataContext db;
		private OwnerRepository()
		{
			db = new FRASSDataContext();
		}
		public static OwnerRepository GetInstance()
		{
			return new OwnerRepository();
		}
		public Owner GetOwner(Int32 ownerid)
		{
			var owner = (from o in db.Owners where o.OwnerID == ownerid select o).FirstOrDefault();
			return owner;
		}
		public void UpdateOwner(Owner owner)
		{
			var o = (from ow in db.Owners where ow.OwnerID == owner.OwnerID select ow).FirstOrDefault();
			o.Address = owner.Address;
			o.City = owner.City;
			o.Name = owner.Name;
			o.StateID = owner.StateID;
			o.Zip = owner.Zip;
			o.Zip4 = owner.Zip4;
			db.SubmitChanges();
		}
		public void AddNewOwner(Owner owner)
		{
			Owner o = new Owner();
			o.Address = owner.Address;
			o.City = owner.City;
			o.Name = owner.Name;
			o.StateID = owner.StateID;
			o.Zip = owner.Zip;
			o.Zip4 = owner.Zip4;
			db.Owners.InsertOnSubmit(o);
			db.SubmitChanges();
		}
		public IEnumerable<IParcelOwnerInfo> GetParcelOwnerInfo()
		{
			var results = from po in db.ParcelOwners
						  join pl in db.ParcelLegals on po.ParcelID equals pl.ParcelID
						  join p in db.Parcels on po.ParcelID equals p.ParcelID
						  select new ParcelOwnerInfo
						  {
							  ParcelOwnerID = po.ParcelOwnerID,
							  ParcelID = p.ParcelID,
							  ParcelNumber = p.ParcelNumber,
							  Acres = p.Acres,
							  BuildingValue = pl.BuildingValue,
							  Legal = pl.Legal,
							  LandValue = pl.LandValue,
							  OwnerID = po.OwnerID
						  };
			return results;
		}
		public List<IAllottee> GetAllottees()
		{
			var a = from pas in db.ParcelAllotmentShares
					join pal in db.ParcelAllottees on pas.ParcelAllotteeID equals pal.ParcelAllotteeID
					join pa in db.ParcelAllotments on pas.ParcelAllotmentID equals pa.ParcelAllotmentID
					join p in db.Parcels on pa.ParcelID equals p.ParcelID
					select new Allottee()
					{
						ParcelID = p.ParcelID,
						FirstName = pal.FirstName,
						LastName = pal.LastName,
						AllotmentNumber = pa.AllotmentNumber,
						AllotteeNumber = pal.AllotteeNumber,
						ParcelNumber = p.ParcelNumber,
						Township = pa.Town,
						Range = pa.Range,
						Section = pa.Section,
						Share = pas.Share,
						Acres = p.Acres
					};
			return a.ToList<IAllottee>();
		}

		private class Allottee : IAllottee
		{
			public int ParcelID { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public string AllotmentNumber { get; set; }
			public int AllotteeNumber { get; set; }
			public string ParcelNumber { get; set; }
			public string Township { get; set; }
			public string Range { get; set; }
			public string Section { get; set; }
			public decimal Share { get; set; }
			public decimal Acres { get; set; }
		}
		private class ParcelOwnerInfo : IParcelOwnerInfo
		{
			public int ParcelOwnerID { get; set; }
			public int ParcelID { get; set; }
			public string ParcelNumber { get; set; }
			public decimal Acres { get; set; }
			public decimal BuildingValue { get; set; }
			public decimal sBuildingValue { get; set; }
			public decimal LandValue { get; set; }
			public string Legal { get; set; }
			public int OwnerID { get; set; }
		}
	}
}
