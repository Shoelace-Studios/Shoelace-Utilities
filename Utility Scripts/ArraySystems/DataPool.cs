using System.Collections.Generic;

namespace ShoelaceStudios.Utilities.ArraySystems
{
    public class DataPool<T> where T : IResetable, new() // <- I didnt realize you could add this. This is nice.
    {
        private static readonly Stack<T> pool = new();

        public static T Get()
        {
            return pool.Count > 0 ? pool.Pop() : new T();
        }

        public static void Return(T item)
        {
            item.Reset();
            pool.Push(item);
        }

        public static void Clear()
        {
            pool.Clear();
        }
    }
}
