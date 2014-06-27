#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(CatchingMiceLevelManagerDefault))]
public class CatchingMicePrefabWriter : Editor
{
	public string saveLocation = Application.dataPath + "/Config/Levels/";

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Save prefab list", GUILayout.Width(100), GUILayout.Height(20)))
		{
			SavePrefabList();
		}

		DrawDefaultInspector();
	}

	private void SavePrefabList()
	{
		CatchingMiceLevelManagerDefault manager = (CatchingMiceLevelManagerDefault)target;

		if (!Directory.Exists(saveLocation))
		{
			Directory.CreateDirectory(saveLocation);
		}

		StreamWriter writer = new StreamWriter(saveLocation + Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + "_prefabs.txt");

		writer.WriteLine("Available prefabs for scene " + Path.GetFileNameWithoutExtension(EditorApplication.currentScene));
		writer.WriteLine();

		// Enemies - mice
		writer.WriteLine("Enemies (mice):");
		foreach (CatchingMiceCharacterMouse enemy in manager.enemyPrefabs)
		{
			writer.WriteLine("\t- " + enemy.name);
		}
		writer.WriteLine();

		// Enemies - patrols
		writer.WriteLine("Enemies (patrols):");
		foreach(CatchingMiceCharacterPatrol enemy in manager.patrolPrefabs)
		{
			writer.WriteLine("\t- " + enemy.name);
		}
		writer.WriteLine();

		// Furniture
		writer.WriteLine("Furniture:");
		foreach (CatchingMiceFurniture item in manager.furniturePrefabs)
		{
			writer.WriteLine("\t- " + item.name);
		}
		writer.WriteLine();

		// Cheese items
		writer.WriteLine("Cheese:");
		foreach (CatchingMiceCheese item in manager.cheesePrefabs)
		{
			writer.WriteLine("\t- " + item.name);
		}
		writer.WriteLine();

		// Hole items
		writer.WriteLine("Mice Hole:");
		foreach (CatchingMiceHole item in manager.holePrefabs)
		{
			writer.WriteLine("\t- " + item.name);
		}
		writer.WriteLine();

		// Traps items
		writer.WriteLine("Traps:");
		foreach (CatchingMiceTrap item in manager.trapPrefabs)
		{
			writer.WriteLine("\t- " + item.name);
		}
		writer.WriteLine();

		// Obstacles
		writer.WriteLine("Obstacles:");
		foreach (CatchingMiceObstacle obstacle in manager.obstaclePrefabs)
		{
			writer.WriteLine("\t- " + obstacle.name);
		}
		writer.WriteLine();

		writer.Close();
	}
}
#endif