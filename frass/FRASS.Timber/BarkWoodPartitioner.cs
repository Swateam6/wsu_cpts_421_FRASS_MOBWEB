// ============================================================================
// File:        BarkWoodPartitioner.cs
// Project:     FRASS Merchandiser – Pulp Integration
// Author:      Dr. William E. Schlosser / Forest Econometrics / D & D Larix, LLC
// Created:     2025-11-23
// Description: 
//   Partitions pulp segment cubic-foot volume into wood and bark components
//   using taper-derived DIB values and species-specific bark thickness
//   equations. This module consumes PulpSegmentVolume results and produces
//   BarkWoodPartitionedSegment records that downstream components can use for
//   specific gravity (SG) weighting and delivered pulp tonnage calculations.
//
//   This module does not apply SG or market prices. Those are handled by
//   SGWeightCalculator and PulpValueIntegrator.
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
    /// Abstraction for a bark thickness provider based on FIA equations.
    /// Implementations are expected to use species-specific bark models
    /// consistent with FIA and the Flex Taper calibration.
    /// </summary>
    public interface IBarkThicknessProvider
    {
        /// <summary>
        /// Returns bark thickness (inches) at a given height and diameter
        /// inside bark (DIB, inches) for a particular species.
        /// </summary>
        /// <param name="speciesCode">Species code matching FIA/FRASS convention.</param>
        /// <param name="heightFeet">Height above stump (feet).</param>
        /// <param name="dibInches">Diameter inside bark (inches).</param>
        double GetBarkThicknessInches(string speciesCode, double heightFeet, double dibInches);
    }

    /// <summary>
    /// Represents a pulp segment with its volume partitioned into wood
    /// and bark components. This is still a purely geometric result.
    /// </summary>
    public sealed class BarkWoodPartitionedSegment
    {
        /// <summary>
        /// Original geometric segment information (DIB and total volume).
        /// </summary>
        public PulpSegmentVolume Segment { get; }

        /// <summary>
        /// Wood volume (cubic feet) for this segment.
        /// </summary>
        public double WoodVolumeCubicFeet { get; }

        /// <summary>
        /// Bark volume (cubic feet) for this segment.
        /// </summary>
        public double BarkVolumeCubicFeet { get; }

        public BarkWoodPartitionedSegment(
            PulpSegmentVolume segment,
            double woodVolumeCubicFeet,
            double barkVolumeCubicFeet)
        {
            Segment = segment ?? throw new ArgumentNullException(nameof(segment));
            WoodVolumeCubicFeet = woodVolumeCubicFeet;
            BarkVolumeCubicFeet = barkVolumeCubicFeet;
        }
    }

    /// <summary>
    /// Uses species-specific bark thickness equations to partition pulp
    /// segment volume into wood and bark components. Relies on DIB-based
    /// PulpSegmentVolume input and a bark thickness provider tied to FIA.
    /// </summary>
    public sealed class BarkWoodPartitioner
    {
        private readonly IBarkThicknessProvider _barkThicknessProvider;

        public BarkWoodPartitioner(IBarkThicknessProvider barkThicknessProvider)
        {
            _barkThicknessProvider = barkThicknessProvider
                ?? throw new ArgumentNullException(nameof(barkThicknessProvider));
        }

        /// <summary>
        /// Partitions a sequence of pulp segments into wood and bark volumes
        /// for a given species. Each segment uses taper-derived DIB values,
        /// and bark thickness is obtained at lower, mid, and upper heights.
        /// </summary>
        /// <param name="speciesCode">Species code for SG and bark equations.</param>
        /// <param name="segments">Pulp segments produced by PulpLogVolumeCalculator.</param>
        public IEnumerable<BarkWoodPartitionedSegment> PartitionSegments(
            string speciesCode,
            IEnumerable<PulpSegmentVolume> segments)
        {
            if (speciesCode == null) throw new ArgumentNullException(nameof(speciesCode));
            if (segments == null) throw new ArgumentNullException(nameof(segments));

            foreach (var segment in segments)
            {
                yield return PartitionSegment(speciesCode, segment);
            }
        }

        /// <summary>
        /// Partitions a single segment into wood and bark volumes using
        /// bark thickness at the lower, mid, and upper points.
        /// </summary>
        public BarkWoodPartitionedSegment PartitionSegment(
            string speciesCode,
            PulpSegmentVolume segment)
        {
            if (speciesCode == null) throw new ArgumentNullException(nameof(speciesCode));
            if (segment == null) throw new ArgumentNullException(nameof(segment));

            double zLow = segment.LowerHeightFeet;
            double zHigh = segment.UpperHeightFeet;
            double zMid = 0.5 * (zLow + zHigh);

            double dibLow = segment.DibLowerInches;
            double dibMid = segment.DibMidInches;
            double dibHigh = segment.DibUpperInches;

            // Bark thickness in inches at lower, mid, and upper positions.
            double barkLow = _barkThicknessProvider.GetBarkThicknessInches(speciesCode, zLow, dibLow);
            double barkMid = _barkThicknessProvider.GetBarkThicknessInches(speciesCode, zMid, dibMid);
            double barkHigh = _barkThicknessProvider.GetBarkThicknessInches(speciesCode, zHigh, dibHigh);

            // Compute wood and total (wood + bark) volumes via three-area approximation.
            double woodVolume = ComputeVolumeCubicFeet(dibLow, dibMid, dibHigh, segment.LengthFeet);
            double totalVolume = ComputeVolumeCubicFeet(
                GetDobFromDibAndBark(dibLow, barkLow),
                GetDobFromDibAndBark(dibMid, barkMid),
                GetDobFromDibAndBark(dibHigh, barkHigh),
                segment.LengthFeet);

            // Bark volume is the difference. Guard against small negative values
            // from rounding by clamping at zero.
            double barkVolume = totalVolume - woodVolume;
            if (barkVolume < 0.0)
            {
                barkVolume = 0.0;
            }

            return new BarkWoodPartitionedSegment(segment, woodVolume, barkVolume);
        }

        /// <summary>
        /// Computes log segment volume (cubic feet) from diameters (inches)
        /// at lower, mid, and upper points, using a three-area frustum-style
        /// approximation. Length is in feet.
        /// </summary>
        private static double ComputeVolumeCubicFeet(
            double dLowerInches,
            double dMidInches,
            double dUpperInches,
            double lengthFeet)
        {
            if (lengthFeet <= 0.0) return 0.0;
            if (dLowerInches <= 0.0 && dUpperInches <= 0.0) return 0.0;

            double r1 = (dLowerInches / 2.0) / 12.0;
            double r2 = (dUpperInches / 2.0) / 12.0;
            double rm = (dMidInches / 2.0) / 12.0;

            double a1 = Math.PI * r1 * r1;
            double a2 = Math.PI * r2 * r2;
            double am = Math.PI * rm * rm;

            double volume = (lengthFeet / 3.0) * (a1 + 4.0 * am + a2);
            return volume;
        }

        /// <summary>
        /// Computes diameter outside bark (DOB, inches) from DIB and bark
        /// thickness (inches). Bark thickness is assumed radial.
        /// </summary>
        private static double GetDobFromDibAndBark(double dibInches, double barkThicknessInches)
        {
            if (dibInches <= 0.0) return 0.0;
            if (barkThicknessInches <= 0.0) return dibInches;

            // DIB is across wood only. Add bark thickness on both sides.
            return dibInches + 2.0 * barkThicknessInches;
        }
    }
}
