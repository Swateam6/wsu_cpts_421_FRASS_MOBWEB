namespace MOBWEB_TEST.Screens;

public partial class DataUploadScreen : ContentPage
{
	public DataUploadScreen()
	{
		InitializeComponent();
	}

	private async void OnBackToHomeClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///HomeScreen");
    }
}