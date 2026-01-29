// ============================================================================
// File:        SGWeightCalculator.cs
// Project:     FRASS Merchandiser – Pulp Integration
// Author:      Dr. William E. Schlosser / Forest Econometrics / D & D Larix, LLC
// Created:     2025-11-23
// Description: 
//   Applies species-specific specific gravity (SG) values to partitioned pulp
//   segment volumes (wood and bark) to calculate delivered green tons.
//   This module consumes BarkWoodPartitionedSegment objects and returns
//   SG-weighted tonnage results for each segment or an entire pulp zone.
//
//   This module does not apply market prices. Pricing is handled by the
//   PulpValueIntegrator.
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

namespace Frass.Merchandiser.Pulp
{
    /// <summary>
    /// Abstraction for a data provider that accesses SG_wood and SG_bark
    /// from the FIA-based Specific Gravity SQL tables.
    /// </summary>
    public interface ISpecificGravityProvider
    {
        /// <summary>
        /// Returns SG_wood (oven-dry weight / green volume) for a species.
        /// </summary>
        double GetWoodSG(string speciesCode);

        /// <summary>
        /// Returns SG_bark (oven-dry weight / green volume) for a species.
        /// </summary>
        double GetBarkSG(string speciesCode);
    }

    /// <summary>
    /// Represents the mass results of SG-applied weighting to a single
    /// pulp segment.
    /// </summary>
    public sealed class SGWeightedSegment
    {
        public BarkWoodPartitionedSegment PartitionedSegment { get; }

        /// <summary>
        /// Wood tons (green tons) derived from V_wood × SG_wood.
        /// </summary>
        public double WoodTons { get; }

        /// <summary>
        /// Bark tons (green tons) derived from V_bark × SG_bark.
        /// </summary>
        public double BarkTons { get; }

        /// <summary>
        /// Total green tons for the pulp segment.
        /// </summary>
        public double TotalTons => WoodTons + BarkTons;

        public SGWeightedSegment(
            BarkWoodPartitionedSegment partitionedSegment,
            double woodTons,
            double barkTons)
        {
            PartitionedSegment = partitionedSegment
                ?? throw new ArgumentNullException(nameof(partitionedSegment));
            WoodTons = woodTons;
            BarkTons = barkTons;
        }
    }

    /// <summary>
    /// Applies FIA SG values to pulp segments to compute wood, bark, and total
    /// delivered green tons. Works directly with the output of the
    /// BarkWoodPartitioner.
    /// </summary>
    public sealed class SGWeightCalculator
    {
        private readonly ISpecificGravityProvider _sgProvider;

        public SGWeightCalculator(ISpecificGravityProvider sgProvider)
        {
            _sgProvider = sgProvider
                ?? throw new ArgumentNullException(nameof(sgProvider));
        }

        /// <summary>
        /// Computes SG-weighted tonnage for a set of partitioned pulp segments.
        /// </summary>
        public IEnumerable<SGWeightedSegment> ComputeSegments(
            string speciesCode,
            IEnumerable<BarkWoodPartitionedSegment> segments)
        {
            if (speciesCode == null) throw new ArgumentNullException(nameof(speciesCode));
            if (segments == null) throw new ArgumentNullException(nameof(segments));

            double sgWood = _sgProvider.GetWoodSG(speciesCode);
            double sgBark = _sgProvider.GetBarkSG(speciesCode);

            foreach (var segment in segments)
            {
                yield return ComputeSegment(sgWood, sgBark, segment);
            }
        }

        /// <summary>
        /// Computes total tons for a single segment.
        /// </summary>
        private SGWeightedSegment ComputeSegment(
            double sgWood,
            double sgBark,
            BarkWoodPartitionedSegment segment)
        {
            if (segment == null) throw new ArgumentNullException(nameof(segment));

            double woodTons = segment.WoodVolumeCubicFeet * sgWood;
            double barkTons = segment.BarkVolumeCubicFeet * sgBark;

            return new SGWeightedSegment(segment, woodTons, barkTons);
        }

        /// <summary>
        /// Computes total tons across all pulp segments for a tree.
        /// </summary>
        public double ComputeTotalTons(
            string speciesCode,
            IEnumerable<BarkWoodPartitionedSegment> segments)
        {
            double total = 0.0;

            foreach (var weighted in ComputeSegments(speciesCode, segments))
            {
                total += weighted.TotalTons;
            }

            return total;
        }
    }
}
