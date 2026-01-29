using System;
using System.Collections.Generic;
namespace FRASS.Timber
{
    public static class FlexTaperModel
    {
        public static double GeometricTaper(double DBH, double H, double h)
        {
            return DBH * ((H - h) / (H - 4.5));
        }

        public static double FlexTaperFunction(double D_geo, double delta, double h, double H, double k)
        {
            return D_geo * (1 + delta * (1 / (1 + Math.Exp(-k * (h - H / 2)))));
        }

        public static List<double> GenerateTaperProfile(double DBH, double H, double delta, double k = 0.1, double epsilon = 1.0)
        {
            List<double> taperProfile = new List<double>();
            double previousD = DBH;

            for (double h = 4.5; h <= H; h += epsilon)
            {
                double D_geo = GeometricTaper(DBH, H, h);
                double D_flex = Math.Min(Math.Min(FlexTaperFunction(D_geo, delta, h, H, k), DBH), previousD);
                taperProfile.Add(D_flex);
                previousD = D_flex;
            }

            return taperProfile;
        }
    }
}