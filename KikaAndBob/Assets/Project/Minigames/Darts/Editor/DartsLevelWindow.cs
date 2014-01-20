using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class DartsLevelWindow : EditorWindow 
{
	[MenuItem ("KikaAndBob/Darts/LevelWindow")]
	static void Init () 
	{
		DartsLevelWindow window = (DartsLevelWindow)EditorWindow.GetWindow (typeof (DartsLevelWindow));
	}

	void OnGUI()
	{
		if( GUILayout.Button ("Create new darts level (root folder)") )
		{ 
			DartsLevelDefinition levelDef = ScriptableObject.CreateInstance<DartsLevelDefinition>();
			AssetDatabase.CreateAsset( levelDef, "Assets/NewDartsLevel.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = levelDef;
		}
	}
}
