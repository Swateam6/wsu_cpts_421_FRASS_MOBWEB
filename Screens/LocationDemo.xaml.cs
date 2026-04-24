using MOBWEB_TEST.Location;
using MOBWEB_TEST.sqllite;
using MOBWEB_TEST.Services;
using System.ComponentModel;

namespace MOBWEB_TEST.Screens;

public partial class LocationDemo : ContentPage
{
    private LocationDisplayBackend? _viewModel;
    private readonly LocalDbService _db;

    // Defining this inside the class ensures the compiler never loses it
    public class LocationOffset
    {
        public double NorthSouthMeters { get; set; }
        public double EastWestMeters { get; set; }
    }

    public LocationDemo(LocalDbService db)
    {
        InitializeComponent();
        _db = db;
        _viewModel = this.BindingContext as LocationDisplayBackend;
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Grab the target plot selected on the previous screen
        var targetPlot = DataService.CurrentPlot;
        if (targetPlot != null)
        {
            TargetCoordinatesLabel.Text = $"Target: {targetPlot.Latitude:F5}, {targetPlot.Longitude:F5}";
            // This enables the button you added in XAML
            StartPlotButton.IsEnabled = true;
        }
        else
        {
            TargetCoordinatesLabel.Text = "Target: None Selected";
            DistanceLabel.Text = "No Plot Selected";
            StartPlotButton.IsEnabled = false;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update navigation values whenever the GPS coordinates change
        if (e.PropertyName == nameof(LocationDisplayBackend.Latitude) ||
            e.PropertyName == nameof(LocationDisplayBackend.Longitude))
        {
            UpdateTargetNavigation();
        }
    }

    private void UpdateTargetNavigation()
    {
        var targetPlot = DataService.CurrentPlot;
        if (targetPlot == null || _viewModel == null) return;

        // Calculate the physical offset between user and plot center
        var offset = CalculateLocationDifference(_viewModel.Latitude, _viewModel.Longitude, targetPlot.Latitude, targetPlot.Longitude);

        // Pythagorean theorem for direct distance: sqrt(a^2 + b^2)
        double totalDistance = Math.Sqrt(Math.Pow(offset.NorthSouthMeters, 2) + Math.Pow(offset.EastWestMeters, 2));

        DistanceLabel.Text = $"{totalDistance:F1} meters";

        string ns = offset.NorthSouthMeters >= 0 ? $"North {offset.NorthSouthMeters:F1}m" : $"South {Math.Abs(offset.NorthSouthMeters):F1}m";
        string ew = offset.EastWestMeters >= 0 ? $"East {offset.EastWestMeters:F1}m" : $"West {Math.Abs(offset.EastWestMeters):F1}m";

        OffsetLabel.Text = $"{ns} | {ew}";
    }

    private async void OnStartPlotClicked(object sender, EventArgs e)
    {
        // Navigate to the data entry screen once the cruiser arrives
        await Shell.Current.GoToAsync("PlotEntryScreen");
    }

    private double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);

    private LocationOffset CalculateLocationDifference(double userLat, double userLon, double targetLat, double targetLon)
    {
        double earthRadius = 6371e3; // Earth radius in meters
        double latRad = DegreesToRadians(targetLat - userLat);
        double lonRad = DegreesToRadians(targetLon - userLon); // Your lonRad fix!

        return new LocationOffset
        {
            NorthSouthMeters = latRad * earthRadius,
            // Calculate East-West meters using the user's current latitude for accuracy
            EastWestMeters = lonRad * earthRadius * Math.Cos(DegreesToRadians(userLat))
        };
    }

    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}