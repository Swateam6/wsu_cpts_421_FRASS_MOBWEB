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
        // 1. Create User
        var testUser = new user_data();
        await _db.AddUserDataAsync(testUser); // ID is auto-generated here!

        // 2. Create Parcel
        var testParcel = new parcel_data
        {
            Date = DateTime.Now.AddDays(-10),
            Acres = 50.0f
            // If you added a UserID foreign key to parcel, it goes here!
        };
        await _db.AddParcelDataAsync(testParcel);

        // 3. Create Stand (Linked to Parcel)
        var testStand = new stand_data
        {
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
            ParcelID = testParcel.Id // <--- THE FOREIGN KEY LINK
        };
        await _db.AddStandDataAsync(testStand);

        // 4. Create Plots (Linked to Stand)
        var plot1 = new plot_data
        {
            Date = DateTime.Now.AddDays(-10),
            Latitude = 46.7312f,
            Longitude = -117.1815f,
            Aspect = 45,
            Slope = 15,
            Elevation = 2500,
            ImagePath = "/images/plot_001.jpg",
            MostMesicTreeSpecies = "Douglas Fir",
            MostMesicBushSpecies = "Hazel",
            ParentStandId = testStand.Id // <--- THE FOREIGN KEY LINK
        };
        await _db.AddPlotDataAsync(plot1);

        var plot2 = new plot_data
        {
            Date = DateTime.Now.AddDays(-5),
            Latitude = 46.7380f,
            Longitude = -117.1900f,
            Aspect = 180,
            Slope = 20,
            Elevation = 2650,
            ImagePath = "/images/plot_002.jpg",
            MostMesicTreeSpecies = "Western Larch",
            MostMesicBushSpecies = "Dogwood",
            ParentStandId = testStand.Id // <--- THE FOREIGN KEY LINK
        };
        await _db.AddPlotDataAsync(plot2);

        // 5. Create Trees (Linked to Plots)
        var tree1 = new tree_data
        {
            Date = DateTime.Now.AddDays(-10),
            Height = 60,
            Species = "Douglas Fir",
            DiameterBreastHeight = 12.5f,
            StumpHeight = 1.0f,
            BaseOfLiveCrown = 20.0f,
            CrownRatio = 55.0f,
            DefectDescription = "None",
            parentPlotId = plot1.Id // <--- THE FOREIGN KEY LINK
        };
        await _db.AddTreeDataAsync(tree1);

        var tree2 = new tree_data
        {
            Date = DateTime.Now.AddDays(-10),
            Height = 45,
            Species = "Ponderosa Pine",
            DiameterBreastHeight = 9.0f,
            StumpHeight = 1.0f,
            BaseOfLiveCrown = 15.0f,
            CrownRatio = 40.0f,
            DefectDescription = "Basal scar",
            DefectTop = 3,
            parentPlotId = plot1.Id // <--- THE FOREIGN KEY LINK
        };
        await _db.AddTreeDataAsync(tree2);

        var tree3 = new tree_data
        {
            Date = DateTime.Now.AddDays(-5),
            Height = 70,
            Species = "Western Larch",
            DiameterBreastHeight = 18.0f,
            StumpHeight = 1.5f,
            BaseOfLiveCrown = 30.0f,
            CrownRatio = 60.0f,
            DefectDescription = "None",
            parentPlotId = plot2.Id // <--- THE FOREIGN KEY LINK
        };
        await _db.AddTreeDataAsync(tree3);
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