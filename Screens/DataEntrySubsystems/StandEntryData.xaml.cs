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
    private void OnBafSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1) // -1 means nothing is selected yet
        {
            // Grab the string they clicked (e.g., "20") and turn it into math
            string selectedString = picker.Items[selectedIndex];
            int selectedBaf = int.Parse(selectedString);

            // Lock it into your global state!
            // (Assuming you have a CurrentStand object in your DataService)
            if (DataService.CurrentStand != null)
            {
                DataService.CurrentStand.BAF = selectedBaf;
                Console.WriteLine($"Stand BAF locked in at: {DataService.CurrentStand.BAF}");
            }
        }
    }
}