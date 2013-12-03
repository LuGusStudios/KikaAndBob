using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLane : FroggerSurface 
{
	public float height = 200;
	public float spawnPerSecond = 0.2f;
	public List<FroggerLaneItem> spawnItems = new List<FroggerLaneItem>();

	protected float spawnTimer = 0;

	void Update()
	{
		if (spawnItems.Count < 1)
		{
			return;
		}

		if (spawnTimer >= 1 / spawnPerSecond)
		{
			Spawn();
			spawnTimer = 0;
		}
		spawnTimer += Time.deltaTime;
	}

	void Spawn()
	{
		int index = Random.Range(0, spawnItems.Count);
		GameObject spawnedItem = (GameObject)Instantiate(spawnItems[index].gameObject);
		spawnedItem.transform.parent = this.transform;
		spawnedItem.transform.localPosition = Vector3.zero;
		spawnedItem.transform.localRotation = Quaternion.identity;

		FroggerLaneItem itemScript = spawnedItem.GetComponent<FroggerLaneItem>();



	}
	
}
