using CommunityToolkit.Mvvm.Messaging;

namespace MOBWEB_TEST.Location
{
    public class LocationService
    {
        private bool _isListening;
        public async Task Start()
        {
            Geolocation.LocationChanged += Geolocation_LocationChanged;
            var request = new GeolocationListeningRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));
            if (request is not null)
            {
                _isListening = await Geolocation.StartListeningForegroundAsync(request);
            }

        }

        public void Stop()
        {
            Geolocation.LocationChanged -= Geolocation_LocationChanged;
            Geolocation.StopListeningForeground();
            _isListening = false;
        }

        private void Geolocation_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            var deviceLocation = new DeviceLocation(e.Location.Latitude, e.Location.Longitude);
            WeakReferenceMessenger.Default.Send(deviceLocation);
        }
    }
}
