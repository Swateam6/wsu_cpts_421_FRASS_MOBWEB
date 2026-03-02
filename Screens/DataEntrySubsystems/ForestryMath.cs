using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MOBWEB_TEST.Screens.DataEntrySubsystems
{
    public static class ForestryMath
    {
        private const double BasalAreaConstant = 0.005454;
        public static double CalculateBasalArea(double dbh)
        {
            if (dbh <= 0) return 0;
            return Math.Round(BasalAreaConstant * Math.Pow(dbh, 2), 3);
        }
        public static double CalculateTreeHeight(double topAngleDegrees, double baseAngleDegrees, double distance)
        {
            double topRadians = topAngleDegrees * (Math.PI / 180.0);
            double baseRadians = baseAngleDegrees * (Math.PI / 180.0);

            // Assuming 'distance' is slope distance to the stump.
            // Automatically flatten it using the base angle.
            double horizontalDistance = distance * Math.Abs(Math.Cos(baseRadians));

            // Run the height formula using the newly flattened distance
            double height = horizontalDistance * (Math.Tan(topRadians) - Math.Tan(baseRadians));

            return Math.Round(height, 1);
        }
        public static double CalculateCrownRatio(double totalHeight, double baseOfLiveCrown)
        {
            double crownRatio = (totalHeight-baseOfLiveCrown) / totalHeight;
            return Math.Round(crownRatio,2);
        }
    }
}
