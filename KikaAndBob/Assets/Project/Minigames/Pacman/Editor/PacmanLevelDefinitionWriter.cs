#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(PacmanLevelDefinition))]
public class PacmanLevelDefinitionWriter : LevelDefinitionWriter
{
	protected override void SaveConfig()
	{
		PacmanLevelDefinition level = (PacmanLevelDefinition)target;

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += PacmanLevelDefinition.ToXML(level);

		string fileName = sceneName + "_level_" + levelPostfix + ".xml";
		string fullPath = saveLocation + fileName;
		StreamWriter writer = new StreamWriter(fullPath);
		writer.Write(rawdata);
		writer.Close();

		Debug.Log("Saved level configuration: " + fileName + " to " + saveLocation);
	}
}
#endif