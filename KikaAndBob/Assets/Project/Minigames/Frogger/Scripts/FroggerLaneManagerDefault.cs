using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneManager : LugusSingletonExisting<FroggerLaneManagerDefault> {
	
}

public class FroggerLaneManagerDefault: MonoBehaviour
{
	List<FroggerLane> lanes = new List<FroggerLane>();

	public void FindLanes()
	{
		FroggerLane[] lanesInScene = (FroggerLane[]) FindObjectsOfType(typeof(FroggerLane));
		lanes = OrderLanes(new List<FroggerLane>(lanesInScene));
	}

	// TO DO: Remove! This is just a quick way of sorting layers (by y location) for testing until we have a customizable way to enter lane sequences.
	protected List<FroggerLane> OrderLanes(List<FroggerLane> unordered)
	{
		List<FroggerLane> ordered = new List<FroggerLane>();
		int listCount = unordered.Count;

		while (ordered.Count < listCount)
		{
			int currentlyLowestIndex = -1;
			float currentlyLowest = Mathf.Infinity;

			for (int i = 0; i < unordered.Count; i++) 
			{
				if (unordered[i].transform.position.y < currentlyLowest)
				{
					currentlyLowestIndex = i;
					currentlyLowest = unordered[i].transform.position.y;
				}
			}

			FroggerLane toBeMoved = unordered[currentlyLowestIndex];
			unordered.Remove(toBeMoved);
			ordered.Add(toBeMoved);
		}

		return ordered;
	}
	
	public FroggerLane GetLane(int index)
	{
		if (index < 0)
		{
			index = 0;
		}
		else if (index >= lanes.Count)
		{
			index = lanes.Count - 1;
		}

		return lanes[index];
	}

	public int GetLaneIndex(FroggerLane lane)
	{
		if(!lanes.Contains(lane))
		{ 
			Debug.LogError("Lane does not exist in lane manager!");
			return -1;
		}

		return lanes.IndexOf(lane);
	}

	public FroggerLane GetLaneAbove(FroggerLane lane)
	{
		return GetLane(GetLaneIndex(lane) + 1);
	}

	public FroggerLane GetLaneBelow(FroggerLane lane)
	{
		return GetLane(GetLaneIndex(lane) - 1);
	}

}
