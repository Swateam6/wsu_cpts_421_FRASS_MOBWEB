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
            var selectedDbPlot = _availablePlots[selectedIndex];

            // 1. In plot_data, the primary key identifier is 'Id'
            int targetPlotNumber = selectedDbPlot.Id;

            // 2. Ensure CurrentStand and its PlotList are initialized
            if (DataService.CurrentStand == null)
            {
                DataService.CurrentStand = new Models.Stand();
            }

            if (DataService.CurrentStand.PlotList == null)
            {
                DataService.CurrentStand.PlotList = new System.Collections.ObjectModel.ObservableCollection<Models.Plot>();
            }

            // 3. Look up if this plot is already tracked in the active stand
            var existingPlot = DataService.CurrentStand.PlotList.FirstOrDefault(p => p.PlotNumber == targetPlotNumber);

            if (existingPlot != null)
            {
                DataService.CurrentPlot = existingPlot;
            }
            else
            {
                // First time selecting this plot: create and track it in CurrentStand
                DataService.CurrentPlot = new Models.Plot
                {
                    PlotNumber = targetPlotNumber,
                    TreeList = new System.Collections.ObjectModel.ObservableCollection<Models.Tree>()
                };
                DataService.CurrentStand.PlotList.Add(DataService.CurrentPlot);
            }

            // 4. Map the SQLite database columns to the active plot
            DataService.CurrentStand.StandId = selectedDbPlot.ParentStandId;
            DataService.CurrentPlot.Latitude = selectedDbPlot.Latitude;
            DataService.CurrentPlot.Longitude = selectedDbPlot.Longitude;
            DataService.CurrentPlot.Slope = selectedDbPlot.Slope;
            DataService.CurrentPlot.Aspect = selectedDbPlot.Aspect;
            DataService.CurrentPlot.size = selectedDbPlot.size > 0 ? selectedDbPlot.size : 20.0;

            CoordinatesLabel.Text = $"Target: {selectedDbPlot.Latitude:F5}, {selectedDbPlot.Longitude:F5}";
            StatusLabel.Text = $"Plot #{targetPlotNumber} Locked from Database.";

            GoTothePlotCenter.IsEnabled = true;
        }
    }

    private async void OntoPlotNavClicked(object sender, EventArgs e)
    {
        // Navigate to the plot navigation screen
        await Shell.Current.GoToAsync("LocationDemo");
    }
}