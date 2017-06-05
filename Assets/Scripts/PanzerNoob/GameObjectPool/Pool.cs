using UnityEngine;
using System.Collections.Generic;

namespace PanzerNoob.GameObjectPool
{
    [System.Serializable]
    public struct Pool
    {
        #if UNITY_EDITOR
        public bool bIsOpen;
        #endif
        
        public GameObject Prefab;
        public int Quantity;
        public int QuantityLoaded;
        public GameObject Root;
        public List<GameObject> Reserve;
        public string Name;

        public Pool(GameObject prefab = null, int quantity = 0, string newName = null)
        {
            Name = newName ?? (prefab != null ? prefab.name : "Unknown Pool");
            Prefab = prefab;
            Quantity = quantity;

            #if UNITY_EDITOR
            bIsOpen = false;
            #endif
            QuantityLoaded = 0;
            Root = null;
            Reserve = new List<GameObject>();
        }

        public void Drop()
        {
            GameObject.Destroy(Root);
        }

        public void Truncate()
        {
            Drop();
            Root = new GameObject(Name);
        }

        public void AddToPool(GameObject go)
        {
            Reserve.Add(go);
            go.transform.parent = Root.transform;
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.SetActive(false);
        }

        public void Init()
        {
            if (Prefab != null)
            {
                Poolable poolable = Prefab.GetComponent<Poolable>();
                if (poolable == null)
                {
                    Prefab.AddComponent<Poolable>();
                }

                QuantityLoaded = 0;
                Root = new GameObject(Name);
                Root.transform.parent = GameObjectPool.Instance.transform;
                Reserve = new List<GameObject>();
            }
            else
            {
                Debug.LogError("Pool must have a prefab. Drop the pool !");
                Drop();
            }
        }

        public void SyncLoad(int quantityPerFrame)
        {
            for (int i = 0; i < quantityPerFrame; ++i)
            {
                GameObject go = GameObject.Instantiate(Prefab);
                go.name = Name + "_" + QuantityLoaded.ToString();
                Poolable poolable = go.GetComponent<Poolable>();
                poolable.Pool = this;

                poolable.AddToPool();

                ++QuantityLoaded;
                ++GameObjectPool.Instance.ElementsLoaded;
            }
        }

        public bool IsNull()
        {
            return Prefab == null;
        }
    }
}