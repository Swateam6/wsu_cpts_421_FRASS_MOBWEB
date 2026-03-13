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
        if (double.TryParse(DbhEntry.Text, out double dBH ))
        {
            DataService.CurrentTree.Dbh = dBH;
        }
        DataService.SaveTreeToPlot();
        DataService.CurrentTree = new Tree();
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;

        SpeciesEntry.Focus();

    }

    // XAML is looking for this too!
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

        await Shell.Current.GoToAsync("PlotEntryScreen");
    }
    private async void OnFinishStandClicked(object sender, EventArgs e)
    {
        DataService.CurrentTree.Species = SpeciesEntry.Text;
        if (double.TryParse(DbhEntry.Text, out double dBH))
        {
            DataService.CurrentTree.Dbh = dBH;
        }
        SpeciesEntry.Text = string.Empty;
        DbhEntry.Text = string.Empty;

        

        // 3. IMPORTANT: Reset the 'Current' objects so the next Stand is a clean slate

        DataService.SaveTreeToPlot();
        DataService.SavePlotToStand();

        DataService.CurrentStand = new Stand();
        await Shell.Current.GoToAsync("///DataEntryScreen");
    }
}