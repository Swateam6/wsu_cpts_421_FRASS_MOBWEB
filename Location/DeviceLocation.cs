namespace MOBWEB_TEST.Location
{
    public class DeviceLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public double Reading { get; set; }
        public double RotationAngle { get; set; }

        public DeviceLocation(double latitude, double longitude, double altitude, double reading, double rotationAngle)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            Reading = reading;
            RotationAngle = rotationAngle;
        }
    }
}
