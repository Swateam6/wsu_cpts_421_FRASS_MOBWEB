namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class PlotEntryScreen : ContentPage
{
	public PlotEntryScreen()
	{
		InitializeComponent();
	}
    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}