using Microsoft.Maui.Devices.Sensors; // Make sure this is at the top!
using MOBWEB_TEST.Services;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class TreeEntryPage : ContentPage
{
    public TreeEntryPage()
    {
        InitializeComponent();
    }

    
    private void OnCaptureBearingClicked(object sender, EventArgs e)
    {
        if (!Compass.Default.IsSupported)
        {
            DisplayAlert("Error", "Compass not supported on this device.", "OK");
            return;
        }

        // Turn on the compass
        if (!Compass.Default.IsMonitoring)
        {
            Compass.Default.ReadingChanged += Compass_ReadingChanged;
            Compass.Default.Start(SensorSpeed.UI);
        }
    }
    private async void OnHeightClicked(object sender, EventArgs e)
    {
        // 1. Validate inputs (Now checking Azimuth too!)
        if (double.TryParse(DbhEntry.Text, out double dbh) &&
            double.TryParse(DistanceEntry.Text, out double distance) &&
            double.TryParse(AzimuthEntry.Text, out double azimuth))
        {
            // 2. Grab your BAF (Defaulting to 20 if the Stand hasn't been set up yet)
            bool methodIsFixed = DataService.CurrentStand.IsFixedPlot;

            double currentSize = DataService.CurrentPlot?.size ?? 20.00;

            // 3. THE BOUNCER: Ask the math class if the tree makes the cut
            bool isTreeIn = ForestyMath.IsTreeIn(dbh, distance, currentSize,methodIsFixed);

            if (isTreeIn)
            {
                // Tree is IN! Save the basic data
                DataService.CurrentTree.Species = SpeciesEntry.Text;
                DataService.CurrentTree.Dbh = dbh;

                // --- THE UPGRADE: PROJECT THE TREE COORDINATES ---
                // Ensure we have a plot center to base the math off of
                if (DataService.CurrentPlot != null)
                {
                    double plotLat = DataService.CurrentPlot.Latitude;
                    double plotLon = DataService.CurrentPlot.Longitude;

                    // Run the spatial math to find exactly where the tree is
                    var treeCoords = ForestyMath.CalculateTreeCoordinates(plotLat, plotLon, distance, azimuth);

                    DataService.CurrentTree.latitude = treeCoords.Latitude;
                    DataService.CurrentTree.longitude = treeCoords.Longitude;
                }
                // -------------------------------------------------

                // Reset the slate for the next potential tree
                SpeciesEntry.Text = string.Empty;
                DbhEntry.Text = string.Empty;
                DistanceEntry.Text = string.Empty;
                AzimuthEntry.Text = string.Empty; // Clear the new field
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
                AzimuthEntry.Text = string.Empty;
                SpeciesEntry.Focus();
            }
        }
        else
        {
            // Catch typos or missing fields
            await DisplayAlert("Invalid Input", "Please enter valid numbers for DBH, Distance, and Bearing.", "OK");
        }
    }
    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        // 1. Grab the heading (0 to 360 degrees relative to Magnetic North)
        double currentHeading = e.Reading.HeadingMagneticNorth;

        // 2. Unsubscribe and turn off the compass immediately to save battery
        Compass.Default.ReadingChanged -= Compass_ReadingChanged;
        Compass.Default.Stop();

        // 3. Update the UI on the Main Thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Round it to a whole number for standard forestry work
            AzimuthEntry.Text = Math.Round(currentHeading).ToString();
        });
    }
}