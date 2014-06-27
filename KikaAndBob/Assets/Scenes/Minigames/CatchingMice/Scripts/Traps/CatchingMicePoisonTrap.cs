using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMicePoisonTrap : CatchingMiceWorldObjectTrapFurniture {

	public int maxAcidTiles = 3;
	public GameObject acidObjectPrefab = null;

	protected List<CatchingMiceTile> acidTiles = new List<CatchingMiceTile>();
	protected List<GameObject> acidObjects = new List<GameObject>();

	public override void SetupGlobal()
	{

		// This trap is not activated automatically
		// It is only activated through player interaction
		// and stays active for a certain time
		base.SetupGlobal();
	}

	protected override void OnPlayerInteract()
	{
		// When the player interacts with the poison trap,
		// it spawns acid spots

		// Remove the current acid tiles and objects
		for (int i = acidTiles.Count - 1; i >= 0; --i)
		{
			StartCoroutine(RemoveAcidObjectRoutine(i));
		}

		acidTiles.Clear();
		acidObjects.Clear();

		if (tileRange <= 0)
		{
			CatchingMiceLogVisualizer.use.LogError("The poison trap has a 0 tile range. It cannot place its acid tiles.");
			return;
		}

		// Spawn new acid objects on random, surrounding tiles
		while (acidTiles.Count < maxAcidTiles)
		{

			// We do +1 at the end, because the float are always rounded down
			// Otherwise, we would not be able to get upper right tile surrounding
			// the trap
			Vector2 gridIndices = Vector2.zero;
			gridIndices.x = LugusRandom.use.Uniform.NextInt(-tileRange, tileRange + 1);
			gridIndices.y = LugusRandom.use.Uniform.NextInt(-tileRange, tileRange + 1);

			// Make sure the acid object is not on the object tile itself
			if (gridIndices != Vector2.zero)
			{
				CatchingMiceTile tile = CatchingMiceLevelManager.use.GetTile(parentTile.gridIndices + gridIndices);
				if ((tile != null) && (!acidTiles.Contains(tile)))
				{
					StartCoroutine(SpawnAcidObjectRoutine(tile));
				}
			}
		}

		// Start the routine checking for enemies walking through the acid
		StartCoroutine(TrapRoutine());
	}

	protected override IEnumerator TrapRoutine()
	{
		// Go over all of the acid tiles in the list
		// and check whether there are any mice walking
		// over it.


		float intervalTime = interval;

		// The trap is limited in time (the interval)
		// After the timer expires, the acid objects should disappear again
		while (CatchingMiceGameManager.use.gameRunning
			&& (health > 0)
			&& (intervalTime > 0))
		{

			for (int i = acidTiles.Count - 1; i >= 0 ; --i)
			{
				CatchingMiceTile acidTile = acidTiles[i];

				foreach (CatchingMiceCharacterMouse enemy in CatchingMiceLevelManager.use.Enemies)
				{
					if (acidTile == enemy.currentTile)
					{
						OnHit(enemy);

						// Immediately removes the tile and object from the list, and lets the object shrink in time
						StartCoroutine(RemoveAcidObjectRoutine(i));

						break;
					}
				}
			}

			intervalTime -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		// When we get here, the remaining acid objects should disappear
		for (int i = acidTiles.Count - 1; i >= 0; --i)
		{
			StartCoroutine(RemoveAcidObjectRoutine(i));
		}

		// Now, the player should come and activate the trap again
	}

	protected IEnumerator SpawnAcidObjectRoutine(CatchingMiceTile targetTile)
	{
		if (targetTile == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The target tile is null...");
			yield break;
		}

		if (acidObjectPrefab == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The acid object prefab is null...");
			yield break;
		}

		GameObject acidObjectCopy = (GameObject)GameObject.Instantiate(acidObjectPrefab);
		acidObjectCopy.transform.parent = this.transform;

		float yOffset = 0f;
		float zOffset = 0.1f;
		if (targetTile.furniture != null)
		{
			yOffset = targetTile.furniture.yOffset;
			zOffset += targetTile.furniture.zOffset;
		}

		acidObjectCopy.transform.position = targetTile.location.yAdd(yOffset).zAdd(-zOffset);

		Vector3 originalScale = acidObjectCopy.transform.localScale;
		acidObjectCopy.transform.localScale = acidObjectCopy.transform.localScale * 0.1f;
		acidObjectCopy.ScaleTo(originalScale).Time(0.5f).Execute();

		acidObjects.Add(acidObjectCopy);
		acidTiles.Add(targetTile);

		yield return new WaitForSeconds(0.5f);
	}

	protected IEnumerator RemoveAcidObjectRoutine(int index)
	{
		// Let the acid object shrink, and destroyed afterwards
		if (acidObjects.Count == 0)
		{
			CatchingMiceLogVisualizer.use.LogError("The list of acid objects is 0, yet a removal has been requested.");
			yield break;
		}

		if (index < 0)
		{
			CatchingMiceLogVisualizer.use.LogError("The index is less than 0...");
			yield break;
		}
		else if (index > (acidObjects.Count - 1))
		{
			CatchingMiceLogVisualizer.use.LogError("The index is out of bounds...");
			yield break;
		}

		GameObject acidObject = acidObjects[index];
		if (acidObject == null)
		{
			CatchingMiceLogVisualizer.use.LogError("The acid object is null...");
			yield break;
		}

		acidObjects.RemoveAt(index);
		acidTiles.RemoveAt(index);

		acidObject.ScaleTo(acidObject.transform.localScale * 0.1f).Time(0.25f).Execute();

		yield return new WaitForSeconds(0.25f);

		GameObject.Destroy(acidObject);

		yield break;
	}
}
