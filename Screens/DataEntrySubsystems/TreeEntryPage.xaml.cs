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
        DataService.CurrentPlot.TreeList.Add(DataService.CurrentTree);

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

        DataService.CurrentPlot.TreeList.Add(DataService.CurrentTree);
        DataService.CurrentStand.PlotList.Add(DataService.CurrentPlot);
        DataService.CurrentTree = new Tree();



        DataService.CurrentPlot = new Plot();
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

        DataService.CurrentPlot.TreeList.Add(DataService.CurrentTree);
        DataService.CurrentStand.PlotList.Add(DataService.CurrentPlot);
        DataService.CurrentTree = new Tree();
        DataService.CurrentPlot = new Plot();
        DataService.CurrentStand = new Stand();
        await Shell.Current.GoToAsync("DataEntryScreen");
    }
}