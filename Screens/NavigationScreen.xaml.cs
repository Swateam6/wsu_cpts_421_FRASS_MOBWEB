using Microsoft.Maui.Devices.Sensors;
using CommunityToolkit.Mvvm.Messaging;
using MOBWEB_TEST.Location;
using MOBWEB_TEST.Services;

namespace MOBWEB_TEST.Screens;

public partial class NavigationScreen : ContentPage
{
    private readonly LocationService _locationService;
    private bool _isAcquiring = false;

    public NavigationScreen()
    {
        InitializeComponent();
        _locationService = new LocationService();

        // Listen for GPS pings from the background service
        WeakReferenceMessenger.Default.Register<DeviceLocation>(this, (sender, deviceLocation) =>
        {
            OnLocationReceived(deviceLocation);
        });
    }

    private void OnLockCenterClicked(object sender, EventArgs e)
    {
        if (_isAcquiring) return; // Prevent spam clicking while already searching

        _isAcquiring = true;
        LockCenterButton.Text = "Acquiring...";
        LockCenterButton.BackgroundColor = Colors.DarkOrange;
        StatusLabel.Text = "Waiting for GPS signal...";

        // Turn on the background listener
        _ = _locationService.Start();
    }

    private void OnLocationReceived(DeviceLocation deviceLocation)
    {
        if (!_isAcquiring) return;

        // 1. We got the ping! Turn off the acquiring flag.
        _isAcquiring = false;

        // 2. Shut off the GPS hardware immediately since we only need the center point
        _locationService.Stop();

        // 3. Save directly to the DataService
        if (DataService.CurrentPlot != null)
        {
            DataService.CurrentPlot.Latitude = deviceLocation.Latitude;
            DataService.CurrentPlot.Longitude = deviceLocation.Longitude;
        }

        // 4. Update the UI
        MainThread.BeginInvokeOnMainThread(() =>
        {
            CoordinatesLabel.Text = $"{deviceLocation.Latitude:F5}, {deviceLocation.Longitude:F5}";
            StatusLabel.Text = "Plot Center Locked successfully.";
            LockCenterButton.Text = "Re-Lock Center";
            LockCenterButton.BackgroundColor = Colors.DarkGreen;
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Safety cleanup if the user leaves the page while it is searching
        _locationService.Stop();
        _isAcquiring = false;
        WeakReferenceMessenger.Default.Unregister<DeviceLocation>(this);
    }

    private async void OnLogTreeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("TreeEntryPage");
    }
}