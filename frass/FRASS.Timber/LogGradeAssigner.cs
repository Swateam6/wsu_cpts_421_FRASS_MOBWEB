// ============================================================================
// File:        LogGradeAssigner.cs
// Project:     FRASS Merchandiser – Pulp Integration
// Author:      Dr. William E. Schlosser / Forest Econometrics / D&D Larix, LLC
// Created:     2025-11-24
// Description: 
//   Assigns sawlog grades based on small-end diameter inside bark (SED DIB),
//   log geometry, species, and crown position using Crown Ratio (CR).
//   This module enforces the biological rule that logs intersecting the live
//   crown must receive a lower grade, while also enforcing the special case
//   that cedar logs never become pulp.
//
//   The strict crown-based grade rule implemented here is:
//     1-Saw in crown → 2-Saw
//     2-Saw in crown → 3-Saw
//     3-Saw in crown → 3-Saw (no change)
//     4-Saw in crown → 4-Saw (no change)
//
//   Cedar that fails sawlog SED criteria is classified as CedarWaste rather
//   than Pulp. Cedar can never be assigned a pulp grade by this module. The
//   sawlog merchandiser must call this class after DIB has been computed from
//   Flex Taper DOB and FIA bark thickness tables, and before Scribner volume
//   lookup and economic valuation.
//
// Intellectual Property Notice:
//   FRASS, Flex Taper, Flex Cruiser, Merchandiser, and associated analytical
//   methods are proprietary to D&D Larix, LLC / Forest Econometrics.
//   Users retain ownership of their input data and generated outputs. The
//   analytical logic implemented here is licensed for operational use and is
//   not transferred as intellectual property.
// ============================================================================

using System;

namespace Frass.Merchandiser
{
    /// <summary>
    /// Assigns sawlog grades based on small-end DIB (SED), log geometry,
    /// species, and crown position derived from Crown Ratio.
    /// Cedar is handled as a special case: cedar can never become pulp.
    /// Cedar that fails sawlog thresholds is classified as CedarWaste.
    /// </summary>
    public sealed class LogGradeAssigner
    {
        /// <summary>
        /// Log grades recognized by the Westside merchandiser.
        /// CedarWaste is non-merchantable cedar with zero economic value.
        /// </summary>
        public enum LogGrade
        {
            Saw1,
            Saw2,
            Saw3,
            Saw4,
            Pulp,
            CedarWaste
        }

        /// <summary>
        /// Tree-level context required for grade assignment.
        /// SpeciesCode must match the codes used in FIA bark and SG tables.
        /// TotalHeightFt is total tree height in feet.
        /// CrownRatio is expressed as a fraction from 0.0 to 1.0.
        /// </summary>
        public sealed class TreeContext
        {
            public string SpeciesCode { get; }
            public double TotalHeightFt { get; }
            public double CrownRatio { get; }  // 0.0 – 1.0

            public TreeContext(string speciesCode, double totalHeightFt, double crownRatio)
            {
                SpeciesCode = speciesCode;
                TotalHeightFt = totalHeightFt;
                CrownRatio = crownRatio;
            }

            /// <summary>
            /// Height at the base of the live crown (feet above stump),
            /// computed as TotalHeight * (1 - CrownRatio).
            /// </summary>
            public double CrownBaseHeightFt =>
                TotalHeightFt * (1.0 - CrownRatio);

            /// <summary>
            /// Indicates whether the tree species is a cedar species.
            /// The list can be extended if additional cedar codes are used.
            /// </summary>
            public bool IsCedar =>
                IsCedarSpeciesCode(SpeciesCode);

            private static bool IsCedarSpeciesCode(string speciesCode)
            {
                if (string.IsNullOrWhiteSpace(speciesCode))
                    return false;

                var code = speciesCode.Trim().ToUpperInvariant();

                // Western redcedar and common aliases. Extend as needed.
                return code == "RC"     // Redcedar
                    || code == "WRC"    // Western redcedar
                    || code == "CEDAR";
            }
        }

        /// <summary>
        /// Log-level geometry for grade assignment.
        /// StartHeightFt and EndHeightFt are measured from stump height.
        /// SmallEndDibIn is the small-end diameter inside bark (SED DIB).
        /// </summary>
        public sealed class LogGeometry
        {
            public double StartHeightFt { get; }
            public double EndHeightFt { get; }
            public double SmallEndDibIn { get; }

            /// <summary>
            /// Computed log length in feet (EndHeightFt - StartHeightFt).
            /// Sawlog lengths and trim rules are managed by the merchandiser.
            /// </summary>
            public double LengthFt => EndHeightFt - StartHeightFt;

            public LogGeometry(double startHeightFt, double endHeightFt, double smallEndDibIn)
            {
                StartHeightFt = startHeightFt;
                EndHeightFt = endHeightFt;
                SmallEndDibIn = smallEndDibIn;
            }
        }

        /// <summary>
        /// Main entry point for grade assignment.
        /// Steps:
        ///   1. Assign base grade from SED DIB and species.
        ///   2. If base grade is a saw grade and the log intersects the live
        ///      crown, apply the crown-based grade reduction rule.
        ///   3. Return the final grade for use in Scribner and valuation logic.
        /// </summary>
        public LogGrade AssignGrade(TreeContext tree, LogGeometry log)
        {
            var baseGrade = AssignBaseGrade(tree, log);

            // Crown effects do not change Pulp or CedarWaste classifications.
            if (baseGrade == LogGrade.Pulp || baseGrade == LogGrade.CedarWaste)
                return baseGrade;

            if (IsInsideCrown(tree, log))
                baseGrade = DropOneGrade(tree, baseGrade);

            return baseGrade;
        }

        /// <summary>
        /// Assigns the base sawlog grade ignoring crown position.
        /// For cedar:
        ///   - Cedar cannot be graded as Pulp.
        ///   - Grades based on SED and length according to Northwest Log Scaling Handbook.
        ///   - Saw1 (C1): SED >= 28", length >= 16'
        ///   - Saw2 (C2): SED >= 10", length >= 16'
        ///   - Saw3 (C3): SED >= 6", length >= 12'
        ///   - Saw4 (C4): SED >= 5", length >= 12'
        ///   - CedarWaste otherwise.
        /// For non-cedar:
        ///   - Logs with SED below the minimum sawlog threshold are Pulp.
        ///
        /// The SED thresholds shown here are placeholders. They must be replaced
        /// with the official Westside rules used by FRASS.
        /// </summary>
        private LogGrade AssignBaseGrade(TreeContext tree, LogGeometry log)
        {
            double sed = log.SmallEndDibIn;
            double length = log.LengthFt;

            string species = tree.SpeciesCode;

            // CEDAR
            if (species == "RC" ||
                species == "WRC" ||
                species == "RW")
            {
                if (length >= 16.0)
                {
                    if (sed >= 28.0)
                        return LogGrade.Saw1;
                    if (sed >= 20.0)
                        return LogGrade.Saw2;
                }
                if (length >= 12.0)
                {
                    if (sed >= 6.0)
                        return LogGrade.Saw3;
                    if (sed >= 5.0)
                        return LogGrade.Saw4;
                }

                return LogGrade.CedarWaste;
            }
            // Douglas Fir
            else if (species == "DF")
            {
                if (length >= 16.0)
                {
                    if (sed >= 30.0)
                        return LogGrade.Saw1;                    
                }
                if (length >= 12.0)
                {
                    if (sed >= 12.0)
                        return LogGrade.Saw2;
                    if (sed >= 6.0)
                        return LogGrade.Saw3;
                    if (sed >= 5.0)
                        return LogGrade.Saw4;
                }
                return LogGrade.Pulp;
            }
            // Western Hemlock and Equivilants
            else if (species == "WH" ||
                species == "YC" ||
                species == "GF" ||
                species == "NF")
            {
                if (length >= 16.0)
                {
                    if (sed >= 24.0)
                        return LogGrade.Saw1;
                    if (sed >= 12.0)
                        return LogGrade.Saw2;
                }
                if (length >= 12.0)
                {                    
                    if (sed >= 6.0)
                        return LogGrade.Saw3;
                    if (sed >= 5.0)
                        return LogGrade.Saw4;
                }
                return LogGrade.Pulp;
            }
            // Red Alder and Equivilants
            else if (species == "RA" ||
                species == "BM")
            {
                if (length >= 8.0)
                {
                    if (sed >= 16.0)
                        return LogGrade.Saw1;
                    if (sed >= 12.0)
                        return LogGrade.Saw2;
                    if (sed >= 10.0)
                        return LogGrade.Saw3;
                    if (sed >= 5.0)
                        return LogGrade.Saw4;
                }
                return LogGrade.Pulp;
            }
            // Cottonwood and Equivilants
            else if (species == "CW" ||
                species == "WI")
            {
                if (length >= 8.0)
                {
                    if (sed >= 10.0)
                        return LogGrade.Saw1;
                    if (sed >= 6.0)
                        return LogGrade.Saw2;
                    if (sed >= 5.0)
                        return LogGrade.Saw3;
                }
                return LogGrade.Pulp;
            }
            // Western Hemlock and Equivilants
            else if (species == "SS")
            {                
                if (length >= 16.0)
                {
                    if (sed >= 6.0)
                        return LogGrade.Saw3;
                    if (sed >= 5.0)
                        return LogGrade.Saw4;
                }
                return LogGrade.Pulp;
            }
            else if (species == "PY" ||
                species == "OT")
            {
                // Pulp only Species
                return LogGrade.Pulp;
            }
            else
            {
                // Default thresholds (example values). Replace with official rules.
                const double minSawSed = 5.5;   // Minimum SED DIB for sawlog
                const double sedSaw1 = 16.0;
                const double sedSaw2 = 12.0;
                const double sedSaw3 = 9.0;

                if (sed < minSawSed)
                    return LogGrade.Pulp;

                if (sed >= sedSaw1)
                    return LogGrade.Saw1;

                if (sed >= sedSaw2)
                    return LogGrade.Saw2;

                if (sed >= sedSaw3)
                    return LogGrade.Saw3;

                return LogGrade.Saw4;
            }
        }

        /// <summary>
        /// Returns true if any part of the log intersects the live crown.
        /// The log is considered to intersect the crown if its top is above
        /// the crown base height.
        /// </summary>
        private bool IsInsideCrown(TreeContext tree, LogGeometry log)
        {
            var crownBase = tree.CrownBaseHeightFt;

            // If the log top is above the crown base, it intersects the crown.
            return log.EndHeightFt > crownBase;
        }

        /// <summary>
        /// Applies the strict crown-based grade reduction rule.
        /// For all species (including cedar):
        ///   Saw1 in crown → Saw2
        ///   Saw2 in crown → Saw3
        ///   Saw3 in crown → Saw3 (no change)
        ///   Saw4 in crown → Saw4 (no change)
        ///
        /// Cedar cannot become Pulp through this mechanism. Any non-saw grades
        /// (Pulp, CedarWaste) are not passed into this method from AssignGrade.
        /// </summary>
        private LogGrade DropOneGrade(TreeContext tree, LogGrade grade)
        {
            switch (grade)
            {
                case LogGrade.Saw1:
                    return LogGrade.Saw2;

                case LogGrade.Saw2:
                    return LogGrade.Saw3;

                case LogGrade.Saw3:
                    return LogGrade.Saw3;

                case LogGrade.Saw4:
                    return LogGrade.Saw4;

                default:
                    return grade;
            }
        }
    }
}
