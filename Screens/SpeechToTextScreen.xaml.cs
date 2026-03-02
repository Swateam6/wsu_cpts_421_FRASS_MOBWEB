namespace MOBWEB_TEST.Screens;

public partial class SpeechToTextScreen : ContentPage
{
	public SpeechToTextScreen()
	{
		InitializeComponent();
	}

    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}