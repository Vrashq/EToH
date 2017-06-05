using UnityEngine;

namespace PanzerNoob.GameObjectPool
{
    public class Poolable : MonoBehaviour
    {
        [HideInInspector] public Pool Pool;
        [HideInInspector] public bool IsInPool = false;

        public void AddToPool()
        {
            MonoBehaviour[] tempScriptRefs = GetComponentsInChildren<MonoBehaviour>();

            foreach (MonoBehaviour t in tempScriptRefs)
            {
                t.StopAllCoroutines();
            }

            transform.parent = Pool.Root.transform;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(false);
            StopAllCoroutines();
            IsInPool = true;
        }
    }
}