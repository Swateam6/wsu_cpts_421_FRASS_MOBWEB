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
        await Shell.Current.GoToAsync("..");
    }

    private async void OnFinishStandClicked(object sender, EventArgs e)
    {
        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();

        // ANTI-GHOST STAND GATE: If the stand has absolutely no plots, abort the SQLite save!
        if (DataService.CurrentStand.PlotList == null || DataService.CurrentStand.PlotList.Count == 0)
        {
            UpdateDescription("No data measured. Skipping SQLite save.");

            // Clean up the memory anyway so it's fresh
            DataService.CurrentStand = new Stand();
            DataService.CurrentPlot = new Plot();
            DataService.CurrentTree = new Tree();

            await Shell.Current.GoToAsync("///DataEntryScreen");
            return; // <-- This stops the ghost save!
        }

        // 1. Create and save the Stand (Keep the rest of your SQL code below this exactly the same)
        var sqlStand = new sqllite.stand_data
        {
            Date = DateTime.Now
        };
        await _database.AddStandDataAsync(sqlStand);

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