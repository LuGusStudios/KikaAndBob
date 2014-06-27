using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_WEBPLAYER
using System.IO;
#endif

public class CatchingMiceLevelLoader
{
	public enum LoadingSource { NONE = -1, SYSTEM_IO = 1, RESOURCES = 2 };

#if !UNITY_WEBPLAYER
	public LoadingSource source = LoadingSource.SYSTEM_IO;
#else
	public LoadingSource source = LoadingSource.RESOURCES;
#endif

	public List<int> levelIndices = new List<int>();

	protected int configLoadingHardCap = 20;

	public List<int> FindLevels()
	{
		levelIndices = new List<int>();

		// Find the indices of the levels that can be loaded
		for (int i = 0; i < configLoadingHardCap; ++i)
		{
			string levelName = Application.loadedLevelName + "_level_" + (i + 1).ToString();

			switch (source)
			{
				case LoadingSource.SYSTEM_IO:
#if !UNITY_WEBPLAYER
					if (File.Exists(Application.dataPath + "/Config/Levels/" + levelName + ".xml"))
					{
						levelIndices.Add(i + 1);
					}
#else
					Debug.LogError("Cannot find configs using System.IO while using a web player.");
#endif
					break;
				case LoadingSource.RESOURCES:

					TextAsset asset = LugusResources.use.Shared.GetTextAsset(levelName);
					if (asset != LugusResources.use.errorTextAsset)
					{
						levelIndices.Add(i + 1);
					}

					break;
			}
		}

		return levelIndices;
	}

	public string GetLevelData(int levelIndex)
	{
		string levelData = string.Empty;

		// Load the level data from either LugusResources or the Config folder
		string levelName = Application.loadedLevelName + "_level_" + (levelIndex).ToString();
		switch (source)
		{
			case LoadingSource.SYSTEM_IO:

#if !UNITY_WEBPLAYER
				string configPath = Application.dataPath + "/Config/Levels/" + levelName + ".xml";
				if (File.Exists(configPath))
				{
					levelData = File.ReadAllText(configPath);
				}
#else
				Debug.LogError("Cannot find configs using System.IO while using a web player.");
#endif
				break;

			case LoadingSource.RESOURCES:

				TextAsset asset = LugusResources.use.Shared.GetTextAsset(levelName);
				if (asset != LugusResources.use.errorTextAsset)
				{
					levelData = asset.text;
				}

				break;
		}

		return levelData;
	}

	public void LoadLevel(int index)
	{
		Debug.Log("LevelLoaderDefault: Attempting to load level: " + index);

		CatchingMiceCrossSceneInfo crossSceneInfo = GetCrossSceneInfo();

		if (crossSceneInfo != null)
		{
			crossSceneInfo.LevelToLoad = index;
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	// TO DO: this will return CrossSceneInfo for relevant game. Will probably be moved elsewhere.
	public static CatchingMiceCrossSceneInfo GetCrossSceneInfo()
	{
		return CatchingMiceCrossSceneInfo.use;
	}
}
