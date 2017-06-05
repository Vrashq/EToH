using System;
using System.Linq;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace PanzerNoob.Tools.ScriptableObjects {
    internal class EndNameEdit : EndNameEditAction
    {
        #region implemented abstract members of EndNameEditAction
        public override void Action (int instanceId, string pathName, string resourceFile)
        {
            AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
        }

        #endregion
    }

    public class ScriptableObjectFactoryWindow : EditorWindow
    {
        private static string[] _names;

        private static Type[] _types;

        private static Type[] Types
        {
            get { return _types; }
            set
            {
                _types = value;
                _names = _types.Select(t => t.FullName).ToArray();
            }
        }

        private int _selectedIndex;

        public static void Init(Type[] scriptableObjects)
        {
            Types = scriptableObjects;
            EditorWindow.GetWindow<ScriptableObjectFactoryWindow>(true, "Create a new ScriptableObject", true).ShowPopup();
        }

        public void OnGUI()
        {
            GUILayout.Label("ScriptableObject Class");
            if(Types.Length == 0) {
                GUILayout.Label("No ScriptableObject available");
            }
            else {
                _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _names);

                if (GUILayout.Button("Create"))
                {
                    ScriptableObjectFactory.Create(_types[_selectedIndex], _names[_selectedIndex]);
                    Close();
                }
            }
        }
    }
}