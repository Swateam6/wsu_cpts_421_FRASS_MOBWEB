namespace MOBWEB_TEST.Screens.DataEntrySubsystems;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;
public partial class PlotEntry : ContentPage
{
    public PlotEntry()
    {
        InitializeComponent();
    }

    private async void OnLogFirstTreeClicked(object sender, EventArgs e)
    {
        // 1. Parse and save the Plot Data to our DataService
        if (int.TryParse(PlotNumEntry.Text, out int plotNum))
            DataService.CurrentPlot.PlotNumber = plotNum;

        if (double.TryParse(SlopeEntry.Text, out double slope))
            DataService.CurrentPlot.Slope = slope;

        if (double.TryParse(AspectEntry.Text, out double aspect))
            DataService.CurrentPlot.Aspect = aspect;

        // Optional:hook up the phone's GPS later, you'd save the coords here
        // DataService.CurrentPlot.GpsLatitude = deviceLat;
        // DataService.CurrentPlot.GpsLongitude = deviceLong;

        DataService.CurrentTree = new Tree();

        
        await Shell.Current.GoToAsync("TreeBaseScreen");
    }
}