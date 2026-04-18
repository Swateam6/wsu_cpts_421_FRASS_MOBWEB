using System;
using System.Numerics;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using MOBWEB_TEST.Services;
using MOBWEB_TEST.Screens.DataEntrySubsystems.GyroscopeSubsystem;
using IntentsUI;
using System.Threading.Tasks;

namespace MOBWEB_TEST.Screens;

public partial class PlotSlopeScreen : ContentPage
{
    private readonly GyroscopeModel _model;
    private bool _isFirstReading = true;
    private DateTime _lastReadingTime;

    public PlotSlopeScreen()
    {
        InitializeComponent();
        _model = new GyroscopeModel();
    }

    // START SENSOR
    private void OnStartClicked(object sender, EventArgs e)
    {
        if (!Gyroscope.Default.IsSupported)
        {
            SlopeLabel.Text = "Gyro not supported";
            return;
        }

        if (Gyroscope.Default.IsMonitoring)
            return;

        _model.InitVectors();
        _model.ClearAll();
        _isFirstReading = true;

        AngleLabel.Text = "Measuring...";
        BaseLabel.Text = "Captured Angle: --";
        SlopeLabel.Text = "Slope: -- %";

        Gyroscope.Default.ReadingChanged += OnGyroscopeReadingChanged;
        Gyroscope.Default.Start(SensorSpeed.UI);
    }

    // STOP SENSOR
    private void OnStopClicked(object sender, EventArgs e)
    {
        if (!Gyroscope.Default.IsMonitoring)
            return;

        Gyroscope.Default.ReadingChanged -= OnGyroscopeReadingChanged;
        Gyroscope.Default.Stop();

        AngleLabel.Text = "Stopped";
    }

    // GYRO UPDATE
    private void OnGyroscopeReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
        var currentTime = DateTime.UtcNow;

        if (_isFirstReading)
        {
            _lastReadingTime = currentTime;
            _isFirstReading = false;
            return;
        }

        double deltaSeconds = (currentTime - _lastReadingTime).TotalSeconds;
        _lastReadingTime = currentTime;

        _model.UpdateAngle((float)deltaSeconds, e.Reading.AngularVelocity);

        Vector3 relative = _model.GetRelativeAngle();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            AngleLabel.Text = $"X: {relative.X:F1}°, Y: {relative.Y:F1}°, Z: {relative.Z:F1}°";
        });
    }

    // ZERO GYRO
    private void OnZeroClicked(object sender, EventArgs e)
    {
        if (!Gyroscope.Default.IsMonitoring)
            return;

        _model.ZeroGyro();

        AngleLabel.Text = "Zeroed";
        BaseLabel.Text = "Captured Angle: --";
        SlopeLabel.Text = "Slope: -- %";
    }

    // CAPTURE BASE ANGLE
    private void OnCaptureBaseClicked(object sender, EventArgs e)
    {
        if (!Gyroscope.Default.IsMonitoring)
            return;

        _model.CaptureBase();

        if (_model.BaseAngle.HasValue)
        {
            double angle = _model.BaseAngle.Value.X;
            BaseLabel.Text = $"Captured Angle: {angle:F2}°";
        }
    }

    // CALCULATE SLOPE
    private void OnCalculateSlopeClicked(object sender, EventArgs e)
    {
        if (!_model.BaseAngle.HasValue)
        {
            SlopeLabel.Text = "Capture angle first";
            return;
        }

        try
        {
            double slope = _model.CalculateSlopePercent();

            SlopeLabel.Text = $"Slope: {slope:F2}%";

            // Save to plot
            if (DataService.CurrentPlot != null)
            {
                DataService.CurrentPlot.Slope = slope;
            }
        }
        catch (Exception ex)
        {
            SlopeLabel.Text = $"Error: {ex.Message}";
        }
    }
    private async void OnStartNavClicked(object sender,EventArgs E)
    {
        await Shell.Current.GoToAsync("NavigationScreen");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (Gyroscope.Default.IsMonitoring)
        {
            Gyroscope.Default.Stop();
            Gyroscope.Default.ReadingChanged -= OnGyroscopeReadingChanged;
        }
    }
   
}