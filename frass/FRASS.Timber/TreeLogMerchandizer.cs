using Frass.Merchandiser;
using FRASS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FRASS.Timber
{
    public class TreeLogMerchandizer
    {
        private GradeToSortCodeMapper _gradeMapper = new GradeToSortCodeMapper();
        private TimberRepository _timberRepository = TimberRepository.GetInstance();
        public List<LogSegment> ProcessTree(TreeInput tree, Dictionary<int, int> scribnerTable,
                                                   double deltaGuess = 0.05, double k = 0.1, double epsilon = 1.0,
                                                   double logLength = 32.0, double trim = 1.5, bool isDemo = false)
        {
            double minSED = GetMinSED(tree.Species);
            
            double IBDBH = tree.DBH - BarkThicknessRegressionWS.EstimateBarkThickness(tree.Species, tree.DBH);
            LogGradeAssigner logGradeAssigner = new LogGradeAssigner();
            double stumpHeight = 1.0;
            if (tree.StumpHeight == null)
            {
                if (IBDBH >= 24)
                {
                    stumpHeight = 2.5;
                }
            }
            else
            {
                stumpHeight = (double)tree.StumpHeight;
            }
            double delta = EstimateDelta(IBDBH, tree.Height, tree.CFV_Target, k, epsilon);
            List<double> taper = FlexTaperModel.GenerateTaperProfile(IBDBH, tree.Height, delta, k, epsilon);

            IFlexTaperSolver flexTaperSolver = new FlexTaperSolver(taper);

            List<double> SEDs = LogSegmenter.GetSEDs(taper, stumpHeight, logLength, trim, minSED);

            List<LogSegment> logs = new List<LogSegment>();
            double position = stumpHeight;
            LogGradeAssigner.TreeContext treeContext = new LogGradeAssigner.TreeContext(tree.Species, tree.Height, tree.CR);

            int speciesId = (int) this._timberRepository.GetSpeciesIdByAbbreviation(tree.Species.Trim());
            int LogMarketReportSpeciesID = this._timberRepository.GetLogMarketReportSpeciesIDBySpeciesId(speciesId);

            for (int i = 0; i < SEDs.Count; i++)
            {
                double sed = SEDs[i];

                LogGradeAssigner.LogGeometry log = new LogGradeAssigner.LogGeometry(position, position + logLength + trim, sed);
                string grade = logGradeAssigner.AssignGrade(treeContext, log).ToString();
                string sortCode = _gradeMapper.GetSortCode(tree.Species, grade) ?? grade;
                int volume = VolumeEstimator.LookupVolume(sed, scribnerTable);

                decimal estimatedValue = 0;
                // Only estimate value if it is for the demo to save time.
                if (isDemo)
                {
                    var result = this._timberRepository.GetTimberGradeAndMarketIds(speciesId, sortCode);
                    int timberGradeId;
                    int? timberMarketId;
                    
                    if (result.HasValue)
                    {
                        timberGradeId = result.Value.TimberGradeID;
                        timberMarketId = result.Value.TimberMarketID;
                        if (timberMarketId != null)
                        {
                            decimal pricePerThousand = this._timberRepository.GetLatestHistoricLogPrice(LogMarketReportSpeciesID, (int)timberMarketId);
                            estimatedValue = (pricePerThousand / 1000) * volume;
                        }
                    }
                }                

                logs.Add(new LogSegment
                {
                    BaseHeight = position,
                    SED = sed,
                    Grade = sortCode,
                    BFVolume = volume,
                    length = (int)logLength,
                    SEDHeight = position + logLength + trim,
                    EstimatedValue = (double?)estimatedValue
                });

                position += logLength + trim;
            }

            position = AddOptimizedLog(flexTaperSolver, position, tree, logs, trim, minSED, scribnerTable, logGradeAssigner, treeContext, isDemo);

            if (tree.Species != "RC" && tree.Species != "WRC" && tree.Species != "THPL")
            {
                AddPulpLogs(flexTaperSolver, position, tree, logs, trim);
            }

            return logs;
        }

        private double AddOptimizedLog(IFlexTaperSolver flexTaperSolver, double position, TreeInput tree, List<LogSegment> logs, double trim, double minSED, Dictionary<int, int> scribnerTable, LogGradeAssigner logGradeAssigner, LogGradeAssigner.TreeContext treeContext, bool isDemo = false)
        {
            int speciesId = (int) this._timberRepository.GetSpeciesIdByAbbreviation(tree.Species.Trim());
            int LogMarketReportSpeciesID = this._timberRepository.GetLogMarketReportSpeciesIDBySpeciesId(speciesId);

            double[] possibleLengths = { 28, 24, 20, 16 };
            foreach (double len in possibleLengths)
            {
                double endHeight = position + len;
                if (endHeight <= tree.Height)
                {
                    double sed = flexTaperSolver.GetDiameterInsideBark(endHeight);
                    if (sed >= minSED)
                    {
                        LogGradeAssigner.LogGeometry logGeo = new LogGradeAssigner.LogGeometry(position, endHeight + trim, sed);
                        string grade = logGradeAssigner.AssignGrade(treeContext, logGeo).ToString();
                        string sortCode = _gradeMapper.GetSortCode(tree.Species, grade) ?? grade;
                        scribnerTable = this._timberRepository.GetScribnerTable((int)len);
                        int volume = VolumeEstimator.LookupVolume(sed, scribnerTable);

                        decimal estimatedValue = 0;
                        // Only estimate value if it is for the demo to save time.
                        if (isDemo)
                        {
                            var result = this._timberRepository.GetTimberGradeAndMarketIds(speciesId, sortCode);
                            int timberGradeId;
                            int? timberMarketId;

                            if (result.HasValue)
                            {
                                timberGradeId = result.Value.TimberGradeID;
                                timberMarketId = result.Value.TimberMarketID;
                                if (timberMarketId != null)
                                {
                                    decimal pricePerThousand = this._timberRepository.GetLatestHistoricLogPrice(LogMarketReportSpeciesID, (int)timberMarketId);
                                    estimatedValue = (pricePerThousand / 1000) * volume;
                                }
                            }
                        }

                        logs.Add(new LogSegment
                        {
                            BaseHeight = position,
                            SED = sed,
                            Grade = sortCode,
                            BFVolume = volume,
                            length = (int)len,
                            SEDHeight = endHeight + trim,
                            EstimatedValue = (double?)estimatedValue
                        });
                        return position + len + trim;
                    }
                }
            }
            return position; // no log added
        }

        private void AddPulpLogs(IFlexTaperSolver flexTaperSolver, double position, TreeInput tree, List<LogSegment> logs, double trim)
        {
            // Generate pulp segments on top of the tree after logs
            PulpLogVolumeCalculator pulpCalc = new PulpLogVolumeCalculator();
            List<PulpSegmentVolume> pulpSegments = pulpCalc.GeneratePulpSegments(flexTaperSolver, position, tree.Height).ToList();
            double totalPulpLength = pulpSegments.Sum(p => p.LengthFeet);
            double remainingLength = totalPulpLength;
            double currentBase = position;
            while (remainingLength >= 16)
            {
                double[] validLengths = { 40, 36, 32, 28, 24, 20, 16 };
                double chosenLength = 0;
                foreach (var len in validLengths)
                {
                    if (len <= remainingLength)
                    {
                        chosenLength = len;
                        break;
                    }
                }
                if (chosenLength > 0)
                {
                    double sedHeight = currentBase + chosenLength;
                    double sed = flexTaperSolver.GetDiameterInsideBark(sedHeight);
                    string pulpSortCode = _gradeMapper.GetSortCode(tree.Species, "Pulp") ?? "Pulp";
                    logs.Add(new LogSegment
                    {
                        BaseHeight = currentBase,
                        SED = sed,
                        Grade = pulpSortCode,
                        BFVolume = 0, // Pulp volume is in cubic feet, not board feet
                        length = (int)chosenLength,
                        SEDHeight = sedHeight + trim
                    });
                    remainingLength -= chosenLength;
                    currentBase += chosenLength;
                }
            }
        }

        public static double EstimateDelta(double DBH, double H, double CFV_target, double k, double epsilon)
        {
            double delta = 0.01;
            double step = 0.004;
            double minStep = 0.0001;
            int maxIter = 1000;
            double tolerance = 0.01;
            double previousDirection = 0;

            List<double> profile = FlexTaperModel.GenerateTaperProfile(DBH, H, delta, k, epsilon);

            for (int i = 0; i < maxIter; i++)
            {
                profile = FlexTaperModel.GenerateTaperProfile(DBH, H, delta, k, epsilon);
                double volume = 0.0;
                for (int j = 1; j < profile.Count; j++)
                {
                    double h1 = 4.5 + (j - 1) * epsilon;
                    double h2 = 4.5 + j * epsilon;

                    double d1 = profile[j - 1];
                    double d2 = profile[j];

                    double r1 = d1 / 24;
                    double r2 = d2 / 24;

                    double height = (h2 - h1);

                    double segVol2 = (1.0 / 3.0) * Math.PI * height * ((r1 * r1) + (r1 * r2) + (r2 * r2));
                    volume += segVol2;
                }

                if (Math.Abs(volume - CFV_target) < tolerance)
                    break;

                double currentDirection;
                if (volume > CFV_target)
                {
                    currentDirection = -1;
                }
                else
                {
                    currentDirection = 1;
                }

                if (previousDirection != 0 && previousDirection == -currentDirection)
                {
                    step = Math.Max(step * 0.5, minStep);
                }

                delta += currentDirection * step;
                previousDirection = currentDirection;

                if (delta < -1)
                {
                    return -1.0;
                }
            }
            return delta;
        }
        private static double GetMinSED(string species)
        {
            if (
                species == "RC" ||
                species == "WRC" ||
                species == "THPL" ||
                species == "RW" ||

                species == "DF" ||

                species == "WH" ||
                species == "YC" ||
                species == "GF" ||
                species == "NF" ||

                species == "RA" ||
                species == "BM" ||

                species == "CW" ||
                species == "WI" ||

                species == "SS")
            {
                return 5.0;
            }
            else if (
                species == "PY" ||
                species == "OT")
            {
                // Pulp only Species
                return 2.0;
            }

            // Default
            return 6.0;
        }
    }
}
