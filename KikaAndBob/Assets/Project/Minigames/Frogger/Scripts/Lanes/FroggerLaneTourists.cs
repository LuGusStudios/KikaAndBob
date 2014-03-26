using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class FroggerLaneTourists : FroggerLane 
{
	public string touristWalkingAnimation = "";
	public string touristIdleAnimation = "";

	public Vector2 idlePositionRange = new Vector2(0f, 1f);	// Should be between 0 and 1
	public Vector2 idleTimeRange = new Vector2(1f, 3f);

	public float touristTryAgainTimer = 0.1f;

	protected List<FroggerLaneItemTourist> tourists = new List<FroggerLaneItemTourist>();
	
	protected override void Update () 
	{
		ScrollBackground();

		if (dynamicSpawnItems.Count < 1 || speed <= 0)
			return;

		if (spawnDistance >= nextInterval)
		{
			GameObject spawned = SpawnLaneItem();
			spawnDistance = 0;
			nextInterval = Random.Range(minGapDistance, maxGapDistance) + spawned.GetComponent<FroggerSurface>().GetSurfaceSize().x;
		}

		float displacement = speed * Time.deltaTime;
		spawnDistance += displacement;

		Vector2 range = new Vector2(-(laneSize.x * 0.5f) + (laneSize.x * idlePositionRange.x), -(laneSize.x * 0.5f) + (laneSize.x * idlePositionRange.y));

		for (int i = dynamicSpawnedItems.Count - 1; i >= 0; i--)
		{
			FroggerLaneItem currentItem = dynamicSpawnedItems[i];

			FroggerLaneItemTourist tourist = currentItem.GetComponent<FroggerLaneItemTourist>();
			if (tourist != null)
			{
				if (tourist.HasIdled
					|| ((tourist.transform.localPosition.x < range.x)
						&& (tourist.transform.localPosition.x > range.y))
					|| (tourist.TryAgainTimer > 0f))
				{
					currentItem.UpdateLaneItem(displacement);
				}
				else if (tourist.State != FroggerLaneItemTourist.TouristState.IDLE)
				{
					float rndm = LugusRandom.use.Uniform.Next(0f, 1f);
					//float rndm = LugusRandom.use.In.Next(0f, 1f);
					//float rndm = LugusRandom.use.Gaussian.Next(0f, 1f);
					float t;
					if (goRight)
					{
						t = Mathf.InverseLerp(range.x, range.y, tourist.transform.localPosition.x);
					}
					else
					{
						t = Mathf.InverseLerp(range.y, range.x, tourist.transform.localPosition.x);
					}

					//Debug.Log("Random value: " + rndm + " T value: " + t);

					if (rndm < t)
					{
						float time = Random.Range(idleTimeRange.x, idleTimeRange.y);
						tourist.StartIdle(time);
					}
					else
					{
						currentItem.UpdateLaneItem(displacement);
						tourist.TryAgainTimer = touristTryAgainTimer;
					}
				}
			}
			else
			{
				currentItem.UpdateLaneItem(displacement);
			}


			if (currentItem.CrossedLevel())
			{
				dynamicSpawnedItems.Remove(currentItem);
				Destroy(currentItem.gameObject);
			}
		}
	}
}
