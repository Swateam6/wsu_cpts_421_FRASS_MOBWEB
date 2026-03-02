using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems
{
    public class SensorService
    {
        // Nullable variables representing "awaiting data"
        private double? currentPitch = null;
        private char? currentDirection = null;

        public bool IsSupported()
        {
            return OrientationSensor.Default.IsSupported && Compass.Default.IsSupported;
        }

        public void StartSensors()
        {
            if (IsSupported())
            {
                if (!OrientationSensor.Default.IsMonitoring && !Compass.Default.IsMonitoring)
                {
                    // 1. Subscribe to events (plug in the cables)
                    Compass.Default.ReadingChanged += OnCompassChanged;
                    OrientationSensor.Default.ReadingChanged += OnReadingChanged;

                    // 2. Start hardware at UI speed
                    OrientationSensor.Default.Start(SensorSpeed.UI);
                    Compass.Default.Start(SensorSpeed.UI);
                }
            }
        }

        public void StopSensors()
        {
            if (IsSupported())
            {
                if (OrientationSensor.Default.IsMonitoring || Compass.Default.IsMonitoring)
                {
                    // 1. Unsubscribe to prevent memory leaks (unplug the cables)
                    Compass.Default.ReadingChanged -= OnCompassChanged;
                    OrientationSensor.Default.ReadingChanged -= OnReadingChanged;

                    // 2. Stop hardware to save battery
                    OrientationSensor.Default.Stop();
                    Compass.Default.Stop();
                }
            }
        }

        private void OnReadingChanged(object? sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading.Orientation;

            double sinPitch = 2.0 * (data.W * data.Y - data.Z * data.X);
            currentPitch = Math.Asin(sinPitch) * (180.0 / Math.PI);
        }

        private void OnCompassChanged(object? sender, CompassChangedEventArgs e)
        {
            double currentHeading = e.Reading.HeadingMagneticNorth;

            if (currentHeading >= 315 || currentHeading < 45)
            {
                currentDirection = 'N';
            }
            else if (currentHeading >= 45 && currentHeading < 135)
            {
                currentDirection = 'E'; // 90 degrees is East
            }
            else if (currentHeading >= 135 && currentHeading < 225)
            {
                currentDirection = 'S'; // 180 degrees is South
            }
            else
            {
                currentDirection = 'W'; // Everything else is West
            }
        }

        public char? GetCurrentDirection()
        {
            return currentDirection;
        }

        public double? GetLatestPitch()
        {
            return currentPitch;
        }
    }
}
