using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(PacmanLevelDefinition))]
public class PacmanLevelDefinitionEditor : Editor 
{
	public string saveLocation = Application.dataPath + "/Config/Pacman/";

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Save config", GUILayout.Width(100), GUILayout.Height(20)))
		{
			SaveConfig();
		}

		DrawDefaultInspector();
	}

	private void SaveConfig()
	{
		PacmanLevelDefinition level = (PacmanLevelDefinition)target;

		if (!Directory.Exists(saveLocation))
			Directory.CreateDirectory(saveLocation);

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += PacmanLevelDefinition.ToXML(level);

		StreamWriter writer = new StreamWriter(saveLocation + level.name + ".xml");
		writer.Write(rawdata);
		writer.Close();
	}
}
