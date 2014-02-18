#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public abstract class PrefabListWriter : Editor 
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

	protected abstract void SavePrefabList();
}
#endif