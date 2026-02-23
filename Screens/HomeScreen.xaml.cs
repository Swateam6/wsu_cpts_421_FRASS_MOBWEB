using System.Runtime.CompilerServices;

namespace MOBWEB_TEST.Screens;

public partial class HomeScreen : ContentPage
{
	public HomeScreen()
	{
		InitializeComponent();
	}
	private async void OnLogoutClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///EntryScreen");
	}

	private async void OnSubmitClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///DataUploadScreen");
    }

	private async void OnProfileClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///ProfileScreen");
    }

	private async void OnDataEntryClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///DataEntryScreen");
    }
}