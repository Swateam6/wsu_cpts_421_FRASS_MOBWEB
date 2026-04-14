namespace MOBWEB_TEST.Screens.DataEntrySubsystems;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;

public partial class TreeEntryPage : ContentPage
{
    public TreeEntryPage()
    {
        InitializeComponent();
    }

    private void OnNextTreeClicked(object sender, EventArgs e)
    {
        DataService.CurrentTree.Species = SpeciesEntry.Text;
        if (double.TryParse(DbhEntry.Text, out double dBH))
        {
            DataService.CurrentTree.Dbh = dBH;
        }

        // Saves the tree and resets the slate (DataService handles the 'new Tree()' now)
        DataService.SaveTreeToPlot();

        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;
        SpeciesEntry.Focus();
    }

    private async void OnNextPlotClicked(object sender, EventArgs e)
    {
        DataService.CurrentTree.Species = SpeciesEntry.Text;
        if (double.TryParse(DbhEntry.Text, out double dBH))
        {
            DataService.CurrentTree.Dbh = dBH;
        }
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;

        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();

        // FIXED 1 (Infinite Stack): Pops this screen off the stack to reveal the existing Plot screen
        await Shell.Current.GoToAsync("..");
    }

    private async void OnFinishStandClicked(object sender, EventArgs e)
    {
        // 1. Save the final tree to the state manager
        DataService.CurrentTree.Species = SpeciesEntry.Text;
        if (double.TryParse(DbhEntry.Text, out double dBH))
        {
            DataService.CurrentTree.Dbh = dBH;
        }
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;

        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();

        // 2. Initialize your database connection
        var db = new sqllite.LocalDbService();
        await db.InitAsync(); // Ensures the mobile .db3 tables exist without freezing the UI

        // 3. TRANSLATE & SAVE
        // Create the SQL Stand
        var sqlStand = new sqllite.stand_data
        {
            Date = DateTime.Now,
            // Map any other top-level stand properties here
        };
        await db.AddStandDataAsync(sqlStand);

        // Loop through the Plots attached to this Stand
        foreach (var uiPlot in DataService.CurrentStand.PlotList)
        {
            var sqlPlot = new sqllite.plot_data
            {
                Date = DateTime.Now,
                Slope = (int)uiPlot.Slope,
                Aspect = (int)uiPlot.Aspect
            };
            await db.AddPlotDataAsync(sqlPlot);

            // Loop through the Trees attached to this Plot
            foreach (var uiTree in uiPlot.TreeList)
            {
                var sqlTree = new sqllite.tree_data
                {
                    Date = DateTime.Now,
                    Species = uiTree.Species ?? "Unknown",
                    DiameterBreastHeight = (float)uiTree.Dbh
                    // Map your gyroscope defect heights here later!
                };
                await db.AddTreeDataAsync(sqlTree);
            }
        }

        // 4. ONLY wipe the state AFTER the database has safely secured the data
        DataService.CurrentStand = new Models.Stand();

        // 5. Return to Home Menu
        await Shell.Current.GoToAsync("///DataEntryScreen");
    }
}