using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PacmanConfigLoader : LugusSingletonExisting<PacmanConfigLoaderDefault>
{

}

public class PacmanConfigLoaderDefault : MonoBehaviour 
{
	private string[] configFiles;
	private string configPath = string.Empty;

	void Awake()
	{
		configPath = Application.dataPath + "/Config/Pacman/";

		if (!Directory.Exists(configPath))
		{
			Debug.LogError("The directory " + configPath + " does not exist.");
			return;
		}

		// Get the config files and load the first one in the list, of there is one
		configFiles = Directory.GetFiles(configPath, "*.xml");
		if (configFiles.Length > 0)
		{
			LoadConfig(0);
		}
	}

	void OnGUI()
	{
		if (!LugusDebug.debug)
		{
			return;
		}

		// Display buttons with the names of the file by removing path and extension information
		for (int i = 0; i < configFiles.Length; ++i)
		{
			string name = configFiles[i];
			if (GUILayout.Button("Level " + Path.GetFileNameWithoutExtension(name)))
			{
				LoadConfig(i);
				PacmanGameManager.use.StartNewLevel(0);
			}
		}
	}

	public void SaveConfig(PacmanLevelDefinition level)
	{
		if (!Directory.Exists(configPath))
		{
			Directory.CreateDirectory(configPath);
		}

		string rawdata = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += PacmanLevelDefinition.ToXML(level);

		StreamWriter writer = new StreamWriter(configPath + level.name + ".xml");
		writer.Write(rawdata);
		writer.Close();
	}

	private void LoadConfig(int index)
	{
		StreamReader reader = new StreamReader(configFiles[index]);
		string rawdata = reader.ReadToEnd();

		TinyXmlReader parser = new TinyXmlReader(rawdata);

		PacmanLevelDefinition level = null;
		while (parser.Read("Level"))
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
				(parser.tagName == "Level"))
			{
				level = PacmanLevelDefinition.FromXML(parser);
				level.name = Path.GetFileNameWithoutExtension(configFiles[index]);
				//SaveConfig(level);
			}
		}

		PacmanLevelDefinition[] levels = {level};
		PacmanLevelManager.use.levels = levels;
	}

	private IEnumerator CheckConfigDirectory()
	{
		while (true)
		{
			configFiles = Directory.GetFiles(configPath, "*.xml");
			yield return new WaitForSeconds(10f);
		}
	}
}
