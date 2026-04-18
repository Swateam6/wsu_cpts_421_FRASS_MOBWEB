using Microsoft.Maui.Devices.Sensors;
using CommunityToolkit.Mvvm.Messaging;
using MOBWEB_TEST.Location;
using MOBWEB_TEST.Services;
// The alias to prevent namespace collisions
using MauiLocation = Microsoft.Maui.Devices.Sensors.Location;

namespace MOBWEB_TEST.Screens;

public partial class NavigationScreen : ContentPage
{
    private MauiLocation _mockPlotCenter;
    private readonly LocationService _locationService;
    private bool _isTracking = false;

    public NavigationScreen()
    {
        InitializeComponent();

        _locationService = new LocationService();

        WeakReferenceMessenger.Default.Register<DeviceLocation>(this, (sender, deviceLocation) =>
        {
            UpdateDistance(deviceLocation);
        });
    }

    private void OnToggleTrackingClicked(object sender, EventArgs e)
    {
        _isTracking = !_isTracking;

        if (_isTracking)
        {
            // Reset the mock center every time you start tracking so it grabs a fresh location
            _mockPlotCenter = null;

            TrackButton.Text = "Stop Tracking";
            TrackButton.BackgroundColor = Colors.DarkRed;
            StatusLabel.Text = "Acquiring Plot Center...";

            // Turn on your teammate's background listener
            _ = _locationService.Start();
        }
        else
        {
            _isTracking = false; // Explicitly kill the tracking flag
            TrackButton.Text = "Start Tracking Distance";
            TrackButton.BackgroundColor = Color.FromArgb("#2B5B84");

            // Turn off the GPS hardware
            _locationService.Stop();

            // Reset the UI so it doesn't look like it's still measuring
            StatusLabel.Text = "Tracking Stopped.";
            DistanceLabel.Text = "-- ft";
        }
    }

    private void UpdateDistance(DeviceLocation deviceLocation)
    {
        if (!_isTracking) return;

        try
        {
            // Convert teammate's object to MAUI object
            var currentLocation = new MauiLocation(deviceLocation.Latitude, deviceLocation.Longitude);

            // AUTO-LOCK LOGIC: If we don't have a center yet, this first ping IS the center!
            if (_mockPlotCenter == null)
            {
                _mockPlotCenter = currentLocation;

                // THIS is where the saving happens, because we finally have the data!
                if (DataService.CurrentPlot != null)
                {
                    DataService.CurrentPlot.Latitude = _mockPlotCenter.Latitude;
                    DataService.CurrentPlot.Longitude = _mockPlotCenter.Longitude;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusLabel.Text = $"Plot Center Locked: {_mockPlotCenter.Latitude:F5}, {_mockPlotCenter.Longitude:F5}";
                    DistanceLabel.Text = "0 ft";
                });

                return;
            }

            // If the center is already locked, calculate the distance
            double distanceKm = MauiLocation.CalculateDistance(_mockPlotCenter, currentLocation, DistanceUnits.Kilometers);
            double distanceFeet = distanceKm * 3280.84;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                DistanceLabel.Text = $"{distanceFeet:F0} ft";
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Tracking error: {ex.Message}");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _locationService.Stop();
        _isTracking = false;
        WeakReferenceMessenger.Default.Unregister<DeviceLocation>(this);
    }
    private async void OnLogTreeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("TreeEntryPage");
    }
}