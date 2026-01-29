// ============================================================================
// File:        PulpValueIntegrator.cs
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

namespace Frass.Merchandiser.Pulp
{
   
    /// <summary>
    /// Computes cubic-foot volume for pulp segments using taper-derived DIB values.
    /// Internally assumes a standard segment length of 4.0 feet, but allows a
    /// configurable length for testing or alternative workflows.
    /// </summary>
    public sealed class PulpValueIntegrator
    {
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
        /// are returned. Negative or pathological DIB values from the taper
        /// solver are defensively clamped to zero.
        /// </summary>
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

                double zMid = 0.5 * (zLow + zHigh);

                // Raw DIB values from Flex Taper.
                double dibLowRaw = taperSolver.GetDiameterInsideBark(zLow);
                double dibHighRaw = taperSolver.GetDiameterInsideBark(zHigh);
                double dibMidRaw = taperSolver.GetDiameterInsideBark(zMid);

                // Clamp any negative values to zero.
                double dibLow = ClampNonNegative(dibLowRaw);
                double dibHigh = ClampNonNegative(dibHighRaw);
                double dibMid = ClampNonNegative(dibMidRaw);

                // If midpoint DIB is zero but ends are positive, approximate it
                // as the average of the two ends. This guards against a single
                // pathological midpoint result without throwing away the segment.
                if (dibMid <= 0.0 && (dibLow > 0.0 || dibHigh > 0.0))
                {
                    dibMid = 0.5 * (dibLow + dibHigh);
                }

                // If after clamping and approximation all three are zero,
                // treat the segment as non-merchantable and skip it.
                if (dibLow <= 0.0 && dibMid <= 0.0 && dibHigh <= 0.0)
                {
                    zLow = zHigh;
                    continue;
                }

                // Determine the small-end DIB for merchantability screening.
                double sedDib = Math.Min(dibLow, dibHigh);

                if (sedDib >= MinimumPulpSedDibInches)
                {
                    double length = zHigh - zLow;
                    double volume = ComputeSegmentVolumeCubicFeet(dibLow, dibMid, dibHigh, length);

                    if (volume > 0.0)
                    {
                        yield return new PulpSegmentVolume(
                            lowerHeightFeet: zLow,
                            upperHeightFeet: zHigh,
                            dibLowerInches: dibLow,
                            dibUpperInches: dibHigh,
                            dibMidInches: dibMid,
                            volumeCubicFeet: volume);
                    }
                }

                zLow = zHigh;
            }
        }

        /// <summary>
        /// Computes the cubic-foot volume for a single segment using taper-derived
        /// DIB at lower, mid, and upper positions. The method uses a frustum-based
        /// approximation for the log segment. Diameters are in inches, length in feet.
        /// Negative or zero diameters will produce zero volume.
        /// </summary>
        public double ComputeSegmentVolumeCubicFeet(
            double dibLowerInches,
            double dibMidInches,
            double dibUpperInches,
            double lengthFeet)
        {
            if (lengthFeet <= 0.0) return 0.0;

            // Clamp diameters again at this level for additional safety.
            double d1 = ClampNonNegative(dibLowerInches);
            double d2 = ClampNonNegative(dibUpperInches);
            double dm = ClampNonNegative(dibMidInches);

            if (d1 <= 0.0 && d2 <= 0.0 && dm <= 0.0)
            {
                return 0.0;
            }

            // Convert diameters (inches) to radii (feet).
            double r1 = (d1 / 2.0) / 12.0;
            double r2 = (d2 / 2.0) / 12.0;
            double rm = (dm / 2.0) / 12.0;

            double a1 = Math.PI * r1 * r1;
            double a2 = Math.PI * r2 * r2;
            double am = Math.PI * rm * rm;

            double volume = (lengthFeet / 3.0) * (a1 + 4.0 * am + a2);
            return volume;
        }

        /// <summary>
        /// Ensures that a diameter cannot be negative. Negative values are
        /// treated as measurement or model artifacts and clamped to zero.
        /// </summary>
        private static double ClampNonNegative(double value)
        {
            return value < 0.0 ? 0.0 : value;
        }
    }
}
