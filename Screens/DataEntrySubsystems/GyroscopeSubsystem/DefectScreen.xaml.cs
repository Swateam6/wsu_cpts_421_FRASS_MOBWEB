using MOBWEB_TEST.Services;
using MOBWEB_TEST.sqllite;
using MOBWEB_TEST.Models;
using System;
using System.Numerics;




namespace MOBWEB_TEST.Screens.DataEntrySubsystems.GyroscopeSubsystem;

public partial class DefectScreen : ContentPage
{
    private readonly LocalDbService _database;
    private readonly DefectGyroscopeController _defectController;

    public DefectScreen(LocalDbService database)
    {
        InitializeComponent();
        _database = database;
        _defectController = new DefectGyroscopeController(this);
    }

    public void UpdateCurrentAngle(string text)
    {
        CurrentAngleLabel.Text = text;
    }

    public void UpdateBaseLabel(Vector3? angle)
    {
        DefectBaseLabel.Text = angle.HasValue
            ? $"Defect Base: X: {angle.Value.X:F1}°, Y: {angle.Value.Y:F1}°, Z: {angle.Value.Z:F1}°"
            : "Defect Base: ---";
    }

    public void UpdateTopLabel(Vector3? angle)
    {
        DefectTopLabel.Text = angle.HasValue
            ? $"Defect Top: X: {angle.Value.X:F1}°, Y: {angle.Value.Y:F1}°, Z: {angle.Value.Z:F1}°"
            : "Defect Top: ---";
    }

    public void UpdateDescription(string message)
    {
        StatusMessageLabel.Text = message;
    }

    private void OnGyroMeasureClicked(object sender, EventArgs e)
    {
        _defectController.StartGyroscope();
    }

    private void OnZeroClicked(object sender, EventArgs e)
    {
        _defectController.ZeroGyroReadings();
    }

    private void OnCaptureBaseClicked(object sender, EventArgs e)
    {
        _defectController.CaptureBase();
    }

    private void OnCaptureTopClicked(object sender, EventArgs e)
    {
        _defectController.CaptureTop();
    }

    private async void OnFinishStandClicked(object sender, EventArgs e)
    {
        // 1. Push active memory into the lists first
        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();

        // 2. ANTI-GHOST GATE: Abort if the cruiser didn't actually measure anything
        if (DataService.CurrentStand.PlotList == null || DataService.CurrentStand.PlotList.Count == 0)
        {
            UpdateDescription("No data measured. Skipping SQLite save.");
            DataService.CurrentStand = new Stand(); // Reset for next time
            await Shell.Current.GoToAsync("///DataEntryScreen");
            return;
        }

        // 3. FIX FK ERROR: Find or create a parent Parcel
        var parcels = await _database.GetAllParcelDataAsync();
        int activeParcelId;

        if (parcels.Count == 0)
        {
            // Create a default parcel for the demo if none exists
            var newParcel = new sqllite.parcel_data { parentUserId = 1, Acres= 160 };
            await _database.AddParcelDataAsync(newParcel);
            activeParcelId = newParcel.Id;
        }
        else
        {
            activeParcelId = parcels.First().Id;
        }

        // 4. STAND: Save with the mandatory parent_parcel_id
        var sqlStand = new sqllite.stand_data
        {
            ParcelID = activeParcelId, // Mandatory FK reference
            Date = DateTime.Now
        };
        await _database.AddStandDataAsync(sqlStand);

        // 5. PLOTS: Loop through memory and link to the new Stand ID
        foreach (var uiPlot in DataService.CurrentStand.PlotList)
        {
            var sqlPlot = new sqllite.plot_data
            {
                ParentStandId = sqlStand.Id, // Linking FK
                Date = DateTime.Now,
                Slope = (int)uiPlot.Slope,
                Aspect = (int)uiPlot.Aspect,
                size = uiPlot.size // BAF or Radius from initialization
            };
            await _database.AddPlotDataAsync(sqlPlot);

            // 6. TREES: Loop through and link to the new Plot ID
            if (uiPlot.TreeList != null)
            {
                foreach (var uiTree in uiPlot.TreeList)
                {
                    var sqlTree = new sqllite.tree_data
                    {
                        parentPlotId = sqlPlot.Id, // Linking FK
                        Date = DateTime.Now,
                        Species = uiTree.Species ?? "Unknown",
                        DiameterBreastHeight = (float)uiTree.Dbh
                    };
                    await _database.AddTreeDataAsync(sqlTree);

                    // 7. DEFECTS: Loop through and link to the new Tree ID
                    if (uiTree.DefectList != null)
                    {
                        foreach (var uiDefect in uiTree.DefectList)
                        {
                            var sqlDefect = new sqllite.defect_data
                            {
                                parentTreeId = sqlTree.Id, // Linking FK
                                Description = uiDefect.Description,
                                BaseAngle = (float)uiDefect.BaseAngle,
                                TopAngle = (float)uiDefect.TopAngle,
                                CalculatedHeight = (float)(uiDefect.topHeight - uiDefect.bottomHeight)
                            };
                            await _database.AddDefectDataAsync(sqlDefect);
                        }
                    }
                }
            }
        }

        // 8. FINAL CLEANUP: Wipe all memory objects
        DataService.CurrentStand = new Stand();
        DataService.CurrentPlot = new Plot();
        DataService.CurrentTree = new Tree();
        DataService.CurrentDefect = new Defects();

        UpdateDescription("Stand data successfully synced to SQLite.");
        await Shell.Current.GoToAsync("///DataEntryScreen");
    }
    // ... (Keep the rest of your loops the same) ...

    private async void OnFinishPlotClicked(object sender, EventArgs e)
    {
        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();
        await Shell.Current.GoToAsync("PlotCoordinateSet");
    }
    private void OnNoDefectClicked(object sender, EventArgs e)
    {
        // 1. Call the bypass method we added to the controller
        _defectController.SaveNoDefect();

        // 2. Clear out the text boxes so the screen is reset for the next tree
        DistanceEntry.Text = string.Empty;
        DefectDescriptionEntry.Text = string.Empty;

        // 3. (Optional "Swag" Move) Automatically route them to the next tree
        // If you uncomment the lines below, the app will instantly save the tree
        // and jump back to navigation, saving the cruiser another click!

        // DataService.SaveTreeToPlot();
        // Shell.Current.GoToAsync("NavigationScreen");
    }
}