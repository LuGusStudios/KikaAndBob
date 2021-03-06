﻿#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(PacmanLevelManagerDefault))]
public class PacmanPrefabListWriter : PrefabListWriter
{
	protected override void SavePrefabList()
	{
		PacmanLevelManagerDefault manager = (PacmanLevelManagerDefault)target;

		if (!Directory.Exists(saveLocation))
		{
			Directory.CreateDirectory(saveLocation);
		}

		StreamWriter writer = new StreamWriter(saveLocation + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + "_prefabs.txt");

		writer.WriteLine("Available prefabs for scene " + Path.GetFileNameWithoutExtension(EditorApplication.currentScene));
		writer.WriteLine();

		// Write the different types of character
		writer.WriteLine("Character types:");
		foreach (PacmanCharacter character in manager.characterPrefabs)
		{
			writer.WriteLine("\t- " + character.name);
		}
		writer.WriteLine();

		// Write the different types of tile items
		writer.WriteLine("Tile item types:");
		foreach (GameObject tileItem in manager.tileItems)
		{
			writer.WriteLine("\t- " + tileItem.name);
		}

		writer.Close();

		Debug.Log("Wrote prefab list to: " + saveLocation);
	}
}
#endif