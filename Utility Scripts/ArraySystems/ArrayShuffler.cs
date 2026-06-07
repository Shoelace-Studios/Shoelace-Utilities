using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.Utilities.ArraySystems
{
    public class ArrayShuffler<T>
    {
        private List<T> items;
        private int currentIndex;

        public ArrayShuffler(IEnumerable<T> collection)
        {
            items = new List<T>(collection);
            Shuffle();
        }

        public T GetNext()
        {
            if (currentIndex >= items.Count)
            {
                Shuffle();
                currentIndex = 0;
            }

            return items[currentIndex++];
        }

        private void Shuffle()
        {
            int n = items.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (items[k], items[n]) = (items[n], items[k]);
            }
        }
    }
}
