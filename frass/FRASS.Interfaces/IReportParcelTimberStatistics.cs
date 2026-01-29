using System.Collections.Generic;

namespace FRASS.Interfaces
{
	public interface IReportParcelTimberStatistics
	{
		int Stand_ID { get; set; }
		string Veg_Label { get; set; }
		int Site_Index { get; set; }
		decimal Riparian_Zone_Acres { get; set; }
		decimal Operable_Land_Acres { get; set; }
		decimal Total_Acres { get; set; }
		decimal BFAcre_PerStand { get; set; }
		decimal TotalBF_PerStand { get; set; }
		decimal StandAmount { get; set; }
		List<decimal> operableAcres { get; set; }
		List<decimal> nonoperableAcres { get; set; }
	}
}
