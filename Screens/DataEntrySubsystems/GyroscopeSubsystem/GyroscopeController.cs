using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems.GyroscopeSubsystem
{
    public class GyroscopeController
    {
        private GyroscopeModel _model;
        private GyroscopeScreen _view;

        private bool _isFirstReading = true;
        private DateTime _lastReadingTime;

        public GyroscopeController(GyroscopeScreen view)
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
                _model.ClearBaseTop();
                _isFirstReading = true;

                // reset view
                _view.UpdateCurrentAngle("Not started");
                _view.UpdateBaseLabel(null);
                _view.UpdateTopLabel(null);
                _view.UpdateDifference(null);
                _view.UpdateHeightResult("Height: -- ft");

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
            _view.UpdateDifference(null);
            _view.UpdateHeightResult("Height: -- ft");
        }
        public void CaptureBase()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }
            _model.CaptureBase();
            _view.UpdateBaseLabel(_model.BaseAngle);
            _view.UpdateDifference(_model.GetDifference());
        }
        public void CaptureTop()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }
            _model.CaptureTop();
            _view.UpdateTopLabel(_model.TopAngle);
            _view.UpdateDifference(_model.GetDifference());
        }

        public void CalculateHeight(string distanceText)
        {
            if (!_model.BaseAngle.HasValue || !_model.TopAngle.HasValue)
            {
                _view.UpdateHeightResult("Please capture both base and top angles first.");
                return;
            }

            if (!double.TryParse(distanceText, out double distance) || distance <= 0)
            {
                _view.UpdateHeightResult("Enter a valid positive distance.");
                return;
            }

            try
            {
                double height = _model.CalculateHeight(distance);
                _view.UpdateHeightResult($"Height: {height:F2} ft");
            }
            catch (Exception ex)
            {
                _view.UpdateHeightResult($"Error: {ex.Message}");
            }
        }
    }
}
