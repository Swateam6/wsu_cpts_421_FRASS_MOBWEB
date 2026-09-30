namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;
using MOBWEB_TEST.sqllite;
using System.Collections.ObjectModel;

public partial class StandEntryData : ContentPage
{
    private readonly LocalDbService _database;
    private List<stand_data> _availableStands = new();

    public StandEntryData(LocalDbService database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Fetch stands from SQLite
        var dbStands = await _database.GetAllStandDataAsync();

        if (dbStands != null && dbStands.Count > 0)
        {
            _availableStands = dbStands;
            StandPicker.ItemsSource = _availableStands
                .Select(s => $"Stand #{s.Id} - {s.HabitatType}")
                .ToList();

            // Default to the first stand or match CurrentStand if already chosen
            if (DataService.CurrentStand != null && DataService.CurrentStand.StandId > 0)
            {
                int matchIndex = _availableStands.FindIndex(s => s.Id == DataService.CurrentStand.StandId);
                StandPicker.SelectedIndex = matchIndex != -1 ? matchIndex : 0;
            }
            else
            {
                StandPicker.SelectedIndex = 0;
            }
        }
        else
        {
            await DisplayAlert("Notice", "No stands found in database. Please run the seeder.", "OK");
        }
    }

    private void OnStandPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (StandPicker.SelectedIndex < 0 || StandPicker.SelectedIndex >= _availableStands.Count)
            return;

        var selectedDbStand = _availableStands[StandPicker.SelectedIndex];

        // Ensure CurrentStand points to this existing database stand
        if (DataService.CurrentStand == null)
        {
            DataService.CurrentStand = new Stand();
        }

        DataService.CurrentStand.StandId = selectedDbStand.Id;

        // Ensure PlotList is initialized and ready
        if (DataService.CurrentStand.PlotList == null)
        {
            DataService.CurrentStand.PlotList = new ObservableCollection<Plot>();
        }
    }

    private void OnPlotTypeSelectedIndexChanged(object sender, EventArgs e)
    {
        if (PlotTypePicker.SelectedIndex == 0) // Variable Radius (Prism)
        {
            VariableSection.IsVisible = true;
            FixedSection.IsVisible = false;
        }
        else if (PlotTypePicker.SelectedIndex == 1) // Fixed Radius
        {
            VariableSection.IsVisible = false;
            FixedSection.IsVisible = true;
        }
    }

    private async void OnStartPlottingClicked(object sender, EventArgs e)
    {
        if (DataService.CurrentStand == null)
        {
            await DisplayAlert("Error", "Please select a stand before continuing.", "OK");
            return;
        }

        // 1. Parse Acres
        if (double.TryParse(AcresEntry.Text, out double acres))
        {
            DataService.CurrentStand.Acres = acres;
        }

        // 2. Set cruising method
        if (PlotTypePicker.SelectedIndex == 0) // Variable Radius (Prism)
        {
            DataService.CurrentStand.IsFixedPlot = false;

            if (BafPicker.SelectedIndex != -1 && double.TryParse(BafPicker.SelectedItem.ToString(), out double selectedBaf))
            {
                if (DataService.CurrentPlot == null)
                {
                    DataService.CurrentPlot = new Plot();
                }
                DataService.CurrentPlot.size = selectedBaf;
            }
        }
        else if (PlotTypePicker.SelectedIndex == 1) // Fixed Radius
        {
            DataService.CurrentStand.IsFixedPlot = true;

            if (double.TryParse(RadiusEntry.Text, out double parsedRadius))
            {
                DataService.CurrentStand.plotSize = parsedRadius;
            }
        }

        // 3. Navigate to plot setup
        await Shell.Current.GoToAsync("PlotCoordinateSet");
    }
}