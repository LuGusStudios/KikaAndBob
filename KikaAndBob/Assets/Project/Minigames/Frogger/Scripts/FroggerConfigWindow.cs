#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class FroggerConfigWindow : EditorWindow {

	[MenuItem ("KikaAndBob/Frogger/Frogger Config Window")]
	static void Init () 
	{
		FroggerConfigWindow window = (FroggerConfigWindow)EditorWindow.GetWindow (typeof (FroggerConfigWindow));
	}
	
	
	
	void OnGUI()
	{
		if( GUILayout.Button ("Create new Frogger level (root folder)") )
		{ 
			FroggerLevelDefinition level = ScriptableObject.CreateInstance<FroggerLevelDefinition>();
			AssetDatabase.CreateAsset( level, "Assets/NewFroggerLevel.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = level;
		}

		if( GUILayout.Button ("Create new Frogger theme (root folder)") )
		{ 
			FroggerTheme level = ScriptableObject.CreateInstance<FroggerTheme>();
			AssetDatabase.CreateAsset( level, "Assets/NewFroggerTheme.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = level;
		}
	}
}
#endif
