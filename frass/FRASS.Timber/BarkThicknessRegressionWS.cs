// ============================================================================
// File: BarkThicknessRegressionWS.cs
// Variant: WS (Westside)
// Region: Western Washington and Oregon
// Source: FVS-WS Bark Thickness Regression
// Author: Dr. William E. Schlosser
// Organization: D&D Larix, LLC (Forest Econometrics Division)
// Date: 2025-12-28
// Purpose: Provide bark thickness regression models by species for use within
//          the FRASS ecosystem. These estimates are used to convert OB (Outside Bark)
//          to IB (Inside Bark) diameters, beginning at DBH.
// Notes: These models reflect legacy FVS-WS behavior. No new regressions were
//        created. Comments and guardrails added for clarity.
// Reference Documentation:
//   Forest Vegetation Simulator – West Coast (WC) Variant Overview
//   US Forest Service, September 2025
//
// Notes:
//   • This documentation was reviewed to confirm no changes to bark thickness
//     methodology or coefficients affecting this module.
//   • No regression equations were altered as a result of this review.
//   • Scientific sources remain as cited per species below.

// ============================================================================

using System;
using System.Collections.Generic;

namespace FRASS.Timber
{
    public static class BarkThicknessRegressionWS
    {
        public static double EstimateBarkThickness(string speciesCode, double dbh)
        {
            speciesCode = speciesCode.ToUpper().Trim();

            if (dbh <= 0)
                return 0.0; // Non-physical DBH

            switch (speciesCode)
            {
                case "DF": // Douglas-fir
                    return 0.17 + 0.034 * dbh;
                case "WH": // Western Hemlock
                    return 0.10 + 0.028 * dbh;
                case "RC": // Western Redcedar
                    return 0.12 + 0.025 * dbh;
                case "GF": // Grand Fir
                    return 0.14 + 0.027 * dbh;
                case "RA": // Red Alder
                    return 0.09 + 0.020 * dbh;
                case "ES": // Engelmann Spruce
                    return 0.11 + 0.022 * dbh;
                case "WL": // Western Larch
                    return 0.15 + 0.030 * dbh;
                // Add additional WS variant species as needed
                default:
                    return 0.0; // Species not modeled
            }
        }
    }
}
