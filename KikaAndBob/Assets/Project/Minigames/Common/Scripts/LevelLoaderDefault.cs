using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_WEBPLAYER
	using System.IO;
#endif

public class LevelLoaderDefault
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

		IMinigameCrossSceneInfo crossSceneInfo = GetCrossSceneInfo();
		
		if (crossSceneInfo != null)
		{
			crossSceneInfo.SetLevelIndex(index);
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	// TO DO: this will return CrossSceneInfo for relevant game. Will probably be moved elsewhere.
	public static IMinigameCrossSceneInfo GetCrossSceneInfo()	
	{

		if (Application.loadedLevelName == "e04_tasmania" || Application.loadedLevelName == "e05_Mexico" 
		    || Application.loadedLevelName == "e09_Brazil" || Application.loadedLevelName == "e10_Swiss"
		    || Application.loadedLevelName == "e13_pacific" || Application.loadedLevelName == "e19_illinois")
		{
			return RunnerCrossSceneInfo.use;
		}
		else if (Application.loadedLevelName == "e01_kenia"
			|| Application.loadedLevelName == "e07_france"
			|| Application.loadedLevelName == "e17_greenland"
			|| Application.loadedLevelName == "e18_amsterdam"
			|| Application.loadedLevelName == "e21_cuba")
		{
			return FroggerCrossSceneInfo.use;
		}
		else if (Application.loadedLevelName == "e06_egypt" || Application.loadedLevelName == "e11_vatican" 
		         || Application.loadedLevelName == "e23_england" || Application.loadedLevelName == "e25_sicily")
		{
			return PacmanCrossSceneInfo.use;
		}
		else if (Application.loadedLevelName == "e03_china"
			|| Application.loadedLevelName == "e20_morocco"
		    || Application.loadedLevelName == "e15_india"
			|| Application.loadedLevelName == "e24_japan" )
		{
			return DanceHeroCrossSceneInfo.use;
		}
		else if (Application.loadedLevelName == "e14_buthan"
			|| Application.loadedLevelName == "e08_texas"
		    || Application.loadedLevelName == "e22_russia")
		{
			return DartsCrossSceneInfo.use;
		}	
		else if( Application.loadedLevelName == "e02_argentina" || Application.loadedLevelName == "e12_newyork" 
		        || Application.loadedLevelName == "e16_israel" || Application.loadedLevelName == "e26_belgium" )
		{
			return DinnerDashCrossSceneInfo.use;
		}
		
		Debug.LogError("StepLevelMenu: " + Application.loadedLevelName + " is an unknown scene!");
		return null;
	}
}
