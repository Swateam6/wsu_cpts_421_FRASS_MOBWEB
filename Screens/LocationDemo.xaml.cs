using MOBWEB_TEST.Location;
using MOBWEB_TEST.sqllite;
using System.ComponentModel;
using MOBWEB_TEST.Services;
using System.Linq;

namespace MOBWEB_TEST.Screens;

public partial class LocationDemo : ContentPage
{
    private LocationDisplayBackend? _viewModel;
    private readonly LocalDbService _db;

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

        var targetPlot = DataService.CurrentPlot;
        if (targetPlot != null)
        {
            TargetCoordinatesLabel.Text = $"Target: {targetPlot.Latitude:F5}, {targetPlot.Longitude:F5}";
            // This line fixes the "does not exist" error by enabling the button
            StartPlotButton.IsEnabled = true;
        }
        else
        {
            TargetCoordinatesLabel.Text = "Target: None Selected";
            StartPlotButton.IsEnabled = false;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LocationDisplayBackend.Latitude) ||
            e.PropertyName == nameof(LocationDisplayBackend.Longitude))
        {
            UpdateTargetNavigation();
        }
    }

    private void UpdateTargetNavigation()
    {
        var targetPlot = DataService.CurrentPlot;
        if (targetPlot == null) return;

        double userLat = _viewModel?.Latitude ?? 0;
        double userLon = _viewModel?.Longitude ?? 0;

        var offset = CalculateLocationDifference(userLat, userLon, targetPlot.Latitude, targetPlot.Longitude);
        double totalDistance = Math.Sqrt(Math.Pow(offset.NorthSouthMeters, 2) + Math.Pow(offset.EastWestMeters, 2));

        DistanceLabel.Text = $"{totalDistance:F1} meters";

        string ns = offset.NorthSouthMeters >= 0 ? $"North {offset.NorthSouthMeters:F1}m" : $"South {Math.Abs(offset.NorthSouthMeters):F1}m";
        string ew = offset.EastWestMeters >= 0 ? $"East {offset.EastWestMeters:F1}m" : $"West {Math.Abs(offset.EastWestMeters):F1}m";
        OffsetLabel.Text = $"{ns} | {ew}";
    }

    private async void OnStartPlotClicked(object sender, EventArgs e)
    {
        // Navigate to the data entry screen once you arrive at the plot
        await Shell.Current.GoToAsync("PlotEntryScreen");
    }

    private double DegreesToRadians(double degrees) => degrees * (Math.PI / 180);

    private LocationOffset CalculateLocationDifference(double userLat, double userLon, double targetLat, double targetLon)
    {
        double earthRadius = 6371e3;
        double latDiff = DegreesToRadians(targetLat - userLat);
        double longDiff = DegreesToRadians(targetLon - userLon);

        return new LocationOffset
        {
            NorthSouthMeters = latDiff * earthRadius,
            EastWestMeters = longDiff * earthRadius * Math.Cos(DegreesToRadians(userLat))
        };
    }

    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}