namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

using Microsoft.Maui.Controls;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

public partial class DefectScreen : ContentPage
{
    private readonly ObservableCollection<Defects> _currentTreeDefects = new();

    public DefectScreen()
    {
        InitializeComponent();
        DefectsCollectionView.ItemsSource = _currentTreeDefects;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadTreePicker();
    }

    private void LoadTreePicker()
    {
        TreeNumberPicker.Items.Clear();

        if (DataService.CurrentPlot?.TreeList != null && DataService.CurrentPlot.TreeList.Count > 0)
        {
            foreach (var tree in DataService.CurrentPlot.TreeList)
            {
                TreeNumberPicker.Items.Add($"Tree #{tree.Id} ({tree.Species})");
            }
            TreeNumberPicker.SelectedIndex = 0;
        }
        else
        {
            TreeNumberPicker.Title = "No trees found in plot";
        }
    }

    private void OnTreeSelected(object sender, EventArgs e)
    {
        RefreshCurrentTreeDefects();
    }

    private void RefreshCurrentTreeDefects()
    {
        _currentTreeDefects.Clear();

        int index = TreeNumberPicker.SelectedIndex;
        if (index < 0 || DataService.CurrentPlot?.TreeList == null || index >= DataService.CurrentPlot.TreeList.Count)
            return;

        var selectedTree = DataService.CurrentPlot.TreeList[index];
        if (selectedTree?.DefectList != null)
        {
            foreach (var defect in selectedTree.DefectList)
            {
                _currentTreeDefects.Add(defect);
            }
        }
    }

    private async void OnAddDefectClicked(object sender, EventArgs e)
    {
        int index = TreeNumberPicker.SelectedIndex;
        if (index < 0 || DataService.CurrentPlot?.TreeList == null || index >= DataService.CurrentPlot.TreeList.Count)
        {
            await DisplayAlert("Validation", "Please select a tree.", "OK");
            return;
        }

        string defectType = DefectTypeEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(defectType))
        {
            await DisplayAlert("Validation", "Please enter a defect type.", "OK");
            DefectTypeEntry.Focus();
            return;
        }

        double.TryParse(DistanceEntry.Text?.Trim(), out double distance);
        double.TryParse(TopAngleEntry.Text?.Trim(), out double topAngle);
        double.TryParse(BottomAngleEntry.Text?.Trim(), out double bottomAngle);

        var selectedTree = DataService.CurrentPlot.TreeList[index];

        var newDefect = new Defects
        {
            ID = selectedTree.Id,
            Description = defectType,
            TopAngle = topAngle,
            bottomHeight = bottomAngle
        };

        if (selectedTree.DefectList == null)
        {
            selectedTree.DefectList = new ObservableCollection<Defects>();
        }

        selectedTree.DefectList.Add(newDefect);

        RefreshCurrentTreeDefects();

        DefectTypeEntry.Text = string.Empty;
        TopAngleEntry.Text = string.Empty;
        BottomAngleEntry.Text = string.Empty;
        DistanceEntry.Text = string.Empty;
    }

    private async void OnDoneRecordingClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SummaryScreen");
    }
}