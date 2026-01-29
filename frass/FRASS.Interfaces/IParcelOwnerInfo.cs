namespace FRASS.Interfaces
{
	public interface IParcelOwnerInfo
	{
		int ParcelOwnerID { get; set; }
		int ParcelID { get; set; }
		string ParcelNumber { get; set; }
		decimal Acres { get; set; }
		decimal BuildingValue { get; set; }
		decimal sBuildingValue { get; set; }
		decimal LandValue { get; set; }
		string Legal { get; set; }
		int OwnerID { get; set; }
	}
}
