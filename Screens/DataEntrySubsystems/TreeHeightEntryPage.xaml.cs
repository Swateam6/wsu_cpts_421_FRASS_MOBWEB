using MOBWEB_TEST.Services;

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
		TreeNumberPicker.Items.Clear();
		for(int i=0;i<DataService.CurrentPlot.TreeList.Count;i++)
		{
			TreeNumberPicker.Items.Add($"Tree {i + 1} - {DataService.CurrentPlot.TreeList[i].Species} ({DataService.CurrentPlot.TreeList[i].Dbh}\")");
        }
    }
	private async void OnTreeNumberSelected(object sender, EventArgs e)
	{
		int index = TreeNumberPicker.SelectedIndex;
		var selectedTree = DataService.CurrentPlot.TreeList[index];
        HeightEntry.Text = selectedTree.Height > 0 ? selectedTree.Height.ToString() : string.Empty;
        HeightEntry.Focus();
    }
	private async void OnSaveHeightClicked(object sender, EventArgs e)
	{
        int index = TreeNumberPicker.SelectedIndex;
        if (index < 0 || index >= DataService.CurrentPlot.TreeList.Count)
            return;
        var selectedTree = DataService.CurrentPlot.TreeList[index];
        if (double.TryParse(HeightEntry.Text, out double height))
        {
            if(height <0)
            {
                await DisplayAlert("Invalid Input","Please Enter a valid height value","OK");
                HeightEntry.Text = string.Empty;
                selectedTree.Height = height;
            }
            selectedTree.Height = height;
            DataService.CurrentTree = selectedTree;

        }
    }
}