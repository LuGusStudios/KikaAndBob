#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ExtractPrefabList : MonoBehaviour 
{	
	protected void Awake()
	{

		string sceneName = Application.loadedLevelName;
		string prefabsPath = Application.dataPath + "/Scenes/Minigames/" + sceneName + "/Prefabs/";

		if (!Directory.Exists(prefabsPath))
		{
			Debug.Log("Path does not exits: " + prefabsPath);
			return;
		}

		StreamWriter stream = new StreamWriter(prefabsPath + Application.loadedLevelName + "_prefablist.txt");
		stream.WriteLine("List of prefabs available for scene: " + Application.loadedLevelName);

		string[] dirs = Directory.GetDirectories(prefabsPath);
		foreach(string dir in dirs)
		{
			stream.WriteLine();
			stream.WriteLine(Path.GetFileNameWithoutExtension(dir) + ":");
			
			string[] prefabsLits = Directory.GetFiles(dir + "/", "*.prefab");
			foreach (string prefab in prefabsLits)
			{
				stream.WriteLine("\t- " + Path.GetFileNameWithoutExtension(prefab));
			}
		}

		string[] prefabsList = Directory.GetFiles(prefabsPath, "*.prefab");
		stream.WriteLine();
		foreach (string prefab in prefabsList)
		{
			stream.WriteLine("- " + Path.GetFileNameWithoutExtension(prefab));
		}

		stream.Close();
	}
}
#endif