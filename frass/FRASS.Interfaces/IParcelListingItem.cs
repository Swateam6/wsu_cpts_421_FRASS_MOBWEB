using System;

namespace FRASS.Interfaces
{
	public interface IParcelListingItem
	{
		Int32 ParcelID { get; set; }
		string ParcelNumber { get; set; }
		decimal Acre { get; set; }
		decimal Hectare { get; set; }
		string Township { get; set; }
		string Range { get; set; }
		string Section { get; set; }
		string County { get; set; }
		string Allotment { get; set; }
		string OwnerStatus { get; set; }
	}
}
