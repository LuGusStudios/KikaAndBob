using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FroggerConfigLoader : LugusSingletonExisting<FroggerConfigLoaderDefault>{

}

public class FroggerConfigLoaderDefault : MonoBehaviour
{
	private List<string> configFiles = new List<string>();
	private string configPath = string.Empty;

	void Awake()
	{
		configPath = Application.dataPath + "/Config/Frogger/";

		if (!Directory.Exists(configPath))
		{
			Debug.LogError("The directory " + configPath + " does not exist.");
			return;
		}

		string[] files = Directory.GetFiles(configPath, "*.xml");
		FroggerLevelDefinition[] levels = new FroggerLevelDefinition[files.Length];
		for (int i = 0; i < files.Length; ++i )
		{
			StreamReader reader = new StreamReader(files[i]);
			string rawdata = reader.ReadToEnd();

			TinyXmlReader parser = new TinyXmlReader(rawdata);

			while (parser.Read())
			{
				if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
					(parser.tagName == "Level"))
				{
					FroggerLevelDefinition level = FroggerLevelDefinition.FromXML(parser);
					level.name = Path.GetFileNameWithoutExtension(files[i]);
					levels[i] = level;
					//SaveConfig(level);
				}
			}
		}

		FroggerLevelManager.use.levels = levels;
	}

	private void SaveConfig(FroggerLevelDefinition level)
	{
		if (!Directory.Exists(configPath))
			Directory.CreateDirectory(configPath);

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += FroggerLevelDefinition.ToXML(level);

		StreamWriter writer = new StreamWriter(configPath + level.name + ".xml");
		writer.Write(rawdata);
		writer.Close();
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		for (int i = 0; i < FroggerLevelManager.use.levels.Length; i++)
		{
			if (GUILayout.Button("Start Level " + i))
			{
				FroggerGameManager.use.StartNewGame(i);
			}
		}
	}
}
