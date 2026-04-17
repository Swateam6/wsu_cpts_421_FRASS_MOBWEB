namespace MOBWEB_TEST.Screens.DataEntrySubsystems;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;

public partial class TreeEntryPage : ContentPage
{
    public TreeEntryPage()
    {
        InitializeComponent();
    }

    private void OnNextTreeClicked(object sender, EventArgs e)
    {
        DataService.CurrentTree.Species = SpeciesEntry.Text;
        if (double.TryParse(DbhEntry.Text, out double dBH))
        {
            DataService.CurrentTree.Dbh = dBH;
        }

        // Saves the tree and resets the slate (DataService handles the 'new Tree()' now)
        DataService.SaveTreeToPlot();

        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;
        SpeciesEntry.Focus();
    }

    private async void OnNextPlotClicked(object sender, EventArgs e)
    {
        DataService.CurrentTree.Species = SpeciesEntry.Text;
        if (double.TryParse(DbhEntry.Text, out double dBH))
        {
            DataService.CurrentTree.Dbh = dBH;
        }
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;

        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();

        // FIXED 1 (Infinite Stack): Pops this screen off the stack to reveal the existing Plot screen
        await Shell.Current.GoToAsync("..");
    }
    private async void OnDefectClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("DefectScreen");
    }
}