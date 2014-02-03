using UnityEngine;
using System.Collections;

[System.Serializable]
public class FroggerLaneDefinition
{
	public static FroggerLaneDefinition FromXML(TinyXmlReader parser)
	{
		FroggerLaneDefinition lane = new FroggerLaneDefinition();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "FroggerLaneDefinition"))
		{
			Debug.Log("FroggerLaneDefition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while (parser.Read("FroggerLaneDefinition"))
		{

		}

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

		rawdata += tabs + "<FroggerLaneDefinition>\r\n";

		// Go over all properties
		rawdata += tabs + "\t<LaneID>" + lane.laneID + "</LaneID>\r\n";
		rawdata += tabs + "\t<GoRight>" + lane.goRight.ToString() + "</GoRight>\r\n";
		rawdata += tabs + "\t<Speed>" + lane.speed.ToString() + "</Speed>\r\n";
		rawdata += tabs + "\t<MinGapDistance>" + lane.minGapDistance.ToString() + "</MinGapDistance>\r\n";
		rawdata += tabs + "\t<MaxGapDistance>" + lane.maxGapDistance.ToString() + "</MaxGapDistance>\r\n";
		rawdata += tabs + "\t<RepeatAllowFactor>" + lane.repeatAllowFactor.ToString() + "</RepeatAllowFactor>\r\n";
		rawdata += tabs + "\t<BackgroundScrollingSpeed>" + lane.backgroundScrollingSpeed.ToString() + "</BackgroundScrollingSpeed>\r\n";

		foreach (FroggerLaneItemDefinition laneitem in lane.spawnItems)
			rawdata += FroggerLaneItemDefinition.ToXML(laneitem, depth + 1);

		rawdata += tabs + "</FroggerLaneDefinition>\r\n";

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
