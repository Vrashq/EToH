using UnityEngine;

namespace PanzerNoob.Tools
{
    public abstract class GenericSingleton<T> : Actor where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                GameObject parent = GameObject.Find("Singletons") ?? new GameObject("Singletons");
                if (_instance != null || (_instance = FindObjectOfType<T>()) != null) {
                    if(_instance.transform.parent != parent.transform) {
                        _instance.transform.SetParent(parent.transform);
                    }
                    return _instance;
                }
                return MakeSingleton(parent.transform);
            }
        }

        public static T CreateInstance () {
            return Instance;
        }

        private static T MakeSingleton (Transform parent) {
            GameObject go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
            go.transform.parent = parent;
            return _instance;
        }
    }
}
