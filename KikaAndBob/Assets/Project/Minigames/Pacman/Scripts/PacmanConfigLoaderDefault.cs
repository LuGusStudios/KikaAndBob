using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PacmanConfigLoader : LugusSingletonExisting<PacmanConfigLoaderDefault>
{

}

public class PacmanConfigLoaderDefault : MonoBehaviour 
{
	private List<string> configFiles = new List<string>();
	private string configPath = string.Empty;

	void Awake()
	{
		configPath = Application.dataPath + "/Config/Pacman/";

		if (!Directory.Exists(configPath))
		{
			Debug.LogError("The directory " + configPath + " does not exist.");
			return;
		}

		string[] files = Directory.GetFiles(configPath, "*.xml");
		PacmanLevelDefinition[] levels = new PacmanLevelDefinition[files.Length];
		for (int i = 0; i < files.Length; ++i)
		{
			StreamReader reader = new StreamReader(files[i]);
			string rawdata = reader.ReadToEnd();

			TinyXmlReader parser = new TinyXmlReader(rawdata);

			while (parser.Read())
			{
				if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
					(parser.tagName == "Level"))
				{
					PacmanLevelDefinition level = PacmanLevelDefinition.FromXML(parser);
					level.name = Path.GetFileNameWithoutExtension(files[i]);
					levels[i] = level;
					//SaveConfig(level);
				}
			}
		}

		PacmanLevelManager.use.levels = levels;
	}

	private void SaveConfig(PacmanLevelDefinition level)
	{
		if (!Directory.Exists(configPath))
			Directory.CreateDirectory(configPath);

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += PacmanLevelDefinition.ToXML(level);

		StreamWriter writer = new StreamWriter(configPath + level.name + ".xml");
		writer.Write(rawdata);
		writer.Close();
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
		{
			return;
		}

		for (int i = 0; i < PacmanLevelManager.use.levels.Length; i++)
		{
			if (GUILayout.Button("Level " + i))
			{
				PacmanGameManager.use.StartNewLevel(i);
			}
		}
	}
}
