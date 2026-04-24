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
        DataService.SaveDefectToTree();
        // 1. Controller preps the math and text (likely updating DataService.CurrentDefect)
        _defectController.SaveDefect(DistanceEntry.Text, DefectDescriptionEntry.Text);

        // 2. Assign the Foreign Key so it links to the current tree
        DataService.CurrentDefect.treeID= DataService.CurrentTree.Id;

        // 3. Insert into SQLite
        if (_database != null)
        {
            //Make Defect Class
            UpdateDescription("Defect saved to database!");
        }


        // Optional: clear UI fields
        DistanceEntry.Text = string.Empty;
        DefectDescriptionEntry.Text = string.Empty;
    }

    private async void OnNextTreeClicked(object sender, EventArgs e)
    {
        DataService.SaveTreeToPlot();
        await Shell.Current.GoToAsync("TreeEntryPage");
    }

    private async void OnFinishStandClicked(object sender, EventArgs e)
    {
        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();
        // 1. Create and save the Stand
        var sqlStand = new sqllite.stand_data
        {
            Date = DateTime.Now
            // Add other stand-level fields here if needed
        };
        await _database.AddStandDataAsync(sqlStand);

        // 2. Loop through Plots in the Stand
        if (DataService.CurrentStand.PlotList != null)
        {
            foreach (var uiPlot in DataService.CurrentStand.PlotList)
            {
                var sqlPlot = new sqllite.plot_data
                {
                    ParentStandId = sqlStand.Id, // Link to Stand
                    Date = DateTime.Now,
                    Slope = (int)uiPlot.Slope,
                    Aspect = (int)uiPlot.Aspect
                };
                await _database.AddPlotDataAsync(sqlPlot);

                // 3. Loop through Trees in each Plot
                if (uiPlot.TreeList != null)
                {
                    foreach (var uiTree in uiPlot.TreeList)
                    {
                        var sqlTree = new sqllite.tree_data
                        {
                            parentPlotId = sqlPlot.Id, // Link to Plot
                            Date = DateTime.Now,
                            Species = uiTree.Species ?? "Unknown",
                            DiameterBreastHeight = (float)uiTree.Dbh
                        };
                        await _database.AddTreeDataAsync(sqlTree);

                        // 4. Loop through Defects in each Tree
                        foreach (var uiDefect in uiTree.DefectList)
                        {
                            var sqlDefect = new sqllite.defect_data
                            {
                                parentTreeId = sqlTree.Id,
                                Description = uiDefect.Description,

                                // Angle to Angle (Safe and correct)
                                BaseAngle = (float)uiDefect.BaseAngle,
                                TopAngle = (float)uiDefect.TopAngle,

                                // Height Calculation (The final result)
                                // This calculates the total 'length' of the defect on the trunk
                                CalculatedHeight = (float)(uiDefect.topHeight - uiDefect.bottomHeight)
                            };

                            await _database.AddDefectDataAsync(sqlDefect);
                        }
                    }
                }
            }
        }

        // 5. Cleanup: Wipe the DataService so the next stand is a fresh start
        DataService.CurrentStand = new Stand();
        DataService.CurrentPlot = new Plot();
        DataService.CurrentTree = new Tree();

        UpdateDescription("Stand data successfully synced to SQLite.");

        // 6. Navigate back to the main dashboard
        await Shell.Current.GoToAsync("///DataEntryScreen");
    }

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