// C# example:
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConsumableManagerWindow : EditorWindow 
{
	[MenuItem ("KikaAndBob/Dinner Dash/Consumable Definitions")]
	static void Init () 
	{
		ConsumableManagerWindow window = (ConsumableManagerWindow)EditorWindow.GetWindow (typeof (ConsumableManagerWindow));
	}
	

	
	void OnGUI()
	{
		if( GUILayout.Button ("Create new Consumable (root folder)") )
		{ 
			ConsumableDefinition level = ScriptableObject.CreateInstance<ConsumableDefinition>();
			AssetDatabase.CreateAsset( level, "Assets/NewConsumable.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = level;
		}
		

	}
}







