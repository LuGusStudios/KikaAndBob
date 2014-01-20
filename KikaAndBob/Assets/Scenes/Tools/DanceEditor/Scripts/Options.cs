using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;

public class Options : LugusSingletonRuntime<Options>
{
	public Vector2 screenOffset = new Vector2(10, 10);
	public string actionFolderName = "ActionFiles";

	void Start()
	{
		LugusConfigProfileDefault profile = new LugusConfigProfileDefault("System");
		profile.Load();

		LugusConfig.use.System = profile;

		Load();
	} 

	void Update()
	{
		if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
			Input.GetKeyDown(KeyCode.S))
			Save();
	}

	void OnGUI()
	{
		int xpos = Screen.width - 100 - (int)screenOffset.x;
		int ypos = (int)screenOffset.y;

		GUILayout.BeginArea(new Rect(xpos, ypos, 100, 50), GUI.skin.box);
		GUILayout.BeginVertical();

		GUIStyle centered = new GUIStyle(GUI.skin.label);
		centered.alignment = TextAnchor.UpperCenter;
		GUILayout.Label("Options", centered);

		if (GUILayout.Button("Save"))
			Save();

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	void Save()
	{
		SaveActionFile(LugusConfig.use.System.GetString("ActionFile", string.Empty));
	}

	void Load()
	{
		LoadAudioFile(LugusConfig.use.System.GetString("AudioClip", string.Empty));
		LoadActionFile(LugusConfig.use.System.GetString("ActionFile", string.Empty));
	}

	void SaveActionFile(string name)
	{

		if (string.IsNullOrEmpty(name))
		{
			Debug.LogError("SaveActionFile(name): The name of the file is null or empty. The data was not saved.");
			return;
		}

		string lanesdata = string.Empty;
		lanesdata += "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		lanesdata += "<Actions>\r\n";

		// Write the name of the audio file these actions are based on
		string clipname = string.Empty;
		if (AudioPlayer.use.Source.clip != null)
			clipname = AudioPlayer.use.Source.clip.name;

		lanesdata += "\t<AudioClip>" + clipname + "</AudioClip>\r\n";
		lanesdata += "\t<Bookmarks>\r\n" + Bookmarks.use.ToXML(2) + "\t</Bookmarks>\r\n";

		// Write the actions of each lane
		foreach (Lane lane in LaneManager.use.Lanes)
			lanesdata += lane.ToXML(1);

		lanesdata += "</Actions>\r\n";

		if (!Directory.Exists(Application.dataPath + "/" + actionFolderName))
			Directory.CreateDirectory(Application.dataPath + "/" + actionFolderName);

		string fullpath = Application.dataPath + "/" + actionFolderName + "/" + name + ".xml";

		StreamWriter writer = new StreamWriter(fullpath);
		writer.Write(lanesdata);
		writer.Close();
	}

	void LoadAudioFile(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogError("LoadAudioFile(name): The name of the audio file to be loaded is null or empty.");
			return;
		}
		
		AudioClip clip = LugusResources.use.Shared.GetAudio(name);

		if (clip == null)
		{
			Debug.LogError("LoadAudioFile(name): The audio resource is null.");
			return;
		}

		AudioPlayer.use.Source.clip = clip;
	}

	void LoadActionFile(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogError("LoadActionFile(name): The name of the action file to be loaded is null or empty.");
			return;
		}

		if (!Directory.Exists(Application.dataPath + "/" + actionFolderName))
		{
			Debug.LogError("LoadActionFile(name): The directory " + Application.dataPath + "/" + actionFolderName + " does not exist.");
			return;
		}

		string fullpath = Application.dataPath + "/" + actionFolderName + "/" + name + ".xml";

		// When we can find the action file, start parsing it with TinyXML.
		// Only when we find an opening tag that matches either a lane or a bookmark, we can translate it.
		if (File.Exists(Application.dataPath + "/" + actionFolderName + "/" + name + ".xml"))
		{
			StreamReader reader = new StreamReader(fullpath, Encoding.Default);
			string rawdata = reader.ReadToEnd();
			reader.Close();

			TinyXmlReader parser = new TinyXmlReader(rawdata);
			while (parser.Read())
			{

				// When encountering other types of tags, we can skip them.
				if (parser.tagType != TinyXmlReader.TagType.OPENING)
					continue;

				if (parser.tagName == "Lane")
				{
					LaneManager.use.CreateLane(parser);
				}
				else if (parser.tagName == "Bookmarks")
				{
					Bookmarks.use.FromXML(parser);
				}
			}
		}
		else
		{
			Debug.Log("LoadActionFile(name): The action file " + name + " does not exist. The file will be created when saving data.");
		}
	}

}
