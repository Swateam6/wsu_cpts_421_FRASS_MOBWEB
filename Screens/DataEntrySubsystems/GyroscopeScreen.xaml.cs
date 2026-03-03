using System.Numerics;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems;

public partial class GyroscopeScreen : ContentPage
{
    //accumulated angle instead of instantaneous angular velocity so its less noisy
    private bool _isFirstReading = true;
    private DateTime _lastReadingTime;
    private Vector3 _totalAngle; // in degrees

    // for zeroing/gyro reset
    private Vector3 _zeroOffset = Vector3.Zero;

    private Vector3? _baseAngle;
    private Vector3? _topAngle;
    public GyroscopeScreen()
	{
		InitializeComponent();
	}


    private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
        var currentTime = DateTime.UtcNow;

        //start timer
        if (_isFirstReading)
        {
            _lastReadingTime = currentTime;
            _isFirstReading = false;
            return;
        }

        // time delta
        double deltaSeconds = (currentTime - _lastReadingTime).TotalSeconds;

        // in degrees
        double degPerSecX = e.Reading.AngularVelocity.X * (180.0 / Math.PI);
        double degPerSecY = e.Reading.AngularVelocity.Y * (180.0 / Math.PI);
        double degPerSecZ = e.Reading.AngularVelocity.Z * (180.0 / Math.PI);

        // over time
        _totalAngle.X += (float)(degPerSecX * deltaSeconds);
        _totalAngle.Y += (float)(degPerSecY * deltaSeconds);
        _totalAngle.Z += (float)(degPerSecZ * deltaSeconds);

        _lastReadingTime = currentTime;

        // limited to one decimal place for readability
        Vector3 relativeAngle = _totalAngle - _zeroOffset;
        TakeMeasurement.TextColor = Colors.White;
        TakeMeasurement.Text = $"X: {relativeAngle.X:F1}°, Y: {relativeAngle.Y:F1}°, Z: {relativeAngle.Z:F1}°";
    }

    private async void OnGyroMeasureClicked(object? sender, EventArgs e)
	{
        if (Gyroscope.Default.IsSupported)
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                //reset all values
                _totalAngle = Vector3.Zero;
                _zeroOffset = Vector3.Zero;
                _isFirstReading = true;
                _baseAngle = null;
                _topAngle = null;
                BaseAngleLabel.Text = "Base: ---";
                TopAngleLabel.Text = "Top: ---";
                DifferenceLabel.Text = "";
                HeightResultLabel.Text = "Height: -- ft";

                //enable gyro, only uses battery during active measurement i think
                Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
                Gyroscope.Default.Start(SensorSpeed.UI);
            }
        }
    }

    private void OnCaptureBaseClicked(object? sender, EventArgs e)
    {
        if (Gyroscope.Default.IsMonitoring)
        {
            _baseAngle = _totalAngle- _zeroOffset; // copy current value
            BaseAngleLabel.Text = $"Base: X: {_baseAngle.Value.X:F1}°, Y: {_baseAngle.Value.Y:F1}°, Z: {_baseAngle.Value.Z:F1}°";
            UpdateDifference();
        }
    }

    private void OnCaptureTopClicked(object? sender, EventArgs e)
    {
        if (Gyroscope.Default.IsMonitoring)
        {
            _topAngle = _totalAngle-_zeroOffset; // copy current value
            TopAngleLabel.Text = $"Top: X: {_topAngle.Value.X:F1}°, Y: {_topAngle.Value.Y:F1}°, Z: {_topAngle.Value.Z:F1}°";
            UpdateDifference();
        }
    }

    private void UpdateDifference()
    {
        if (_baseAngle.HasValue && _topAngle.HasValue)
        {
            Vector3 diff = _topAngle.Value - _baseAngle.Value;
            DifferenceLabel.Text = $"Difference: X: {diff.X:F1}°, Y: {diff.Y:F1}°, Z: {diff.Z:F1}°";
        }
        else
        {
            DifferenceLabel.Text = "";
        }
    }

    private void OnCalculateHeightClicked(object sender, EventArgs e)
    {
        //sanity check for inputs
        if (!_baseAngle.HasValue || !_topAngle.HasValue)
        {
            HeightResultLabel.Text = "Please capture both base and top angles first.";
            return;
        }

        if (!double.TryParse(DistanceEntry.Text, out double distance) || distance <= 0)
        {
            HeightResultLabel.Text = "Enter a valid positive distance.";
            return;
        }

        //treating x axis as is pitch, adjust w/ bill's feedback
        double deltaDegrees = _topAngle.Value.X - _baseAngle.Value.X;
        double deltaRadians = deltaDegrees * Math.PI / 180.0;

        double height = distance * Math.Tan(deltaRadians);
        HeightResultLabel.Text = $"Height: {height:F2} ft";
    }

    private void OnZeroClicked(object sender, EventArgs e)
    {
        if (Gyroscope.Default.IsMonitoring)
        {
            //set current angle as new zero
            _zeroOffset = _totalAngle;

            // reset
            _baseAngle = null;
            _topAngle = null;
            BaseAngleLabel.Text = "Base: ---";
            TopAngleLabel.Text = "Top: ---";
            DifferenceLabel.Text = "";
            HeightResultLabel.Text = "Height: -- ft";
        }
    }
}