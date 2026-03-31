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

    public void SaveDefect()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }
            //PLACEHOLDER, will be added after tree data is working
            _view.UpdateDescription();
        }
    }
}