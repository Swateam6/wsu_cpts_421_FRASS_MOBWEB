namespace MOBWEB_TEST.Screens;

public partial class DataEntryScreen : ContentPage
{
	public DataEntryScreen()
	{
		InitializeComponent();
	}
    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }

    private async void OnMesicSubsystemClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///MesicSubsystemScreen");
    }
    private async void OnPlotEntryClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///PlotEntryScreen");
    }
}