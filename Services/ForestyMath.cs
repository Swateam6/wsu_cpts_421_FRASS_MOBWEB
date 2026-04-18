using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBWEB_TEST.Services
{
    public class ForestyMath
    {
        public bool IsTreeIn(double dbh, double distanceToTree, int baf)
        {
            double prf = 8.696 / Math.Sqrt(baf);
            double limitingDistance = dbh * prf;
            return distanceToTree <= limitingDistance;
        }
    }
}
