using MOBWEB_TEST.Services;
using MOBWEB_TEST.sqllite;

namespace MOBWEB_TEST.Screens;

public partial class PlotSlopeScreen : ContentPage
{
    
    private double _calculatedSlopePercent = 0.0;

    public PlotSlopeScreen()
    {
        InitializeComponent();
    }

    // Auto-calculates slope percent as the cruiser types the degree angle
    private void OnAngleTextChanged(object sender, TextChangedEventArgs e)
    {
        if (double.TryParse(e.NewTextValue?.Trim(), out double degrees))
        {
            // % Slope = tan(degrees in radians) * 100
            _calculatedSlopePercent = Math.Tan(degrees * (Math.PI / 180.0)) * 100.0;
            SlopeLabel.Text = $"{_calculatedSlopePercent:F1} %";
        }
        else
        {
            _calculatedSlopePercent = 0.0;
            SlopeLabel.Text = "0.0 %";
        }
    }

    private async void OnSaveAndProceedClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SlopeAngleEntry.Text?.Trim()) ||
            !double.TryParse(SlopeAngleEntry.Text?.Trim(), out _))
        {
            await DisplayAlert("Validation", "Please enter a valid slope angle.", "OK");
            SlopeAngleEntry.Focus();
            return;
        }

        // Commit slope value to the current active plot
        if (DataService.CurrentPlot != null)
        {
            DataService.CurrentPlot.Slope = _calculatedSlopePercent;
        }

        // Navigate forward to tree measurement
        await Shell.Current.GoToAsync("TreeEntryPage");
    }
}