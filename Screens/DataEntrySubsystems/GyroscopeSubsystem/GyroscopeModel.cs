using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace MOBWEB_TEST.Screens.DataEntrySubsystems.GyroscopeSubsystem
{
    public class GyroscopeModel
    {
        // raw angle vector, absolute, not reset by zeroing
        public Vector3 TotalAngle { get; private set; }

        // for zeroing/gyro reset, subtracted from total angle to get relative angle
        public Vector3 ZeroOffset { get; private set; }

        // Captured angles, relative to zero offset, at the time of measurement
        public Vector3? BaseAngle { get; private set; }
        public Vector3? TopAngle { get; private set; }

        public void UpdateAngle(float deltaSeconds, Vector3 angularVelocityRadPerSec)
        {
            // convert to degrees and integrate over time to get angle
            float degPerSecX = angularVelocityRadPerSec.X * (180f / MathF.PI);
            float degPerSecY = angularVelocityRadPerSec.Y * (180f / MathF.PI);
            float degPerSecZ = angularVelocityRadPerSec.Z * (180f / MathF.PI);

            // Fix: create a new Vector3 and assign it to TotalAngle (struct property cannot be modified directly)
            TotalAngle = new Vector3(
                TotalAngle.X + degPerSecX * deltaSeconds,
                TotalAngle.Y + degPerSecY * deltaSeconds,
                TotalAngle.Z + degPerSecZ * deltaSeconds
            );
        }

        public void ZeroGyro()
        {
            ZeroOffset = TotalAngle;
            ClearBaseTop();
        }
        public void ClearBaseTop()
        {
            BaseAngle = null;
            TopAngle = null;
        }
        public void InitVectors()
        {
            TotalAngle = Vector3.Zero;
            ZeroOffset = Vector3.Zero;
        }
        public void CaptureTop()
        {
            TopAngle = TotalAngle - ZeroOffset;
        }
        public void CaptureBase()
        {
            BaseAngle = TotalAngle - ZeroOffset;
        }
        public Vector3 GetRelativeAngle()
        {
            return TotalAngle - ZeroOffset;
        }
        public Vector3? GetDifference()
        {
            if (BaseAngle.HasValue && TopAngle.HasValue)
                return TopAngle.Value - BaseAngle.Value;
            return null;
        }
        public double CalculateHeight(double distanceFeet)
        {
            // should never reach here due to checks in controller but failsafe
            if (!BaseAngle.HasValue || !TopAngle.HasValue)
                throw new InvalidOperationException("Base and top angles required, " +
                    "should be checked before reaching here");

            //TODO change away from x axis to y
            // currently using x to ease manual input until voice system is working
            double deltaDegrees = TopAngle.Value.X - BaseAngle.Value.X;
            double deltaRadians = deltaDegrees * Math.PI / 180.0;
            return (distanceFeet * Math.Tan(deltaRadians));
        }
    }
}
