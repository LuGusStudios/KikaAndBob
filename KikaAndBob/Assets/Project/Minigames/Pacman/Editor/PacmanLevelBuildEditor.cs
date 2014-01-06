using UnityEngine;
using UnityEditor;
using System.Collections;

public class PacmanLevelBuildEditor : EditorWindow 
{
	protected int levelIndex = 0;

	[MenuItem ("KikaAndBob/Pacman/LevelVisualizer")]
	static void Init () 
	{
		PacmanLevelBuildEditor window = (PacmanLevelBuildEditor)EditorWindow.GetWindow (typeof (PacmanLevelBuildEditor));
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
			PacmanLevelDefinition level = ScriptableObject.CreateInstance<PacmanLevelDefinition>();
			AssetDatabase.CreateAsset( level, "Assets/NewPacmanLevel.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = level;
		}
	}
}