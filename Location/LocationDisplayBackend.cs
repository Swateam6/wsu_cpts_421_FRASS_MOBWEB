using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace MOBWEB_TEST.Location
{
    public partial class LocationDisplayBackend : ObservableObject
    {
        private readonly LocationService _locationService;

        [ObservableProperty]
        private double latitude;

        [ObservableProperty]
        private double longitude;

        [ObservableProperty]
        private double altitude;

        [ObservableProperty]
        private double reading;

        [ObservableProperty]
        private double rotationAngle;

        [ObservableProperty]
        private bool isListening;

        [ObservableProperty]
        private string listeningButtonText;

        public LocationDisplayBackend()
        {
            _locationService = new LocationService();
            ListeningButtonText = "Start Listening";
            WeakReferenceMessenger.Default.Register<DeviceLocation>(this, (sender, deviceLocation) =>
            {
                Latitude = deviceLocation.Latitude;
                Longitude = deviceLocation.Longitude;
                Altitude = deviceLocation.Altitude;
                Reading = deviceLocation.Reading;
                RotationAngle = deviceLocation.RotationAngle;

            });
        }

        [RelayCommand]
        private void ChangeListeningMode()
        {
            if(!IsListening)
            {
                _ = _locationService.Start();
                IsListening = true;
                ListeningButtonText = "Stop Listening";
            }
            else
            {
                _locationService.Stop();
                IsListening = false;
                ListeningButtonText = "Start Listening";
            }
        }
    }
}
