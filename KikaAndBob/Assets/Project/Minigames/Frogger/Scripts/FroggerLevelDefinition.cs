using UnityEngine;
using System.Collections;

public class FroggerLevelDefinition : ScriptableObject {

	[System.Serializable]
	public class LaneDefinition
	{
		public string laneID = "";
		public bool goRight = true;
		public float speed = 4;
		public float minGapDistance = 4;
		public float maxGapDistance = 6;
		public float repeatAllowFactor = 0.2f;
		public string[] laneItems = new string[0];

		public LaneDefinition()
		{
			laneID = "";
			goRight = true;
			speed = 4;
			minGapDistance = 4;
			maxGapDistance = 6;
			repeatAllowFactor = 0.2f;
			laneItems = new string[0];
		}
	}

	public LaneDefinition[] lanes;

}
