using System;
using System.Numerics;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems.GyroscopeSubsystem;

public partial class DefectScreen : ContentPage
{
    private DefectGyroscopeController _defectController;
	public DefectScreen()
	{
		InitializeComponent();
        _defectController = new DefectGyroscopeController(this);
	}
    public void UpdateCurrentAngle(string text)
    {
        CurrentAngleLabel.Text = text;
    }
    public void UpdateBaseLabel(Vector3? angle)
    {
        DefectBaseLabel.Text = angle.HasValue
            ? $"Base: X: {angle.Value.X:F1}°, Y: {angle.Value.Y:F1}°, Z: {angle.Value.Z:F1}°"
            : "Base: ---";
    }
    public void UpdateTopLabel(Vector3? angle)
    {
        DefectTopLabel.Text = angle.HasValue
            ? $"Top: X: {angle.Value.X:F1}°, Y: {angle.Value.Y:F1}°, Z: {angle.Value.Z:F1}°"
            : "Top: ---";
    }
    private void OnGyroMeasureClicked(object sender, EventArgs e)
    {
        _defectController.StartGyroscope();
    }
    private void OnZeroClicked(object sender, EventArgs e)
    {
        _defectController.ZeroGyroReadings();
    }
    private void OnCaptureBaseClicked(object sender, EventArgs e)
    {
        _defectController.CaptureBase();
    }
    private void OnCaptureTopClicked(object sender, EventArgs e)
    {
        _defectController.CaptureTop();
    }
    private async void OnReturnClicked(object? sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("///GyroscopeScreen");
    }
}