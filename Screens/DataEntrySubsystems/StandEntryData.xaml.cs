namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;

public partial class StandEntryData : ContentPage
{
    public StandEntryData()
    {
        InitializeComponent();
    }

    // NEW: Toggles the UI based on what they select
    private void OnPlotTypeSelectedIndexChanged(object sender, EventArgs e)
    {
        if (PlotTypePicker.SelectedIndex == 0) // Variable Radius (Prism)
        {
            VariableSection.IsVisible = true;
            FixedSection.IsVisible = false;
        }
        else if (PlotTypePicker.SelectedIndex == 1) // Fixed Radius
        {
            VariableSection.IsVisible = false;
            FixedSection.IsVisible = true;
        }
    }

    private async void OnStartPlottingClicked(object sender, EventArgs e)
    {
        if (DataService.CurrentStand == null) return;

        // 1. Parse the Acres
        if (double.TryParse(AcresEntry.Text, out double acres))
        {
            DataService.CurrentStand.Acres = acres;
        }

        // 2. Set the Boolean and the Value based on the UI selection
        if (PlotTypePicker.SelectedIndex == 0) // Variable Radius (Prism)
        {
            // 1. Set the boolean in the Stand
            DataService.CurrentStand.IsFixedPlot = false;

            // 2. Store the BAF in DataService to pass it to the Plot class later
            if (BafPicker.SelectedIndex != -1)
            {
                double selectedBaf = double.Parse(BafPicker.SelectedItem.ToString());
                DataService.CurrentPlot.size = selectedBaf;
            }
        }
        else if (PlotTypePicker.SelectedIndex == 1) // Fixed Radius
        {
            // 1. Set the boolean in the Stand
            DataService.CurrentStand.IsFixedPlot = true;

            // 2. Store the Radius in DataService to pass it to the Plot class later
            if (double.TryParse(RadiusEntry.Text, out double parsedRadius))
            {
                DataService.CurrentStand.plotSize = parsedRadius;
            }
        }

        // 3. Clear the UI for the next session
        AcresEntry.Text = string.Empty;
        RadiusEntry.Text = string.Empty;
        BafPicker.SelectedIndex = -1;
        PlotTypePicker.SelectedIndex = -1;

        // 4. Proceed to the plotting screen
        await Shell.Current.GoToAsync("PlotCoordinateSet");
    }
}