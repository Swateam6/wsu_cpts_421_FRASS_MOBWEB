using System;
using System.Numerics;
using MOBWEB_TEST.Services;

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
            if (!Gyroscope.Default.IsMonitoring) return;

            // 1. Validation: Distance Check (Already there)
            if (!double.TryParse(distanceText, out double distance) || distance <= 0)
            {
                _view.UpdateDescription("Please enter a valid distance.");
                return;
            }

            // 2. NEW Validation: Description Null/Empty Check
            if (string.IsNullOrWhiteSpace(descriptionText))
            {
                // Highlight the error in the UI
                _view.UpdateDescription("Error: Defect description cannot be empty.");
                return; // Stop here so we don't save a blank record
            }

            // 3. If validation passes, proceed with the save
            double baseAngleDegrees = _model.BaseAngle?.X ?? 0;
            double topAngleDegrees = _model.TopAngle?.X ?? 0;

            // ... (Trig math goes here) ...

            DataService.CurrentDefect.Description = descriptionText;

            // Final check: Did they actually capture angles?
            if (baseAngleDegrees == 0 && topAngleDegrees == 0)
            {
                _view.UpdateDescription("Error: No angles captured. Aim and hit Capture.");
                return;
            }

            DataService.SaveDefectToTree();
            _view.UpdateDescription("Defect successfully saved to tree!");
        }
        public void SaveNoDefect()
        {
            // 1. Reset the UI and Model just in case they started measuring and changed their mind
            _model.ClearAll();
            _view.UpdateBaseLabel(null);
            _view.UpdateTopLabel(null);

            // 2. Hardcode the "Clean" state into the DataService
            DataService.CurrentDefect.Description = "None";

            // Depending on how your DataService properties are named, set heights to 0
            // DataService.CurrentDefect.BaseHeight = 0; 
            // DataService.CurrentDefect.TopHeight = 0;

            // 3. Save to the database
            DataService.SaveDefectToTree();

            // 4. Update the UI so the cruiser gets immediate feedback
            _view.UpdateDescription("No defect recorded. Tree is clean!");

            // Optional: Stop the gyro to save battery since they are done with this tree
            StopGyroscope();
        }

    }
}