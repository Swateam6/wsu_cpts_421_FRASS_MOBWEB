using System;
using System.Numerics;
using Microsoft.Maui.ApplicationModel;
using MOBWEB_TEST.Services;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems.GyroscopeSubsystem
{
    public class GyroscopeController
    {
        private readonly GyroscopeModel _model;
        // CHANGED: Now uses the Interface so any screen can connect to it
        private readonly GyroscopeScreen _view;

        private bool _isFirstReading = true;
        private DateTime _lastReadingTime;

        public GyroscopeController(GyroscopeScreen view)
        {
            _model = new GyroscopeModel();
            _view = view;
        }

        public void StartGyroscope()
        {
            if (!Gyroscope.Default.IsSupported)
            {
                _view.UpdateCurrentAngle("Gyroscope not supported on this device.");
                return;
            }

            if (Gyroscope.Default.IsMonitoring)
            {
                return;
            }

            _model.InitVectors();
            _model.ClearAll();
            _isFirstReading = true;

            _view.UpdateCurrentAngle("Not started");
            _view.UpdateBaseLabel(null);
            _view.UpdateTopLabel(null);
            _view.UpdateLiveCrownBase(null);
            _view.UpdateDifference(null);
            _view.UpdateHeightResult("Height: -- ft");
            _view.UpdateLiveCrownRatioResult("Live Crown Ratio: -- %");
            _view.UpdateLiveCrownHeightResult("Live Crown Height: -- ft");

            Gyroscope.Default.ReadingChanged += OnGyroscopeReadingChanged;
            Gyroscope.Default.Start(SensorSpeed.UI);
        }

        public void StopGyroscope()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }

            Gyroscope.Default.ReadingChanged -= OnGyroscopeReadingChanged;
            Gyroscope.Default.Stop();
        }

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
            _view.UpdateCurrentAngle($"X: {relative.X:F1}°, Y: {relative.Y:F1}°, Z: {relative.Z:F1}°");
        }

        public void ZeroGyroReadings()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }

            _model.ZeroGyro();

            _view.UpdateBaseLabel(null);
            _view.UpdateTopLabel(null);
            _view.UpdateLiveCrownBase(null);
            _view.UpdateDifference(null);
            _view.UpdateHeightResult("Height: -- ft");
            _view.UpdateLiveCrownRatioResult("Live Crown Ratio: -- %");
            _view.UpdateLiveCrownHeightResult("Live Crown Height: -- ft");

            // SAFETY NET: Only reset tree data if a tree actually exists
            if (DataService.CurrentTree != null)
            {
                DataService.CurrentTree.BaseAngle = 0;
                DataService.CurrentTree.TopAngle = 0;
                DataService.CurrentTree.LiveCrownBaseAngle = 0;
                DataService.CurrentTree.Height = 0;
                DataService.CurrentTree.CrownRatioPercent = 0;
                DataService.CurrentTree.LiveCrownHeight = 0;
                DataService.CurrentTree.BaseLiveCrown = 0;
            }
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

            // SAFETY NET
            if (_model.BaseAngle.HasValue && DataService.CurrentTree != null)
            {
                DataService.CurrentTree.BaseAngle = _model.BaseAngle.Value.X;
            }
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

            // SAFETY NET
            if (_model.TopAngle.HasValue && DataService.CurrentTree != null)
            {
                DataService.CurrentTree.TopAngle = _model.TopAngle.Value.X;
            }
        }

        public void CaptureLiveCrownBase()
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                return;
            }

            _model.CaptureLiveCrownBase();
            _view.UpdateLiveCrownBase(_model.LiveCrownBaseAngle);

            // SAFETY NET
            if (_model.LiveCrownBaseAngle.HasValue && DataService.CurrentTree != null)
            {
                DataService.CurrentTree.LiveCrownBaseAngle = _model.LiveCrownBaseAngle.Value.X;
            }
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

                // SAFETY NET
                if (DataService.CurrentTree != null)
                {
                    DataService.CurrentTree.Height = height;
                }
            }
            catch (Exception ex)
            {
                _view.UpdateHeightResult($"Error: {ex.Message}");
            }
        }

        public void CalculateLiveCrownRatio(string distanceText)
        {
            if (!_model.BaseAngle.HasValue || !_model.LiveCrownBaseAngle.HasValue || !_model.TopAngle.HasValue)
            {
                _view.UpdateLiveCrownRatioResult("Please capture base, top, and live crown base angles first.");
                return;
            }

            if (!double.TryParse(distanceText, out double distance) || distance <= 0)
            {
                _view.UpdateLiveCrownRatioResult("Enter a valid positive distance.");
                return;
            }

            try
            {
                var liveCrownOutputs = _model.CalculateCrownRatio(distance);
                double liveCrownRatio = liveCrownOutputs.Item1;
                double liveCrownHeight = liveCrownOutputs.Item2;

                _view.UpdateLiveCrownRatioResult($"Live Crown Ratio: {liveCrownRatio:F2} %");
                _view.UpdateLiveCrownHeightResult($"Live Crown Height: {liveCrownHeight:F2} ft");

                // SAFETY NET
                if (DataService.CurrentTree != null)
                {
                    DataService.CurrentTree.CrownRatioPercent = liveCrownRatio;
                    DataService.CurrentTree.LiveCrownHeight = liveCrownHeight;
                    DataService.CurrentTree.BaseLiveCrown = liveCrownHeight;
                }
            }
            catch (Exception ex)
            {
                _view.UpdateLiveCrownRatioResult($"Error: {ex.Message}");
            }
        }

        public void CalculateSlope()
        {
            if (!_model.BaseAngle.HasValue)
            {
                _view.UpdateCurrentAngle("Please capture base angle first.");
                return;
            }

            try
            {
                double slopePercent = _model.CalculateSlopePercent();

                // SAFETY NET: Ensure Plot actually exists before saving
                if (DataService.CurrentPlot != null)
                {
                    DataService.CurrentPlot.Slope = slopePercent;
                }

                _view.UpdateCurrentAngle($"Slope: {slopePercent:F2}%");
            }
            catch (Exception ex)
            {
                _view.UpdateCurrentAngle($"Error: {ex.Message}");
            }
        }
    }
}