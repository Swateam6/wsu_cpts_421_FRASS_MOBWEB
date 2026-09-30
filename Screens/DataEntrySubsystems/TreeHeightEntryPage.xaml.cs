using MOBWEB_TEST.Services;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class TreeHeightEntryPage : ContentPage
{
    private double TopAngle;
    private double bottomAngle;
    private double crownTopAngle;
    private double calculatedHeight;
    private double crownRad;
    private double crownHeight;
    private double crownRatio;

    public TreeHeightEntryPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PopulatePicker();
    }

    private void PopulatePicker()
    {
        TreeNumberPicker.Items.Clear();

        if (DataService.CurrentPlot?.TreeList == null || DataService.CurrentPlot.TreeList.Count == 0)
        {
            TreeNumberPicker.Title = "No trees found in this plot";
            return;
        }

        for (int i = 0; i < DataService.CurrentPlot.TreeList.Count; i++)
        {
            var tree = DataService.CurrentPlot.TreeList[i];
            TreeNumberPicker.Items.Add($"Tree #{tree.Id} - {tree.Species} ({tree.Dbh}\")");
        }

        // Auto-select the last added tree
        TreeNumberPicker.SelectedIndex = DataService.CurrentPlot.TreeList.Count - 1;
    }

    private void OnTreeNumberSelected(object sender, EventArgs e)
    {
        int index = TreeNumberPicker.SelectedIndex;
        if (index < 0 || DataService.CurrentPlot?.TreeList == null || index >= DataService.CurrentPlot.TreeList.Count)
            return;

        var selectedTree = DataService.CurrentPlot.TreeList[index];
    }

    private async void OnCalculateHeightClicked(object sender, EventArgs e)
    {
        if (!double.TryParse(TopAngleEntry.Text?.Trim(), out TopAngle))
        {
            await DisplayAlert("Validation", "Please enter a valid top angle.", "OK");
            TopAngleEntry.Focus();
            return;
        }
        if (!double.TryParse(BottomAngleEntry.Text?.Trim(), out bottomAngle))
        {
            await DisplayAlert("Validation", "Please enter a valid bottom angle.", "OK");
            BottomAngleEntry.Focus();
            return;
        }
        if (!double.TryParse(DistanceEntry.Text?.Trim(), out double distance) || distance <= 0)
        {
            await DisplayAlert("Validation", "Please enter a valid distance.", "OK");
            DistanceEntry.Focus();
            return;
        }
        if (!double.TryParse(CrownAngleEntry.Text?.Trim(), out crownTopAngle))
        {
            await DisplayAlert("Validation", "Please enter a valid crown base angle.", "OK");
            CrownAngleEntry.Focus();
            return;
        }

        double topRad = TopAngle * (Math.PI / 180.0);
        double bottomRad = bottomAngle * (Math.PI / 180.0);
        crownRad = crownTopAngle * (Math.PI / 180.0);

        calculatedHeight = distance * (Math.Tan(topRad) - Math.Tan(bottomRad));
        crownHeight = distance * (Math.Tan(crownRad) - Math.Tan(bottomRad));
        
        if (calculatedHeight < 0) calculatedHeight = 0;
        crownRatio = calculatedHeight > 0 ? (crownHeight / calculatedHeight) : 0;

        CalculatedHeightLabel.Text = $"Calculated Height: {calculatedHeight:F1} ft";
        CalculatedCrownLabel.Text = $"Live Crown: {crownHeight:F1} ft, Ratio: {crownRatio * 100:F0}%";
    }

    private async void OnSaveHeightClicked(object sender, EventArgs e)
    {
        int index = TreeNumberPicker.SelectedIndex;
        if (index < 0 || DataService.CurrentPlot?.TreeList == null || index >= DataService.CurrentPlot.TreeList.Count)
        {
            await DisplayAlert("Select Tree", "Please select a tree to log height for.", "OK");
            return;
        }

        // Apply height values directly to the tree inside the plot list
        var selectedTree = DataService.CurrentPlot.TreeList[index];
        selectedTree.Height = calculatedHeight;
        selectedTree.LiveCrownHeight = crownHeight;
        selectedTree.CrownRatioPercent = crownRatio * 100;

        await DisplayAlert("Saved", $"Height of {calculatedHeight:F1}' assigned to Tree #{selectedTree.Id}.", "OK");

    }
    private async void OnDefectClickClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("DefectScreen");
    }
}