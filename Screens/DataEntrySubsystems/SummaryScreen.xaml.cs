using System.Collections.ObjectModel;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;
using MOBWEB_TEST.sqllite;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class SummaryScreen : ContentPage
{
    private readonly LocalDbService _db;
    private readonly ObservableCollection<TreeValidationItem> _treeItems = new();

    public class TreeValidationItem
    {
        public int PlotNumber { get; set; }
        public int TreeNumber { get; set; }
        public string DisplayTitle => $"Plot {PlotNumber} - Tree #{TreeNumber}";
        public string DetailsText { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public Color StatusColor { get; set; } = Colors.Green;
        public bool HasMissingValues { get; set; }
    }

    public SummaryScreen(LocalDbService db)
    {
        InitializeComponent();
        _db = db;
        TreesCollectionView.ItemsSource = _treeItems;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // 1. Safety flush: If the cruiser left the final tree or final plot active in memory, flush it now
        if (DataService.CurrentTree.Dbh > 0 || !string.IsNullOrEmpty(DataService.CurrentTree.Species)) 
        {
            DataService.SaveTreeToPlot(); 
        }

        if (DataService.CurrentPlot.TreeList.Count > 0 || DataService.CurrentPlot.Slope > 0) 
        {
            DataService.SavePlotToStand(); 
        }

        // 2. Populate and audit all trees across the entire finished stand
        RefreshAuditView();
    }

    private async void RefreshAuditView()
    {
        _treeItems.Clear();

        var plots = DataService.CurrentStand.PlotList;
        if (plots == null || plots.Count == 0)
        {
            await DisplayAlert("Error", "No plots found in the current stand to save.", "OK");
            SaveStandSqlButton.IsEnabled = true;
            return;
        }
        var allTrees = new List<(int PlotNum, Tree Tree)>();

        foreach (var plot in plots)
        {
            if (plot.TreeList != null)
            {
                foreach (var tree in plot.TreeList)
                {
                    allTrees.Add((plot.PlotNumber, tree));
                }
            }
        }

        bool anyIncomplete = false;
        int totalDefects = 0;

        foreach (var item in allTrees)
        {
            var tree = item.Tree;
            totalDefects += tree.DefectList?.Count ?? 0;

            // Audit for missing/empty values
            var missingFields = new List<string>();
            if (string.IsNullOrWhiteSpace(tree.Species)) missingFields.Add("Species");
            if (tree.Dbh <= 0) missingFields.Add("DBH");
            if (tree.Height <= 0) missingFields.Add("Height");

            bool hasIssues = missingFields.Count > 0;
            if (hasIssues) anyIncomplete = true;

            string details = $"Species: {(string.IsNullOrEmpty(tree.Species) ? "None" : tree.Species)} | DBH: {tree.Dbh:F1}\" | Ht: {tree.Height:F0}' | Defects: {tree.DefectList?.Count ?? 0}";

            _treeItems.Add(new TreeValidationItem
            {
                PlotNumber = item.PlotNum,
                TreeNumber = tree.Id,
                DetailsText = details,
                HasMissingValues = hasIssues,
                StatusText = hasIssues ? $"Missing: {string.Join(", ", missingFields)}" : "Complete",
                StatusColor = hasIssues ? Colors.OrangeRed : Colors.Green
            });
        }

        DataQualityWarningLabel.IsVisible = anyIncomplete;

     

        StandTotalsLabel.Text = $"Totals: {plots.Count} Plots | {allTrees.Count} Trees | {totalDefects} Defects";
    }

    private async void OnSaveStandSqlClicked(object sender, EventArgs e)
    {
        // 1. Data check warnings
        if (_treeItems.Any(t => t.HasMissingValues))
        {
            bool proceedAnyway = await DisplayAlert(
                "Missing Data Warning",
                "Some trees have missing height, DBH, species, or distance values. Do you still want to upload?",
                "Upload Anyway",
                "Review");

            if (!proceedAnyway) return;
        }

        bool confirm = await DisplayAlert(
            "Final Stand Commit",
            "This will upload the entire stand, all plots, trees, and defects into SQLite. Proceed?",
            "Upload",
            "Cancel");

        if (!confirm) return;

        SaveStandSqlButton.IsEnabled = false;

        // 2. Commit the Stand hierarchy into the local SQLite database
        var currentStand = DataService.CurrentStand;
        var plots = currentStand.PlotList;

        bool success = await _db.SaveStandBatchAsync(currentStand);

        SaveStandSqlButton.IsEnabled = true;

        if (success)
        {
            // 3. Move stand into parcel history in memory
            DataService.SaveStandToParcel(); 

            await DisplayAlert(
                "Upload Successful",
                $"Stand saved to database with {plots.Count} plots and {_treeItems.Count} trees.",
                "OK");

            // 4. Return to the main menu/dashboard
            await Shell.Current.GoToAsync("//DataEntryScreen");
        }
        else
        {
            await DisplayAlert("Upload Failed", "An error occurred writing data to SQLite. Please try again.", "OK");
        }
    }
    private async void OnFinishedPlotClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Complete Plot",
            "Finish this plot and proceed to the next plot setup?",
            "Yes",
            "Cancel");

        if (!confirm) return;

        // 1. Flush any uncommitted tree data
        if (DataService.CurrentTree.Dbh > 0 || !string.IsNullOrEmpty(DataService.CurrentTree.Species))
        {
            DataService.SaveTreeToPlot();
        }

        // 2. Commit plot to stand
        DataService.SavePlotToStand();
        DataService.StartNewPlot();

        await DisplayAlert("Plot Added", "Plot saved to stand session.", "OK");

        // 3. Navigate to next plot coordinate setup
        await Shell.Current.GoToAsync("PlotCoordinateSet");
    }
}