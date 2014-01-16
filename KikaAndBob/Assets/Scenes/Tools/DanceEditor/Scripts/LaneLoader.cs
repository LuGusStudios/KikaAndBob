using UnityEngine;
using System.IO;
using System.Collections;

public class LaneLoader {

	public static void LoadLanes()
	{
		StreamReader reader = new StreamReader(Application.dataPath + "/ActionFiles/ChinaSong02.xml");
		string rawdata = reader.ReadToEnd();
		reader.Close();

		TinyXmlReader parser = new TinyXmlReader(rawdata);

		DanceHeroLevel.use.mode = DanceHeroLevel.TimeProgressionMode.PER_LANE;

		int laneCount = 0;
		while (parser.Read())
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "AudioClip"))
			{
				Debug.Log("This action file is made for the audio clip " + parser.content);
			}
			else if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "Lane"))
			{
				ParseLane(parser, laneCount);
				++laneCount;
			}
		}
	}

	private static void ParseLane(TinyXmlReader parser, int laneNr)
	{
		if (laneNr > DanceHeroLevel.use.lanes.Count - 1)
			return;

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) && (parser.tagName != "Lane"))
			return;

		DanceHeroLane lane = DanceHeroLevel.use.lanes[laneNr];

		while (parser.Read("Lane"))
		{

		}
	}

}
