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
        DataService.CurrentStand.StandId = StandIdEntry.Text;
        DataService.CurrentStand.CruiserName = CruiserEntry.Text;
        // ...

        // ADD THIS: Clear the UI text boxes so they are blank for the next stand
        StandIdEntry.Text = string.Empty;
        CruiserEntry.Text = string.Empty;
        MarketEntry.Text = string.Empty;
        AcresEntry.Text = string.Empty;

        await Shell.Current.GoToAsync("PlotEntryScreen");
    }
}