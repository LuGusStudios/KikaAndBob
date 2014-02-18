#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(DartsLevelDefinition))]
public class DartsLevelDefinitionWriter : LevelDefinitionWriter
{
	protected override void SaveConfig()
	{
		DartsLevelDefinition level = (DartsLevelDefinition)target;

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += DartsLevelDefinition.ToXML(level);

		string fileName = sceneName + "_level_" + levelPostfix + ".xml";
		string fullPath = saveLocation + fileName;
		StreamWriter writer = new StreamWriter(fullPath);
		writer.Write(rawdata);
		writer.Close();

		Debug.Log("Saved level configuration: " + fileName + " to " + saveLocation);

		// Test shizzle
		StreamReader reader = new StreamReader(fullPath);
		DartsLevelDefinition testLevel = DartsLevelDefinition.FromXML(reader.ReadToEnd());

		string testData = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		testData += DartsLevelDefinition.ToXML(testLevel);

		string testFileName = sceneName + "_level_" + levelPostfix + "_test.xml";
		string testFullPath = saveLocation + testFileName;

		StreamWriter testWriter = new StreamWriter(testFullPath);
		testWriter.Write(testData);
		testWriter.Close();
	}
}
#endif