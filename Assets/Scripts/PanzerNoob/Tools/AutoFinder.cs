using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace PanzerNoob.Tools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoFinder : Attribute
    {
        public enum Mode { Self = 0, Property = 4, Children = 1, ImmediateChildren = 5, Parent = 2, World = 3 }

        private static BindingFlags _searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private readonly string   _hierarchy;
        private readonly Mode     _mode;
        private readonly bool     _autoAdd;

        public AutoFinder() : this(Mode.Self, "", false) { }

        public AutoFinder(bool autoAdd = false) : this(Mode.Self, "", autoAdd) { }

        public AutoFinder(Mode mode = Mode.Children, string hierarchy = "", bool autoAdd = false)
        {
            _mode = mode;
            _hierarchy = hierarchy;
            _autoAdd = autoAdd;
        }

        public void Execute(Actor actor, FieldInfo field)
        {
            Type type = field.FieldType;
            bool hasMultipleTargets = type.IsList() || type.IsArray;

            if((!hasMultipleTargets && !type.IsSubclassOf(typeof(Component))) 
                || (hasMultipleTargets && !type.GetElementType().IsSubclassOf(typeof(Component)))
            ) {
                throw new Exception("This type must be a subclass of Component");
            }

            // Multiple targets Array
            if (hasMultipleTargets)
            {
                Type targetType, listType;
                if(type.IsList()) {
                    targetType = type.GetGenericArguments()[0];
                }
                else if(type.IsArray) {
                    targetType = type.GetElementType();
                }
                else {
                    throw new Exception("Error !!!!!");
                }

                listType = typeof(List<>).MakeGenericType(new[]{targetType});
                ConstructorInfo ctor = listType.GetConstructor(new Type[]{});
                var list = ctor.Invoke(new object[]{});

                List<Transform> targets = GetMultipleTargets(actor, field, targetType);
                
                /*
                MethodInfo methodInfo = listType.GetMethod("Add");
                methodInfo.Invoke(list, new[] {actor.transform});
                Debug.Log(listType.GetProperty("Count").GetValue(list, null));
                */
                // Fill list here using method above
                MethodInfo addMethod = listType.GetMethod("Add");
                foreach(Transform target in targets) {
                    Component comp = target.GetComponent(targetType);
                    if(comp != null) {
                        addMethod.Invoke(list, new[] {comp});
                    }
                }
                // End fill

                if(type.IsList()) {
                    field.SetValue(actor, list);
                }
                else if(type.IsArray) {
                    field.SetValue(actor, listType.GetMethod("ToArray").Invoke(list, null));
                }
            }
            // Single target
            else {
                Transform target = GetSingleTarget(actor, field, type);
                if (target != null)
                {
                    Component component = target.GetComponent(type);

                    if (component == null && _autoAdd)
                        target.gameObject.AddComponent(type);

                    field.SetValue(actor, component);
                }
                else
                {
                    Debug.LogError("No target found. Please check your AutoFinder attribute");
                }
            }
        }

        private Transform GetSingleTarget(Actor actor, FieldInfo field, Type type)
        {
            Transform target = null;
            Transform actorTransform = actor.WithComponent<Transform>();
            switch (_mode)
            {
                case Mode.World:
                    LogSlowWarning();
                    if(!string.IsNullOrEmpty(_hierarchy)) {
                        target = GameObject.Find(_hierarchy).transform;
                    }
                    if(target == null) {
                        target = (GameObject.FindObjectOfType(type) as Component).transform;
                    }
                    break;
                case Mode.Children:
                    List<Transform> targets = GetChildrenRecursive(actorTransform);
                    if(targets.Count > 0) {
                        foreach(Transform element in targets) {
                            if(element.GetComponent(type)) {
                                target = element.transform;
                                break;
                            }
                        }
                    }
                    break;
                case Mode.Parent:
                    target = actorTransform.parent;
                    break;
                case Mode.Self:
                    target = actorTransform;
                    break;
                case Mode.Property:
                    PropertyInfo property = type.GetProperty(_hierarchy, _searchFlags);
                    if(property != null) {
                        object value = property.GetValue(actor, null);
                        if(value != null) {
                            PropertyInfo targetTransform = type.GetProperty("transform", _searchFlags);
                            if(targetTransform != null) {
                                target = (Transform)targetTransform.GetValue(value, null);
                            }
                        }
                    }
                    break;
            }
            return target;
        }

        private List<Transform> GetMultipleTargets(Actor actor, FieldInfo field, Type type)
        {
            List<Transform> targets = new List<Transform>();
            Transform actorTransform = actor.WithComponent<Transform>();
            switch (_mode)
            {
                case Mode.World:
                    LogSlowWarning();
                    UnityEngine.Object[] gos = GameObject.FindObjectsOfType(type);
                    foreach(UnityEngine.Object go in gos) {
                        GameObject gameObject = (GameObject)go;
                        if(go.GetType() == type && gameObject != null) {
                            targets.Add((Transform)go.GetType().GetProperty("transform").GetValue(go, null));
                        }
                    }
                    break;
                case Mode.Children:
                    targets.AddRange(GetChildrenRecursive(actorTransform));
                    break;
                case Mode.ImmediateChildren:
                    foreach(Transform child in actor.transform) {
                        if(child.GetComponent(type)) {
                            targets.Add(child.transform);
                        }
                    }
                    break;
                case Mode.Parent:
                    targets.AddRange(GetParentsRecursive(actorTransform));
                    break;
                case Mode.Self:
                    targets.Add(actorTransform);
                    break;
                case Mode.Property:
                    Debug.LogError("Method not implemented yet");
                    Debug.Break();
                    break;
            }
            return targets;
        }

        private List<Transform> GetParentsRecursive(Transform element)
        {
            List<Transform> parents = new List<Transform>();
            while (element.parent != null)
            {
                parents.Add(element.parent);
                element = element.parent;
            }
            return parents;
        }

        private List<Transform> GetChildrenRecursive(Transform element, List<Transform> children = null)
        {
            if (children == null) children = new List<Transform>();
            if (element.childCount > 0)
            {
                foreach (Transform child in element)
                {
                    children.Add(child);
                    GetChildrenRecursive(child, children);
                }
            }
            return children;
        }

        private void LogSlowWarning () {
            Debug.LogWarning("Be careful. World search can be very slow. Prefer the standard assignation instead via the inspector");
        }
    }
}
