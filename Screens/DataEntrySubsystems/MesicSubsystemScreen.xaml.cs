namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class MesicSubsystemScreen : ContentPage
{
	public MesicSubsystemScreen()
	{
		InitializeComponent();
	}
    private async void OnBackToHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HomeScreen");
    }
}