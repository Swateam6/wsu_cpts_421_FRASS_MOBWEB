namespace MOBWEB_TEST.Screens;

public partial class LocationDemo : ContentPage
{
	public LocationDemo() 
	{
		InitializeComponent();
	}
    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}