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
    private async void OnSeedKamiakPlotsClicked(object sender, EventArgs e)
    {
        try
        {
            await _db.InitAsync(); // Ensure tables exist
            OutputLabel.Text = "Seeding 78 Kamiak Plot Centers...";

            await _db.SeedKamiakStand();

            OutputLabel.Text = "Success! 78 Plots are now in the DB.";
        }
        catch (Exception ex)
        {
            OutputLabel.Text = $"Seeding Error: {ex.Message}";
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
        // 1. User
        var user = new user_data { };
        await _db.AddUserDataAsync(user); // SQLite assigns an ID (e.g., 1)

        // 2. Parcel (Linked to User)
        var parcel = new parcel_data
        {
            parentUserId = user.Id,
            Date = DateTime.Now.AddDays(-10),
            Acres = 50.0f
        };
        await _db.AddParcelDataAsync(parcel);

        // 3. Stand (Linked to Parcel)
        var stand = new stand_data
        {
            ParcelID = parcel.Id,
            Date = DateTime.Now.AddDays(-10),
            FvsVariant = "PN",
            SiteIndex = "85",
            HabitatType = "DF/PINE",
            Acres = 12.5f
        };
        await _db.AddStandDataAsync(stand);

        // 4. Plot (Linked to Stand)
        var plot1 = new plot_data
        {
            ParentStandId = stand.Id,
            Date = DateTime.Now.AddDays(-10),
            Latitude = 46.72839242116149f,
            Longitude = -117.16437432142496f,
            MostMesicTreeSpecies = "Douglas Fir"
        };
        await _db.AddPlotDataAsync(plot1);
        var plot2 = new plot_data
        {
            ParentStandId = stand.Id,
            Date = DateTime.Now.AddDays(-10),
            Latitude = 46.72912857248971f,
            Longitude = -117.166915320386f,
            MostMesicTreeSpecies = "Ponderosa Pine"
        };
        await _db.AddPlotDataAsync(plot2);
        var plot3 = new plot_data
        {
            ParentStandId = stand.Id,
            Date = DateTime.Now.AddDays(-10),
            Latitude = 46.72818426202383f,
            Longitude = -117.16629304219533f,
            MostMesicTreeSpecies = "Ponderosa Pine"
        };
        await _db.AddPlotDataAsync(plot3);

        // 5. Trees & Their Defects (The Relational Split)

        // --- Tree #1 in Plot 1 ---
        var tree1 = new tree_data
        {
            parentPlotId = plot1.Id,
            Species = "Douglas Fir",
            DiameterBreastHeight = 12.5f,
            Height = 60,
          
        };
        await _db.AddTreeDataAsync(tree1);

        // Link a Defect to Tree #1
        await _db.AddDefectDataAsync(new defect_data
        {
            parentTreeId = tree1.Id,
            Description = "None",
            BaseAngle = 0,
            TopAngle = 0
        });

        // --- Tree #2 in Plot 1 ---
        var tree2 = new tree_data
        {
            parentPlotId = plot1.Id,
            Species = "Ponderosa Pine",
            DiameterBreastHeight = 9.0f,
            Height = 45,
           
        };
        await _db.AddTreeDataAsync(tree2);

        // Link a Defect to Tree #2
        await _db.AddDefectDataAsync(new sqllite.defect_data
        {
            parentTreeId = tree2.Id,
            Description = "Basal scar",
            BaseAngle = 0,
            TopAngle = 3,
            CalculatedHeight = 3.5f
        });
    }

    private async Task PrintDatabaseContents()
    {
        var trees = await _db.GetAllTreeDataAsync();
        var plots = await _db.GetAllPlotDataAsync();
        var stands = await _db.GetAllStandDataAsync();
        var parcels = await _db.GetAllParcelDataAsync();
        var users = await _db.GetAllUserDataAsync();
        var defects = await _db.GetAllDefectDataAsync();

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
            // Added formatting to print the new tree coordinates
            output += $"  ID:{t.Id} | PlotFK:{t.parentPlotId} | Species:{t.Species} | DBH:{t.DiameterBreastHeight} | Height: {t.Height}\n";

        output += $"\n── defect_data ({defects.Count} records) ──\n";
        foreach (var d in defects)
        {
            output += $"  ID:{d.Id} | TreeFK:{d.parentTreeId} | {d.Description} | Ht:{d.CalculatedHeight:F1}ft | (B:{d.BaseAngle}°, T:{d.TopAngle}°)\n";
        }

        OutputLabel.Text = output;
    }

    private async void OnPrintKamiakPlotsClicked(object sender, EventArgs e)
    {
        try
        {
            await _db.InitAsync();
            OutputLabel.Text = "Retrieving Kamiak plots...\n";

            // 1. Find the Kamiak Stand in the database
            var stands = await _db.GetAllStandDataAsync();
            var kamiakStand = stands.FirstOrDefault(s => s.HabitatType == "Kamiak Butte");

            if (kamiakStand == null)
            {
                OutputLabel.Text = "Kamiak Butte stand not found. Make sure SeedKamiakStand() ran successfully on startup.";
                return;
            }

            // 2. Get only the plots tied to that specific Stand ID
            var plots = await _db.GetPlotsForStandAsync(kamiakStand.Id);

            // 3. Format the output to read cleanly
            var output = "═══════════════════════════════════════════\n";
            output += $"KAMIAK BUTTE PLOTS ({plots.Count} records)\n";
            output += "═══════════════════════════════════════════\n\n";

            for (int i = 0; i < plots.Count; i++)
            {
                var p = plots[i];
                // Formatting to 6 decimal places to match your GeoJSON precision
                output += $"  [Plot {i + 1}] ID:{p.Id} | Lat: {p.Latitude:F6} | Lon: {p.Longitude:F6}\n";
            }

            OutputLabel.Text = output;
        }
        catch (Exception ex)
        {
            OutputLabel.Text = $"Error: {ex.Message}";
        }
    }


    private async Task ClearAllTables()
    {
        // delete from top down (Children first, then parents)

        var allDefects = await _db.GetAllDefectDataAsync();
        foreach (var d in allDefects) await _db.DeleteDefectDataAsync(d);


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

        await _db.ResetIncrements();
    }
}