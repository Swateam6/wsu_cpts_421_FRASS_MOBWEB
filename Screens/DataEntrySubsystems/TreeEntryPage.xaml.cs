namespace MOBWEB_TEST.Screens.DataEntrySubsystems;
using MOBWEB_TEST.Models;
using MOBWEB_TEST.Services;

public partial class TreeEntryPage : ContentPage
{
    public TreeEntryPage()
    {
        InitializeComponent();
    }

    private async void OnDefectClicked(object sender, EventArgs e)
    {
        DataService.CurrentTree.Species = SpeciesEntry.Text;
        if (double.TryParse(DbhEntry.Text, out double dBH))
        {
            DataService.CurrentTree.Dbh = dBH;
        }

        // Saves the tree and resets the slate (DataService handles the 'new Tree()' now)
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;
        SpeciesEntry.Focus();
        await Shell.Current.GoToAsync("DefectScreen");
    }
}