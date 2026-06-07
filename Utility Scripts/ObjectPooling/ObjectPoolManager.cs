using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ShoelaceStudios.Utilities.ObjectPooling
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }
        private class PoolableData
        {
            public ObjectPool objectPool;
            public int requiredSize;
            public List<IPoolableSpawner> accocitatedSpawners;
            public PoolableData(int startSize, IPoolableSpawner spawner = null)
            {
                requiredSize = startSize;

                if (spawner != null)
                    accocitatedSpawners = new List<IPoolableSpawner> { spawner };
                else
                    accocitatedSpawners = new List<IPoolableSpawner>();
            }
            public void AddToSpawners(int sizeIncrement, IPoolableSpawner newSpawner = null)
            {
                requiredSize += sizeIncrement;

                if (newSpawner != null)
                    accocitatedSpawners.Add(newSpawner);
            }
        }
        private Dictionary<PoolableObject, PoolableData> objectPools;

        [SerializeField] private Transform poolContainer;

        private void Awake()
        {
            #region Singleton
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            #endregion
            objectPools = new Dictionary<PoolableObject, PoolableData>();
            AddSceneObjectsToPool();
            AddPoolableSpawnersToPool();
            InitializeObjectPools();
        }
        private void AddSceneObjectsToPool()
        {
            PoolableObject[] sceneObjects = FindObjectsOfType<PoolableObject>();
            foreach (PoolableObject item in sceneObjects) {
                if (objectPools.ContainsKey(item)) {
                    objectPools[item].AddToSpawners(1);
                } else {
                    objectPools.Add(item, new PoolableData(1));
                }

                item.Parent = GetSpawnerPool(item);
            }
        }
        private void AddPoolableSpawnersToPool()
        {
            IPoolableSpawner[] spawners = FindObjectsOfType<MonoBehaviour>().OfType<IPoolableSpawner>().ToArray();
            foreach (IPoolableSpawner item in spawners) {
                AddSpawner(item);
            }
        }
        private void InitializeObjectPools()
        {
            //Create pools
            foreach (KeyValuePair<PoolableObject, PoolableData> poolData in objectPools) {
                PoolableObject prefab = poolData.Key;
                int size = poolData.Value.requiredSize;

                objectPools[prefab].objectPool = ObjectPool.CreateInstance(prefab, size, poolContainer);

                if (objectPools[prefab].accocitatedSpawners.Count > 0) {
                    foreach (IPoolableSpawner spawner in objectPools[prefab].accocitatedSpawners) {
                        spawner.SetObjectPool(objectPools[prefab].objectPool);
                    }
                }
            }
        }

        public PoolableObject GetFromPool(PoolableObject prefab)
        {
            if (objectPools.ContainsKey(prefab)) {
                return objectPools[prefab].objectPool.GetObject();
            } else {
                Debug.LogError("No object pool found for the given prefab.");
                return null;
            }
        }

        public void AddSpawner(IPoolableSpawner spawner)
        {
            foreach (IPoolableSpawner subSpawn in spawner.GetSubSpawns()) {
                AddSpawner(subSpawn);
            }

            PoolableObject[] objs = spawner.GetPoolableObjects();

            if (objs.Length == 0)
                return;

            foreach (PoolableObject obj in objs) {
                if (objectPools.ContainsKey(obj)) {
                    objectPools[obj].AddToSpawners(spawner.GetSpawnLimitSize(), spawner);
                } else {
                    objectPools.Add(obj, new PoolableData(spawner.GetSpawnLimitSize(), spawner));
                }
            }
        }

        public ObjectPool GetSpawnerPool(PoolableObject spawn)
        {
            if (!objectPools.ContainsKey(spawn)) {
                return null;
            }

            return objectPools[spawn].objectPool;
        }
    }
}
