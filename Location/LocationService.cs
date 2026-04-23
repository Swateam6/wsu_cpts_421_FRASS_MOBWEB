using CommunityToolkit.Mvvm.Messaging;
using MOBWEB_TEST.Screens;

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
            Compass.Default.Start(SensorSpeed.Game);
            var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
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
                360 - _currentHeading);
            WeakReferenceMessenger.Default.Send(deviceLocation);
            
        }

        private void Compass_CompassReadingChanged(object? sender, CompassChangedEventArgs e)
        {
            _currentHeading = e.Reading.HeadingMagneticNorth;
        }
    }
}
