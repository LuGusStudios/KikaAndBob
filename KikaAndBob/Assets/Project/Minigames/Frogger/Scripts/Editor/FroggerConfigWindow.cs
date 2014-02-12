#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class FroggerConfigWindow : EditorWindow {

	protected FroggerLevelDefinition levelToBuild = null;

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

		levelToBuild = (FroggerLevelDefinition) EditorGUILayout.ObjectField(levelToBuild, typeof(FroggerLevelDefinition), false);
		                                        
		if( GUILayout.Button ("Build level") )
		{ 
			if (levelToBuild != null)
			{
				FroggerLevelManager.use.SetupLocal();
				FroggerLevelManager.use.BuildLevel(levelToBuild);
			}
		}
	}
}
#endif
