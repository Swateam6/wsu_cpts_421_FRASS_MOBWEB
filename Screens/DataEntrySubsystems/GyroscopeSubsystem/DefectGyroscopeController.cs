using System;
using System.Numerics;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems.GyroscopeSubsystem
{

    public class DefectGyroscopeController
    {
        private GyroscopeModel _model;
        private DefectScreen _view;
        private bool _isFirstReading = true;
        private DateTime _lastReadingTime;

        public DefectGyroscopeController(DefectScreen view)
        {
            _model = new GyroscopeModel();
            _view = view;
        }

        public void StartGyroscope()
        {
            // check if gyro enabled and off
            if (!Gyroscope.Default.IsMonitoring || !Gyroscope.Default.IsSupported)
            {
                // reset on init
                _model.InitVectors();
                _model.ClearAll();
                _isFirstReading = true;

                // reset view
                _view.UpdateCurrentAngle("Not started");
                _view.UpdateBaseLabel(null);
                _view.UpdateTopLabel(null);

                // start reading
                Gyroscope.Default.ReadingChanged += OnGyroscopeReadingChanged;
                Gyroscope.Default.Start(SensorSpeed.UI);
            }
        }
        public void StopGyroscope()
        {
            if (Gyroscope.Default.IsMonitoring)
            {
                Gyroscope.Default.ReadingChanged -= OnGyroscopeReadingChanged;
                Gyroscope.Default.Stop();
            }
        }
        private void OnGyroscopeReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var currentTime = DateTime.UtcNow;

            // time delta for integration
            if (_isFirstReading)
            {
                _lastReadingTime = currentTime;
                _isFirstReading = false;
                return;
            }

            double deltaSeconds = (currentTime - _lastReadingTime).TotalSeconds;
            _lastReadingTime = currentTime;

            _model.UpdateAngle((float)deltaSeconds, e.Reading.AngularVelocity);

            // update display w/ new angle, to one decimal
            Vector3 relative = _model.GetRelativeAngle();
            _view.UpdateCurrentAngle($"X: {relative.X:F1}°, Y: {relative.Y:F1}°, Z: {relative.Z:F1}°");
        }

        public void ZeroGyroReadings()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }

            // reset gyro to current angle, and clear base/top
            _model.ZeroGyro();
            _view.UpdateBaseLabel(null);
            _view.UpdateTopLabel(null);
        }
        public void CaptureBase()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }
            _model.CaptureBase();
            _view.UpdateBaseLabel(_model.BaseAngle);
        }
        public void CaptureTop()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }
            _model.CaptureTop();
            _view.UpdateTopLabel(_model.TopAngle);
        }

        public void SaveDefect(string distanceText, string descriptionText)
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }

            // 1. Validate the distance input 
            if (!double.TryParse(distanceText, out double distance) || distance <= 0)
            {
                _view.UpdateDescription("Enter a valid positive distance.");
                return;
            }

            // 2. Safely grab the angles (Default to 0 if null)
            double baseAngleDegrees = _model.BaseAngle?.X ?? 0;
            double topAngleDegrees = _model.TopAngle?.X ?? 0;

            // 3. Convert degrees to Radians for C# Math library
            double baseAngleRad = baseAngleDegrees * (Math.PI / 180.0);
            double topAngleRad = topAngleDegrees * (Math.PI / 180.0);

            // 4. Calculate heights using Trigonometry
            // If baseAngle is negative (looking down), Math.Tan will be negative, 
            // which accurately represents depth below eye level.
            double bottomHeight = distance * Math.Tan(baseAngleRad);
            double topHeight = distance * Math.Tan(topAngleRad);

            // 5. Save to your global DataService
            Services.DataService.CurrentDefect.bottomHeight = bottomHeight;
            Services.DataService.CurrentDefect.topHeight = topHeight;

            // Save the actual text, but use a fallback if the cruiser left it blank
            Services.DataService.CurrentDefect.Description = string.IsNullOrWhiteSpace(descriptionText)
                ? "Unspecified Defect"
                : descriptionText;

            Services.DataService.SaveDefectToTree();

            // 6. Reset the UI and Model for the next defect
            _view.UpdateDescription("Defect Saved successfully.");
            _model.ClearAll();
            _view.UpdateBaseLabel(null);
            _view.UpdateTopLabel(null);
        }
    }
}