#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(FroggerLevelDefinition))]
public class FroggerLevelDefinitionWriter : Editor
{
	public string saveLocation = Application.dataPath + "/Config/Levels/";

	protected string sceneName = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
	//protected string levelPostfix = "1";

	protected bool askConfirmation = false;

	protected FroggerLevelDefinition level; 

	public override void OnInspectorGUI()
	{
		level = (FroggerLevelDefinition)target;

		GUILayout.BeginHorizontal();
		GUILayout.Label("Level name:");
		GUILayout.Label(sceneName + "_level_" + level.levelPostFix, GUILayout.MaxWidth(150));
		GUILayout.EndHorizontal();

		if (!askConfirmation)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Level nr:");
			//levelPostfix = GUILayout.TextField(levelPostfix, GUILayout.MaxWidth(150));
		
			level.levelPostFix = EditorGUILayout.IntField(level.levelPostFix, GUILayout.MaxWidth(150));
	
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Save level", GUILayout.Width(100), GUILayout.Height(20))) 
			{
				Save();
			}
		}
		else
		{
			GUILayout.Label("A level configuration with the same name already exists.");
			GUILayout.Label("Are you sure you want to overwrite the existing file?");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Yes", GUILayout.Width(50)))
			{
				askConfirmation = false;
				SaveConfig();
			}
			if (GUILayout.Button("No", GUILayout.Width(50)))
			{
				askConfirmation = false;
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.Space(20);

		DrawDefaultInspector();
	}

	private void Save()
	{
		if (!Directory.Exists(saveLocation))
		{
			Directory.CreateDirectory(saveLocation);
		}

		string fileName = sceneName + "_level_" + level.levelPostFix.ToString() + ".xml";
		string fullPath = saveLocation + fileName;
		if (File.Exists(fullPath))
		{
			askConfirmation = true;
			return;
		}

		SaveConfig();
	}

	private void SaveConfig()
	{
		FroggerLevelDefinition level = (FroggerLevelDefinition)target;
			
		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += FroggerLevelDefinition.ToXML(level);

		string fileName = sceneName + "_level_" + level.levelPostFix.ToString() + ".xml";
		string fullPath = saveLocation + fileName;
		StreamWriter writer = new StreamWriter(fullPath);
		writer.Write(rawdata);
		writer.Close();

		Debug.Log("Saved level configuration: " + fileName + " to " + saveLocation);
	}
}
#endif