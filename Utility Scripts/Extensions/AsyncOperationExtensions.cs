using System.Threading.Tasks;
using UnityEngine;

namespace ShoelaceStudios.Utilities.Extensions
{
    public static class AsyncOperationExtensions
    {
        /// <summary>
        /// Extension method that converts an AsyncOperation into a Task.
        /// </summary>
        /// <param name="asyncOperation">The AsyncOperation to convert.</param>
        /// <returns>A Task that represents the completion of the AsyncOperation.</returns>
        public static Task AsTask(this AsyncOperation asyncOperation)
        {
            TaskCompletionSource<bool> tcs = new();
            asyncOperation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
