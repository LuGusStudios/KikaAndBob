using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCornFlakesTrap : CatchingMiceWorldObjectTrapFurniture {

	public GameObject cornflakesObjectPrefab = null;
	public int spawnTilesPerUse = 3;
	public int totalSpawnTiles = 9;
	
	protected List<GameObject> cornFlakeObjects = new List<GameObject>();

	// This trap is not activated automatically
	// It is only activated through player interaction
	// and stays active for a certain time

	protected override void OnPlayerInteract()
	{
		// When the player interacts, trap spawns cornflakes that distracts mice.

		if (totalSpawnTiles <= 0)
		{
			CatchingMiceLogVisualizer.use.Log("CatchingMiceCornFlakesTrap: Cornflakes is out of ammo.");
			return;
		}
	
		List<CatchingMiceTile> surroundingTiles = new List<CatchingMiceTile>( CatchingMiceLevelManager.use.GetTilesAround(parentTile, tileRange));

		for (int i = 0; i < spawnTilesPerUse; i++) 
		{
			if (surroundingTiles.Count <= 0 || totalSpawnTiles <= 0)
				break;

			CatchingMiceTile randomTile = null;
			int tryCount = 0;

			while( randomTile == null && tryCount < 20 )
			{
				randomTile = surroundingTiles[Random.Range(0, surroundingTiles.Count)];

				if (randomTile.cheese != null)
				{
					surroundingTiles.Remove(randomTile);
					randomTile = null;
				}
			}

			if (randomTile == null)
				continue;

			totalSpawnTiles --;

			surroundingTiles.Remove(randomTile);

			GameObject spawnedCornflakes = (GameObject)GameObject.Instantiate(cornflakesObjectPrefab);
			spawnedCornflakes.transform.parent = this.transform;
			spawnedCornflakes.transform.position = randomTile.waypoint.transform.position.zAdd(-0.1f);

			randomTile.cheese = spawnedCornflakes.GetComponentInChildren<CatchingMiceCheese>();
			CatchingMiceLevelManager.use.FakeCheeseTiles.Add(randomTile);

			randomTile.tileType |= CatchingMiceTile.TileType.Cheese;
		}
	}
}
