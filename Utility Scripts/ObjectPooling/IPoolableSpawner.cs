namespace ShoelaceStudios.Utilities.ObjectPooling
{
    public interface IPoolableSpawner
    {
        public PoolableObject[] GetPoolableObjects();
        public void SetObjectPool(ObjectPool pool);
        public IPoolableSpawner[] GetSubSpawns();
        int GetSpawnLimitSize();
    }
}
