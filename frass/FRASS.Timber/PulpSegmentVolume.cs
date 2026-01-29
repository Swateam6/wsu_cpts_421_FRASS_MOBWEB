using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRASS.Timber
{
    /// <summary>
    /// Represents the geometric results for a single internal pulp segment.
    /// This is a purely geometric result: no SG, no prices.
    /// </summary>
    public sealed class PulpSegmentVolume
    {
        /// <summary>
        /// Height at the lower end of the segment (feet above stump).
        /// </summary>
        public double LowerHeightFeet { get; }

        /// <summary>
        /// Height at the upper end of the segment (feet above stump).
        /// </summary>
        public double UpperHeightFeet { get; }

        /// <summary>
        /// Segment length in feet.
        /// </summary>
        public double LengthFeet => UpperHeightFeet - LowerHeightFeet;

        /// <summary>
        /// DIB at the lower end (inches).
        /// </summary>
        public double DibLowerInches { get; }

        /// <summary>
        /// DIB at the upper end (inches).
        /// </summary>
        public double DibUpperInches { get; }

        /// <summary>
        /// Estimated DIB at the midpoint (inches), interpolated from taper.
        /// </summary>
        public double DibMidInches { get; }

        /// <summary>
        /// Geometric cubic-foot volume for the segment (inside bark).
        /// </summary>
        public double VolumeCubicFeet { get; }

        public PulpSegmentVolume(
            double lowerHeightFeet,
            double upperHeightFeet,
            double dibLowerInches,
            double dibUpperInches,
            double dibMidInches,
            double volumeCubicFeet)
        {
            LowerHeightFeet = lowerHeightFeet;
            UpperHeightFeet = upperHeightFeet;
            DibLowerInches = dibLowerInches;
            DibUpperInches = dibUpperInches;
            DibMidInches = dibMidInches;
            VolumeCubicFeet = volumeCubicFeet;
        }
    }
}
