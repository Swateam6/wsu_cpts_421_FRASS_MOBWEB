using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FRASS.DAL.Context;
using FRASS.Interfaces;
using System.Data.Linq;
using System.Data.SqlClient;

namespace FRASS.DAL.Repositories
{
	public class TimberRepository
	{
		private FRASSDataContext db;
		private TimberRepository()
		{
			db = new FRASSDataContext();
		}
		public static TimberRepository GetInstance()
		{
			return new TimberRepository();
		}

		public void UpdateSpecies(Specy species)
		{
			var s = (from sp in db.Species where sp.SpeciesID == species.SpeciesID select sp).FirstOrDefault();
			if (s != null)
			{
				s.Abbreviation = species.Abbreviation;
				s.CommonName = species.CommonName;
				s.Latin = species.Latin;
				s.LatinAbbreviation = species.LatinAbbreviation;
				s.Taxa = s.Taxa;
				db.SubmitChanges();
			}
		}
		public void AddNewSpecies(Specy species)
		{
			species.SpeciesID = (from ss in db.Species select ss.SpeciesID).Max(uu => uu) + 1;
			db.Species.InsertOnSubmit(species);
			db.SubmitChanges();
		}
		public Specy GetSpecies(Int32 speciesID)
		{
			var s = (from sp in db.Species where sp.SpeciesID == speciesID select sp).FirstOrDefault();
			return s;
		}

		public List<Specy> GetSpecies()
		{
			return (from sp in db.Species select sp).ToList<Specy>();
		}

		public List<LogMarketReportSpecy> GetLogMarketReportSpecies()
		{
			return (from l in db.LogMarketReportSpecies select l).ToList<LogMarketReportSpecy>();
		}
		public LogMarketReportSpecy GetLogMarketReportSpecies(Int32 logMarketReportSpeciesID)
		{
			return (from l in db.LogMarketReportSpecies where l.LogMarketReportSpeciesID == logMarketReportSpeciesID select l).FirstOrDefault();
		}
		public List<TimberGrade> GetTimberGrades()
		{
			return (from t in db.TimberGrades select t).ToList<TimberGrade>();
		}

		public List<TimberMarket> GetTimberMarkets()
		{
			return (from t in db.TimberMarkets select t).ToList<TimberMarket>();
		}

		public List<v_HistoricLogPrice> GetHistoricLogPrice()
		{
			return (from hlp in db.v_HistoricLogPrices select hlp).ToList<v_HistoricLogPrice>();
		}
		public void AddHistoricLogPrices(List<HistoricLogPrice> prices)
		{
			foreach (HistoricLogPrice price in prices)
			{
				db.HistoricLogPrices.InsertOnSubmit(price);
			}
			db.SubmitChanges();
		}
		public void AddHistoricLogPrice(IHistoricPrice price)
		{
			AddHistoricLogPrices(ConvertToHistoricLogPrices(price));
		}
		public void DeleteHistoricLogPrice(List<HistoricLogPrice> prices)
		{
			var ps = from p in db.HistoricLogPrices
					 where (from pr in prices select pr.HistoricLogPriceID).Contains(p.HistoricLogPriceID)
					 select p;
			db.HistoricLogPrices.DeleteAllOnSubmit(ps);
			db.SubmitChanges();
		}
		public void EditHistoricLogPrices(IHistoricPrice prices)
		{
			var deletes = GetHistoricLogPrice(prices.Year, prices.Month, prices.LogMarketReportSpeciesID);
			db.HistoricLogPrices.DeleteAllOnSubmit(deletes);
			db.SubmitChanges();

			AddHistoricLogPrices(ConvertToHistoricLogPrices(prices));
		}
		public IQueryable<HistoricLogPrice> GetHistoricLogPrice(Int32 year, Int32 month, Int32 LogMarketReportSpeciesID)
		{
			var h = from hlp in db.HistoricLogPrices
					where hlp.Year == year && hlp.Month == month && hlp.LogMarketReportSpeciesID == LogMarketReportSpeciesID
					select hlp;
			return h;
		}

		public IQueryable<HistoricLogPrice> GetHistoricLogPrices(Int32 LogMarketReportSpeciesID, Int32 TimberMarketID)
		{
			var h = from hlp in db.HistoricLogPrices
					where hlp.TimberMarketID == TimberMarketID && hlp.LogMarketReportSpeciesID == LogMarketReportSpeciesID
					select hlp;
			return h;
		}

        public Dictionary<int, int> GetScribnerTable(int length)
        {
            // Try to get scribner values from database first
            var dbScribnerTable = GetScribnerTableFromDatabase(length);
            if (dbScribnerTable != null && dbScribnerTable.Count > 0)
            {
                return dbScribnerTable;
            }

            throw new Exception("ERROR WHEN GETTING SCRIBNER TABLE");
            //return GetCalculatedScribnerTable();
        }

        /// <summary>
        /// Retrieves scribner values from the database scribner table
        /// </summary>
        /// <returns>Dictionary of SED to BF volume, or null if table doesn't exist</returns>
        private Dictionary<int, int> GetScribnerTableFromDatabase(int length)
        {
            try
            {
                var scribnerValues = this.GetScribnerValues(length);
                if (scribnerValues != null && scribnerValues.Count > 0)
                {
                    // s.BoardFeet s.SED_IB s.Length_ft
                    return scribnerValues.ToDictionary(s => s.SED_IB, s => s.BoardFeet);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving scribner table from database: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves scribner volume values from the database for a specific log length
        /// Note: This method needs to be adapted to match your actual scribner table structure
        /// </summary>
        /// <param name="length">Log length in feet</param>
        /// <returns>List of scribner values with SED and corresponding BF volume</returns>
        public List<ScribnerValue> GetScribnerValues(int length)
        {
            try
            {
                var scribnerList = db.ExecuteQuery<ScribnerValue>(
                    "SELECT SED_IB, Length_ft, BoardFeet FROM [dbo].[Scribner] WHERE Length_ft = {0}",
                    length
                ).ToList();

                if (scribnerList.Count == 0)
                {
                    return GetScribnerValuesFromCSV(length);
                }
                return scribnerList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving scribner values: {ex.Message}");
                return GetScribnerValuesFromCSV(length);
            }
        }

		/// <summary>
		/// Backup function to retrieve scribner volume values from CSV file for a specific log length
		/// </summary>
		/// <param name="length">Log length in feet</param>
		/// <returns>List of scribner values with SED and corresponding BF volume read from CSV</returns>
		public List<ScribnerValue> GetScribnerValuesFromCSV(int length)
		{
			var result = new List<ScribnerValue>();

			try
			{
				string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\FRASS.DAL\\Scribner_BFc_cleaned_for_dbf.csv");
				string[] lines = File.ReadAllLines(csvFilePath);

				// Skip header
				for (int i = 1; i < lines.Length; i++)
				{
					string[] parts = lines[i].Split(',');
					if (parts.Length >= 3)
					{
						int sedIb = int.Parse(parts[0]);
						int lengthFt = int.Parse(parts[1]);
						int boardFeet = (int)double.Parse(parts[2]);

						if (lengthFt == length)
						{
							var scribnerValue = new ScribnerValue
							{
								SED_IB = sedIb,
								Length_ft = lengthFt,
								BoardFeet = boardFeet
							};
							result.Add(scribnerValue);
						}
					}
				}

				// Debug: Log what we retrieved
				System.Diagnostics.Debug.WriteLine($"Retrieved {result.Count} scribner values from CSV for length {length}ft");
				if (result.Count > 0)
				{
					System.Diagnostics.Debug.WriteLine($"First few values: SED={result[0].SED_IB}, BF={result[0].BoardFeet}, Length={result[0].Length_ft}");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("No scribner values found for the specified length in CSV.");
				}

				return result;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error reading scribner values from CSV: {ex.Message}");
				return new List<ScribnerValue>();
			}
		}

		private List<HistoricLogPrice> ConvertToHistoricLogPrices(IHistoricPrice price)
		{
			var hlps = new List<HistoricLogPrice>();
			if (price.SMPrice.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 11, price.SMPrice.Value));
			}
			if (price.Saw2Price.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 2, price.Saw2Price.Value));
			}
			if (price.Saw3Price.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 3, price.Saw3Price.Value));
			}
			if (price.Saw4Price.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 4, price.Saw4Price.Value));
			}
			if (price.Saw4CNPrice.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 5, price.Saw4CNPrice.Value));
			}
			if (price.PulpPrice.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 10, price.PulpPrice.Value));
			}
			if (price.CamprunPrice.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 6, price.CamprunPrice.Value));
			}
			if (price.Export12Price.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 8, price.Export12Price.Value));
			}
			if (price.Export8Price.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 9, price.Export8Price.Value));
			}
			if (price.ChipPrice.HasValue)
			{
				hlps.Add(GetHistoricLogPrice(price, 14, price.ChipPrice.Value));
			}
			return hlps;
		}

		private HistoricLogPrice GetHistoricLogPrice(IHistoricPrice historicPrice, int timberMarketID, decimal price)
		{
			var hlp = new HistoricLogPrice();
			hlp.Year = historicPrice.Year;
			hlp.Month = historicPrice.Month;
			hlp.LogMarketReportSpeciesID = historicPrice.LogMarketReportSpeciesID;
			hlp.TimberMarketID = timberMarketID;
			hlp.Price = price;

			return hlp;
		}

		public TreeData GetRandomTreeFrom2025()
		{
			try
			{
				var result = db.ExecuteQuery<TreeData>(
					"SELECT TOP 1 Species, DBH, Height, CR, VolPerTreeCU, VolPerTreeBF FROM [dbo].[2025] ORDER BY NEWID()"
				).FirstOrDefault();
				return result;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error retrieving random tree from 2025 table: {ex.Message}");
				return null;
			}
		}
        public int? GetSpeciesIdByAbbreviation(string abbreviation)
        {
            var species = (from sp in db.Species where sp.Abbreviation == abbreviation select sp).FirstOrDefault();
            return species?.SpeciesID;
        }

        public (int TimberGradeID, int? TimberMarketID)? GetTimberGradeAndMarketIds(int speciesId, string sortCode)
        {
            var timberGrade = (from tg in db.TimberGrades
                               where tg.SpeciesID == speciesId && tg.SortCode == sortCode
                               select new { tg.TimberGradeID, tg.TimberMarketID }).FirstOrDefault();
            if (timberGrade == null) return null;
            return (timberGrade.TimberGradeID, timberGrade.TimberMarketID);
        }

        public decimal GetLatestHistoricLogPrice(Int32 LogMarketReportSpeciesID, Int32 TimberMarketID)
        {
            var latestPrice = (from hlp in db.HistoricLogPrices
                               where hlp.LogMarketReportSpeciesID == LogMarketReportSpeciesID && hlp.TimberMarketID == TimberMarketID
                               orderby hlp.Year descending, hlp.Month descending
                               select hlp).FirstOrDefault();

            return latestPrice != null ? latestPrice.Price : 0;
        }
        public Int32 GetLogMarketReportSpeciesIDByAbbreviation(string abbreviation)
        {
            var species = (from l in db.LogMarketReportSpecies where l.LogMarketSpeciesAbbreviations == abbreviation select l).FirstOrDefault();
            return species != null ? species.LogMarketReportSpeciesID : 0;
        }

        public Int32 GetLogMarketReportSpeciesIDBySpeciesId(Int32 speciesId)
        {
            var species = (from l in db.LogMarketReportSpecies where l.SpeciesID == speciesId select l).FirstOrDefault();
            return species != null ? species.LogMarketReportSpeciesID : 0;
        }
    }

	/// <summary>
	/// Data transfer object for tree data from 2025 table
	/// </summary>
	[Serializable]
	public class TreeData
	{
		public string Species { get; set; }
		public double DBH { get; set; }
		public double Height { get; set; }
		public double CR { get; set; }
		public double VolPerTreeCU { get; set; }
		public double VolPerTreeBF { get; set; }
	}

	/// <summary>
	/// Data transfer object for scribner volume values
	/// </summary>
	public class ScribnerValue
	{
		public int SED_IB { get; set; }
		public int Length_ft{ get; set; }

		public int BoardFeet { get; set; }
	}
}
