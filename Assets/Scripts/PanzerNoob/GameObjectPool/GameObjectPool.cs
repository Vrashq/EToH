using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PanzerNoob.GameObjectPool
{
    using PanzerNoob.Tools;

    [Serializable]
    public struct PoolConfiguration
    {
        public string Name;
        public GameObject Prefab;
        public int Quantity;
    }

    [Serializable]
    public class LoadEvent : UnityEvent<float> {}

    public class GameObjectPool : GenericSingleton<GameObjectPool>
    {
        /*********
        * Static *
        *********/
        public const string Version = "2.4.2";

        public static T GetAvailableObject<T>(string poolName) where T : IPoolable
        {
            GameObject go = GetAvailableObject(poolName);
            if(go != null) {
                return go.GetComponent<T>();
            }
            return default(T);
        }

        public static GameObject GetAvailableObject(string poolName)
        {
            foreach (Pool pool in Instance.Pools)
            {
                if (string.Compare(pool.Name, poolName, StringComparison.Ordinal) == 0)
                {
                    //Theo: J'ai changé la facon de récup les objets car parfois le compte entre
                    //les enfants et la reserve se dérèglais et on récupérais des objets encore instantiés
                    if (pool.Root.transform.childCount > 0)
                    {
                        GameObject go = pool.Root.transform.GetChild(0).gameObject;
                        go.transform.parent = null;
                        go.SetActive(true);

                        go.GetComponent<Poolable>().IsInPool = false;

                        IPoolable iPoolable = go.GetComponent<IPoolable>();
                        if (iPoolable != null)
                        {
                            iPoolable.OnGetFromPool();
                        }

                        return go;
                    }
                    else
                    {
                        Debug.LogError("GameObjectPool >>>> Not enough items in this pool: " + poolName);
                        return null;
                    }
                }
            }

            Debug.LogError("GameObjectPool >>>> The pool doesn't exists: " + poolName);
            return null;
        }

        public static void AddObjectIntoPool(GameObject go)
        {
            Poolable poolableComponent = go.GetComponent<Poolable>();
            if (poolableComponent != null)
            {
                poolableComponent.AddToPool();

                IPoolable iPoolable = go.GetComponent<IPoolable>();
                if (iPoolable != null)
                {
                    iPoolable.OnReturnToPool();
                }
            }
            else
            {
                Debug.LogError("GameObjectPool >>>> You try to push back in a non-poolable GameObject. It may have some issues.");
            }
        }

        public static bool PoolExists(string poolName)
        {
            return GetPool(poolName) != null;
        }

        public static Pool? GetPool(string poolName)
        {
            foreach (Pool pool in Instance.Pools)
            {
                if (string.Compare(pool.Name, poolName, StringComparison.Ordinal) == 0)
                {
                    return pool;
                }
            }

            Debug.LogError("GameObjectPool >>>> The pool doesn't exists: " + poolName);
            return null;
        }

        /***********
        * Instance *
        ***********/
        public List<Pool> Pools = new List<Pool>();

        public int NumberOfInstancesPerFrame = 1000;
        public bool BCanLoad = true;
        public bool BInitOnLoad;
        public bool BIsLoading;
        public LoadEvent LoadStart;
        public LoadEvent LoadProgress;
        public LoadEvent LoadEnd;
        public float ElementsLoaded;
        public float ElementsToLoad;
        public Pool CurrentPoolLoading;

        public float Progress
        {
            get { return ElementsLoaded / ElementsToLoad; }
        }

        protected void OnActorStart()
        {
            if (BInitOnLoad)
            {
                Load();
            }
        }

        public void Load()
        {
            StartCoroutine(Load_Implementation());
        }

        public IEnumerator Load_Implementation()
        {
            if (BCanLoad)
            {
                BCanLoad = false;
                yield return StartCoroutine(LoadPoolsAsync());
            }
        }

        public Pool AddPool(GameObject prefab, int quantity = 1000, string poolName = null)
        {
            for (int i = 0; i < Pools.Count; ++i)
            {
                if (Pools[i].Name.Equals(prefab.name))
                {
                    Pool poolToAdd = Pools[i];
                    poolToAdd.Quantity += quantity;
                    Pools[i] = poolToAdd;
                    return Pools[i];
                }
            }
            Pool pool = new Pool(prefab)
            {
                Prefab = prefab,
                Quantity = quantity,
                Name = poolName ?? prefab.name
            };
            Pools.Add(pool);
            return pool;
        }

        public void AddPool(PoolConfiguration config)
        {

            AddPool(config.Prefab, config.Quantity, config.Name);
        }

        public void RemovePool(Pool pool)
        {
            Pools.Remove(pool);
        }

        public void DuplicatePool(Pool pool)
        {
            Pool newPool = new Pool
            {
                Prefab = pool.Prefab,
                Quantity = pool.Quantity
            };
            Pools.Add(newPool);
        }

        public IEnumerator LoadPoolsAsync()
        {
            BIsLoading = true;
            ElementsLoaded = 0;
            ElementsToLoad = 0;

            for (int p = 0; p < Pools.Count; ++p)
            {
                ElementsToLoad += Pools[p].Quantity;
            }

            LoadStart.Invoke(0);
            for (int p = 0; p < Pools.Count; ++p)
            {
                Pool pool = Pools[p];
                CurrentPoolLoading = pool;
                pool.Init();
                while (pool.QuantityLoaded < pool.Quantity)
                {
                    int diff = Mathf.Min(pool.Quantity - pool.QuantityLoaded, NumberOfInstancesPerFrame);
                    pool.SyncLoad(diff);

                    LoadProgress.Invoke(Progress);
                    Pools[p] = pool;

                    yield return null;
                }
            }
            LoadEnd.Invoke(100);
            BIsLoading = false;
        }

        public void DropAll()
        {
            for (int i = 0; i < Pools.Count; ++i)
            {
                Pools[i].Drop();
            }
            Pools.Clear();
            BCanLoad = true;
        }

        public void TruncateAll()
        {
            for (int i = 0; i < Pools.Count; ++i)
            {
                Pools[i].Truncate();
            }
            BCanLoad = true;
        }
    }
}