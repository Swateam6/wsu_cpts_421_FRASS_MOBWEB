namespace FRASS.Interfaces
{
	public interface IStandData
	{
		string Abbreviation { get; set; }
		decimal Acres { get; set; }
		decimal Board_SN { get; set; }
		int LogMarketReportSpeciesID { get; set; }
		string LogMarketSpecies { get; set; }
		string Market { get; set; }
		int OrderID { get; set; }
		decimal PctBrd { get; set; }
		int ReportYear { get; set; }
		string SortCode { get; set; }
		int SpeciesID { get; set; }
		int TimberGradeID { get; set; }
		int TimberMarketID { get; set; }
	}
}
