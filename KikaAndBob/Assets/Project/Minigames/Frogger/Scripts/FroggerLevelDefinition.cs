using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FroggerLevelDefinition : ScriptableObject {

	public static FroggerLevelDefinition FromXML(TinyXmlReader parser)
	{
		// Check whether the parser is at the correct tag first.
		// Next, we parse the properties of the levels, and the lanes.
		// The lanes are enumerated in reverse order, from a design perspective,
		// so they need to be reordered back at the end.

		FroggerLevelDefinition level = ScriptableObject.CreateInstance<FroggerLevelDefinition>();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Level"))
		{
			Debug.Log("FroggerLevelDefition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		List<FroggerLaneDefinition> lanes = new List<FroggerLaneDefinition>();

		while (parser.Read("Level"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "BackgroundMusicName":
						level.backgroundMusicName = parser.content;
						break;
					case "Lane":
						lanes.Add(FroggerLaneDefinition.FromXML(parser));
						break;
				}
			}
		}

		lanes.Reverse();
		level.lanes = lanes.ToArray();

		return level;
	}

	public static string ToXML(FroggerLevelDefinition level)
	{
		// Write all of the level's properties to XML.
		// A level's lanes are enumerated in reverse order, to ease editing the files.

		string rawdata = string.Empty;

		if (level == null)
		{
			Debug.Log("FroggerLevelDefinition.ToXML(): The level to be serialized is null.");
			return rawdata;
		}

		rawdata += "<Level>\r\n";
		rawdata += "\t<BackgroundMusicName>" + level.backgroundMusicName + "</BackgroundMusicName>\r\n";

		rawdata += "\t<Lanes>\r\n";
		for (int i = level.lanes.Length - 1; i >= 0; --i )
		{
			rawdata += FroggerLaneDefinition.ToXML(level.lanes[i], 2);
		}
		rawdata += "\t</Lanes>\r\n";

		rawdata += "</Level>\r\n";

		return rawdata;
	}

	public string backgroundMusicName = "";
	public FroggerLaneDefinition[] lanes;

	// Arrays of serialized classes are not created with default values
	// Instead, initialize values once in OnEnable (which runs AFTER deserialization), checking for null / zero value
	// http://forum.unity3d.com/threads/155352-Serialization-Best-Practices-Megapost
	void OnEnable()
	{
		if (lanes == null || lanes.Length < 1)
		{
			// Since we always want at least one lane in the Frogger games, it makes sense to initially provide one lane already.
			// This way, the constructor for the array's type will get called, which sets the default values.
			// Further array entries are always copied from the last entry anyway, so any default values are copied.
			Debug.Log("Initializing FroggerLevelDefinition");
			lanes = new FroggerLaneDefinition[1];
		}
	}
}
