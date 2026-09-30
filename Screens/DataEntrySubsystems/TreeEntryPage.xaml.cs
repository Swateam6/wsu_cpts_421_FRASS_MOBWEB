using Microsoft.Maui.Devices.Sensors;
using MOBWEB_TEST.Services;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class TreeEntryPage : ContentPage
{
    public TreeEntryPage()
    {
        InitializeComponent();
    }

    private async void OnSaveTreeData(object sender, EventArgs e)
    {
        if (DataService.CurrentTree == null)
        {
            await DisplayAlert("No Tree Selected", "Please enter tree details first.", "OK");
            return;
        }

        string species = SpeciesEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(species))
        {
            await DisplayAlert("Missing Species", "Please enter a valid species code.", "OK");
            SpeciesEntry.Focus();
            return;
        }

        if (!double.TryParse(DbhEntry.Text?.Trim(), out double dbh) || dbh <= 0)
        {
            await DisplayAlert("Invalid DBH", "Please enter a valid DBH greater than 0.", "OK");
            DbhEntry.Focus();
            return;
        }

        double.TryParse(DistanceEntry.Text?.Trim(), out double distance);

        bool methodIsFixed = DataService.CurrentStand?.IsFixedPlot ?? false;
        double currentSize = DataService.CurrentPlot?.size ?? 20.00;

        if (distance > 0)
        {
            bool isTreeIn = ForestyMath.IsTreeIn(dbh, distance, currentSize, methodIsFixed);
            if (!isTreeIn)
            {
                await DisplayAlert("Tree OUT", $"At {distance} ft away, a {dbh}\" tree is out of the plot.", "OK");
                return;
            }
        }

        // 1. Assign values to active tree
        DataService.CurrentTree.Species = species;
        DataService.CurrentTree.Dbh = dbh;
        DataService.CurrentTree.Id = (DataService.CurrentPlot.TreeList?.Count ?? 0)+1;

        // 2. Commit into CurrentPlot.TreeList
        DataService.SaveTreeToPlot();
        
        await DisplayAlert("Saved", $"Tree #{DataService.CurrentTree.Id} saved to plot.", "OK");
        DataService.StartNewTree();

        // 3. Clear inputs for next tree
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;
        DistanceEntry.Text = string.Empty;
        SpeciesEntry.Focus();
    }

    private async void OnNewTreeClicked(object sender, EventArgs e)
    {
        OnSaveTreeData(sender, e);
    }

    private async void OnHeightClicked(object sender, EventArgs e)
    {
        int newTreeNumber = (DataService.CurrentPlot.TreeList?.Count ?? 0) + 1;

       

        // 4. Alert using the exact number that was assigned
       
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;
        DistanceEntry.Text = string.Empty;

        await Shell.Current.GoToAsync("TreeHeightEntryPage");
    }

    private void UpdateSamplingVisibility(bool isPps)
    {
        DistanceLayout.IsVisible = !isPps;

        if (isPps)
        {
            DistanceEntry.Text = string.Empty;
        }
    }
}