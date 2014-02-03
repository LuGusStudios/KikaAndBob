using UnityEngine;
using System.Collections;

[System.Serializable]
public class FroggerLevelDefinition : ScriptableObject {

	public static FroggerLevelDefinition FromXML(TinyXmlReader parser)
	{
		FroggerLevelDefinition level = ScriptableObject.CreateInstance<FroggerLevelDefinition>();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "FroggerLevelDefinition"))
		{
			Debug.Log("FroggerLevelDefition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while (parser.Read("FroggerLevelDefinition"))
		{

		}

		return level;
	}

	public static string ToXML(FroggerLevelDefinition level, int depth)
	{
		string rawdata = string.Empty;

		if (level == null)
		{
			Debug.Log("FroggerLevelDefinition.ToXML(): The level to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; ++i)
			tabs += "\t";

		rawdata += tabs + "<FroggerLevelDefinition>\r\n";
		rawdata += tabs + "\t<BackgroundMusicName>" + level.backgroundMusicName + "</BackgroundMusicName>\r\n";

		foreach (FroggerLaneDefinition lane in level.lanes)
			rawdata += FroggerLaneDefinition.ToXML(lane, depth + 1);

		rawdata += tabs + "</FroggerLevelDefinition>\r\n";

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
