using MOBWEB_TEST.Location;
namespace MOBWEB_TEST.Screens;

public partial class LocationDemo : ContentPage
{
    private LocationDisplayBackend? _viewModel;

    public LocationDemo()
    {
        InitializeComponent();
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
    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}