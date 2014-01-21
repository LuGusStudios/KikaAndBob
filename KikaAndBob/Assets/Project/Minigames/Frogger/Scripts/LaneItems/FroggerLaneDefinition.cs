using UnityEngine;
using System.Collections;

[System.Serializable]
public class FroggerLaneDefinition
{
	public string laneID = "Enter new lane definition";
	public bool goRight = true;
	public float speed = 4;
	public float minGapDistance = 4;
	public float maxGapDistance = 6;
	public float repeatAllowFactor = 0.2f;
	public float backgroundScrollingSpeed = 0;
	public FroggerLaneItemDefinition[] spawnItems;
}
