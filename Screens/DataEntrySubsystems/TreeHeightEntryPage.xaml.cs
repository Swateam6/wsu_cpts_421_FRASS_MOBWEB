namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class TreeHeightEntryPage : ContentPage
{
	public TreeHeightEntryPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
		TreeNumberPicker
    }
	private async void OnTreeNumberSelected(object sender, EventArgs e)
	{

	}
}