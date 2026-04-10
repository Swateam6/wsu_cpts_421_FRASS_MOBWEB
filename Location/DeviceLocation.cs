namespace MOBWEB_TEST.Location
{
    public class DeviceLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DeviceLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
