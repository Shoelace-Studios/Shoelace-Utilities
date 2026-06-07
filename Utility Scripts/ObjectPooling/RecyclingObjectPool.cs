using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace ShoelaceStudios.Utilities.ObjectPooling
{
    public class RecyclingObjectPool<T> where T : class
    {
        private readonly Queue<T> activeQueue = new Queue<T>();
        private readonly HashSet<T> activeSet = new HashSet<T>();
        private readonly ObjectPool<T> pool;
        private readonly int maxActive;
        private readonly float lifetime;

        public RecyclingObjectPool(Func<T> createFunc, Action<T> onGet, Action<T> onRelease, int maxActiveCount, float lifetime)
        {
            maxActive = maxActiveCount;
            pool = new ObjectPool<T>(createFunc, onGet, onRelease);
            this.lifetime = lifetime;
        }

        public T Get()
        {
            if (activeQueue.Count >= maxActive)
            {
                var oldest = activeQueue.Dequeue();
                activeSet.Remove(oldest);
                pool.Release(oldest); // Recycle the oldest
            }

            T item = pool.Get();
            activeQueue.Enqueue(item);
            activeSet.Add(item);
            LifetimeAsync(item);
            return item;
        }

        private async void LifetimeAsync(T item)
        {
            await Awaitable.WaitForSecondsAsync(Mathf.Max(0.5f, lifetime - 1f));

            // Only release if still active
            if (activeSet.Contains(item))
            {
                Release(item);
            }
        }

        public void Release(T item)
        {
            if (!activeSet.Contains(item))
                return;

            activeSet.Remove(item);

            // Rebuild the queue without this item
            Queue<T> newQueue = new Queue<T>();
            while (activeQueue.Count > 0)
            {
                var current = activeQueue.Dequeue();
                if (!EqualityComparer<T>.Default.Equals(current, item))
                    newQueue.Enqueue(current);
            }
            while (newQueue.Count > 0)
                activeQueue.Enqueue(newQueue.Dequeue());

            pool.Release(item);
        }
    }
}
