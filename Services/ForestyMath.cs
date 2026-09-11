using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBWEB_TEST.Services
{
    public static class ForestyMath
    {

        private const double EarthRadiusFeet = 20902231.0;
        public static bool IsTreeIn(double distance, double dbh, double plotParameter, bool isFixedRadius)
        {
            if (!isFixedRadius)
            {
                // plotParameter = The constant radius (e.g., 37.2)
                double prf = 8.696 / Math.Sqrt(plotParameter);
                double limitingDistance = dbh * prf;

                return distance <= limitingDistance;
            }
            return true;
        }
    }
}
