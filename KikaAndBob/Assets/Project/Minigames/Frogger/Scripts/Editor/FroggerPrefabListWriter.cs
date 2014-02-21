#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(FroggerLevelManagerDefault))]
public class FroggerPrefabListWriter : PrefabListWriter
{
	protected override void SavePrefabList()
	{
		FroggerLevelManagerDefault manager = (FroggerLevelManagerDefault)target;

		if (!Directory.Exists(saveLocation))
		{
			Directory.CreateDirectory(saveLocation);
		}

		StreamWriter writer = new StreamWriter(saveLocation + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + "_prefabs.txt");

		writer.WriteLine("Available prefabs for scene " + Path.GetFileNameWithoutExtension(EditorApplication.currentScene));
		writer.WriteLine();

		// Write the different types of lanes
		writer.WriteLine("Lane types:");
		foreach(GameObject lane in manager.lanePrefabs)
		{
			writer.WriteLine("\t- " + lane.name);
		}
		writer.WriteLine();

		// Write the different types of lane items
		writer.WriteLine("Lane item types:");
		foreach(GameObject laneItem in manager.laneItemPrefabs)
		{
			writer.WriteLine("\t- " + laneItem.name);
		}

		writer.Close();
	}
}
#endif