using Microsoft.Maui.Controls;
using MOBWEB_TEST.Services;
using MOBWEB_TEST.sqllite;
using System;
using System.Collections.Generic;

namespace MOBWEB_TEST.Screens;

public partial class PlotCoordinateSet : ContentPage
{
    private readonly LocalDbService _database;
    private List<plot_data> _availablePlots;

    // Inject the DB Service into the constructor
    public PlotCoordinateSet(LocalDbService database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Fetch the 78 Kamiak plots from the SQLite Database
        // Note: You can filter this by DataService.CurrentStand.Id if you want!
        _availablePlots = await _database.GetAllPlotDataAsync();

        // 2. Populate the Picker UI
        if (_availablePlots != null && _availablePlots.Count > 0)
        {
            var plotNames = new List<string>();
            for (int i = 0; i < _availablePlots.Count; i++)
            {
                // Formats it cleanly like: "Plot 1 (46.8660, -117.1695)"
                plotNames.Add($"Plot {i + 1} ({_availablePlots[i].Latitude:F4}, {_availablePlots[i].Longitude:F4})");
            }

            PlotPicker.ItemsSource = plotNames;
            StatusLabel.Text = $"Loaded {_availablePlots.Count} plots from database.";
        }
        else
        {
            StatusLabel.Text = "No plots found in database. Did you run the Kamiak Seeder?";
        }
    }

    private void OnPlotSelected(object sender, EventArgs e)
    {
        int selectedIndex = PlotPicker.SelectedIndex;

        if (selectedIndex != -1 && _availablePlots != null)
        {
            // 1. Get the actual SQL plot object from the hidden list
            var selectedPlot = _availablePlots[selectedIndex];

            // 2. Assign the DB coordinates to your active UI DataService
            if (DataService.CurrentPlot == null)
            {
                DataService.CurrentPlot = new Models.Plot();
            }

            DataService.CurrentPlot.Latitude = selectedPlot.Latitude;
            DataService.CurrentPlot.Longitude = selectedPlot.Longitude;

            // 3. Update the UI to show they are locked in
            CoordinatesLabel.Text = $"Target: {selectedPlot.Latitude:F5}, {selectedPlot.Longitude:F5}";
            StatusLabel.Text = "Plot Center Locked from Database.";

            // 4. Enable the Log Tree button!
            LogTreeButton.IsEnabled = true;
        }
    }

    private async void OnLogTreeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("TreeEntryPage");
    }
}