using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FroggerLaneDefinition
{
	public static FroggerLaneDefinition FromXML(TinyXmlReader parser)
	{
		FroggerLaneDefinition lane = new FroggerLaneDefinition();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "Lane"))
		{
			Debug.Log("FroggerLaneDefition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		List<FroggerLaneItemDefinition> laneitems = new List<FroggerLaneItemDefinition>();
		while (parser.Read("Lane"))
		{
			if (parser.tagType == TinyXmlReader.TagType.OPENING)
			{
				switch (parser.tagName)
				{
					case "LaneID":
						lane.laneID = parser.content;
						break;
					case "GoRight":
						lane.goRight = bool.Parse(parser.content);
						break;
					case "Speed":
						lane.speed = float.Parse(parser.content);
						break;
					case "MinGapDistance":
						lane.minGapDistance = float.Parse(parser.content);
						break;
					case "MaxGapDistance":
						lane.maxGapDistance = float.Parse(parser.content);
						break;
					case "RepeatAllowFactor":
						lane.repeatAllowFactor = float.Parse(parser.content);
						break;
					case "BackgroundScrollingSpeed":
						lane.backgroundScrollingSpeed = float.Parse(parser.content);
						break;
					case "LaneItem":
						laneitems.Add(FroggerLaneItemDefinition.FromXML(parser));
						break;
				}
			}
		}

		lane.spawnItems = laneitems.ToArray();

		return lane;
	}

	public static string ToXML(FroggerLaneDefinition lane, int depth)
	{
		string rawdata = string.Empty;

		if (lane == null)
		{
			Debug.Log("FroggerLaneDefinition.ToXML(): The lane to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; i++)
			tabs += "\t";

		rawdata += tabs + "<Lane>\r\n";

		// Go over all properties
		rawdata += tabs + "\t<LaneID>" + lane.laneID + "</LaneID>\r\n";
		rawdata += tabs + "\t<GoRight>" + lane.goRight.ToString() + "</GoRight>\r\n";
		rawdata += tabs + "\t<Speed>" + lane.speed.ToString() + "</Speed>\r\n";
		rawdata += tabs + "\t<MinGapDistance>" + lane.minGapDistance.ToString() + "</MinGapDistance>\r\n";
		rawdata += tabs + "\t<MaxGapDistance>" + lane.maxGapDistance.ToString() + "</MaxGapDistance>\r\n";
		rawdata += tabs + "\t<RepeatAllowFactor>" + lane.repeatAllowFactor.ToString() + "</RepeatAllowFactor>\r\n";
		rawdata += tabs + "\t<BackgroundScrollingSpeed>" + lane.backgroundScrollingSpeed.ToString() + "</BackgroundScrollingSpeed>\r\n";

		rawdata += tabs + "\t<LaneItems>\r\n";
		foreach (FroggerLaneItemDefinition laneitem in lane.spawnItems)
		{
			rawdata += FroggerLaneItemDefinition.ToXML(laneitem, depth + 2);
		}
		rawdata += tabs + "\t</LaneItems>\r\n";

		rawdata += tabs + "</Lane>\r\n";

		return rawdata;
	}

	public string laneID = "Enter new lane definition";
	public bool goRight = true;
	public float speed = 4;
	public float minGapDistance = 4;
	public float maxGapDistance = 6;
	public float repeatAllowFactor = 0.2f;
	public float backgroundScrollingSpeed = 0;
	public FroggerLaneItemDefinition[] spawnItems;
}
