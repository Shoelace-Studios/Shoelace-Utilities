using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShoelaceStudios.Utilities.DataStore
{
    [Serializable]
    public class Database<T> where T : IDataStoreItem
    {
        public Metadata Metadata;
        public T[] Data;
    }

    public class DataStore<T> where T : IDataStoreItem
    {
        protected Dictionary<string, T> data = new();

        public Metadata Metadata { get; private set; }
        public int Count => data?.Count ?? 0;

        public void Load(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"[DataStore<{typeof(T).Name}>] Cannot load - JSON string is null or empty");
                return;
            }

            try
            {
                Database<T> db = JsonUtility.FromJson<Database<T>>(json);

                if (db == null)
                {
                    Debug.LogError($"[DataStore<{typeof(T).Name}] Failed to deserialize JSON. Structure may be invalid.");
                    return;
                }

                if (db.Data == null)
                {
                    Debug.LogError($"[DataStore<{typeof(T).Name}] 'Data' array is null. Check capitalization - must be 'Data', not 'data'");
                    return;
                }

                if (db.Metadata.Version == null)
                {
                    Debug.LogError($"[DataStore<{typeof(T).Name}] 'Metadata' is missing or null. Check capitalization - must be 'Metadata', not 'metadata'");
                }

                Metadata = db.Metadata;
                data = new Dictionary<string, T>(db.Data.Length);

                foreach (T t in db.Data)
                {
                    if (data.ContainsKey(t.Key))
                    {
                        Debug.LogError($"[DataStore<{typeof(T).Name}] Duplicate key '{t.Key}' found");
                    }
                    data[t.Key] = t;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Datastore<<{typeof(T).Name}] Exception: {ex.Message}");
            }
        }


        public void LoadFromResources(string filename)
        {
            TextAsset asset = Resources.Load<TextAsset>(filename);

            if (asset == null)
            {
                Debug.LogError($"[{typeof(T).Name}] Failed to load '{filename}'. Check path is relative to Resources folder (no extension)");
                return;
            }

            if (string.IsNullOrEmpty(asset.text))
            {
                Debug.LogError($"[{typeof(T).Name}] '{filename}' loaded but text content is empty");
                return;
            }

            Load(asset.text);
        }

        public void Add(T item)
        {
            data[item.Key] = item;
        }

        public T Get(string id)
        {
            return data[id];
        }
        public bool TryGet(string id, out T value)
        {
            return data.TryGetValue(id, out value);
        }
        public bool Contains(string id)
        {
            return data.ContainsKey(id);
        }
        public IEnumerable<T> All => data.Values;
    }
}
