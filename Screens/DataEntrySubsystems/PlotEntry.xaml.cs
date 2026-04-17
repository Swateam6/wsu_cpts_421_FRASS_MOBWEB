namespace MOBWEB_TEST.Screens.DataEntrySubsystems;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;

public partial class PlotEntry : ContentPage
{
    public PlotEntry()
    {
        InitializeComponent();
    }

    private async void ToPlotSlopeClicked(object sender, EventArgs e)
    {
        // 1. Parse and save the Plot Data to our DataService

        if (double.TryParse(SlopeEntry.Text, out double slope))
            DataService.CurrentPlot.Slope = slope;

        if (double.TryParse(AspectEntry.Text, out double aspect))
            DataService.CurrentPlot.Aspect = aspect;

        // FIXED 1 (Ghost Data): Clear the UI text boxes so they are blank when the user returns via "Next Plot"
        
        SlopeEntry.Text = string.Empty;
        AspectEntry.Text = string.Empty;

        // FIXED 2: Removed the redundant 'new Tree()' call. DataService handles that automatically.

        // 2. Navigate to the Plot Entry Page
        await Shell.Current.GoToAsync("PlotSlopeScreen");

    }
}