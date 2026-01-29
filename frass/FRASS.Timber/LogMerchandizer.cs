namespace FRASS.Timber
{
    public static class LogMerchandizer
    {
        public static string AssignGrade(double CR, double logBaseHeight, double totalHeight)
        {
            double crownBase = totalHeight * (1 - CR);

            if (logBaseHeight < crownBase)
                return "SM&Better";
            else if (logBaseHeight < totalHeight * 0.75)
                return "D2";
            else if (logBaseHeight < totalHeight * 0.90)
                return "D3";
            else if (logBaseHeight < totalHeight)
                return "D4";
            else
                return "Pulp";
        }
    }
}