namespace FRASS.Interfaces
{
	public interface IReportParcelTimberStatistic
	{
		int Stand_ID { get; set; }
		string Veg_Label { get; set; }
		int Site_Index { get; set; }
		decimal Board_SN { get; set; }
		decimal Acres { get; set; }
		StandStats StandStats { get; set; }
		decimal pctbrd { get; set; }
	}
}
