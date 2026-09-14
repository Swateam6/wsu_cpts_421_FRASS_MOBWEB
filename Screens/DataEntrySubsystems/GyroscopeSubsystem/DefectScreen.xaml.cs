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
    private async void OnSaveDefectClicked(object sender, EventArgs e)
    {
        // 1. Let the controller handle the math and update DataService.CurrentDefect
        _defectController.SaveDefect(DistanceEntry.Text, DefectDescriptionEntry.Text);

        // 2. Push the completed defect into the current tree's list
        DataService.SaveDefectToTree();

        // 3. UI Cleanup
        DistanceEntry.Text = string.Empty;
        DefectDescriptionEntry.Text = string.Empty;
        UpdateDescription("Defect saved to tree memory!");

    }

    private async void OnNextTreeClicked(object sender, EventArgs e)
    {
        // 1. Commit active defect into the current tree
        DataService.SaveDefectToTree();

        // 2. Commit the completed tree into the current plot
        DataService.SaveTreeToPlot();

        // 3. Reset active tree and defect memory for the next measurement
        DataService.CurrentTree = new Tree();
        DataService.CurrentDefect = new Defects();

        // 4. Navigate back to start the next tree
        await Shell.Current.GoToAsync("TreeEntryPage");
    }
    private async void OnFinishStandClicked(object sender, EventArgs e)
    {
        // 1. Push active memory into the lists
        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();
        DataService.SaveStandToParcel();

        // ---------------------------------------------------------
        // THE IMPENETRABLE WALL: Count the actual, physical trees
        // ---------------------------------------------------------
        int totalRealTrees = 0;
        if (DataService.CurrentStand?.PlotList != null)
        {
            foreach (var uiPlot in DataService.CurrentStand.PlotList)
            {
                if (uiPlot.TreeList != null)
                {
                    // A real tree MUST have a DBH greater than 0
                    totalRealTrees += uiPlot.TreeList.Count(t => t.Dbh > 0);
                }
            }
        }

        // 2. ABORT GATE: If no real trees exist, kill the process immediately
        if (totalRealTrees == 0)
        {
            UpdateDescription("No trees measured. Ghost Stand blocked.");

            // Clean up memory
            DataService.CurrentStand = new Stand();
            DataService.CurrentPlot = new Plot();
            DataService.CurrentTree = new Tree();
            DataService.CurrentDefect = new Defects();

            await Shell.Current.GoToAsync("///DataEntryScreen");
            return; // <--- SQLITE IS NEVER TOUCHED
        }

        // 3. PARCEL FK
        var parcels = await _database.GetAllParcelDataAsync();
        int activeParcelId = parcels.Count == 0 ? 1 : parcels.First().Id;

        // 4. STAND: Check if we are using an existing Stand or need a new one
        int activeStandId = DataService.CurrentStand.StandId;

        if (activeStandId == 0)
        {
            // Only generate a new Stand if we didn't inherit one from the Kamiak database
            var sqlStand = new sqllite.stand_data
            {
                ParcelID = activeParcelId,
                Date = DateTime.Now
            };
            await _database.AddStandDataAsync(sqlStand);
            activeStandId = sqlStand.Id;
        }

        // 5. PLOTS: Second layer of defense
        foreach (var uiPlot in DataService.CurrentStand.PlotList)
        {
            // SKIP EMPTY PLOTS
            bool plotHasRealTrees = uiPlot.TreeList != null && uiPlot.TreeList.Any(t => t.Dbh > 0);
            if (!plotHasRealTrees) continue;

            int activePlotId = uiPlot.PlotNumber;

            if (activePlotId == 0)
            {
                // Only generate a new "Ghost" Plot if this isn't a pre-existing Kamiak plot
                var sqlPlot = new sqllite.plot_data
                {
                    ParentStandId = activeStandId,
                    Date = DateTime.Now,
                    Slope = (int)uiPlot.Slope,
                    Aspect = (int)uiPlot.Aspect,
                    size = uiPlot.size
                };
                await _database.AddPlotDataAsync(sqlPlot);
                activePlotId = sqlPlot.Id;
            }

            // 6. TREES: Link directly to the REAL activePlotId
            foreach (var uiTree in uiPlot.TreeList)
            {
                if (uiTree.Dbh <= 0) continue;

                var sqlTree = new sqllite.tree_data
                {
                    parentPlotId = activePlotId, // Links to either the Kamiak Plot or the newly created one
                    Date = DateTime.Now,
                    Species = uiTree.Species ?? "Unknown",
                    DiameterBreastHeight = (float)uiTree.Dbh,
                    Height = (int)uiTree.Height
                };
                await _database.AddTreeDataAsync(sqlTree);

                // ... (Keep your Defect loop the exact same below this) ...

                // 7. DEFECTS
                if (uiTree.DefectList != null)
                {
                    foreach (var uiDefect in uiTree.DefectList)
                    {
                        var sqlDefect = new sqllite.defect_data
                        {
                            parentTreeId = sqlTree.Id,
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

        // 8. FINAL CLEANUP
        DataService.CurrentStand = new Stand();
        DataService.CurrentPlot = new Plot();
        DataService.CurrentTree = new Tree();
        DataService.CurrentDefect = new Defects();

        UpdateDescription("Stand data successfully synced to SQLite.");
        await Shell.Current.GoToAsync("///DataEntryScreen");
    }

    private async void OnFinishPlotClicked(object sender, EventArgs e)
    {
        DataService.SaveDefectToTree();
        DataService.SaveTreeToPlot();

        var currentPlot = DataService.CurrentPlot;
        var currentStand = DataService.CurrentStand;

        // 1. Count trees only in the CURRENT plot
        int realTreesInPlot = 0;
        if (currentPlot?.TreeList != null)
        {
            realTreesInPlot = currentPlot.TreeList.Count(t => t.Dbh > 0);
        }

        if (realTreesInPlot == 0)
        {
            UpdateDescription("No valid trees measured. Ghost plot blocked.");

            // Clean up plot-level memory
            DataService.CurrentPlot = new Plot();
            DataService.CurrentTree = new Tree();
            DataService.CurrentDefect = new Defects();

            await Shell.Current.GoToAsync("PlotCoordinateSet");
            return;
        }

        // 2. Ensure parent Stand exists in SQLite
        int activeStandId = currentStand.StandId;
        if (activeStandId == 0)
        {
            var parcels = await _database.GetAllParcelDataAsync();
            int activeParcelId = parcels.Count == 0 ? 1 : parcels.First().Id;

            var sqlStand = new sqllite.stand_data
            {
                ParcelID = activeParcelId,
                Date = DateTime.Now
            };
            await _database.AddStandDataAsync(sqlStand);
            activeStandId = sqlStand.Id;
            currentStand.StandId = activeStandId;
        }

        // 3. Handle Plot ID (use existing Kamiak ID or create new row)
        int activePlotId = currentPlot.PlotNumber;
        if(activePlotId==0)
        {
            var sqlPlot = new sqllite.plot_data
            {
                ParentStandId = activeStandId,
                Date= DateTime.Now,
                Slope = (int)currentPlot.Slope,
                Aspect=(int)currentPlot.Aspect,

            };
            await _database.AddPlotDataAsync(sqlPlot);
            activePlotId = sqlPlot.Id;
            currentPlot.PlotNumber = activePlotId;
        }
        if(currentPlot.TreeList!=null)
        {
            foreach(var uiTree in currentPlot.TreeList)
            {
                if(uiTree.DefectList !=null)
                {
                    foreach(var uiDefect in uiTree.DefectList)
                    {
                        var sqlDefect = new sqllite.defect_data
                        {
                            parentTreeId = uiTree.Id,
                            Description = uiDefect.Description,
                            BaseAngle = uiDefect.BaseAngle,
                            TopAngle = uiDefect.TopAngle,
                            CalculatedHeight = (float)(uiDefect.topHeight - uiDefect.bottomHeight)
                        };
                        await _database.AddDefectDataAsync(sqlDefect);
                    }
                }
            }
        }

        // 5. Clean up plot-level state and navigate back
        DataService.CurrentPlot = new Plot();
        DataService.CurrentTree = new Tree();
        DataService.CurrentDefect = new Defects();

        await Shell.Current.GoToAsync("PlotCoordinateSet");
    }
}