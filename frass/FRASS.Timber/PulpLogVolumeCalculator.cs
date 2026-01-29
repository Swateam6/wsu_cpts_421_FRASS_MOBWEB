// ============================================================================
// File:        PulpLogVolumeCalculator.cs
// Project:     FRASS Merchandiser – Pulp Integration
// Author:      Dr. William E. Schlosser / Forest Econometrics / D & D Larix, LLC
// Created:     2025-11-23
// Description: 
//   Provides geometry-based cubic-foot volume calculations for pulp segments
//   using Flex Taper derived diameters inside bark (DIB). Designed to work
//   with a 4-foot internal segment length for taper accuracy, regardless of
//   the final delivered pulp log lengths (16–40 feet).
//
//   This module does not apply specific gravity (SG) or market prices. Those
//   are handled by downstream components (SGWeightCalculator, PulpValueIntegrator).
//
// Intellectual Property Notice:
//   FRASS, Flex Taper, Flex Cruiser, Merchandiser, and associated analytical
//   methods are proprietary to D & D Larix, LLC / Forest Econometrics.
//   Users retain ownership of their input data and generated outputs. The
//   analytical logic implemented here is licensed for operational use and is
//   not transferred as intellectual property.
// ============================================================================

using System;
using System.Collections.Generic;
using FRASS.Timber;

namespace FRASS.Timber
{
    /// <summary>
    /// Computes cubic-foot volume for pulp segments using taper-derived DIB values.
    /// Internally assumes a standard segment length of 4.0 feet, but allows a
    /// configurable length for testing or alternative workflows.
    /// </summary>
    public sealed class PulpLogVolumeCalculator
    {
        /// <summary>
        /// Default internal segment length in feet for pulp calculations.
        /// </summary>
        public const double DefaultPulpSegmentLengthFeet = 4.0;

        /// <summary>
        /// Minimum pulp small-end DIB (inches) for a segment to be considered
        /// merchantable pulp. This follows the Washington DNR guidance that
        /// pulp SED can be as low as 2 inches inside bark.
        /// </summary>
        public const double MinimumPulpSedDibInches = 2.0;

        /// <summary>
        /// Generates internal pulp segments between a given start and end height,
        /// using the Flex Taper solver to obtain DIB at each segment boundary.
        /// Only segments whose small-end DIB meets or exceeds the pulp minimum
        /// are returned.
        /// </summary>
        /// <param name="taperSolver">Flex Taper DIB provider.</param>
        /// <param name="startHeightFeet">Height (feet) where pulp zone begins (top of last sawlog).</param>
        /// <param name="endHeightFeet">Total height (feet) of the tree or pulp cutoff.</param>
        /// <param name="segmentLengthFeet">Internal segment length (feet). Default is 4.0 feet.</param>
        public IEnumerable<PulpSegmentVolume> GeneratePulpSegments(
            IFlexTaperSolver taperSolver,
            double startHeightFeet,
            double endHeightFeet,
            double segmentLengthFeet = DefaultPulpSegmentLengthFeet)
        {
            if (taperSolver == null) throw new ArgumentNullException(nameof(taperSolver));
            if (segmentLengthFeet <= 0.0) throw new ArgumentOutOfRangeException(nameof(segmentLengthFeet));
            if (endHeightFeet <= startHeightFeet) yield break;

            double zLow = startHeightFeet;

            while (zLow < endHeightFeet)
            {
                double zHigh = zLow + segmentLengthFeet;
                if (zHigh > endHeightFeet)
                {
                    zHigh = endHeightFeet;
                }

                // Obtain DIB at lower, upper, and midpoint locations from Flex Taper.
                double dibLow = taperSolver.GetDiameterInsideBark(zLow);
                double dibHigh = taperSolver.GetDiameterInsideBark(zHigh);
                double zMid = 0.5 * (zLow + zHigh);
                double dibMid = taperSolver.GetDiameterInsideBark(zMid);

                // Determine the small-end DIB for merchantability screening.
                double sedDib = Math.Min(dibLow, dibHigh);

                if (sedDib >= MinimumPulpSedDibInches)
                {
                    double volume = ComputeSegmentVolumeCubicFeet(dibLow, dibMid, dibHigh, zHigh - zLow);

                    yield return new PulpSegmentVolume(
                        lowerHeightFeet: zLow,
                        upperHeightFeet: zHigh,
                        dibLowerInches: dibLow,
                        dibUpperInches: dibHigh,
                        dibMidInches: dibMid,
                        volumeCubicFeet: volume);
                }

                zLow = zHigh;
            }
        }

        /// <summary>
        /// Computes the cubic-foot volume for a single segment using taper-derived
        /// DIB at lower, mid, and upper positions. The method uses a frustum-based
        /// approximation for the log segment. Diameters are in inches, length in feet.
        /// </summary>
        /// <param name="dibLowerInches">DIB at the lower end (inches).</param>
        /// <param name="dibMidInches">DIB at the midpoint (inches).</param>
        /// <param name="dibUpperInches">DIB at the upper end (inches).</param>
        /// <param name="lengthFeet">Segment length (feet).</param>
        public double ComputeSegmentVolumeCubicFeet(
            double dibLowerInches,
            double dibMidInches,
            double dibUpperInches,
            double lengthFeet)
        {
            if (lengthFeet <= 0.0) return 0.0;
            if (dibLowerInches <= 0.0 && dibUpperInches <= 0.0) return 0.0;

            // Convert diameters (inches) to radii (feet).
            double r1 = (dibLowerInches / 2.0) / 12.0;
            double r2 = (dibUpperInches / 2.0) / 12.0;
            double rm = (dibMidInches / 2.0) / 12.0;

            double a1 = Math.PI * r1 * r1;
            double a2 = Math.PI * r2 * r2;
            double am = Math.PI * rm * rm;

            // Three-area frustum approximation for log volume in cubic feet.
            double volume = (lengthFeet / 3.0) * (a1 + 4.0 * am + a2);
            double totest = volume;
            return volume;
        }
    }
}
