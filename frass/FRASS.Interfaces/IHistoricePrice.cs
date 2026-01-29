namespace FRASS.Interfaces
{
	public interface IHistoricPrice
	{
		int Year { get; set; }
		int Month { get; set; }
		int LogMarketReportSpeciesID { get; set; }
		decimal? SMPrice { get; set; }
		decimal? Saw2Price { get; set; }
		decimal? Saw3Price { get; set; }
		decimal? Saw4Price { get; set; }
		decimal? Saw4CNPrice { get; set; }
		decimal? PulpPrice { get; set; }
		decimal? CamprunPrice { get; set; }
		decimal? Export12Price { get; set; }
		decimal? Export8Price { get; set; }
		decimal? ChipPrice { get; set; }
	}
}
