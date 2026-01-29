using System;
using System.Collections.Generic;
namespace FRASS.Timber
{
    public static class VolumeEstimator
    {
        public static int LookupVolume(double SED, Dictionary<int, int> volumeTable)
        {
            int diameter = (int)Math.Truncate(SED);
            return volumeTable.TryGetValue(diameter, out int volume) ? volume : 0;
        }
    }
}