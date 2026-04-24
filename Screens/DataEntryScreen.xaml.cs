namespace MOBWEB_TEST.Screens;

public partial class DataEntryScreen : ContentPage
{
    public DataEntryScreen()
    {
        InitializeComponent();
    }

    private async void OnBackToHomeClicked(object sender, EventArgs e)
    {
        // KEEP the slashes here, because Home is in your AppShell.xaml main menu
        await Shell.Current.GoToAsync("///HomeScreen");
    }

    private async void OnMesicSubsystemClicked(object sender, EventArgs e)
    {
        // REMOVE slashes. This pushes the hidden sub-page.
        await Shell.Current.GoToAsync("MesicSubsystemScreen");
    }

    private async void OnStandEntryClicked(object sender, EventArgs e)
    {
        // REMOVE slashes
        await Shell.Current.GoToAsync("StandEntryData");
    }

    private async void OnPlotEntryClicked(object sender, EventArgs e)
    {
        // REMOVE slashes
        await Shell.Current.GoToAsync("PlotEntryScreen");
    }

    private async void OnTreeEntryClicked(object sender, EventArgs e)
    {
        // REMOVE slashes
        await Shell.Current.GoToAsync("TreeEntryPage");
    }

    private async void OnGyroscopeClicked(object? sender, EventArgs e)
    {
        // REMOVE slashes
        await Shell.Current.GoToAsync("GyroscopeScreen");
    }

}