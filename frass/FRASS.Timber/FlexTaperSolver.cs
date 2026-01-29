using FRASS.Timber;
using System;
using System.Collections.Generic;

namespace FRASS.Timber
{
    internal class FlexTaperSolver : IFlexTaperSolver
    {
        List<double> Taper {  get; set; }

        internal FlexTaperSolver(List<double> taper)
        {
            this.Taper = taper;
        }

        public double GetDiameterInsideBark(double heightFeet)
        {
            //TODO Make this use inside bark diameter
            int index = (int)Math.Ceiling(heightFeet - 5);
            if (index < 0 || index >= this.Taper.Count)
            {
                return 0.0; // Return 0 for out-of-bounds heights
            }
            double diameter = this.Taper[index];
            return diameter;
        }
    }
}
