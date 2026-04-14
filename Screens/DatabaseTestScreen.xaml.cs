using MOBWEB_TEST.sqllite;

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
        // LocalDbService constructor creates all five tables and owns the DB path
        _db = new LocalDbService();
    }

    private async void OnPopulateDatabaseClicked(object sender, EventArgs e)
    {
        try
        {
            await _db.InitAsync(); // <-- ADD THIS: Ensures tables exist before populating
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
            await _db.InitAsync(); // <-- ADD THIS: Ensures tables exist before querying
            OutputLabel.Text = "Retrieving database contents...";
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
        //trees
        var trees = new List<tree_data>
        {
            new tree_data
            {
                Id = 1,
                Date = DateTime.Now.AddDays(-10),
                Height = 60,
                Species = "Douglas Fir",
                DiameterBreastHeight = 12.5f,
                StumpHeight = 1.0f,
                BaseOfLiveCrown = 20.0f,
                CrownRatio = 55.0f,
                DefectDescription = "None",
                DefectBase = 0,
                DefectTop = 0
            },
            new tree_data
            {
                Id = 2,
                Date = DateTime.Now.AddDays(-10),
                Height = 45,
                Species = "Ponderosa Pine",
                DiameterBreastHeight = 9.0f,
                StumpHeight = 1.0f,
                BaseOfLiveCrown = 15.0f,
                CrownRatio = 40.0f,
                DefectDescription = "Basal scar",
                DefectBase = 0,
                DefectTop = 3
            },
            new tree_data
            {
                Id = 3,
                Date = DateTime.Now.AddDays(-5),
                Height = 70,
                Species = "Western Larch",
                DiameterBreastHeight = 18.0f,
                StumpHeight = 1.5f,
                BaseOfLiveCrown = 30.0f,
                CrownRatio = 60.0f,
                DefectDescription = "None",
                DefectBase = 0,
                DefectTop = 0
            },
            new tree_data
            {
                Id = 4,
                Date = DateTime.Now,
                Height = 35,
                Species = "Grand Fir",
                DiameterBreastHeight = 7.5f,
                StumpHeight = 1.0f,
                BaseOfLiveCrown = 10.0f,
                CrownRatio = 45.0f,
                DefectDescription = "Fork at 20ft",
                DefectBase = 18,
                DefectTop = 22
            }
        };
        foreach (var t in trees)
            await _db.AddTreeDataAsync(t);

        // Plots(contain treres)
        var plots = new List<plot_data>
        {
            new plot_data
            {
                Id = 1,
                Date = DateTime.Now.AddDays(-10),
                Latitude = 46.7312f,
                Longitude = -117.1815f,
                Aspect = 45,
                Slope = 15,
                Elevation = 2500,
                ImagePath = "/images/plot_001.jpg",
                MostMesicTreeSpecies = "Douglas Fir",
                MostMesicBushSpecies = "Hazel",
                tree_ids_in_parcel = new List<int> { 1, 2 }
            },
            new plot_data
            {
                Id = 2,
                Date = DateTime.Now.AddDays(-5),
                Latitude = 46.7380f,
                Longitude = -117.1900f,
                Aspect = 180,
                Slope = 20,
                Elevation = 2650,
                ImagePath = "/images/plot_002.jpg",
                MostMesicTreeSpecies = "Western Larch",
                MostMesicBushSpecies = "Dogwood",
                tree_ids_in_parcel = new List<int> { 3, 4 }
            }
        };
        foreach (var p in plots)
            await _db.AddPlotDataAsync(p);

        // stand with the plots
        var stands = new List<stand_data>
        {
            new stand_data
            {
                Id = 1,
                Date = DateTime.Now.AddDays(-10),
                FvsVariant = "PN",
                SiteIndex = "85",
                HabitatType = "DF/PINE",
                Acres = 12.5f,
                Latitude = 46.7312f,
                Longitude = -117.1815f,
                Aspect = 45.0f,
                Slope = 15.0f,
                Elevation = 2500.0f,
                plot_ids_in_parcel = new List<int> { 1, 2 }
            }
        };
        foreach (var s in stands)
            await _db.AddStandDataAsync(s);

        // parcel with the stand
        var parcels = new List<parcel_data>
        {
            new parcel_data
            {
                Id = 1,
                Date = DateTime.Now.AddDays(-10),
                Acres = 50.0f,
                stand_ids_in_parcel = new List<int> { 1 }
            }
        };
        foreach (var p in parcels)
            await _db.AddParcelDataAsync(p);

        // user with the parcel
        var users = new List<user_data>
        {
            new user_data
            {
                Id = 1,
                parcel_ids_in_parcel = new List<int> { 1 }
            }
        };
        foreach (var u in users)
            await _db.AddUserDataAsync(u);
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

        output += $"── tree_data ({trees.Count} records) ──\n";
        if (trees.Count == 0)
            output += "  (empty)\n";
        foreach (var t in trees)
            output += $"  ID:{t.Id} | {t.Species} | {t.Height}ft | DBH:{t.DiameterBreastHeight}in | Crown:{t.CrownRatio}% | {t.DefectDescription}\n";

        output += $"\n── plot_data ({plots.Count} records) ──\n";
        if (plots.Count == 0)
            output += "  (empty)\n";
        foreach (var p in plots)
            output += $"  ID:{p.Id} | ({p.Latitude:F4},{p.Longitude:F4}) | Elev:{p.Elevation}ft | {p.MostMesicTreeSpecies}/{p.MostMesicBushSpecies} | Trees:[{string.Join(",", p.tree_ids_in_parcel)}]\n";

        output += $"\n── stand_data ({stands.Count} records) ──\n";
        if (stands.Count == 0)
            output += "  (empty)\n";
        foreach (var s in stands)
            output += $"  ID:{s.Id} | {s.FvsVariant} | SI:{s.SiteIndex} | {s.Acres}ac | Plots:[{string.Join(",", s.plot_ids_in_parcel)}]\n";

        output += $"\n── parcel_data ({parcels.Count} records) ──\n";
        if (parcels.Count == 0)
            output += "  (empty)\n";
        foreach (var p in parcels)
            output += $"  ID:{p.Id} | {p.Acres}ac | Stands:[{string.Join(",", p.stand_ids_in_parcel)}]\n";

        output += $"\n── user_data ({users.Count} records) ──\n";
        if (users.Count == 0)
            output += "  (empty)\n";
        foreach (var u in users)
            output += $"  ID:{u.Id} | Parcels:[{string.Join(",", u.parcel_ids_in_parcel)}]\n";

        output += "\n═══════════════════════════════════════════";

        OutputLabel.Text = output;
    }


    private async Task ClearAllTables()
    {
        // delete from top down
        var allUsers = await _db.GetAllUserDataAsync();
        foreach (var u in allUsers) await _db.DeleteUserDataAsync(u);

        var allParcels = await _db.GetAllParcelDataAsync();
        foreach (var p in allParcels) await _db.DeleteParcelDataAsync(p);

        var allStands = await _db.GetAllStandDataAsync();
        foreach (var s in allStands) await _db.DeleteStandDataAsync(s);

        var allPlots = await _db.GetAllPlotDataAsync();
        foreach (var p in allPlots) await _db.DeletePlotDataAsync(p);

        var allTrees = await _db.GetAllTreeDataAsync();
        foreach (var t in allTrees) await _db.DeleteTreeDataAsync(t);
    }
}