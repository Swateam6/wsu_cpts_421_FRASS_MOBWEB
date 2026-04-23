using MOBWEB_TEST.Location;
using MOBWEB_TEST.sqllite;
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
        var plots = await _db.GetAllPlotDataAsync();
        var output = "═══════════════════════════════════════════\n";
        output += $"\n── plot_data ({plots.Count} records) ──\n";

        double userLatitude = _viewModel?.Latitude ?? 0;
        double userLongitude = _viewModel?.Longitude ?? 0;
        foreach (var p in plots)
        {
            var offset = CalculateLocationDifference(userLatitude, userLongitude, p.Latitude, p.Longitude);
            string northSouthDistance = offset.NorthSouthMeters >= 0
               ? $"{offset.NorthSouthMeters:F1}m N"
               : $"{Math.Abs(offset.NorthSouthMeters):F1}m S";
            string eastWestDistance = offset.EastWestMeters >= 0
                ? $"{offset.EastWestMeters:F1}m E"
                : $"{Math.Abs(offset.EastWestMeters):F1}m W";
            output += $"  ID:{p.Id} | StandFK:{p.ParentStandId} | ({p.Latitude:F4},{p.Longitude:F4}) | Elev:{p.Elevation}ft\n";
            output += $"       Offset: {northSouthDistance} | {eastWestDistance}\n";

        }
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