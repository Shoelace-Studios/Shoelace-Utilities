using System;

namespace ShoelaceStudios.Utilities.DataStore.Examples
{
    [Serializable] // This needs to be seralizeable or it wotn work
    public struct EnemyData : IDataStoreItem
    {
        public string Id;
        public string Name; // The vals here need to be exact matches to the json so json needs exactly "Name" not "name" or it will fail.
        public int Health;
        public float Speed;
        public string[] Abilities;

        public string Key => Id;
    }
}
