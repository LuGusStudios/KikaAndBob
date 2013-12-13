using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneManager : LugusSingletonExisting<FroggerLaneManagerDefault> {

}

public class FroggerLaneManagerDefault: MonoBehaviour
{
	protected List<FroggerLane> lanes = new List<FroggerLane>();
	protected float levelLengthLanePixels = -1;
	protected float levelLengthLaneCenters = -1;

//	public void FindLanes()
//	{
//		lanes.Clear();
//		levelLengthLanePixels = -1;
//		levelLengthLaneCenters = -1;
//		FroggerLane[] lanesInScene = (FroggerLane[]) FindObjectsOfType(typeof(FroggerLane));
//		lanes = OrderLanes(new List<FroggerLane>(lanesInScene));
//	}

	public void SetLanes(List<FroggerLane> _lanes)
	{
		levelLengthLanePixels = -1;
		levelLengthLaneCenters = -1;
		lanes.Clear();
		lanes = _lanes;
	}

	public List<FroggerLane> GetLanes()
	{
		return lanes;
	}

	// TO DO: Remove! This is just a quick way of sorting layers (by y location) for testing until we have a customizable way to enter lane sequences.
	protected List<FroggerLane> OrderLanes(List<FroggerLane> unordered)
	{
		List<FroggerLane> ordered = new List<FroggerLane>();
		int listCount = unordered.Count;

		// order lanes in list
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

//		// automatically depth-sort lanes
//		for (int i = 0; i < ordered.Count; i++) 
//		{
//			ordered[i].transform.position = ordered[i].transform.position.z(i * 10);
//		}


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

	// gets level size between bottom of the bottom sprite and the top of the topmost sprite in SCREEN COORDINATES
	public float GetLevelLengthLanePixels()
	{
		if (lanes.Count < 1)
			return 0;

		// unless level has changed, there's no point to recalculating levelLength - it will get reset to -1 every time the level is rebuilt
		if (levelLengthLanePixels >= 0)
			return levelLengthLanePixels;

		Bounds firstSpriteBounds = lanes[0].GetComponent<SpriteRenderer>().sprite.bounds;
		Bounds lastSpriteBounds = lanes[lanes.Count - 1].GetComponent<SpriteRenderer>().sprite.bounds;

		levelLengthLanePixels = LugusCamera.game.WorldToScreenPoint(lanes[lanes.Count - 1].transform.position + lastSpriteBounds.max).y -
			LugusCamera.game.WorldToScreenPoint(lanes[0].transform.position + firstSpriteBounds.min).y;

		return levelLengthLanePixels;
	}

	// gets level size between center of bottom lane and center of top lane
	public float GetLevelLengthLaneCenters()
	{
		if (lanes.Count < 1)
			return 0;
		
		// unless level has changed, there's no point to recalculating levelLength - it will get reset to -1 every time the level is rebuilt
		if (levelLengthLaneCenters >= 0)
			return levelLengthLaneCenters;
		
		levelLengthLaneCenters = Vector2.Distance(GetBottomLaneCenter(), GetTopLaneCenter());
		
		return levelLengthLaneCenters;
	}

	public Vector2 GetBottomLaneCenter()
	{
		if (lanes.Count < 1)
			return Vector2.zero;

		return lanes[0].GetCenterPoint();
	}

	public Vector2 GetTopLaneCenter()
	{
		if (lanes.Count < 1)
			return Vector2.zero;
		
		return lanes[lanes.Count - 1].GetCenterPoint();
	}

	public float GetBottomLaneBottomPixel()
	{
		if (lanes.Count < 1)
			return 0;

		if (lanes[0] == null)
			return 0;

		return (lanes[0].transform.position + lanes[0].GetComponent<SpriteRenderer>().sprite.bounds.min).y;
	}

	public float GetTopLaneTopPixel()
	{
		if (lanes.Count < 1)
			return 0;

		if (lanes[lanes.Count - 1] == null)
			return Screen.height;
		
		return (lanes[lanes.Count - 1].transform.position + lanes[lanes.Count - 1].GetComponent<SpriteRenderer>().sprite.bounds.max).y;
	}
}
