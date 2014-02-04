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
	public float scrollingSpeed = 0;
	public List<FroggerLaneItem> dynamicSpawnItems = new List<FroggerLaneItem>();
	public Dictionary<float, FroggerLaneItem> staticSpawnItems = new Dictionary<float, FroggerLaneItem>();

	protected float height = 200;
	protected Vector2 laneSize = Vector3.one;
	protected float nextInterval = 0;
	protected int lastItemIndex = -1;
	protected float spawnDistance = 0;
	protected List<FroggerLaneItem> dynamicSpawnedItems = new List<FroggerLaneItem>();	// includes all items that need to be moved (e.g. excludes things like rocks)
	protected List<FroggerLaneItem> staticSpawnedItems = new List<FroggerLaneItem>();	// includes all items that need to be moved (e.g. excludes things like rocks)
	protected Transform scrollingBackground = null;
	protected Vector2 scrollingOffset = Vector2.zero;

	protected void Awake()
	{
		SetUpLocal();
	}

	public override void SetUpLocal()
	{
		base.SetUpLocal();
		
		if (surfaceCollider != null)
		{
			laneSize = surfaceCollider.size;
			height = laneSize.y;
		}
		else
		{
			Debug.Log(name + ": Missing BoxCollider2D");
		}
	}
	
	public virtual void SetUpLane()
	{
		// if needed, make visual copies of the lane to allow scrolling lane backgrounds
		// only necessary if speed is something not zero
		if (scrollingSpeed != 0)
		{
			Transform backgroundParent = new GameObject("BackgroundParent").transform;
			backgroundParent.transform.position = this.transform.position;
			backgroundParent.parent = this.transform;
			scrollingBackground = backgroundParent;

		  	SpriteRenderer originalSpriteRender = this.GetComponent<SpriteRenderer>();

			Transform middleCopy = new GameObject("Copy").transform;
			SpriteRenderer spriteRenderer = middleCopy.gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = originalSpriteRender.sprite;
			middleCopy.position = this.transform.position;
			middleCopy.localEulerAngles = this.transform.localEulerAngles;
			middleCopy.parent = backgroundParent;

			GameObject go = (GameObject)Instantiate(middleCopy.gameObject);
			Transform sideCopy = go.transform;
			sideCopy.position = middleCopy.position;
			sideCopy.parent = backgroundParent;

			if (scrollingSpeed > 0)
				sideCopy.Translate(new Vector3(-spriteRenderer.bounds.size.x, 0, 0), Space.World);
			else
				sideCopy.Translate(new Vector3(spriteRenderer.bounds.size.x, 0, 0), Space.World);

			originalSpriteRender.enabled = false;
		}

		FillStaticItems();
		FillDynamicItems();
	}

	protected virtual void FillDynamicItems()
	{
		// now create spawnable dynamic items
		if (dynamicSpawnItems.Count < 1)
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

	protected virtual void FillStaticItems()
	{
		if (staticSpawnItems.Count < 1)
			return;

		// just ensures this also works in the editor
		if (surfaceCollider == null)
		{
			SetUpLocal();
		}

		foreach(KeyValuePair<float, FroggerLaneItem> item in staticSpawnItems)
		{
			if (item.Value == null)
			{
				Debug.LogError("This static spawn item is null! It probably does not have a LaneItem component?");
				continue;
			}

			GameObject spawned = (GameObject) Instantiate(item.Value.gameObject);
			
			spawned.transform.parent = this.transform;
			spawned.transform.localPosition = Vector3.zero;
			spawned.transform.localRotation = Quaternion.identity;
			
			if (item.Value.behindPlayer) // center transform, so first subtract half the lane size, then position between 0 - 1 lane Length
				spawned.transform.localPosition = new Vector3(-(laneSize.x * 0.5f) + ((laneSize.x * item.Key)), 0, -1);
			else
				spawned.transform.localPosition = new Vector3(-(laneSize.x * 0.5f) + ((laneSize.x * item.Key)), 0, -10);


			// make the height of the spawned item's collider equal to the lane's height - this way it will vertically cover the entire lane no matter what the height that was set
			BoxCollider2D itemCollider = spawned.GetComponent<BoxCollider2D>();
			itemCollider.size = itemCollider.size.y(height/itemCollider.transform.localScale.y); // compensate for potential sprite scaling !
			itemCollider.center = itemCollider.center.y(surfaceCollider.center.y);

			staticSpawnedItems.Add(spawned.GetComponent<FroggerLaneItem>());
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
		return transform.position.v2() + surfaceCollider.center;
	}
	
	protected void Update()
	{
		// move background if there is a moving one
		if (scrollingBackground != null)
		{
			scrollingOffset = scrollingOffset.x(scrollingOffset.x + (scrollingSpeed * Time.deltaTime));
			
			if (Mathf.Abs(scrollingOffset.x) >= GetSurfaceSize().x)
			{
				scrollingOffset = scrollingOffset.x(0);
			}
			
			scrollingBackground.localPosition = scrollingOffset;
		}

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

		for (int i = dynamicSpawnedItems.Count - 1; i >= 0; i--) 
		{
			FroggerLaneItem currentItem = dynamicSpawnedItems[i];
			currentItem.UpdateLaneItem(displacement);

			if (currentItem.CrossedLevel())
			{
				dynamicSpawnedItems.Remove(currentItem);
				Destroy(currentItem.gameObject);
			}
		}
	}

	private GameObject SpawnLaneItem()
	{
		// just ensures this also works in the editor
		if (surfaceCollider == null)
		{
			SetUpLocal();
		}

		// create random item
		int index = Random.Range(0, dynamicSpawnItems.Count);

		// prevent repetitions with some factor
		if (dynamicSpawnItems.Count > 1)
		{
			while (index == lastItemIndex && Random.value > repeatAllowFactor)
			{
				index = Random.Range(0, dynamicSpawnItems.Count);
			}
		}

		lastItemIndex = index;

		GameObject spawnedItem = (GameObject)Instantiate(dynamicSpawnItems[index].gameObject);

		FroggerLaneItem itemScript = spawnedItem.GetComponent<FroggerLaneItem>();
		itemScript.goRight = goRight;
		itemScript.SetLaneDistance(GetSurfaceSize().x + itemScript.GetSurfaceSize().x * 2);

		// make the height of the spawned item's collider equal to the lane's height - this way it will vertically cover the entire lane no matter what the height that was set
		BoxCollider2D itemCollider = spawnedItem.GetComponent<BoxCollider2D>();
		itemCollider.size = itemCollider.size.y(height/itemCollider.transform.localScale.y); // compensate for potential sprite scaling !
		itemCollider.center = itemCollider.center.y(surfaceCollider.center.y);
	
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

		dynamicSpawnedItems.Add(itemScript);

		return spawnedItem;
	}
}
