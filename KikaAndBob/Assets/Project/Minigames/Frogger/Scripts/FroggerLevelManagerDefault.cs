using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLevelManager : LugusSingletonExisting<FroggerLevelManagerDefault> {
	
}

public class FroggerLevelManagerDefault : MonoBehaviour
{
	public FroggerLevelDefinition[] levels;
	public GameObject[] lanePrefabs;
	public GameObject[] laneItemPrefabs;

	public void LoadLevel(string levelName)
	{

	}

	public void LoadLevel(int levelIndex)
	{
		if (levelIndex < 0 || levelIndex >= levels.Length)
		{
			Debug.LogError("Level index was out of bounds!");
			return;
		}

		Debug.Log("Loading level: " + levelIndex);

		BuildLevel(levels[levelIndex]);
	}

	protected void BuildLevel(FroggerLevelDefinition level)
	{
		if (level == null)
		{
			Debug.LogError("Level definition was null!");
			return;
		}

		Transform lanesRoot = GameObject.Find("Level/Lanes").transform;
		int laneIndex = 0;
		float currentY = 0;
		List<FroggerLane> lanes = new List<FroggerLane>();

		// clear existing level
		for (int i = lanesRoot.childCount - 1; i >= 0; i--) 
		{
			Destroy(lanesRoot.GetChild(i).gameObject);
		}

		// set up new level
		foreach (FroggerLevelDefinition.LaneDefinition laneDefinition in level.lanes) 
		{
			GameObject prefab = FindLane(laneDefinition.laneID);

			if (prefab == null)
				continue;

			GameObject laneGameObject = (GameObject)Instantiate(prefab);
			FroggerLane laneScript = laneGameObject.GetComponent<FroggerLane>();
			BoxCollider2D laneCollider = laneGameObject.GetComponent<BoxCollider2D>();

			laneGameObject.transform.parent = lanesRoot;

			// place lane
			// at this point currentY is set to the top of the collider of the previous lane in world coordinates

			// first we calculate how much space there is between the new lane's transform and the bottom of its collider
			// BoxCollider2D does not have a Bounds property, which makes this a bit more of a PITA than it should be.
			float heightBelowTransform = Mathf.Abs(( laneCollider.size.y * 0.5f) - laneCollider.center.y);
			currentY += heightBelowTransform;

			// place the lane where it belongs, 
			// at X 0, lining up with the previous lane's collider on the Y axis and 10 units 
			// further in the Z direction for every lane to leave room for stuff in between (player, lane items etc.)
			laneGameObject.transform.localPosition = new Vector3(0, currentY, laneIndex * 10);

			// finally, increase currentY to the top of the new lane's collider
			float heightAboveTransform = Mathf.Abs(( laneCollider.size.y * 0.5f) + laneCollider.center.y);
			currentY += heightAboveTransform;
	
			// read values from scriptable object
			laneScript.goRight = laneDefinition.goRight;
			laneScript.speed = laneDefinition.speed;
			laneScript.minGapDistance = laneDefinition.minGapDistance;
			laneScript.maxGapDistance = laneDefinition.maxGapDistance;
			laneScript.repeatAllowFactor = laneDefinition.repeatAllowFactor;
			laneScript.spawnItems = FindLaneItems(laneDefinition.spawnItems);
	
			// initially fill lane with lane items
			laneScript.FillLane();

			lanes.Add(laneScript);

			laneIndex++;
		}

		// add the final lane manually because its active area is nowhere near the sprite size, which requires special treatment and makes it an allround PITA
		//AppendFinalLane(currentY, laneIndex, lanesRoot);

		FroggerLaneManager.use.SetLanes(lanes);

		Debug.Log("Finished building level.");
	}


	protected GameObject FindLane(string laneID)
	{
		if (string.IsNullOrEmpty(laneID))
		{
			Debug.LogError("Lane ID was empty or null!");
			return null;
		}

		foreach(GameObject go in lanePrefabs)
		{
			if (go.name == laneID)
				return go;
		}

		Debug.LogError(laneID + " was not found as a lane prefab.");

		return null;
	}

	protected List<FroggerLaneItem> FindLaneItems(string[] ids)
	{
		List<FroggerLaneItem> laneItems = new List<FroggerLaneItem>();
		foreach(string id in ids)
		{
			if (string.IsNullOrEmpty(id))
			{
				Debug.LogError("Lane item ID was empty or null!");
				continue;
			}

			GameObject foundPrefab = null;
			foreach(GameObject go in laneItemPrefabs)
			{
				if (go.name == id)
				{
					foundPrefab = go;
					break;
				}
			}

			if (foundPrefab == null)
			{
				Debug.LogError(id + " was not found as a lane item prefab.");
				continue;
			}

			laneItems.Add(foundPrefab.GetComponent<FroggerLaneItem>());
		}

		return laneItems;
	}
	
}

