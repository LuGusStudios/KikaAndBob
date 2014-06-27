#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(CatchingMiceLevelDefinition))]
public class CatchingMiceLevelDefinitionWriter : Editor
{
	public string saveLocationConfig = Application.dataPath + "/Config/Levels/";
	public string saveLocationResources = Application.dataPath + "/Resources/Shared/Text/";

	protected string sceneName = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
	protected string levelPostfix = "1";

	protected bool askConfirmationConfig = false;
	protected bool askConfirmationResources = false;

	public override void OnInspectorGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Level name:");
		GUILayout.Label(sceneName + "_level_" + levelPostfix.ToString(), GUILayout.MaxWidth(150));
		GUILayout.EndHorizontal();

		if (!askConfirmationConfig && !askConfirmationResources)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Level nr:");
			levelPostfix = GUILayout.TextField(levelPostfix.ToString(), GUILayout.MaxWidth(150));
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Save in config", GUILayout.Width(150), GUILayout.Height(20)))
			{
				Save(saveLocationConfig);
			}

			if (GUILayout.Button("Save in resources", GUILayout.Width(150), GUILayout.Height(20)))
			{
				Save(saveLocationResources);
			}
		}
		else if (askConfirmationConfig)
		{
			GUILayout.Label("A level configuration with the same name already exists.");
			GUILayout.Label("Are you sure you want to overwrite the existing file?");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Yes", GUILayout.Width(50)))
			{
				askConfirmationConfig = false;
				SaveConfig(saveLocationConfig);
			}
			if (GUILayout.Button("No", GUILayout.Width(50)))
			{
				askConfirmationConfig = false;
			}
			GUILayout.EndHorizontal();
		}
		else if (askConfirmationResources)
		{
			GUILayout.Label("A level configuration with the same name already exists.");
			GUILayout.Label("Are you sure you want to overwrite the existing file?");

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Yes", GUILayout.Width(50)))
			{
				askConfirmationResources = false;
				SaveConfig(saveLocationResources);
			}
			if (GUILayout.Button("No", GUILayout.Width(50)))
			{
				askConfirmationResources = false;
			}
			GUILayout.EndHorizontal();
		}


		GUILayout.Space(20);

		DrawDefaultInspector();
	}

	private void Save(string saveLocation)
	{
		if (!Directory.Exists(saveLocationConfig))
		{
			Directory.CreateDirectory(saveLocationConfig);
		}

		string fileName = sceneName + "_level_" + levelPostfix.ToString() + ".xml";
		string fullPath = saveLocation + fileName;
		if (File.Exists(fullPath))
		{
			if (saveLocation == saveLocationConfig)
			{
				askConfirmationConfig = true;
			}
			else if (saveLocation == saveLocationResources)
			{
				askConfirmationResources = true;
			}

			return;
		}

		SaveConfig(saveLocation);
	}

	private void SaveConfig(string saveLocation)
	{
		CatchingMiceLevelDefinition level = (CatchingMiceLevelDefinition)target;

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += CatchingMiceLevelDefinition.ToXML(level);

		string fileName = sceneName + "_level_" + levelPostfix.ToString() + ".xml";
		string fullPath = saveLocation + fileName;
		StreamWriter writer = new StreamWriter(fullPath);
		writer.Write(rawdata);
		writer.Close();

		Debug.Log("Saved level configuration: " + fileName + " to " + saveLocation);
	}
}
#endif