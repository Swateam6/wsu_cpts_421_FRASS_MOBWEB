using Microsoft.Maui.Devices.Sensors; // Make sure this is at the top!
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
        // 1. Guard against null active tree reference
        if (DataService.CurrentTree == null)
        {
            await DisplayAlert("No Tree Selected", "Please add a new tree or select an existing tree first.", "OK");
            return;
        }

        // 2. Validate species
        string species = SpeciesEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(species))
        {
            await DisplayAlert("Missing Species", "Please enter a valid species code.", "OK");
            SpeciesEntry.Focus();
            return;
        }

        // 3. Validate DBH
        if (!double.TryParse(DbhEntry.Text?.Trim(), out double dbh) || dbh <= 0)
        {
            await DisplayAlert("Invalid DBH", "Please enter a valid DBH greater than 0.", "OK");
            DbhEntry.Focus();
            return;
        }

        // 4. Validate Distance (if used for limiting distance checks)
        double.TryParse(DistanceEntry.Text?.Trim(), out double distance);

        // 5. In/Out Limiting Distance Check
        bool methodIsFixed = DataService.CurrentStand?.IsFixedPlot ?? false;
        double currentSize = DataService.CurrentPlot?.size ?? 20.00;

        // Run math check if a distance was supplied
        if (distance > 0)
        {
            bool isTreeIn = ForestyMath.IsTreeIn(dbh, distance, currentSize, methodIsFixed);
            if (!isTreeIn)
            {
                await DisplayAlert("Tree OUT", $"At {distance} ft away, a {dbh}\" tree is out of the plot.", "OK");
                return;
            }
        }

        // 6. Update the current tree's properties
        DataService.CurrentTree.Species = species;
        DataService.CurrentTree.Dbh = dbh;

        // 7. Persist to SQLite
        DataService.SaveTreeToPlot();

        await DisplayAlert("Saved", $"Tree #{DataService.CurrentPlot.TreeList[DataService.CurrentTree.Id]} saved successfully.", "OK");
    }
    private async void OnNewTreeClicked(object sender, EventArgs e)
    {
        // 1. Validate Species
        string species = SpeciesEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(species))
        {
            await DisplayAlert("Missing Species", "Please enter a species code before adding a tree.", "OK");
            SpeciesEntry.Focus();
            return;
        }

        // 2. Validate DBH
        if (!double.TryParse(DbhEntry.Text?.Trim(), out double dbh) || dbh <= 0)
        {
            await DisplayAlert("Invalid DBH", "Please enter a valid DBH greater than 0.", "OK");
            DbhEntry.Focus();
            return;
        }

        // 3. Optional: Validate Distance if required for fixed/variable plots
        double.TryParse(DistanceEntry.Text?.Trim(), out double distance);

        // 4. Update model / Pass values to service
        // (If your DataService uses a CurrentTree instance:)
        if (DataService.CurrentTree != null)
        {
            DataService.CurrentTree.Species = species;
            DataService.CurrentTree.Dbh = dbh;
            // DataService.CurrentTree.Distance = distance;
        }

        // Save to the current plot collection & SQLite
        DataService.SaveTreeToPlot();

        // 5. Reset input fields for the next tree entry
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;
        DistanceEntry.Text = string.Empty;

        // Ready for the next tree tally
        SpeciesEntry.Focus();
    }
    private async void OnHeightClicked(object sender, EventArgs e)
    {
        if (double.TryParse(DbhEntry.Text, out double dbh) &&
            double.TryParse(DistanceEntry.Text, out double distance))
        {
            // 2. Grab your BAF (Defaulting to 20 if the Stand hasn't been set up yet)
            bool methodIsFixed = DataService.CurrentStand.IsFixedPlot;

            double currentSize = DataService.CurrentPlot?.size ?? 20.00;

            // 3. THE BOUNCER: Ask the math class if the tree makes the cut
            bool isTreeIn = ForestyMath.IsTreeIn(dbh, distance, currentSize, methodIsFixed);

            if (isTreeIn)
            {
                // Tree is IN! Save the basic data
                DataService.CurrentTree.Species = SpeciesEntry.Text;
                DataService.CurrentTree.Dbh = dbh;

                // Reset the slate for the next potential tree
                SpeciesEntry.Text = string.Empty;
                DbhEntry.Text = string.Empty;
                DistanceEntry.Text = string.Empty;
                SpeciesEntry.Focus();

                await Shell.Current.GoToAsync("TreeHeightEntryPage");
            }
            else
            {
                // Tree is OUT. Stop them from going to the Gyro screen!



                await DisplayAlert("Tree OUT", $"At {distance}ft away, a {dbh}\" tree is out of the plot. Move to the next tree.", "OK");

                // Clear the slate so they can measure the next tree, but don't navigate.
                SpeciesEntry.Text = string.Empty;
                DbhEntry.Text = string.Empty;
                DistanceEntry.Text = string.Empty;
                SpeciesEntry.Focus();
            }
        }
        else
        {
            // Catch typos or missing fields
            await DisplayAlert("Invalid Input", "Please enter valid numbers for DBH, Distance, and Bearing.", "OK");
        }
    }
}