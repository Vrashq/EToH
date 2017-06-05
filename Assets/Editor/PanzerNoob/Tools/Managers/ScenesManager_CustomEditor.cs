using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace PanzerNoob.Managers {
	[CustomEditor(typeof(ScenesManager))]
	public class ScenesManager_CustomEditor : Editor
	{
		private ScenesManager _target;
		private List<Scene> _scenes;
		private List<string> _scenesNames;
		private int _selectedSceneToLoad = 0;

		void OnEnable () {
			_target = (ScenesManager)target;
			_scenes = new List<Scene>();
			_scenesNames = new List<string>();
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.BeginHorizontal("box");
			{
				EditorGUILayout.LabelField("Scenes Manager");
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical("box");
			{
				EditorGUILayout.BeginHorizontal("box");
				{
					EditorGUILayout.LabelField("Scenes Loaded");
				}
				EditorGUILayout.EndHorizontal();

				Dictionary<string, Scene> scenes = _target.ScenesDic;
				if(scenes.Count > 0) {
					EditorGUILayout.BeginVertical("box");
					{
						foreach(KeyValuePair<string, Scene> scene in scenes) {
							bool isActive = _target.IsSceneActive(scene.Key);
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.LabelField(scene.Key);
								EditorGUILayout.LabelField(isActive ? "Active" : "");
							}
							EditorGUILayout.EndHorizontal();
						}
					}
					EditorGUILayout.EndVertical();
				}
				else {
					EditorGUILayout.BeginVertical("box");
					{
						EditorGUILayout.LabelField("Null");
					}
					EditorGUILayout.EndVertical();
				}
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");
			{
				EditorGUILayout.BeginHorizontal("box");
				{
					EditorGUILayout.LabelField("Load Scene");
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal("box");
				{
					_scenes.Clear();
					_scenesNames.Clear();

					Scene currentScene;
					for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
					{
						// TODO Get scene into the build settings
						currentScene = SceneManager.GetSceneByBuildIndex(i);
						if(!_target.IsSceneLoaded(currentScene.name)) {
							_scenes.Add(currentScene);
							_scenesNames.Add(currentScene.name);
						}
					}

					_selectedSceneToLoad = EditorGUILayout.Popup(_selectedSceneToLoad, _scenesNames.ToArray());
					if(GUILayout.Button("Load Selected Scene")) {
						ScenesManager.Instance.LoadScene(_selectedSceneToLoad);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
	}
}
