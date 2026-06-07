using System.Collections.Generic;

namespace ShoelaceStudios.Utilities.ArraySystems
{
    public static class PoolableArray<T>
    {
        private static readonly Stack<T[]> _pool = new();

        public static T[] Rent(int length)
        {
            while (_pool.Count > 0)
            {
                T[] array = _pool.Pop();
                if (array.Length >= length) return array;
            }

            return new T[length];
        }

        public static void Return(T[] array)
        {
            if (array == null)
            {
				#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning("Trying to return a null array to pool");
				#endif
                return;
            }

            _pool.Push(array);
        }

        public static T[] RentCopy(IEnumerable<T> source, out int count)
        {
            // Try to avoid extra allocations by using a temporary list once.
            if (source is ICollection<T> collection)
            {
                T[] arr = Rent(collection.Count);
                collection.CopyTo(arr, 0);
                count = collection.Count;
                return arr;
            }

            // Otherwise, fill a temp list first
            List<T> temp = PoolableList<T>.Rent();
            foreach (T item in source)
                temp.Add(item);

            T[] rented = Rent(temp.Count);
            for (int i = 0; i < temp.Count; i++)
                rented[i] = temp[i];

            count = temp.Count;
            PoolableList<T>.Return(temp);
            return rented;
        }

        public static T[] RentCopy(IEnumerable<T> source)
        {
            return RentCopy(source, out _);
        }
    }


    public static class PoolableList<T>
    {
        private static readonly Stack<List<T>> _pool = new();

        public static List<T> Rent()
        {
            if (_pool.Count > 0)
            {
                List<T> list = _pool.Pop();
                list.Clear();
                return list;
            }

            return new List<T>();
        }

        public static void Return(List<T> list)
        {
            if (list == null)
            {
				#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning("Trying to return a null array to pool");
				#endif
                return;
            }

            list.Clear();
            _pool.Push(list);
        }

        public static List<T> RentCopy(IEnumerable<T> source)
        {
            List<T> list = Rent();
            list.AddRange(source);
            return list;
        }
    }
}
