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
        public static double CalculateTreeHeight(double distance, double topAngleDegrees, double baseAngleDegrees)
        {
            if(distance <=0)
            {
                return -1.0;
            }
            double topRad = topAngleDegrees * (Math.PI / 180.0);
            double baseRad = baseAngleDegrees * (Math.PI / 180.0);

            // 2. Apply the Universal Tangent Formula
            double height = distance * (Math.Tan(topRad) - Math.Tan(baseRad));

            // 3. Return rounded to 1 decimal place (e.g., 85.4 feet)
            return Math.Round(height, 1);
        }
        public static double CalculateCrownRatio(double totalHeight, double baseOfLiveCrown)
        {
            double crownRatio = (totalHeight-baseOfLiveCrown) / totalHeight;
            return Math.Round(crownRatio,2);
        }
    }
}
