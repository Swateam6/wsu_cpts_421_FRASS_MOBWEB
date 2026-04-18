
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;

public partial class TreeEntryPage : ContentPage
{
    public TreeEntryPage()
    {
        InitializeComponent();
    }

    private async void OnHeightClicked(object sender, EventArgs e)
    {
        // 1. Make sure they actually typed numbers
        if (double.TryParse(DbhEntry.Text, out double dbh) &&
            double.TryParse(DistanceEntry.Text, out double distance))
        {
            // 2. Grab your BAF (Defaulting to 20 if the Stand hasn't been set up yet)
            int currentBaf = DataService.CurrentStand?.BAF ?? 20;

            // 3. THE BOUNCER: Ask the math class if the tree makes the cut
            bool isTreeIn = ForestryMath.IsTreeIn(dbh, distance, currentBaf);

            if (isTreeIn)
            {
                // Tree is IN! Save the data to the current tree object
                DataService.CurrentTree.Species = SpeciesEntry.Text;
                DataService.CurrentTree.Dbh = dbh;

                // If you added Distance to your Tree model, save it here too:
                // DataService.CurrentTree.DistanceFromCenter = distance;

                // Reset the slate
                SpeciesEntry.Text = string.Empty;
                DbhEntry.Text = string.Empty;
                DistanceEntry.Text = string.Empty;
                SpeciesEntry.Focus();

                // Move to the next step
                await Shell.Current.GoToAsync("GyroscopeScreen");
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
            // Catch typos (like typing "letters" into the number box)
            await DisplayAlert("Invalid Input", "Please enter valid numbers for DBH and Distance.", "OK");
        }
    }
}
