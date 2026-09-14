namespace MOBWEB_TEST.Screens.DataTransferSubsystems;

public partial class DataOverviewScreen : ContentPage
{
	public DataOverviewScreen()
	{
		InitializeComponent();
	}

    private async void OnUploadDataClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("DataUploadScreen");
    }

    private async void OnDownloadDataClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("DataDownloadScreen");
    }

    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}