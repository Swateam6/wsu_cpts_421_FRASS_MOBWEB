using System;
using System.Collections.Generic;
using System.Linq;
using FRASS.Timber;
using FRASS.Interfaces;
using FRASS.DAL.Repositories;

namespace FRASS.BLL.Models.Services
{
    public class TimberProcessingService
    {
        private TimberRepository _timberRepository;
        private TreeLogMerchandizer treeLogMerchandizer = new TreeLogMerchandizer();

        public TimberProcessingService()
        {
            _timberRepository = TimberRepository.GetInstance();
        }

        /// <summary>
        /// Processes timber for a stand using FRASS.Timber algorithms
        /// </summary>
        /// <param name="standId">The stand ID</param>
        /// <param name="parcelId">The parcel ID</param>
        /// <param name="treeInputs">List of tree inputs for the stand</param>
        /// <returns>List of processed log segments</returns>
        public List<LogSegment> ProcessStandTimber(int standId, int parcelId, List<TreeInput> treeInputs)
        {
            var allLogSegments = new List<LogSegment>();

            // Get scribner table for volume calculations
            // TODO: GET LENGTH IN FEET
            var scribnerTable = this._timberRepository.GetScribnerTable(32);

            foreach (var treeInput in treeInputs)
            {
                try
                {
                    var logSegments = this.treeLogMerchandizer.ProcessTree(treeInput, scribnerTable);
                    allLogSegments.AddRange(logSegments);
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other trees
                    System.Diagnostics.Debug.WriteLine($"Error processing tree: {ex.Message}");
                }
            }

            return allLogSegments;
        }

        /// <summary>
        /// Calculates stand volumes using FRASS.Timber algorithms
        /// </summary>
        /// <param name="standId">The stand ID</param>
        /// <param name="parcelId">The parcel ID</param>
        /// <param name="treeInputs">List of tree inputs for the stand</param>
        /// <returns>Dictionary of volume calculations by grade</returns>
        public Dictionary<string, decimal> CalculateStandVolumes(int standId, int parcelId, List<TreeInput> treeInputs)
        {
            var volumesByGrade = new Dictionary<string, decimal>();
            var logSegments = ProcessStandTimber(standId, parcelId, treeInputs);

            foreach (var segment in logSegments)
            {
                if (!volumesByGrade.ContainsKey(segment.Grade))
                {
                    volumesByGrade[segment.Grade] = 0;
                }
                volumesByGrade[segment.Grade] += segment.BFVolume;
            }

            return volumesByGrade;
        }

        /// <summary>
        /// Converts stand data to tree inputs for processing
        /// This is a bridge method until tree-level inventory is implemented
        /// </summary>
        /// <param name="standData">Current stand data</param>
        /// <returns>List of tree inputs derived from stand data</returns>
        public List<TreeInput> ConvertStandDataToTreeInputs(List<IStandData> standData)
        {
            var treeInputs = new List<TreeInput>();

            // Group by species and create representative trees
            var speciesGroups = standData.GroupBy(sd => sd.SpeciesID);

            foreach (var speciesGroup in speciesGroups)
            {
                // Calculate average DBH and height from stand data
                // This is a simplified approach - in reality you'd need actual tree measurements
                var avgBoardFeet = speciesGroup.Average(sd => sd.Board_SN);
                var totalAcres = speciesGroup.Sum(sd => sd.Acres);

                // Estimate number of trees based on volume and acres
                // This is a rough estimation - actual implementation would need tree inventory
                var estimatedTreesPerAcre = 300; // Typical for managed forests
                var totalTrees = (int)(totalAcres * estimatedTreesPerAcre);

                if (totalTrees > 0)
                {
                    // Create representative trees for this species
                    for (int i = 0; i < Math.Min(totalTrees, 10); i++) // Limit for performance
                    {
                        var treeInput = new TreeInput
                        {
                            Species = speciesGroup.First().Abbreviation,
                            DBH = EstimateDBH(avgBoardFeet),
                            Height = EstimateHeight(avgBoardFeet),
                            CR = 0.7, // Typical crown ratio
                            CFV_Target = (double)(avgBoardFeet / totalTrees)
                        };
                        treeInputs.Add(treeInput);
                    }
                }
            }

            return treeInputs;
        }

        /// <summary>
        /// Processes a stand using existing stand data converted to tree inputs
        /// </summary>
        /// <param name="standId">The stand ID</param>
        /// <param name="parcelId">The parcel ID</param>
        /// <param name="standData">Current stand data</param>
        /// <returns>Dictionary of volumes by grade</returns>
        public Dictionary<string, decimal> ProcessStandFromStandData(int standId, int parcelId, List<IStandData> standData)
        {
            var treeInputs = ConvertStandDataToTreeInputs(standData);
            return CalculateStandVolumes(standId, parcelId, treeInputs);
        }


        private double EstimateDBH(decimal boardFeetPerTree)
        {
            // Rough estimation - actual implementation would use forest inventory data
            // DBH = f(board feet) - simplified relationship
            return Math.Sqrt((double)boardFeetPerTree / 0.4); // Rough approximation
        }

        private double EstimateHeight(decimal boardFeetPerTree)
        {
            // Rough estimation - actual implementation would use forest inventory data
            // Height = f(board feet) - simplified relationship
            return Math.Pow((double)boardFeetPerTree / 10.0, 0.5) * 50; // Rough approximation
        }
    }
}
