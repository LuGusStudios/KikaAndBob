// C# example:
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BackgroundThemeWindow : EditorWindow 
{
	[MenuItem ("KikaAndBob/Runner/BackgroundThemes")]
	static void Init () 
	{
		BackgroundThemeWindow window = (BackgroundThemeWindow)EditorWindow.GetWindow (typeof (BackgroundThemeWindow));
	}
	 

	
	void OnGUI()
	{
		if( GUILayout.Button ("Create new BackgroundTheme (root folder)") )
		{ 
			BackgroundTheme level = ScriptableObject.CreateInstance<BackgroundTheme>();
			AssetDatabase.CreateAsset( level, "Assets/NewBackgroundTheme.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = level;
		}
		

	}
}







