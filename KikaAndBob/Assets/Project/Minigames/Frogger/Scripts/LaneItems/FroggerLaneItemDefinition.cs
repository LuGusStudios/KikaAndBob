﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class FroggerLaneItemDefinition 
{
	
	public static FroggerLaneItemDefinition FromXML(TinyXmlReader parser)
	{
		FroggerLaneItemDefinition laneitem = new FroggerLaneItemDefinition();

		if ((parser.tagType != TinyXmlReader.TagType.OPENING) ||
			(parser.tagName != "FroggerLaneItemDefinition"))
		{
			Debug.Log("FroggerLaneItemDefition.FromXML(): unexpected tag type or tag name.");
			return null;
		}

		while (parser.Read("FroggerLaneItemDefinition"))
		{
			
		}

		return laneitem;
	}

	public static string ToXML(FroggerLaneItemDefinition laneitem, int depth)
	{
		string rawdata = string.Empty;

		if (laneitem == null)
		{
			Debug.Log("FroggerLaneItemDefinition.ToXML(): The lane item to be serialized is null.");
			return rawdata;
		}

		string tabs = string.Empty;
		for (int i = 0; i < depth; i++)
			tabs += "\t";

		rawdata += tabs + "<FroggerLaneItemDefinition>\r\n";

		rawdata += tabs + "\t<SpawnID>" + laneitem.spawnID + "</SpawnID>\r\n";
		rawdata += tabs + "\t<Positioning>" + laneitem.positioning.ToString() + "</Positioning>\r\n";

		rawdata += tabs + "</FroggerLaneItemDefinition>\r\n";

		return rawdata;
	}

	public string spawnID = "";
	public float positioning = -1.0f; 
	
	public FroggerLaneItemDefinition()
	{
		spawnID = "";
		positioning = -1f;
	}

}
