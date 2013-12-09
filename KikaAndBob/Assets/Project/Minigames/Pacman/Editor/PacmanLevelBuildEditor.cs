using UnityEngine;
using UnityEditor;
using System.Collections;

public class PacmanLevelBuildEditor : EditorWindow 
{

	[MenuItem ("KikaAndBob/Pacman/LevelVisualizer")]
	static void Init () 
	{
		PacmanLevelBuildEditor window = (PacmanLevelBuildEditor)EditorWindow.GetWindow (typeof (PacmanLevelBuildEditor));
	}
	
	
	
	void OnGUI()
	{
		if( GUILayout.Button ("Build Level") )
		{ 
			PacmanLevelManager.use.BuildLevel("levelDefault");
		}
	}
}