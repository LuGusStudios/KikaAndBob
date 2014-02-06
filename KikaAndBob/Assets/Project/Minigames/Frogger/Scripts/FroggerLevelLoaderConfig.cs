using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FroggerLevelLoaderConfig : MonoBehaviour
{

	public static FroggerLevelLoaderConfig use
	{
		get
		{
			FroggerLevelLoaderConfig loader = (FroggerLevelLoaderConfig)GameObject.FindObjectOfType<FroggerLevelLoaderConfig>();
			if (loader == null)
			{
				GameObject container = new GameObject("FroggerLevelLoaderConfig");
				loader = container.AddComponent<FroggerLevelLoaderConfig>();
				DontDestroyOnLoad(container);
			}

			return loader;
		}
	}

	private string[] configFiles;
	private string configPath = string.Empty;
	private string levelNamePattern = string.Empty;

	void Awake()
	{
		DontDestroyOnLoad(this.transform.gameObject);

		configPath = Application.dataPath + "/Config/Frogger/";
		levelNamePattern = Application.loadedLevelName + "_level_*.xml";

		if (!Directory.Exists(configPath))
		{
			Debug.LogError("The directory " + configPath + " does not exist.");
			return;
		}

		// Get the config files and load the first one in the list, if there is one
		configFiles = Directory.GetFiles(configPath, levelNamePattern);
		if (configFiles.Length > 0)
		{
			LoadLevel(0);
		}
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
		{
			return;
		}

		// Display buttons with the names of the file by removing path and extension information.
		// When the button of a level is pushed, its config is loaded from file, 
		// and the loaded scriptable object is placed as the only one in the list of levels,
		// so its always index 0.
		for (int i = 0; i < configFiles.Length; ++i)
		{
			string name = configFiles[i];
			if (GUILayout.Button("Level " + Path.GetFileNameWithoutExtension(name)))
			{
				LoadLevel(i);
				FroggerGameManager.use.StartNewGame(0);
			}
		}
	}

	private void SaveLevel(FroggerLevelDefinition level)
	{
		if (!Directory.Exists(configPath))
		{
			Directory.CreateDirectory(configPath);
		}

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += FroggerLevelDefinition.ToXML(level);

		StreamWriter writer = new StreamWriter(configPath + level.name + ".xml");
		writer.Write(rawdata);
		writer.Close();
	}

	private void LoadLevel(int index)
	{
		StreamReader reader = new StreamReader(configFiles[index]);
		string rawdata = reader.ReadToEnd();

		TinyXmlReader parser = new TinyXmlReader(rawdata);

		FroggerLevelDefinition level = null;
		while (parser.Read("Level"))
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
				(parser.tagName == "Level"))
			{
				level = FroggerLevelDefinition.FromXML(parser);
				level.name = Path.GetFileNameWithoutExtension(configFiles[index]);
			}
		}

		if (level != null)
		{
			Application.LoadLevel(Application.loadedLevelName);

			FroggerLevelDefinition[] levels = { level };
			FroggerLevelManager.use.levels = levels;
		}
	}

	private IEnumerator CheckConfigDirectory()
	{
		while (true)
		{
			configFiles = Directory.GetFiles(configPath, levelNamePattern);
			yield return new WaitForSeconds(10f);
		}
	}
}
