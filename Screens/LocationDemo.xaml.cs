using MOBWEB_TEST.Location;
using MOBWEB_TEST.sqllite;
using MOBWEB_TEST.Services;
using System.ComponentModel;

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
            StartPlotButton.IsEnabled = true;
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

        var offset = CalculateLocationDifference(_viewModel.Latitude, _viewModel.Longitude, targetPlot.Latitude, targetPlot.Longitude);
        double totalDistance = Math.Sqrt(Math.Pow(offset.NorthSouthMeters, 2) + Math.Pow(offset.EastWestMeters, 2));

        DistanceLabel.Text = $"{totalDistance:F1}m";
        OffsetLabel.Text = $"N/S: {offset.NorthSouthMeters:F1}m | E/W: {offset.EastWestMeters:F1}m";
    }

    private async void OnStartPlotClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("PlotEntryScreen");

    private LocationOffset CalculateLocationDifference(double userLat, double userLon, double targetLat, double targetLon)
    {
        double earthRadius = 6371e3;
        double latRad = (targetLat - userLat) * (Math.PI / 180);
        double lonRad = (targetLon - userLon) * (Math.PI / 180);

        return new LocationOffset
        {
            NorthSouthMeters = latRad * earthRadius,
            EastWestMeters = lonRad * earthRadius * Math.Cos(userLat * (Math.PI / 180))
        };
    }

    private async void OnBackToHomeClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("///HomeScreen");
}

public class LocationOffset
{
    public double NorthSouthMeters { get; set; }
    public double EastWestMeters { get; set; }
    }

    // 2. The math method
    private LocationOffset CalculateLocationDifference(double userLat, double userLon, double targetLat, double targetLon)
    {
        double earthRadius = 6371e3;
        double latRad = (targetLat - userLat) * (Math.PI / 180);
        double lonRad = (targetLon - userLon) * (Math.PI / 180);

        return new LocationOffset
        {
            NorthSouthMeters = latRad * earthRadius,
            EastWestMeters = lonRad * earthRadius * Math.Cos(userLat * (Math.PI / 180))
        };
    }