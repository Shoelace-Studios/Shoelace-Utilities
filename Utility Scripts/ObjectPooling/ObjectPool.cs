using System.Collections.Generic;
using UnityEngine;
namespace ShoelaceStudios.Utilities.ObjectPooling
{
    public class ObjectPool
    {
        private PoolableObject Prefab;
        private int Size;
        private List<PoolableObject> AvailableObjectsPool; //maybe a queue for performance

        private ObjectPool(PoolableObject Prefab, int Size)
        {
            this.Prefab = Prefab;
            this.Size = Size;
            AvailableObjectsPool = new List<PoolableObject>(Size);
        }

        public static ObjectPool CreateInstance(PoolableObject Prefab, int Size, Transform container)
        {
            ObjectPool pool = new ObjectPool(Prefab, Size);

            // set up naming so the scene is less messy
            GameObject poolGameObject = new GameObject(Prefab + " Pool");
            poolGameObject.transform.parent = container;
            pool.CreateObjects(poolGameObject);

            return pool;
        }

        private void CreateObjects(GameObject parent)
        {
            for (int i = 0; i < Size; i++) {
                PoolableObject poolableObject = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity, parent.transform);
                poolableObject.Parent = this;
                poolableObject.gameObject.SetActive(false); // PoolableObject handles re-adding the object to the AvailableObjects
            }
        }

        public PoolableObject GetObject()
        {
            PoolableObject instance = AvailableObjectsPool[0];

            AvailableObjectsPool.RemoveAt(0);

            instance.gameObject.SetActive(true);

            return instance;
        }

        public void ReturnObjectToPool(PoolableObject Object)
        {
            AvailableObjectsPool.Add(Object);
        }
    }
}
