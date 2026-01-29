// ============================================================================
// File:        BarkAdapter.cs
// Project:     FRASS Merchandiser – Bark and Diameter Conversion
// Author:      Dr. William E. Schlosser / Forest Econometrics / D & D Larix, LLC
// Created:     2025-11-23
// Description: 
//   Provides species-specific conversion between diameter outside bark (DOB)
//   and diameter inside bark (DIB), using FIA-based bark thickness equations
//   stored in SQL. This module is the necessary bridge between field and model
//   measurements (typically DOB) and the Flex Taper and Merchandiser logic
//   that require DIB.
//
//   This adapter does not compute bark thickness itself. It depends on a
//   provider abstraction that reads bark thickness from SQL tables populated
//   from authoritative FIA bark datasets (WC and EC).
//
// Intellectual Property Notice:
//   FRASS, Flex Taper, Flex Cruiser, Merchandiser, and associated analytical
//   methods are proprietary to D & D Larix, LLC / Forest Econometrics.
//   Users retain ownership of their input data and generated outputs. The
//   analytical logic implemented here is licensed for operational use and is
//   not transferred as intellectual property.
// ============================================================================

using System;

namespace Frass.Merchandiser.Bark
{
    /// <summary>
    /// Abstraction that provides bark thickness (inches) for a species at a
    /// given height above stump, expressed in feet. Implementations must read
    /// from the FIA-based bark thickness SQL tables (WC and EC datasets).
    /// </summary>
    public interface IBarkThicknessProvider
    {
        /// <summary>
        /// Returns bark thickness in inches (one side) for the specified species
        /// at a given height above stump (feet).
        /// </summary>
        double GetBarkThicknessInches(string speciesCode, double heightFeet);
    }

    /// <summary>
    /// Provides species-specific DOB <-> DIB conversions using bark thickness
    /// values supplied by an IBarkThicknessProvider.
    /// </summary>
    public sealed class BarkAdapter
    {
        private readonly IBarkThicknessProvider _barkProvider;

        public BarkAdapter(IBarkThicknessProvider barkProvider)
        {
            _barkProvider = barkProvider
                ?? throw new ArgumentNullException(nameof(barkProvider));
        }

        /// <summary>
        /// Converts diameter outside bark (DOB, inches) to diameter inside bark
        /// (DIB, inches) using species-specific bark thickness for the given
        /// height. Negative results are clamped to zero.
        /// </summary>
        public double ConvertDobToDib(
            string speciesCode,
            double heightFeet,
            double dobInches)
        {
            if (speciesCode == null) throw new ArgumentNullException(nameof(speciesCode));
            if (dobInches <= 0.0) return 0.0;

            double barkThickness = _barkProvider.GetBarkThicknessInches(speciesCode, heightFeet);

            // DIB = DOB - 2 * barkThickness (both sides)
            double dib = dobInches - 2.0 * barkThickness;

            return dib < 0.0 ? 0.0 : dib;
        }

        /// <summary>
        /// Converts diameter inside bark (DIB, inches) to diameter outside bark
        /// (DOB, inches) using species-specific bark thickness for the given
        /// height. Negative or zero inputs are clamped to zero.
        /// </summary>
        public double ConvertDibToDob(
            string speciesCode,
            double heightFeet,
            double dibInches)
        {
            if (speciesCode == null) throw new ArgumentNullException(nameof(speciesCode));
            if (dibInches <= 0.0) return 0.0;

            double barkThickness = _barkProvider.GetBarkThicknessInches(speciesCode, heightFeet);

            // DOB = DIB + 2 * barkThickness (both sides)
            double dob = dibInches + 2.0 * barkThickness;

            return dob < 0.0 ? 0.0 : dob;
        }
    }
}
