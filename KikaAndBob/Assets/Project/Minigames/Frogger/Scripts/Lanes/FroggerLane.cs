using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FroggerLane : FroggerSurface 
{
	protected float height = 200;
	protected Vector2 laneSize = Vector3.one;
	protected BoxCollider2D boxCollider2D = null;
	public float spawnPerSecond = 0.2f;
	public List<FroggerLaneItem> spawnItems = new List<FroggerLaneItem>();

	protected float spawnTimer = 0;

	private void Awake()
	{
		boxCollider2D = GetComponent<BoxCollider2D>();
		
		if (boxCollider2D != null)
		{
			laneSize = boxCollider2D.size;
			height = laneSize.y;
		}
	}

	public float GetHeight()
	{
		return height;
	}

	// the transform of the lane is not a good indication for its functional center - e.g. on the last lane, the walkable area is considerably smaller than than the sprite
	// instead we return the transform position, offset by the collider center (which is in local coords)
	public Vector2 GetCenterPoint()
	{
		return transform.position.v2() + boxCollider2D.center;
	}

	private void Update()
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

	private void Spawn()
	{
		// create random item
		int index = Random.Range(0, spawnItems.Count);
		GameObject spawnedItem = (GameObject)Instantiate(spawnItems[index].gameObject);

		// make the height of the spawned item's collider equal to the lane's height - this way it will vertically cover the entire lane
		BoxCollider2D itemCollider = spawnedItem.GetComponent<BoxCollider2D>();
		itemCollider.size = itemCollider.size.y(height);

		itemCollider.center = itemCollider.center.y(boxCollider2D.center.y);
	
		spawnedItem.transform.parent = this.transform;

		// place item just past edge of the lane: get edge of lane and add half of the sprite's size
		spawnedItem.transform.localPosition = new Vector3(-((laneSize.x * 0.5f) + itemCollider.size.x * 0.5f), 0, -1);
		spawnedItem.transform.localRotation = Quaternion.identity;

	}
	
}
