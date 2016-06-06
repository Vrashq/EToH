using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GameObjectPool))]
public class CustomEditorGameObjectPool : Editor
{

	[MenuItem("GameObject/Create GameObject Pool")]
	public static void CreateGameObjecrPool ()
	{
		GameObjectPool test = (GameObjectPool)FindObjectOfType(typeof(GameObjectPool));
		if(test == null)
		{
			GameObject gop = new GameObject("GameObjectPool");
			gop.AddComponent<GameObjectPool>();
		}
	}

	private Vector2 _scrollPosition;

	public override void OnInspectorGUI()
	{
		GameObjectPool myTarget = (GameObjectPool)target;
		List<Pool> poolsToRemove = new List<Pool>();
		
		Rect r = EditorGUILayout.BeginVertical();
		{
			r.height = 20;
			EditorGUI.ProgressBar(r, myTarget.Progress, "Loading in progress");
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical("box");
		{
			EditorGUILayout.BeginVertical("box");
			{
				EditorGUILayout.LabelField("Pools availables", EditorStyles.boldLabel);
				EditorGUILayout.BeginVertical();
				{
					for (var p = 0; p < myTarget.Pools.Count; ++p)
					{
						Pool pool = myTarget.Pools[p];
						pool.Name = pool.Prefab != null ? pool.Prefab.name : "Need a Prefab !";

						EditorGUILayout.BeginVertical("box");
						{
							EditorGUILayout.LabelField(pool.Name, EditorStyles.boldLabel);
							
							Rect rp = EditorGUILayout.BeginVertical();
							{
								rp.height = 16;
								EditorGUI.ProgressBar(r, pool.QuantityLoaded / pool.Quantity, "Pool loading");
							}
							EditorGUILayout.EndVertical();
							
							pool.Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab: ", pool.Prefab, typeof(GameObject), false);
							pool.Quantity = EditorGUILayout.IntField("Quantity: ", pool.Quantity);
							EditorGUILayout.BeginHorizontal();
							{
								if (GUILayout.Button("Duplicate Pool"))
								{
									myTarget.DuplicatePool(pool);
								}
								if (GUILayout.Button("Delete Pool"))
								{
									poolsToRemove.Add(pool);
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();
						myTarget.Pools[p] = pool;
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
			if (GUILayout.Button("Add Pool"))
			{
				myTarget.AddPool();
			}

			for (var p = 0; p < poolsToRemove.Count; ++p)
			{
				myTarget.RemovePool(poolsToRemove[p]);
			}
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.Separator();

		EditorGUILayout.BeginVertical("box");
		{
			EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
			myTarget.NumberOfInstancesPerFrame = EditorGUILayout.IntField("Quantity Generated per frame: ", myTarget.NumberOfInstancesPerFrame);
			myTarget.InitOnLoad = EditorGUILayout.Toggle("Init on Load", myTarget.InitOnLoad);
			SerializedProperty loadStart = serializedObject.FindProperty("LoadStart");
			SerializedProperty loadProgress = serializedObject.FindProperty("LoadProgress");
			SerializedProperty loadEnd = serializedObject.FindProperty("LoadEnd");

			EditorGUILayout.PropertyField(loadStart);
			EditorGUILayout.PropertyField(loadProgress);
			EditorGUILayout.PropertyField(loadEnd);

			serializedObject.ApplyModifiedProperties();

		}
		EditorGUILayout.EndVertical();

		SceneView.RepaintAll();
	}
}