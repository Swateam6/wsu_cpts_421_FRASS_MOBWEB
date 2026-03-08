using System.Numerics;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class GyroscopeScreen : ContentPage
{
    private GyroscopeSubsystem.GyroscopeController _controller;

    public GyroscopeScreen()
    {
        InitializeComponent();
        _controller = new GyroscopeSubsystem.GyroscopeController(this);
    }

    public void UpdateCurrentAngle(string text)
    {
        CurrentAngleLabel.Text = text;
    }
    public void UpdateBaseLabel(Vector3? angle)
    {
        BaseAngleLabel.Text = angle.HasValue
            ? $"Base: X: {angle.Value.X:F1}°, Y: {angle.Value.Y:F1}°, Z: {angle.Value.Z:F1}°"
            : "Base: ---";
    }
    public void UpdateLiveCrownBase(Vector3? angle)
    {
        LiveCrownBaseLabel.Text = angle.HasValue
            ? $"Live Crown Base: X: {angle.Value.X:F1}°, Y: {angle.Value.Y:F1}°, Z: {angle.Value.Z:F1}°"
            : "Live Crown Base: ---";
    }
    public void UpdateTopLabel(Vector3? angle)
    {
        TopAngleLabel.Text = angle.HasValue
            ? $"Top: X: {angle.Value.X:F1}°, Y: {angle.Value.Y:F1}°, Z: {angle.Value.Z:F1}°"
            : "Top: ---";
    }
    public void UpdateDifference(Vector3? diff)
    {
        DifferenceLabel.Text = diff.HasValue
            ? $"Difference: X: {diff.Value.X:F1}°, Y: {diff.Value.Y:F1}°, Z: {diff.Value.Z:F1}°"
            : "";
    }
    public void UpdateHeightResult(string text)
    {
        HeightResultLabel.Text = text;
    }
    public void UpdateLiveCrownRatioResult(string text)
    {
        LiveCrownRatioLabel.Text = text;
    }
    public void UpdateLiveCrownHeightResult(string text)
    {
        BaseOfLiveCrownLabel.Text = text;
    }   



    // Forward button presses to controller
    private void OnGyroMeasureClicked(object sender, EventArgs e)
    {
        _controller.StartGyroscope();
    }

    private void OnZeroClicked(object sender, EventArgs e)
    {
        _controller.ZeroGyroReadings();
    }

    private void OnCaptureBaseClicked(object sender, EventArgs e)
    {
        _controller.CaptureBase();
    }
    private void OnCaptureLiveCrownBaseClicked(object sender, EventArgs e)
    {
        _controller.CaptureLiveCrownBase();
    }
    private void OnCaptureTopClicked(object sender, EventArgs e)
    {
        _controller.CaptureTop();
    }

    private void OnCalculateHeightClicked(object sender, EventArgs e)
    {
        _controller.CalculateHeight(DistanceEntry.Text);
    }
    private void OnCalculateLiveCrownRatioClicked(object sender, EventArgs e)
    {
        _controller.CalculateLiveCrownRatio(DistanceEntry.Text);
    }
    // boilerplate to kill gyro when leaving page
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _controller.StopGyroscope();
    }
}