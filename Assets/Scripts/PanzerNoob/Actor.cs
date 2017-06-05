using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  
using UnityEngine;

namespace PanzerNoob
{
    using Managers;
    using Tools;

    public abstract class Actor : MonoBehaviour
    {
        private readonly static BindingFlags _searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        
        private readonly Dictionary<Type, Component> _cachedComponents = new Dictionary<Type, Component>();

        /// <summary>
        /// A generic method equivalent to the standard GetComponent but with a cache system to speed up component finding
        /// </summary>
        /// <typeparam name="T">The type of component to get</typeparam>
        /// <param name="Action">The Action callback to launch</param>
        /// <return name="T">The target component</return>
        public T WithComponent<T> (Action<T> action = null) where T : Component
        {
            Type type = typeof(T);
            T cached = null;
            bool contains = _cachedComponents.ContainsKey (type);
            if (contains) {
                cached = (T)_cachedComponents[type];
            }
            //if the dictionary has the component and the component hasn't been removed, we're all done!
            if (cached == null) {
                cached = gameObject.GetComponent<T>();
                //If the component doesn't exists, we add it to the gameObject
                if(cached != null) {
                    _cachedComponents[type] = cached;
                }
            }
            if(action != null) action (cached);
            return cached;
        }

        /// <summary>
        /// Find automatically all the AutoFinder Attribute in the actor and launch the execution of each one
        /// </summary>
        private void AutoFindFields ()
        {
            List<FieldInfo> fields = GetType().GetFields(_searchFlags)
                                    .Where(prop => Attribute.IsDefined(prop, typeof(AutoFinder)))
                                    .ToList();
            foreach (FieldInfo field in fields)
            {
                foreach (AutoFinder autofind in field.GetCustomAttributes(typeof(AutoFinder), true))
                {
                    autofind.Execute(this, field);
                }
            }
        }

        /// <summary>
        /// A generic method equivalent to the standard GetComponent but with a cache system to speed up component finding
        /// </summary>
        /// <param name="Action">The Action coroutine to launch</param>
        /// <param name="float">The delay before running the method</param>
        /// <return name="Coroutine">The runned coroutine</return>
        public Coroutine Invoke (Action method, float delay = 0f) {
            return StartCoroutine(Invoke_implementation(method, delay));
        }

        /// <summary>
        /// The implementation of the method Invoke
        /// </summary>
        /// <param name="Action">The Action coroutine to launch</param>
        /// <param name="float">The delay before running the method</param>
        /// <return name="IEnumerator">The runned IEnumerator</return>
        internal IEnumerator Invoke_implementation (Action method, float delay) {
            while(delay > 0) {
                delay -= TimeManager.Instance.DeltaTime;
                yield return null;
            }
            method.Invoke();
        }

        protected virtual void Awake() { 
            AutoFindFields();
            CallMethodByNameIfExists("OnActorAwake");
        }
        protected virtual void OnEnable()         { CallMethodByNameIfExists("OnActorEnable"); }
        protected virtual void OnDisable()        { CallMethodByNameIfExists("OnActorDisable"); }
        protected virtual void Start()            { CallMethodByNameIfExists("OnActorStart"); }
        protected virtual void Update()           { CallMethodByNameIfExists("OnActorUpdate"); }
        protected virtual void OnCollisionEnter(Collision collision)    { CallMethodByNameIfExists("OnActorCollisionEnter", collision); }
        protected virtual void OnTriggerEnter(Collider collider)        { CallMethodByNameIfExists("OnActorTriggerEnter", collider); }
        protected virtual void OnTriggerExit(Collider collider)         { CallMethodByNameIfExists("OnActorTriggerExit", collider); }

        /// <summary>
        /// Call a method by its name on this actor
        /// </summary>
        /// <param name="string">The name of the method to call</param>
        /// <return name="bool">return true if the method exists</return>
        internal bool CallMethodByNameIfExists (string name, params object[] parameters) {
            MethodInfo method = GetType().GetMethod(name, _searchFlags);
            if(method != null)
            {
                if(method.ReturnType == typeof(IEnumerator)) {
                    StartCoroutine((IEnumerator)method.Invoke(this, parameters));
                } 
                else {
                    method.Invoke(this, parameters);
                }
                return true;
            }
            return false;
        }
    }
}
