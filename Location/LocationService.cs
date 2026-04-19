using CommunityToolkit.Mvvm.Messaging;

namespace MOBWEB_TEST.Location
{
    public class LocationService
    {
        private bool _isListening;
        private double _currentHeading = 0;
        private double _currentRotationAngle = 0;

        public async Task Start()
        {
            Geolocation.LocationChanged += Geolocation_LocationChanged;
            Compass.Default.ReadingChanged += Compass_CompassReadingChanged;
            Compass.Default.Start(SensorSpeed.UI);
            var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));
            if (request is not null)
            {
                _isListening = await Geolocation.StartListeningForegroundAsync(request);
            }
        }
            
        public void Stop()
        {
            Geolocation.LocationChanged -= Geolocation_LocationChanged;
            Compass.Default.ReadingChanged -= Compass_CompassReadingChanged;
            Geolocation.StopListeningForeground();
            Compass.Default.Stop();

            _isListening = false;
        }

        private void Geolocation_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            var deviceLocation = new DeviceLocation(
                e.Location.Latitude,
                e.Location.Longitude,
                e.Location.Altitude ?? -1,
                _currentHeading,
                360 - _currentHeading); // used for compass display rotation
            WeakReferenceMessenger.Default.Send(deviceLocation);
        }

        private void Compass_CompassReadingChanged(object? sender, CompassChangedEventArgs e)
        {
            _currentHeading = e.Reading.HeadingMagneticNorth;
        }
    }
}
