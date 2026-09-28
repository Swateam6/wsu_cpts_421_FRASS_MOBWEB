using MOBWEB_TEST.sqllite;
using System.Net.Http.Headers;

namespace MOBWEB_TEST.Screens.DataTransferSubsystems;

public partial class DataUploadScreen : ContentPage
{
    private readonly LocalDbService _dbService = new LocalDbService();

    public DataUploadScreen()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await PrintDatabaseContents();
    }
    private async void OnBackToHubClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("DataOverviewScreen");
    }
    private async void OnUploadDataClicked(object? sender, EventArgs e)
	{
        string dbPath = _dbService.GetCurrentDatabasePath();
        using var stream = File.OpenRead(dbPath);
        bool success = await UploadDatabaseFileAsync(stream,Path.GetFileName(dbPath), "https://frass.forest-econometrics.com/App_Data/MobileUploads"); 
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


    private async Task PrintDatabaseContents()
    {
        try
        {
            var trees = await _dbService.GetAllTreeDataAsync();
            var plots = await _dbService.GetAllPlotDataAsync();
            var stands = await _dbService.GetAllStandDataAsync();
            var parcels = await _dbService.GetAllParcelDataAsync();
            var users = await _dbService.GetAllUserDataAsync();
            var defects = await _dbService.GetAllDefectDataAsync();

            var output = "═══════════════════════════════════════════\n";
            output += "DATABASE CONTENTS\n";
            output += "═══════════════════════════════════════════\n\n";

            output += $"── user_data ({users.Count} records) ──\n";
            foreach (var u in users)
                output += $"  ID:{u.Id}\n";

            output += $"\n── parcel_data ({parcels.Count} records) ──\n";
            foreach (var p in parcels)
                output += $"  ID:{p.Id} | Acres:{p.Acres}ac\n";

            output += $"\n── stand_data ({stands.Count} records) ──\n";
            foreach (var s in stands)
                output += $"  ID:{s.Id} | ParcelFK:{s.ParcelID} | {s.FvsVariant} | SI:{s.SiteIndex} | {s.Acres}ac\n";

            output += $"\n── plot_data ({plots.Count} records) ──\n";
            foreach (var p in plots)
                output += $"  ID:{p.Id} | StandFK:{p.ParentStandId} | ({p.Latitude:F4},{p.Longitude:F4}) | Elev:{p.Elevation}ft\n";

            output += $"\n── tree_data ({trees.Count} records) ──\n";
            foreach (var t in trees)
                output += $"  ID:{t.Id} | PlotFK:{t.parentPlotId} | Species:{t.Species} | DBH:{t.DiameterBreastHeight} | Height: {t.Height}\n";

            output += $"\n── defect_data ({defects.Count} records) ──\n";
            foreach (var d in defects)
            {
                output += $"  ID:{d.Id} | TreeFK:{d.parentTreeId} | {d.Description} | Ht:{d.CalculatedHeight:F1}ft | (B:{d.BaseAngle}°, T:{d.TopAngle}°)\n";
            }

            OutputLabel.Text = output;
        }
        catch (Exception ex)
        {
            OutputLabel.Text = $"Failed to load database contents:\n{ex.Message}";
        }

    }
}