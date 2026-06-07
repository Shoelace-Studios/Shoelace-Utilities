using UnityEngine;

namespace ShoelaceStudios.Utilities
{
    public static class SpringMath
    {
        /// <summary>
        /// Computes the spring acceleration (force per unit mass) that would move a scalar value toward its target.
        /// </summary>
        public static float GetSpringAcceleration(float current, float target, float velocity, float frequency, float damping)
        {
            float f = 2f * Mathf.PI * frequency;
            float g = 2f * damping * f;
            float displacement = current - target;
            return -f * f * displacement - g * velocity;
        }

        /// <summary>
        /// Computes the spring acceleration (force per unit mass) for a Vector3.
        /// </summary>
        public static Vector3 GetSpringAcceleration(Vector3 current, Vector3 target, Vector3 velocity, float frequency, float damping)
        {
            float f = 2f * Mathf.PI * frequency;
            float g = 2f * damping * f;
            Vector3 displacement = current - target;
            return -f * f * displacement - g * velocity;
        }

        /// <summary>
        /// Computes the spring torque (angular acceleration) between two quaternions.
        /// Returns an angular acceleration vector in radians/sec^2.
        /// </summary>
        public static Vector3 GetSpringTorque(Quaternion current, Quaternion target, Vector3 angularVelocity, float frequency, float damping)
        {
            Quaternion delta = target * Quaternion.Inverse(current);
            if (delta.w < 0f)
            {
                delta.x = -delta.x;
                delta.y = -delta.y;
                delta.z = -delta.z;
                delta.w = -delta.w;
            }

            delta.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;

            Vector3 angularDisplacement = axis * (angle * Mathf.Deg2Rad);

            float f = 2f * Mathf.PI * frequency;
            float g = 2f * damping * f;

            return -f * f * angularDisplacement - g * angularVelocity;
        }

        /// <summary>
        /// Integrates position and velocity manually using computed spring acceleration.
        /// (For non-physics contexts)
        /// </summary>
        public static void IntegrateSpring(
            ref Vector3 position, ref Vector3 velocity, Vector3 target, float frequency,
            float damping, float deltaTime)
        {
            Vector3 acceleration = GetSpringAcceleration(position, target, velocity, frequency, damping);
            velocity += acceleration * deltaTime;
            position += velocity * deltaTime;
        }
    }

    /// <summary>
    /// Stateless PID helper for any numeric or vector value.
    /// Produces a force/acceleration term steering 'current' toward 'target'.
    /// Be sure to understand the kp, ki, and kd. Will require finetuning to work right
    /// </summary>
    public static class PIDMath
    {
        /// <summary>
        /// Computes a PID response for a scalar value.
        /// </summary>
        public static float ComputePID(
            float current,
            float target,
            ref float integral,
            ref float lastError,
            float kp, float ki, float kd,
            float deltaTime,
            float integralLimit = Mathf.Infinity)
        {
            float error = target - current;

            // Integral accumulation (clamped to avoid runaway)
            integral += error * deltaTime;
            integral = Mathf.Clamp(integral, -integralLimit, integralLimit);

            // Derivative term (change in error)
            float derivative = (error - lastError) / Mathf.Max(deltaTime, 1e-6f);
            lastError = error;

            // PID output
            return kp * error + ki * integral + kd * derivative;
        }

        /// <summary>
        /// Computes a PID response for a Vector3 value.
        /// </summary>
        public static Vector3 ComputePID(
            Vector3 current,
            Vector3 target,
            ref Vector3 integral,
            ref Vector3 lastError,
            float kp, float ki, float kd,
            float deltaTime,
            float integralLimit = Mathf.Infinity)
        {
            Vector3 error = target - current;

            // Integral accumulation (clamped per-axis)
            integral += error * deltaTime;
            integral = Vector3.ClampMagnitude(integral, integralLimit);

            // Derivative term
            Vector3 derivative = (error - lastError) / Mathf.Max(deltaTime, 1e-6f);
            lastError = error;

            // PID output (force/acceleration)
            return kp * error + ki * integral + kd * derivative;
        }

        /// <summary>
        /// Applies a PID update directly to position/velocity (Transform-style integration).
        /// </summary>
        public static void IntegratePID(
            ref Vector3 position,
            ref Vector3 velocity,
            Vector3 target,
            ref Vector3 integral,
            ref Vector3 lastError,
            float kp, float ki, float kd,
            float deltaTime)
        {
            Vector3 accel = ComputePID(position, target, ref integral, ref lastError, kp, ki, kd, deltaTime);
            velocity += accel * deltaTime;
            position += velocity * deltaTime;
        }
    }
}
