using MOBWEB_TEST.Location;
using MOBWEB_TEST.sqllite;
using System.ComponentModel;
using MOBWEB_TEST.Services;
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

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LocationDisplayBackend.Reading))
        {
            UpdateCompassDisplay(_viewModel?.Reading ?? 0);
            OnPrintContentsClicked(this, EventArgs.Empty);
        }
    }

    private void UpdateCompassDisplay(double heading)
    {
        // Update compass needle rotation
        var compassNeedle = this.FindByName<Grid>("compassNeedle");
        if (compassNeedle != null)
        {
            compassNeedle.Rotation = heading;
        }

        // Update cardinal/ordinal direction text
        string direction = GetCardinalDirection(heading);
        var directionLabel = this.FindByName<Label>("DirectionTextLabel");
        if (directionLabel != null)
        {
            directionLabel.Text = direction;
        }
    }

    private string GetCardinalDirection(double heading)
    {
        // Normalize heading to 0-360
        heading = ((heading % 360) + 360) % 360;

        return heading switch
        {
            >= 348.75 or < 11.25 => "N",
            >= 11.25 and < 33.75 => "NNE",
            >= 33.75 and < 56.25 => "NE",
            >= 56.25 and < 78.75 => "ENE",
            >= 78.75 and < 101.25 => "E",
            >= 101.25 and < 123.75 => "ESE",
            >= 123.75 and < 146.25 => "SE",
            >= 146.25 and < 168.75 => "SSE",
            >= 168.75 and < 191.25 => "S",
            >= 191.25 and < 213.75 => "SSW",
            >= 213.75 and < 236.25 => "SW",
            >= 236.25 and < 258.75 => "WSW",
            >= 258.75 and < 281.25 => "W",
            >= 281.25 and < 303.75 => "WNW",
            >= 303.75 and < 326.25 => "NW",
            >= 326.25 and < 348.75 => "NNW",
            _ => "Unknown"
        };
    }
    private async void OnPrintContentsClicked(object sender, EventArgs e)
    {
        try
        {
            OutputLabel.Text = "Retrieving database contents...\n";
            await RetriveParcelData();
        }
        catch (Exception ex)
        {
            OutputLabel.Text = $"Error: {ex.Message}";
        }
    }
    private async Task RetriveParcelData()
    {
        // 1. Grab the plot you selected on the previous screen
        var targetPlot = DataService.CurrentPlot;

        if (targetPlot == null)
        {
            OutputLabel.Text = "No plot selected! Go back and pick a plot first.";
            return;
        }

        // 2. Get current user position from the ViewModel
        double userLat = _viewModel?.Latitude ?? 0;
        double userLon = _viewModel?.Longitude ?? 0;

        // 3. Calculate the offsets
        var offset = CalculateLocationDifference(userLat, userLon, targetPlot.Latitude, targetPlot.Longitude);

        // 4. Calculate Total Euclidean Distance (Straight Line)
        // Formula: sqrt(NS^2 + EW^2)
        double totalDistance = Math.Sqrt(Math.Pow(offset.NorthSouthMeters, 2) + Math.Pow(offset.EastWestMeters, 2));

        // 5. Format the Output for the Demo
        var output = "═══════════════════════════════════════════\n";
        output += "           TARGET NAVIGATION               \n";
        output += "═══════════════════════════════════════════\n\n";
        output += $" TARGET PLOT: ({targetPlot.Latitude:F5}, {targetPlot.Longitude:F5})\n\n";

        output += $" DIRECT DISTANCE: {totalDistance:F1} meters\n";
        output += "───────────────────────────────────────────\n";

        output += offset.NorthSouthMeters >= 0
            ? $" WALK NORTH: {offset.NorthSouthMeters:F1}m\n"
            : $" WALK SOUTH: {Math.Abs(offset.NorthSouthMeters):F1}m\n";

        output += offset.EastWestMeters >= 0
            ? $" WALK EAST:  {offset.EastWestMeters:F1}m\n"
            : $" WALK WEST:  {Math.Abs(offset.EastWestMeters):F1}m\n";

        OutputLabel.Text = output;
    }
    private double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
    private LocationOffset CalculateLocationDifference(double userLatitude, double userLongitude, double targetLatitude, double targetLongitude)
    {
        // Earth radius in meters
        double earthRadius = 6371e3;

        // Convert to radians
        double userLatDeg = DegreesToRadians(userLatitude);
        double targetLatDeg = DegreesToRadians(targetLatitude);
        double latDiff = DegreesToRadians(targetLatitude - userLatitude);
        double longDiff = DegreesToRadians(targetLongitude - userLongitude);

        // Calculate north/south distance (difference in latitude)
        double northSouthMeters = latDiff * earthRadius;

        // Calculate east/west distance (difference in longitude at the user's latitude)
        double eastWestMeters = longDiff * earthRadius * Math.Cos(userLatDeg);

        return new LocationOffset
        {
            NorthSouthMeters = northSouthMeters,
            EastWestMeters = eastWestMeters
        };
    }

    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}

public class LocationOffset
{
    public double NorthSouthMeters { get; set; }
    public double EastWestMeters { get; set; }
}