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
            if (isFixedRadius)
            {
                // plotParameter = The constant radius (e.g., 37.2)
                return distance <= plotParameter;
            }
            else
            {
                // plotParameter = The BAF (e.g., 20)
                // Calculating the PRF (Plot Radius Factor)
                double prf = 8.696 / Math.Sqrt(plotParameter);
                double limitingDistance = dbh * prf;

                return distance <= limitingDistance;
            }
        }
        public static (double Latitude, double Longitude) CalculateTreeCoordinates(
        double plotLat, double plotLon, double distanceFeet, double bearingDegrees)
        {
            // Convert to Radians
            double lat1 = plotLat * (Math.PI / 180.0);
            double lon1 = plotLon * (Math.PI / 180.0);
            double bearingRad = bearingDegrees * (Math.PI / 180.0);

            // Angular distance
            double ad = distanceFeet / EarthRadiusFeet;

            // Calculate new latitude
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(ad) +
                                    Math.Cos(lat1) * Math.Sin(ad) * Math.Cos(bearingRad));

            // Calculate new longitude
            double lon2 = lon1 + Math.Atan2(Math.Sin(bearingRad) * Math.Sin(ad) * Math.Cos(lat1),
                                            Math.Cos(ad) - Math.Sin(lat1) * Math.Sin(lat2));

            // Convert back to Degrees
            return (lat2 * (180.0 / Math.PI), lon2 * (180.0 / Math.PI));
        }
    }
}
