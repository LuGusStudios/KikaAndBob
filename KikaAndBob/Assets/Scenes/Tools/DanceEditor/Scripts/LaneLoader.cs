using UnityEngine;
using System.IO;
using System.Collections;

public class LaneLoader {

	public static void LoadLanes()
	{

		// Load the action file from Resources and start parsing.
		// Each lane is parsed separately.
		// The lanes in the editor are numbered from top to bottom.
		// The game's (you just lost it) lanes are numbered from bottom to top.

		string rawdata = LugusResources.use.Shared.providers[0].GetText(LugusResources.use.Shared.URL, "ChinaSong02").text;
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
				ParseLane(parser, DanceHeroLevel.use.lanes.Count - 1 - laneCount);
				++laneCount;
			}
		}
	}

	private static void ParseLane(TinyXmlReader parser, int laneNr)
	{
		// Example Lane xml:
		/**
		 *	<Lane>
		 *		<Item>
		 *			<Time>4.096</Time>
		 *			<Duration>0.256</Duration>
		 *		</Item>
		 *	</Lane>
		 **/

		// When parsing an item, its time is compared to the
		// previous item's time in this lane and a delay is set.
		// Items whose time is too close to the beginning of the music
		// are ignored and will not spawn in the game.

		if (laneNr > DanceHeroLevel.use.lanes.Count - 1)
			return;

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) && (parser.tagName != "Lane"))
			return;

		string laneName = "Lane" + (laneNr + 1).ToString();
		DanceHeroLane lane = DanceHeroLevel.use.GetLane(laneName);

		// Delay between the action point and spawning from the character
		float constDelay = Vector2.Distance(lane.transform.position.v2(), lane.actionPoint.position.v2()) / lane.speed;
		float prevTime = 0.0f;

		// Parse the lane
		int itemCount = 0;
		while (parser.Read("Lane"))
		{
			if ((parser.tagType == TinyXmlReader.TagType.OPENING) && (parser.tagName == "Item"))
			{
				float time = 0.0f;
				float duration = 0.0f;

				// Parse the lane item
				while (parser.Read("Item"))
				{
					if (parser.tagType != TinyXmlReader.TagType.OPENING)
						continue;

					if (parser.tagName == "Time")
						time = float.Parse(parser.content.Trim());
					else if (parser.tagName == "Duration")
						duration = float.Parse(parser.content.Trim());
				}

				float delay = time - prevTime;

				if (itemCount == 0)
					delay -= constDelay;

				if (delay > 0f)
				{
					prevTime = time;
					lane.AddItem(delay, duration);
					++itemCount;
				}
			}
		}
	}

}
