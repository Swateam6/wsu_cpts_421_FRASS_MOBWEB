using FRASS.DAL;
using FRASS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FRASS.Timber
{
    public class GradeToSortCodeMapper
    {
        private TimberRepository _timberRepository;
        private Dictionary<string, Dictionary<string, string>> _gradeToSortCodeCache;

        public GradeToSortCodeMapper()
        {
            _timberRepository = TimberRepository.GetInstance();
            _gradeToSortCodeCache = new Dictionary<string, Dictionary<string, string>>();
            LoadMappings();
        }

        private void LoadMappings()
        {
            var speciesList = _timberRepository.GetSpecies();
            var timberGrades = _timberRepository.GetTimberGrades();

            foreach (var species in speciesList)
            {
                var speciesMappings = new Dictionary<string, string>();
                var speciesGrades = timberGrades.Where(tg => tg.SpeciesID == species.SpeciesID).ToList();

                // Map ProcessTree grades to sort codes
                // Saw1 -> 1, Saw2 -> 2, etc.
                // Pulp -> P4 (or appropriate pulp code for species)
                // CedarWaste -> special handling

                // For saw grades, find sort codes that match the pattern
                for (int gradeNum = 1; gradeNum <= 5; gradeNum++)
                {
                    var sortCode = GetSortCodeForGrade(speciesGrades, gradeNum);
                    if (!string.IsNullOrEmpty(sortCode))
                    {
                        speciesMappings[$"Saw{gradeNum}"] = sortCode;
                    }
                }

                // For pulp, find P4 or appropriate pulp code
                var pulpCode = speciesGrades.FirstOrDefault(tg => tg.SortCode.StartsWith("P"))?.SortCode ?? "P4";
                speciesMappings["Pulp"] = pulpCode;

                // For CedarWaste, use pulp or special
                speciesMappings["CedarWaste"] = "CedarWaste";

                _gradeToSortCodeCache[species.Abbreviation] = speciesMappings;
            }
        }

        private string GetSortCodeForGrade(List<TimberGrade> speciesGrades, int gradeNum)
        {
            // For conifers, D1, D2, etc.
            // For hardwoods, H1, H2, etc.
            // For cedar, C1, C2, etc.
            var possibleCodes = new List<string>();

            if (gradeNum == 1)
            {
                possibleCodes.AddRange(new[] { "D1", "H1", "B1", "C1" });
            }
            else
            {
                possibleCodes.AddRange(new[] { $"D{gradeNum}", $"H{gradeNum}", $"B{gradeNum}", $"C{gradeNum}" });
            }

            foreach (var code in possibleCodes)
            {
                var match = speciesGrades.FirstOrDefault(tg => tg.SortCode == code);
                if (match != null)
                {
                    return match.SortCode;
                }
            }

            return null;
        }

        public string GetSortCode(string speciesAbbreviation, string grade)
        {
            if (_gradeToSortCodeCache.TryGetValue(speciesAbbreviation, out var speciesMappings))
            {
                if (speciesMappings.TryGetValue(grade, out var sortCode))
                {
                    return sortCode;
                }
            }
            return null;
        }

        public int GetTimberGradeID(string speciesAbbreviation, string grade)
        {
            var sortCode = GetSortCode(speciesAbbreviation, grade);
            if (string.IsNullOrEmpty(sortCode))
                return 0;

            var timberGrades = _timberRepository.GetTimberGrades();
            var species = _timberRepository.GetSpecies().FirstOrDefault(s => s.Abbreviation == speciesAbbreviation);
            if (species == null)
                return 0;

            var timberGrade = timberGrades.FirstOrDefault(tg => tg.SpeciesID == species.SpeciesID && tg.SortCode == sortCode);
            return timberGrade?.TimberGradeID ?? 0;
        }
    }
}
