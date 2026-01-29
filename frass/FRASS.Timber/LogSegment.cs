namespace FRASS.Timber
{
    public class LogSegment
    {
        public double BaseHeight { get; set; }
        public double SED { get; set; }
        public string Grade { get; set; }
        public int BFVolume { get; set; }
        public int length { get; set; }
        public double SEDHeight { get; set; }
        public double? EstimatedValue { get; set; }
    }
}