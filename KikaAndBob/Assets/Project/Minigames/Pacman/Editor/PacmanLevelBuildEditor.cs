using UnityEngine;
using UnityEditor;
using System.Collections;

public class PacmanLevelBuildEditor : EditorWindow 
{
	protected int levelIndex = 0;

	int width = 13;
	int height = 13;
	string level = "o";
	string previousLevel = "o";
	Transform cursor = null;
	Transform levelParent = null;

	[MenuItem ("KikaAndBob/Pacman/LevelVisualizer")]
	static void Init () 
	{
		PacmanLevelBuildEditor window = (PacmanLevelBuildEditor)EditorWindow.GetWindow (typeof (PacmanLevelBuildEditor));
	}
	
	protected void Update () 
	{
		if (levelParent == null)
		{
			levelParent = GameObject.Find("LevelParent").transform;
		}

//		if (cursor == null)
//		{
//			GameObject go = GameObject.Find("LevelCursor");
//
//			if (go != null)
//			{
//				cursor = go.transform;
//			}
//			else
//			{
//				cursor = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
//				cursor.gameObject.name = "LevelCursor";
//				cursor.localScale = cursor.localScale * 0.5f;
//			}
//		}
//		else
//		{
//			int xIndex = 0;
//			int yIndex = 0;
//
//			yIndex = Mathf.FloorToInt(level.Length / width);
//			xIndex = level.Length - (yIndex * width);
//
//
//
//			cursor.position = levelParent.position + PacmanLevelManager.use.levelTiles[xIndex, yIndex].location.v3();
//		}

		if (previousLevel != level)
		{
			previousLevel = level;
			PacmanLevelManager.use.BuildLevelDebug(level, width, height);
		}
	}
	
	void OnGUI()
	{
		levelIndex = EditorGUILayout.IntField("Level index", levelIndex);

		if( GUILayout.Button ("Build Level " + levelIndex))
		{ 
			PacmanLevelManager.use.BuildLevel(levelIndex);
		}
		if( GUILayout.Button ("Create new Pacman level (root folder)") )
		{ 
			PacmanLevelDefinition levelDef = ScriptableObject.CreateInstance<PacmanLevelDefinition>();
			AssetDatabase.CreateAsset( levelDef, "Assets/NewPacmanLevel.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = levelDef;
		}
		if( GUILayout.Button ("Build level string") )
		{ 
			PacmanLevelManager.use.BuildLevelDebug(level, width, height);
		}

		width = EditorGUILayout.IntField(width);
		height = EditorGUILayout.IntField(height);
		level = EditorGUILayout.TextField(level);
	}
}