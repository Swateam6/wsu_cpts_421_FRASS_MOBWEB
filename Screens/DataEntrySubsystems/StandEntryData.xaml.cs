namespace MOBWEB_TEST.Screens.DataEntrySubsystems;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;
public partial class StandEntryData : ContentPage
{
    public StandEntryData()
    {
        InitializeComponent();
    }

    private async void OnStartPlottingClicked(object sender, EventArgs e)
    {
        // ... your parsing logic ...
        // ...
        // ADD THIS: Clear the UI text boxes so they are blank for the next stand
        AcresEntry.Text = string.Empty;

        await Shell.Current.GoToAsync("PlotEntryScreen");
    }
}