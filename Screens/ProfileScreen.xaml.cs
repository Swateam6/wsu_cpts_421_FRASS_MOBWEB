namespace MOBWEB_TEST.Screens;

public partial class ProfileScreen : ContentPage
{
	public ProfileScreen()
	{
		InitializeComponent();
	}

    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}