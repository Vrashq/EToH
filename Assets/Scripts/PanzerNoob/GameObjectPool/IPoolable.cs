using UnityEngine;
using System.Collections;

namespace PanzerNoob.GameObjectPool
{
    public interface IPoolable
    {
        void OnGetFromPool();
        void OnReturnToPool();
    }
}