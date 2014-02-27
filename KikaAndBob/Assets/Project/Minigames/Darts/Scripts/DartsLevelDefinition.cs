using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsLevelDefinition : ScriptableObject 
{
	public float levelDuration = 0.0f;
	public int minimumScore = 0;
	public string backgroundMusicName = "";

	public static DartsLevelDefinition FromXML(string rawData)
	{
		TinyXmlReader parser = new TinyXmlReader(rawData);

		while (parser.Read())
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) &&
				(parser.tagName == "Level"))
			{
				return DartsLevelDefinition.FromXML(parser);
			}
		}

		// If we end up here, then no level tag was found...
		return null;
	}

	public static DartsLevelDefinition FromXML(TinyXmlReader parser)
	{
		// Check whether the parser is at the correct tag first.
		// Next, we parse the properties of the levels, and the lanes.
		// The lanes are enumerated in reverse order, from a design perspective,
		// so they need to be reordered back at the end.

		DartsLevelDefinition level = ScriptableObject.CreateInstance<DartsLevelDefinition>();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Level"))
		{
			Debug.Log("DartsLevelDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		List<DartsGroupDefinition> groups = new List<DartsGroupDefinition>();
		while (parser.Read("Level"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "Group":
						groups.Add(DartsGroupDefinition.FromXML(parser));
						break;
					case "BackgroundMusicName":
					level.backgroundMusicName = parser.content;
						break;
					case "LevelDuration":
						level.levelDuration = float.Parse(parser.content);
						break;
					case "MinimumScore":
						level.minimumScore = int.Parse(parser.content);
						break;
				}
			}
		}

		level.groupDefinitions = groups.ToArray();
		return level;
	}

	public static string ToXML(DartsLevelDefinition level)
	{
		string rawData = string.Empty;

		if (level == null)
		{
			Debug.Log("DartsLevelDefinition.ToXML(): The level to be serialized is null.");
			return rawData;
		}

		rawData += "<Level>\r\n";

		rawData += "\t<BackgroundMusicName>" + level.backgroundMusicName + "</BackgroundMusicName>\r\n";

		rawData += "\t<LevelDuration>" + level.levelDuration.ToString() + "</LevelDuration>\r\n";

		rawData += "\t<MinimumScore>" + level.minimumScore.ToString() + "</MinimumScore>\r\n";

		rawData += "\t<Groups>\r\n";
		foreach (DartsGroupDefinition group in level.groupDefinitions)
		{
			rawData += DartsGroupDefinition.ToXML(group, 2);
		}
		rawData += "\t</Groups>\r\n";
		rawData += "</Level>\r\n";

		return rawData;
	}

	public DartsGroupDefinition[] groupDefinitions;

	// Arrays of serialized classes are not created with default values
	// Instead, initialize values once in OnEnable (which runs AFTER deserialization), checking for null / zero value
	// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
	protected void OnEnable()
	{
		
	}

}

[System.Serializable]
public class DartsGroupDefinition
{
	public static DartsGroupDefinition FromXML(TinyXmlReader parser)
	{
		DartsGroupDefinition group = new DartsGroupDefinition();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Group"))
		{
			Debug.Log("DartsGroupDefinition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while(parser.Read("Group"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "ID":
						group.id = parser.content.Trim();
						break;
					case "ItemsOnScreen":
						group.itemsOnScreen = float.Parse(parser.content.Trim());
						break;
					case "MinTimeBetweenShows":
						group.minTimeBetweenShows = float.Parse(parser.content.Trim());
						break;
					case "AutoHideTimes":
						DataRange range = new DataRange(0f, 1f);
						while (parser.Read("AutoHideTimes"))
						{
							if (parser.tagType == TinyXmlReader.TagType.OPENING)
							{
								switch (parser.tagName)
								{
									case "Min":
										range.from = float.Parse(parser.content.Trim());
										break;
									case "Max":
										range.to = float.Parse(parser.content.Trim());
										break;
								}
							}
						}
						group.autoHideTimes = range;
						break;
					case "AvoidRepeat":
						group.avoidRepeat = bool.Parse(parser.content.Trim());
						break;
				}
			}
		}

		return group;
	}

	public static string ToXML(DartsGroupDefinition group, int depth)
	{
		string rawData = string.Empty;

		if (group == null)
		{
			Debug.Log("DartsGroupDefinition.ToXML(): The level to be serialized is null.");
			return rawData;
		}

		// A string representing the indentation of the tags
		string tabs = string.Empty;
		for (int i = 0; i < depth; i++)
			tabs += "\t";

		rawData += tabs + "<Group>\r\n";
		rawData += tabs + "\t<ID>" + group.id + "</ID>\r\n";
		rawData += tabs + "\t<MinTimeBetweenShows>" + group.minTimeBetweenShows.ToString() + "</MinTimeBetweenShows>\r\n";
		rawData += tabs + "\t<AutoHideTimes>\r\n";
		rawData += tabs + "\t\t<Min>" + group.autoHideTimes.from.ToString() + "</Min>\r\n";
		rawData += tabs + "\t\t<Max>" + group.autoHideTimes.to.ToString() + "</Max>\r\n";
		rawData += tabs + "\t</AutoHideTimes>\r\n";
		rawData += tabs + "\t<AvoidRepeat>" + group.avoidRepeat.ToString() + "</AvoidRepeat>\r\n";
		rawData += tabs + "</Group>\r\n";

		return rawData;
	}

	public string id = "";
	public float itemsOnScreen = 1.0f;
	public float minTimeBetweenShows = 1.0f;
	public DataRange autoHideTimes = new DataRange(2.0f, 4.0f);
	public bool avoidRepeat = false;
}
