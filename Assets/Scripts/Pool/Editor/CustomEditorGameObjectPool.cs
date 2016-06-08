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

	public override void OnInspectorGUI()
	{
		// DrawDefaultInspector();
		GameObjectPool myTarget = (GameObjectPool)target;
		List<Pool> poolsToRemove = new List<Pool>();

		EditorGUILayout.BeginVertical("box");
		{
			GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
			style.richText = true;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 15;
			GUILayout.Label("GameObject Pool Manager v" + GameObjectPool.VERSION, style, GUILayout.Height(20));
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginVertical("box");
		{
			EditorGUILayout.BeginVertical();
			{
				for (var p = 0; p < myTarget.Pools.Count; ++p)
				{
					Pool pool = myTarget.Pools[p];
					pool.Name = pool.Prefab != null ? pool.Prefab.name : "Unkown pool";

					EditorGUILayout.BeginVertical("box");
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField(pool.Name + "  " + (pool.Reserve != null ? pool.Reserve.Count : 0) + "/" + pool.Quantity, EditorStyles.boldLabel);
							GUILayout.FlexibleSpace();
							GUIStyle styleButton = new GUIStyle(EditorStyles.miniButtonRight);
							if(GUILayout.Button("", styleButton, GUILayout.Width(25), GUILayout.Height(20)))
							{
								pool.bIsOpen = !pool.bIsOpen;
							}
						}
						EditorGUILayout.EndHorizontal();
						
						if(pool.bIsOpen)
						{
							GUILayout.Space(8);
							
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
					}
					EditorGUILayout.EndVertical();
					myTarget.Pools[p] = pool;
				}
			}
			EditorGUILayout.EndVertical();
			DropAreaNewPool(myTarget);

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
			myTarget.bInitOnLoad = EditorGUILayout.Toggle("Load on Start", myTarget.bInitOnLoad);
			SerializedProperty loadStart = serializedObject.FindProperty("LoadStart");
			SerializedProperty loadProgress = serializedObject.FindProperty("LoadProgress");
			SerializedProperty loadEnd = serializedObject.FindProperty("LoadEnd");

			EditorGUILayout.PropertyField(loadStart);
			EditorGUILayout.PropertyField(loadProgress);
			EditorGUILayout.PropertyField(loadEnd);

			serializedObject.ApplyModifiedProperties();
			
			if(GUILayout.Button("Generate static data files")) 
			{
				CreateStaticDataFiles();
			}
		}
		EditorGUILayout.EndVertical();

		SceneView.RepaintAll();
	}
 
    static void DropAreaNewPool (GameObjectPool myTarget)
    {
		GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
		style.normal.textColor = Color.black;
		style.richText = true;
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = 15;
		
		Event evt = Event.current;
		Rect drop_area = EditorGUILayout.BeginHorizontal("box");
		{
			EditorGUILayout.LabelField("Drop Prefab to create new pool", style, GUILayout.Height(50));
		}
		EditorGUILayout.EndVertical();
     
        switch (evt.type) {
        case EventType.DragUpdated:
        case EventType.DragPerform:
            if (!drop_area.Contains (evt.mousePosition))
                return;
             
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
            if (evt.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag ();
             
                foreach (Object dragged_object in DragAndDrop.objectReferences) {
					if((GameObject)dragged_object != null)
					{
						myTarget.AddPool((GameObject)dragged_object);
					}
                }
            }
            break;
        }
    }
	
	static void CreateStaticDataFiles ()
	{
		
	}
}