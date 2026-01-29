using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRASS.Timber
{
    /// <summary>
    /// Abstraction for a Flex Taper solver that returns diameter inside bark (DIB)
    /// at a given height above stump, measured in feet.
    /// </summary>
    public interface IFlexTaperSolver
    {
        /// <summary>
        /// Returns diameter inside bark (inches) at a given height (feet above stump).
        /// </summary>
        double GetDiameterInsideBark(double heightFeet);
    }
}
