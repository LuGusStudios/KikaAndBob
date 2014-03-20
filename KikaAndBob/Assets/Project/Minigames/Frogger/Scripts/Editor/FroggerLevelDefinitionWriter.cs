#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(FroggerLevelDefinition))]
public class FroggerLevelDefinitionWriter : LevelDefinitionWriter
{
	protected override void SaveConfig(string saveLocation)
	{
		FroggerLevelDefinition level = (FroggerLevelDefinition)target;

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += FroggerLevelDefinition.ToXML(level);

		string fileName = sceneName + "_level_" + levelPostfix.ToString() + ".xml";
		string fullPath = saveLocation + fileName;
		StreamWriter writer = new StreamWriter(fullPath);
		writer.Write(rawdata);
		writer.Close();

		Debug.Log("Saved level configuration: " + fileName + " to " + saveLocation);
	}
}
#endif