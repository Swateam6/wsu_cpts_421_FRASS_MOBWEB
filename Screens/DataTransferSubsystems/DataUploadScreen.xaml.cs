using MOBWEB_TEST.sqllite;
using System.Net.Http.Headers;

namespace MOBWEB_TEST.Screens.DataTransferSubsystems;

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

    private readonly LocalDbService _dbService = new LocalDbService();
	private async void OnUploadDataClicked(object? sender, EventArgs e)
	{
        string dbPath = _dbService.GetCurrentDatabasePath();
        using var stream = File.OpenRead(dbPath);
        bool success = await UploadDatabaseFileAsync(stream,Path.GetFileName(dbPath), "https://yourserver.com/upload"); // placeholder
        if (success)
        {
            await DisplayAlert("Success", "Database uploaded successfully.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to upload database.", "OK");
        }
    }
    public async Task<bool> UploadDatabaseFileAsync(Stream fileStream, string fileName, string uploadUrl)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5); // db files can be large

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);

        streamContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/octet-stream");

        content.Add(streamContent, "file", fileName);

        var response = await client.PostAsync(uploadUrl, content);
        return response.IsSuccessStatusCode;
    }
}