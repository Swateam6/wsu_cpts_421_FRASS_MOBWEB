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
        // 3. Grab the text from the UI and pack it into the DataService
        DataService.CurrentStand.StandId = StandIdEntry.Text;
        DataService.CurrentStand.CruiserName = CruiserEntry.Text;
        DataService.CurrentStand.Market = MarketEntry.Text;
        DataService.CurrentStand.CruiseDate = DateTime.Now; 

        // We use TryParse for numbers so the app doesn't crash if the box is empty!
        if (double.TryParse(AcresEntry.Text, out double acres))
        {
            DataService.CurrentStand.Acres = acres;
        }

        // 4. Move to the next screen in the Wizard
        // (Make sure "PlotEntryScreen" matches the route name you registered in AppShell.xaml.cs)
        await Shell.Current.GoToAsync("PlotEntryScreen");
    }
}