using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PanzerNoob.Tools.ScriptableObjects {
    public class ScriptableObjectFactory
    {
        [MenuItem("Assets/Create/ScriptableObject")]
        public static void CreateScriptableObject()
        {
            Assembly assembly = GetAssembly ();
            
            // Get all classes derived from ScriptableObject
            Type[] allScriptableObjects = (from t in assembly.GetTypes()
                where t.IsSubclassOf(typeof(ScriptableObject)) where !t.IsAbstract
                select t).ToArray();

            // Show the selection window.
            ScriptableObjectFactoryWindow.Init(allScriptableObjects);
        }

        public static ScriptableObject Create (Type type, string name) {
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                asset.GetInstanceID(),
                ScriptableObject.CreateInstance<EndNameEdit>(),
                string.Format("{0}.asset", name),
                AssetPreview.GetMiniThumbnail(asset),
                null
            );
            return asset;
        }

        private static Assembly GetAssembly ()
        {
            return Assembly.Load (new AssemblyName ("Assembly-CSharp"));
        }
    }
}