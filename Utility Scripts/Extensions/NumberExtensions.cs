using UnityEngine;
#if ENABLED_UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

namespace ShoelaceStudios.Utilities.Extensions
{
    public static class NumberExtensions
    {
        public static float PercentageOf(this int part, int whole)
        {
            if (whole == 0) return 0; // Handling division by zero

            return (float)part / whole;
        }

        public static bool Approx(this float f1, float f2)
        {
            return Mathf.Approximately(f1, f2);
        }

        public static bool IsOdd(this int i)
        {
            return i % 2 == 1;
        }

        public static bool IsEven(this int i)
        {
            return i % 2 == 0;
        }

        public static int AtLeast(this int value, int min)
        {
            return Mathf.Max(value, min);
        }

        public static int AtMost(this int value, int max)
        {
            return Mathf.Min(value, max);
        }

		#if ENABLED_UNITY_MATHEMATICS
        public static half AtLeast(this half value, half max) => MathfExtension.Max(value, max);
        public static half AtMost(this half value, half max) => MathfExtension.Min(value, max);
		#endif

        public static float AtLeast(this float value, float min)
        {
            return Mathf.Max(value, min);
        }

        public static float AtMost(this float value, float max)
        {
            return Mathf.Min(value, max);
        }

        public static double AtLeast(this double value, double min)
        {
            return MathfExtension.Max(value, min);
        }

        public static double AtMost(this double value, double min)
        {
            return MathfExtension.Min(value, min);
        }
    }
}
