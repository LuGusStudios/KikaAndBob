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
			Debug.LogError("PacmanConfigLoader: The directory " + configPath + " does not exist.");
			return;
		}

		string[] files = Directory.GetFiles(configPath, "*.xml");

		// using a list instead of an array here, so we only add an entry if the xml was parsed successfully
		//PacmanLevelDefinition[] levels = new PacmanLevelDefinition[files.Length];
		List<PacmanLevelDefinition> levels = new List<PacmanLevelDefinition>();	

		for (int i = 0; i < files.Length; ++i)
		{
			StreamReader reader = new StreamReader(files[i]);
			string rawdata = reader.ReadToEnd();

			TinyXmlReader parser = new TinyXmlReader(rawdata);

			bool success = true;

			PacmanLevelDefinition level = null;

			while (parser.Read())
			{
				try
				{
					if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
						(parser.tagName == "Level"))
					{
						level = PacmanLevelDefinition.FromXML(parser);
						level.name = Path.GetFileNameWithoutExtension(files[i]);
						//levels.Add(level);
						//SaveConfig(level);
					}
				}
				catch(System.Exception e)
				{
					Debug.LogError("PacmanConfigLoader: There was an error parsing XML file: " + files[i] + ". Skipping this file. Error reprinted below: ");
					Debug.LogError(e.ToString());

					success = false;
					break;
				}
			}

			if (success)
				levels.Add(level);
		}

		//PacmanLevelManager.use.levels = levels;
		PacmanLevelManager.use.levels = levels.ToArray();
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
}
