using Microsoft.Maui.Controls;
using MOBWEB_TEST.Services;
using MOBWEB_TEST.sqllite;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MOBWEB_TEST.Screens;

public partial class PlotCoordinateSet : ContentPage
{
    private readonly LocalDbService _database;
    private List<plot_data> _availablePlots;
    private List<string> _plotDisplayNames;

    public PlotCoordinateSet(LocalDbService database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Fetch the Kamiak stand from SQLite
        var stands = await _database.GetAllStandDataAsync();
        var kamiak = stands.FirstOrDefault(s => s.HabitatType == "Kamiak Butte");

        // THE MISSING STEP: Actually grab the plots from the database!
        if (kamiak != null)
        {
            _availablePlots = await _database.GetPlotsForStandAsync(kamiak.Id);
        }
        else
        {
            // Fallback in case the seeder hasn't run properly yet
            _availablePlots = await _database.GetAllPlotDataAsync();
        }

        // 2. Now check if we have data to display
        if (_availablePlots != null && _availablePlots.Count > 0)
        {
            _plotDisplayNames = new List<string>();
            for (int i = 0; i < _availablePlots.Count; i++)
            {
                _plotDisplayNames.Add($"Plot {i + 1} ({_availablePlots[i].Latitude:F4}, {_availablePlots[i].Longitude:F4})");
            }

            PlotList.ItemsSource = _plotDisplayNames;
            StatusLabel.Text = $"Loaded {_availablePlots.Count} plots from database.";
        }
        else
        {
            StatusLabel.Text = "No plots found. Did you run the Kamiak Seeder?";
        }
    }

    private void OnPlotSelected(object sender, SelectionChangedEventArgs e)
    {
        string selectedString = e.CurrentSelection.FirstOrDefault() as string;

        if (selectedString != null && _availablePlots != null)
        {
            int selectedIndex = _plotDisplayNames.IndexOf(selectedString);
            var selectedPlot = _availablePlots[selectedIndex];

            // Update active memory for navigation
            if (DataService.CurrentPlot == null)
            {
                DataService.CurrentPlot = new Models.Plot();
            }
            CoordinatesLabel.Text = $"Target: {selectedPlot.Latitude:F5}, {selectedPlot.Longitude:F5}";
            StatusLabel.Text = "Plot Center Locked from Database.";

            DataService.CurrentPlot.PlotNumber = selectedPlot.Id;
            DataService.CurrentStand.StandId = selectedPlot.ParentStandId;

            DataService.CurrentPlot.Latitude = selectedPlot.Latitude;
            DataService.CurrentPlot.Longitude = selectedPlot.Longitude;

            // 2. Enable the "Go To It DAWG" button
            GoTothePlotCenter.IsEnabled = true;
        }
    }

    private async void OntoPlotNavClicked(object sender, EventArgs e)
    {
        // Navigate to the plot navigation screen
        await Shell.Current.GoToAsync("LocationDemo");
    }
}