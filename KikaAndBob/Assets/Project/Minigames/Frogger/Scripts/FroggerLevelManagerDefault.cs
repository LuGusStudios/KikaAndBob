using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLevelManager : LugusSingletonExisting<FroggerLevelManagerDefault> {
	
}

public class FroggerLevelManagerDefault : MonoBehaviour
{
	public FroggerLevelDefinition[] levels;
	public GameObject[] lanePrefabs;

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
			float heightBelowTransform = (( laneCollider.size.y * 0.5f) + laneCollider.center.y);
			currentY += heightBelowTransform;

			laneGameObject.transform.localPosition = new Vector3(0, currentY, laneIndex * 10);

			float heightAboveTransform = (( laneCollider.size.y * 0.5f) - laneCollider.center.y);
			currentY += heightAboveTransform;
		
			lanes.Add(laneScript);

			laneIndex++;
		}

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
	
}

