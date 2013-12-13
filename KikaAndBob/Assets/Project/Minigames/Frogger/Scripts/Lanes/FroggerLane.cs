using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FroggerLane : FroggerSurface 
{
	public bool goRight = true;
	public float speed = 2;
	public float minGapDistance = 2;
	public float maxGapDistance = 4;
	public float repeatAllowFactor = 0.5f;
	public List<FroggerLaneItem> spawnItems = new List<FroggerLaneItem>();

	protected float height = 200;
	protected Vector2 laneSize = Vector3.one;
	protected BoxCollider2D boxCollider2D = null;
	protected float nextInterval = 0;
	protected int lastItemIndex = -1;
	protected float spawnDistance = 0;
	protected List<FroggerLaneItem> spawnedItems = new List<FroggerLaneItem>();

	private void Awake()
	{
		boxCollider2D = GetComponent<BoxCollider2D>();
		
		if (boxCollider2D != null)
		{
			laneSize = boxCollider2D.size;
			height = laneSize.y;
		}
	}

	public void FillLane()
	{
		if (spawnItems.Count < 1)
			return;

		float laneCompletion = 0;
		float lastItemWidth = 0;

		// keep spawning item until the lane is completely full (then make one more to ensure there can also be logs halfway off the screen towards the end)
		while(laneCompletion < laneSize.x)
		{
			GameObject newItem = SpawnLaneItem();

			if (goRight)
			{
				newItem.transform.Translate(new Vector3(laneCompletion, 0, 0), Space.World);
			}
			else
			{
				newItem.transform.Translate(new Vector3(-laneCompletion, 0, 0), Space.World);
			}

			lastItemWidth = newItem.GetComponent<FroggerLaneItem>().GetSurfaceSize().x;
			lastItemWidth += Random.Range(minGapDistance, maxGapDistance);

			laneCompletion += lastItemWidth;
		}

		nextInterval = lastItemWidth;
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
		if (spawnItems.Count < 1 || speed <= 0)
		{
			return;
		}
	
		if (spawnDistance >= nextInterval)
		{
			GameObject spawned = SpawnLaneItem();
			spawnDistance = 0;
			nextInterval = Random.Range(minGapDistance, maxGapDistance) + spawned.GetComponent<FroggerSurface>().GetSurfaceSize().x;
		}

		float displacement = speed * Time.deltaTime;
		spawnDistance += displacement;

		for (int i = spawnedItems.Count - 1; i >= 0; i--) 
		{
			FroggerLaneItem currentItem = spawnedItems[i];
			currentItem.UpdateLaneItem(displacement);

			if (currentItem.CrossedLevel())
			{
				spawnedItems.Remove(currentItem);
				Destroy(currentItem.gameObject);
			}
		}

//		foreach(FroggerLaneItem item in spawnedItems)
//		{
//			item.UpdateLaneItem(displacement);
//		}

//		foreach(Transform t in transform)
//		{
//			if (goRight)
//				t.Translate(t.right.normalized * displacement);
//			else
//				t.Translate(-1 * t.right.normalized * displacement);
//
//			t.GetComponent<FroggerLaneItem>().UpdateLaneItem();
//		}
	}

	private GameObject SpawnLaneItem()
	{
		// create random item
		int index = Random.Range(0, spawnItems.Count);

		// prevent repetitions with some factor
		if (spawnItems.Count > 1)
		{
			while (index == lastItemIndex && Random.value > repeatAllowFactor)
			{
				index = Random.Range(0, spawnItems.Count);
			}
		}

		lastItemIndex = index;

		GameObject spawnedItem = (GameObject)Instantiate(spawnItems[index].gameObject);

		FroggerLaneItem itemScript = spawnedItem.GetComponent<FroggerLaneItem>();
		itemScript.goRight = goRight;
		itemScript.SetLaneDistance(GetSurfaceSize().x + itemScript.GetSurfaceSize().x * 2);

		// make the height of the spawned item's collider equal to the lane's height - this way it will vertically cover the entire lane no matter what the height that was set
		BoxCollider2D itemCollider = spawnedItem.GetComponent<BoxCollider2D>();
		itemCollider.size = itemCollider.size.y(height/itemCollider.transform.localScale.y); // compensate for potential sprite scaling !
		itemCollider.center = itemCollider.center.y(boxCollider2D.center.y);
	
		spawnedItem.transform.parent = this.transform;
		spawnedItem.transform.localPosition = Vector3.zero;
		spawnedItem.transform.localRotation = Quaternion.identity;

		// place item just past edge of the lane: get edge of lane and add half of the sprite's size
		if (goRight)
		{
			if (itemScript.behindPlayer)
				spawnedItem.transform.localPosition = new Vector3(-((laneSize.x * 0.5f) + itemScript.GetSurfaceSize().x * 0.5f), 0, -1);
			else
				spawnedItem.transform.localPosition = new Vector3(-((laneSize.x * 0.5f) + itemScript.GetSurfaceSize().x * 0.5f), 0, -10);


			if (spawnedItem.GetComponent<FlippedIncorrectly>() == null)
				spawnedItem.transform.localScale = spawnedItem.transform.localScale.x(spawnedItem.transform.localScale.x * -1f);
		}
		else
		{
			if (itemScript.behindPlayer)
				spawnedItem.transform.localPosition = new Vector3(((laneSize.x * 0.5f) + itemScript.GetSurfaceSize().x * 0.5f), 0, -1);
			else
				spawnedItem.transform.localPosition = new Vector3(((laneSize.x * 0.5f) + itemScript.GetSurfaceSize().x * 0.5f), 0, -10);

			if (spawnedItem.GetComponent<FlippedIncorrectly>() != null)
				spawnedItem.transform.localScale = spawnedItem.transform.localScale.x(spawnedItem.transform.localScale.x * -1f);
		}

		spawnedItems.Add(itemScript);

		return spawnedItem;
	}
}
