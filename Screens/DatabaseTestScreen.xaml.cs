using MOBWEB_TEST.sqllite;
using System.Linq;
using System.Collections.Generic;

namespace MOBWEB_TEST.Screens;

public partial class DatabaseTestScreen : ContentPage
{
    private LocalDbService _db;

    public DatabaseTestScreen()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _db = new LocalDbService();
    }

    private async void OnPopulateDatabaseClicked(object sender, EventArgs e)
    {
        try
        {
            await _db.InitAsync();
            OutputLabel.Text = "Populating database...";
            await PopulateSampleData();
            OutputLabel.Text = "Database populated successfully!";
        }
        catch (Exception ex)
        {
            OutputLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async void OnPrintContentsClicked(object sender, EventArgs e)
    {
        try
        {
            await _db.InitAsync();
            OutputLabel.Text = "Retrieving database contents...\n";
            await PrintDatabaseContents();
        }
        catch (Exception ex)
        {
            OutputLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async void OnClearDatabaseClicked(object sender, EventArgs e)
    {
        try
        {
            bool confirm = await DisplayAlert("Confirm", "Clear ALL records from all tables?", "Yes", "No");
            if (!confirm) return;

            await ClearAllTables();
            OutputLabel.Text = "All tables cleared successfully!";
        }
        catch (Exception ex)
        {
            OutputLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }

    // ── Populate ─────────────────────────────────────────────────────────────

    private async Task PopulateSampleData()
    {
        // 1. Users (Top of the hierarchy - no parent)
        var users = new List<user_data>
    {
        new user_data { } // SQLite will assign this Id = 1
    };
        foreach (var u in users) await _db.AddUserDataAsync(u);

        // 2. Parcels (Points up to User #1)
        var parcels = new List<parcel_data>
    {
        new parcel_data { parentUserId = 1, Date = DateTime.Now.AddDays(-10), Acres = 50.0f } // SQLite assigns Id = 1
    };
        foreach (var p in parcels) await _db.AddParcelDataAsync(p);

        // 3. Stands (Points up to Parcel #1)
        var stands = new List<stand_data>
    {
        new stand_data { ParcelID = 1, Date = DateTime.Now.AddDays(-10), FvsVariant = "PN", SiteIndex = "85", HabitatType = "DF/PINE", Acres = 12.5f, Latitude = 46.7312f, Longitude = -117.1815f, Aspect = 45.0f, Slope = 15.0f, Elevation = 2500.0f } // SQLite assigns Id = 1
    };
        foreach (var s in stands) await _db.AddStandDataAsync(s);

        // 4. Plots (Both point up to Stand #1)
        var plots = new List<plot_data>
    {
        new plot_data { ParentStandId = 1, Date = DateTime.Now.AddDays(-10), Latitude = 46.7312f, Longitude = -117.1815f, Aspect = 45, Slope = 15, Elevation = 2500, ImagePath = "/images/plot_001.jpg", MostMesicTreeSpecies = "Douglas Fir", MostMesicBushSpecies = "Hazel" }, // SQLite assigns Id = 1
        new plot_data { ParentStandId= 1, Date = DateTime.Now.AddDays(-5), Latitude = 46.7380f, Longitude = -117.1900f, Aspect = 180, Slope = 20, Elevation = 2650, ImagePath = "/images/plot_002.jpg", MostMesicTreeSpecies = "Western Larch", MostMesicBushSpecies = "Dogwood" }  // SQLite assigns Id = 2
    };
        foreach (var p in plots) await _db.AddPlotDataAsync(p);

        // 5. Trees (Point up to their respective Plots)
        var trees = new List<tree_data>
    {
        // These two trees point to Plot #1
        new tree_data { parentPlotId = 1, Date = DateTime.Now.AddDays(-10), Height = 60, Species = "Douglas Fir", DiameterBreastHeight = 12.5f, StumpHeight = 1.0f, BaseOfLiveCrown = 20.0f, CrownRatio = 55.0f, DefectDescription = "None", DefectBase = 0, DefectTop = 0 },
        new tree_data { parentPlotId = 1, Date = DateTime.Now.AddDays(-10), Height = 45, Species = "Ponderosa Pine", DiameterBreastHeight = 9.0f, StumpHeight = 1.0f, BaseOfLiveCrown = 15.0f, CrownRatio = 40.0f, DefectDescription = "Basal scar", DefectBase = 0, DefectTop = 3 },
        
        // These two trees point to Plot #2
        new tree_data { parentPlotId = 2, Date = DateTime.Now.AddDays(-5), Height = 70, Species = "Western Larch", DiameterBreastHeight = 18.0f, StumpHeight = 1.5f, BaseOfLiveCrown = 30.0f, CrownRatio = 60.0f, DefectDescription = "None", DefectBase = 0, DefectTop = 0 },
        new tree_data { parentPlotId = 2, Date = DateTime.Now, Height = 35, Species = "Grand Fir", DiameterBreastHeight = 7.5f, StumpHeight = 1.0f, BaseOfLiveCrown = 10.0f, CrownRatio = 45.0f, DefectDescription = "Fork at 20ft", DefectBase = 18, DefectTop = 22 }
    };
        foreach (var t in trees) await _db.AddTreeDataAsync(t);
    }

    private async Task PrintDatabaseContents()
    {
        var trees = await _db.GetAllTreeDataAsync();
        var plots = await _db.GetAllPlotDataAsync();
        var stands = await _db.GetAllStandDataAsync();
        var parcels = await _db.GetAllParcelDataAsync();
        var users = await _db.GetAllUserDataAsync();

        var output = "═══════════════════════════════════════════\n";
        output += "DATABASE CONTENTS\n";
        output += "═══════════════════════════════════════════\n\n";

        output += $"── user_data ({users.Count} records) ──\n";
        foreach (var u in users)
            output += $"  ID:{u.Id}\n";

        output += $"\n── parcel_data ({parcels.Count} records) ──\n";
        foreach (var p in parcels)
            output += $"  ID:{p.Id} | Acres:{p.Acres}ac\n";

        output += $"\n── stand_data ({stands.Count} records) ──\n";
        foreach (var s in stands)
            output += $"  ID:{s.Id} | ParcelFK:{s.ParcelID} | {s.FvsVariant} | SI:{s.SiteIndex} | {s.Acres}ac\n";

        output += $"\n── plot_data ({plots.Count} records) ──\n";
        foreach (var p in plots)
            output += $"  ID:{p.Id} | StandFK:{p.ParentStandId} | ({p.Latitude:F4},{p.Longitude:F4}) | Elev:{p.Elevation}ft\n";

        output += $"\n── tree_data ({trees.Count} records) ──\n";
        foreach (var t in trees)
            output += $"  ID:{t.Id} | PlotFK:{t.parentPlotId} | {t.Species} | DBH:{t.DiameterBreastHeight}\n";

        OutputLabel.Text = output;
    }


    private async Task ClearAllTables()
    {
        // delete from top down (Children first, then parents)
        var allTrees = await _db.GetAllTreeDataAsync();
        foreach (var t in allTrees) await _db.DeleteTreeDataAsync(t);

        var allPlots = await _db.GetAllPlotDataAsync();
        foreach (var p in allPlots) await _db.DeletePlotDataAsync(p);

        var allStands = await _db.GetAllStandDataAsync();
        foreach (var s in allStands) await _db.DeleteStandDataAsync(s);

        var allParcels = await _db.GetAllParcelDataAsync();
        foreach (var p in allParcels) await _db.DeleteParcelDataAsync(p);

        var allUsers = await _db.GetAllUserDataAsync();
        foreach (var u in allUsers) await _db.DeleteUserDataAsync(u);
    }
}