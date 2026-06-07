using UnityEngine;
namespace ShoelaceStudios.Utilities.ObjectPooling
{
    public class PoolableObject : MonoBehaviour
    {
        public ObjectPool Parent;

        public virtual void OnDisable()
        {
            Parent.ReturnObjectToPool(this);
        }
    }
}
