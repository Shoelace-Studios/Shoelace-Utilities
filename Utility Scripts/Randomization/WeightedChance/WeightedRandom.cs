using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShoelaceStudios.Utilities
{
    [Serializable]
    public class WeightedRandom<T>
    {
        [Serializable]
        public struct Entry
        {
            public T Value;
            [Min(0)] public float Weight;

            public Color Color;

            public Entry(T value, float weight)
            {
                Value = value;
                Weight = weight;
                Color = Random.ColorHSV(.7f, 1f, .8f, 1f, .8f, 1f);
            }
        }

        [SerializeField] private List<Entry> entries = new();
        public List<Entry> Entries => entries;
        public int Count => entries.Count;
        public bool IsEmpty => entries == null || entries.Count == 0;
        public float TotalWeight => entries.Sum(e => Mathf.Max(0f, e.Weight));

        #region Core Random Selection

        public T GetRandom()
        {
            return GetRandomInternal(entries);
        }

        public T GetRandomFromSubset(IEnumerable<T> allowed)
        {
            List<Entry> allowedEntries = entries.Where(e => allowed.Contains(e.Value)).ToList();
            return GetRandomInternal(allowedEntries);
        }

        private T GetRandomInternal(List<Entry> targetEntries)
        {
            if (targetEntries == null || targetEntries.Count == 0)
                return default;

            float totalWeight = targetEntries.Sum(e => Mathf.Max(0f, e.Weight));
            if (totalWeight <= 0f)
                return targetEntries[Random.Range(0, targetEntries.Count)].Value;

            float rand = Random.value * totalWeight;
            float cumulative = 0f;

            foreach (Entry entry in targetEntries)
            {
                cumulative += Mathf.Max(entry.Weight, 0f);
                if (rand <= cumulative)
                    return entry.Value;
            }

            return targetEntries[^1].Value; // fallback
        }

        public bool TryGetRandom(out T value)
        {
            value = GetRandom();
            return !EqualityComparer<T>.Default.Equals(value, default);
        }

        public T DrawRandomAndRemove()
        {
            T result = GetRandom();
            RemoveEntry(result);
            return result;
        }

        #endregion

        #region Entry Management

        public void AddEntry(T value, float weight)
        {
            entries.Add(new Entry(value, weight));
        }

        public bool RemoveEntry(T value)
        {
            int index = entries.FindIndex(e => EqualityComparer<T>.Default.Equals(e.Value, value));
            if (index >= 0)
            {
                entries.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool RemoveRandomEntry()
        {
            if (IsEmpty) return false;

            entries.RemoveAt(Random.Range(0, entries.Count));
            return true;
        }

        public void KeepRandomSubset(int count)
        {
            if (count >= Count) return;

            entries = entries.OrderBy(_ => Random.value).Take(count).ToList();
        }

        #endregion

        #region Utilities

        public void Normalize()
        {
            float total = entries.Sum(e => e.Weight);
            if (total <= 0f) return;

            for (int i = 0; i < entries.Count; i++)
            {
                Entry e = entries[i];
                e.Weight /= total;
                entries[i] = e;
            }
        }

        public static WeightedRandom<T> FromSubset(params (T Value, float Weight)[] subset)
        {
            WeightedRandom<T> wr = new();
            foreach ((T value, float weight) in subset)
                wr.AddEntry(value, weight);
            return wr;
        }

        #endregion
    }
}
