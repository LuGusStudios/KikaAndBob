using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLevelManager : LugusSingletonExisting<FroggerLevelManagerDefault> {
	
}

public class FroggerLevelManagerDefault : MonoBehaviour
{
	public FroggerLevelDefinition[] levels;
	public GameObject foreground = null;
	public GameObject[] lanePrefabs;
	public GameObject[] laneItemPrefabs;
	protected LugusAudioTrackSettings musicSettings = null;

	void Awake()
	{
		musicSettings = new LugusAudioTrackSettings();
		musicSettings.Loop(true).Volume(0.2f);
	}

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

		FroggerLevelDefinition level = levels[levelIndex];

		BuildLevel(level);

		PlayLevelMusic(level);
	}

	protected void PlayLevelMusic(FroggerLevelDefinition level)
	{
		LugusAudio.use.SFX().StopAll();
		LugusAudio.use.Music().StopAll();

		if (!string.IsNullOrEmpty(level.backgroundMusicName))
		{
			LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio(level.backgroundMusicName), true, musicSettings);
		}
		else
		{
			Debug.LogWarning("Level music name was null or empty.");
		}
	}

	public void BuildLevel(FroggerLevelDefinition level)
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
	
		#if UNITY_EDITOR
		// clear existing level
			for (int i = lanesRoot.childCount - 1; i >= 0; i--) 
			{
				DestroyImmediate(lanesRoot.GetChild(i).gameObject);
			}
		#else
			for (int i = lanesRoot.childCount - 1; i >= 0; i--) 
			{
				Destroy(lanesRoot.GetChild(i).gameObject);
			}
		#endif

		// set up new level
		foreach (FroggerLaneDefinition laneDefinition in level.lanes) 
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
			laneScript.scrollingSpeed = laneDefinition.backgroundScrollingSpeed;
			laneScript.dynamicSpawnItems = FindMovingLaneItems(laneDefinition.spawnItems);
			laneScript.staticSpawnItems = FindStaticLaneItems(laneDefinition.spawnItems, laneGameObject);

			// we can't link lane item sounds to lanes directly, since we can't predict what sort of lane items will be placed on which lane
			// instead, LaneItems have a lanePresenceSound property which, if not null, will be added to the lane's enter sounds
//			foreach(FroggerLaneItem laneItem in laneScript.dynamicSpawnItems)
//			{
//				if (laneItem.lanePresenceSoundKey != null && !laneScript.enterSoundKeys.Contains(laneItem.lanePresenceSoundKey))
//				{
//					laneScript.enterSoundKeys.Add(laneItem.lanePresenceSoundKey);
//				}
//			}
	
			// initially fill lane with lane items
			laneScript.SetUpLane();

			lanes.Add(laneScript);

			laneIndex++;
		}

		// add the final lane manually because its active area is nowhere near the sprite size, which requires special treatment and makes it an allround PITA
		//AppendFinalLane(currentY, laneIndex, lanesRoot);

		FroggerLaneManager.use.SetLanes(lanes);

		// finally create a fake lane to cover up any room between the bottom lane's collider and the bottom of the sprite (which is what the camera will clamp to)
		// there is no easy way to calculate the overlap needed here, so just add a bit of customizable overlap

		if (lanes.Count > 0)
		{
			float overlap = 0.1f;
			GameObject foregroundObject = (GameObject)Instantiate(foreground);
			foregroundObject.transform.parent = lanesRoot;
			foregroundObject.transform.localPosition = new Vector3(0, lanes[0].transform.localPosition.y - lanes[0].GetComponent<SpriteRenderer>().bounds.extents.y + overlap, -10);
		}

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

	protected List<FroggerLaneItem> FindMovingLaneItems(FroggerLaneItemDefinition[] ids)
	{
		List<FroggerLaneItem> laneItems = new List<FroggerLaneItem>();
		foreach(FroggerLaneItemDefinition laneItemDefinition in ids)
		{
			if (laneItemDefinition.positioning > 0)
				continue;

			if (string.IsNullOrEmpty(laneItemDefinition.spawnID))
			{
				Debug.LogError("Lane item ID was empty or null!");
				continue;
			}

			GameObject foundPrefab = null;
			foreach(GameObject go in laneItemPrefabs)
			{
				if (go.name == laneItemDefinition.spawnID)
				{
					foundPrefab = go;
					break;
				}
			}

			if (foundPrefab == null)
			{
				Debug.LogError(laneItemDefinition.spawnID + " was not found as a lane item prefab.");
				continue;
			}

			laneItems.Add(foundPrefab.GetComponent<FroggerLaneItem>());
		}

		return laneItems;
	}

	protected Dictionary<float, FroggerLaneItem> FindStaticLaneItems(FroggerLaneItemDefinition[] ids, GameObject parentLane)
	{
		Dictionary<float, FroggerLaneItem> laneItems = new Dictionary<float, FroggerLaneItem>();

		foreach(FroggerLaneItemDefinition laneItemDefinition in ids)
		{
			if (laneItemDefinition.positioning <= 0)
				continue;
			
			if (string.IsNullOrEmpty(laneItemDefinition.spawnID))
			{
				Debug.LogError("Lane item ID was empty or null!");
				continue;
			}

			if (laneItems.ContainsKey(Mathf.Clamp(laneItemDefinition.positioning, 0, 1)))
			{
				Debug.LogError("There is already something placed at exactly this location on this lane. Lane: " + parentLane.name +" Item: " + laneItemDefinition.spawnID + " Position: " + laneItemDefinition.positioning);
				continue;
			}
			
			GameObject foundPrefab = null;
			foreach(GameObject go in laneItemPrefabs)
			{
				if (go.name == laneItemDefinition.spawnID)
				{
					foundPrefab = go;
					break;
				}
			}
			
			if (foundPrefab == null)
			{
				Debug.LogError("Lane: " + parentLane.name + ". " +  laneItemDefinition.spawnID + " was not found as a lane item prefab.");
				continue;
			}

			laneItems.Add(Mathf.Clamp(laneItemDefinition.positioning, 0, 1), foundPrefab.GetComponent<FroggerLaneItem>());
		}
		
		return laneItems;
	}
}

