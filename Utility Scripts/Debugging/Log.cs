using System.Diagnostics;
namespace ShoelaceStudios.Utilities.Debugging
{
    public static class Log
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEBUG")]
        public static void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEBUG")]
        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEBUG")]
        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
