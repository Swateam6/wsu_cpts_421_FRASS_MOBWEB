using System;
using System.Collections.Generic;
namespace FRASS.Timber
{
    public static class LogSegmenter
    {
        public static List<double> GetSEDs(List<double> taperProfile, double stumpHeight, double logLength, double trim, double minSED)
        {
            List<double> SEDs = new List<double>();
            double position = stumpHeight;
            double segmentLength = logLength + trim;

            while (position + logLength < 4.5 + taperProfile.Count)
            {
                int index = (int)(position + segmentLength - 4);
                if (index < 0 || index >= taperProfile.Count) break;

                double sed = taperProfile[index];
                if (sed < minSED) break;

                SEDs.Add(sed);
                position += segmentLength;
            }

            return SEDs;
        }
    }
}
