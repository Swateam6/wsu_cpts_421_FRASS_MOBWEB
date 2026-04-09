using Microsoft.Maui.Devices.Sensors;
using MOBWEB_TEST.Models; // Adjust based on where your Plot.cs lives

namespace MOBWEB_TEST.Screens.PlotNavSubsystem;

public partial class PlotNavPage : ContentPage
{
    private bool _isNavigating = false;
    private Location _targetLocation;

    public PlotNavPage()
    {
        InitializeComponent();

        // These coordinates are set roughly near WSU in Pullman!
        _targetLocation = new Location(46.7317, -117.1687);
        PlotNameLabel.Text = "Demo Plot (Pullman)";
    }

    private async void OnToggleNavClicked(object sender, EventArgs e)
    {
        if (_isNavigating)
        {
            _isNavigating = false;
            ToggleNavButton.Text = "Start Navigation";
            if (Application.Current.Resources.TryGetValue("Primary", out var primaryColor))
            {
                ToggleNavButton.BackgroundColor = (Color)primaryColor;
            }
            return;
        }

        // CRITICAL: Request GPS permissions from the user
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission Denied", "Navigation requires GPS access.", "OK");
                return;
            }
        }

        _isNavigating = true;
        ToggleNavButton.Text = "Stop Navigation";
        ToggleNavButton.BackgroundColor = Colors.DarkRed;

        // Fire up the background tracking loop
        _ = StartNavigationLoop();
    }

    private async Task StartNavigationLoop()
    {
        while (_isNavigating)
        {
            try
            {
                // Ping the phone's GPS hardware
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
                var currentLocation = await Geolocation.Default.GetLocationAsync(request);

                if (currentLocation != null && _targetLocation != null)
                {
                    // Calculate Distance (MAUI handles the Haversine math automatically)
                    double distanceInMiles = Location.CalculateDistance(currentLocation, _targetLocation, DistanceUnits.Miles);
                    double distanceInFeet = distanceInMiles * 5280;

                    // Calculate Bearing (Basic Trigonometry)
                    double bearing = CalculateBearing(currentLocation, _targetLocation);

                    // Push the new numbers to the UI
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DistanceLabel.Text = $"{Math.Round(distanceInFeet)} ft";
                        BearingLabel.Text = $"{Math.Round(bearing)}°";

                        // If they are within 15 feet, they hit the plot center!
                        if (distanceInFeet < 15)
                        {
                            DistanceLabel.TextColor = Colors.DarkOrange;
                            DistanceLabel.Text = "ARRIVED";
                            _isNavigating = false;
                            ToggleNavButton.Text = "Navigation Complete";
                        }
                        else
                        {
                            DistanceLabel.TextColor = Colors.DarkGreen;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GPS Error: {ex.Message}");
            }

            // Wait 2 seconds before checking again to prevent draining the battery
            await Task.Delay(2000);
        }
    }

    // Helper method to find the true compass angle between two GPS coordinates
    private double CalculateBearing(Location start, Location target)
    {
        double lat1 = start.Latitude * (Math.PI / 180.0);
        double lon1 = start.Longitude * (Math.PI / 180.0);
        double lat2 = target.Latitude * (Math.PI / 180.0);
        double lon2 = target.Longitude * (Math.PI / 180.0);

        double dLon = lon2 - lon1;
        double y = Math.Sin(dLon) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

        double brng = Math.Atan2(y, x);
        brng = brng * (180.0 / Math.PI);
        brng = (brng + 360.0) % 360.0;

        return brng;
    }
}