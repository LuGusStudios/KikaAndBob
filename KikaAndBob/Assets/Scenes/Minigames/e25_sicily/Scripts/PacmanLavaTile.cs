using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanLavaTile : PacmanTileItem 
{
	public GameObject lavaTrailPrefab = null;	
	protected float updateCheckSpeed = 2.0f;
	public List<PacmanTile> surroundingLavaTiles = new List<PacmanTile>();

	public override void Initialize ()
	{
		//parentTile.tileType = PacmanTile.TileType.Lethal;

		surroundingLavaTiles.Clear();

		foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
		{
			foreach (GameObject tileItem in tile.tileItems)
			{
				if (tileItem.GetComponent<PacmanLavaTile>() != null)
				{
					surroundingLavaTiles.Add(tile);
					break;
				}

				if (tileItem.GetComponent<PacmanLavaTileStart>() != null)
				{
					surroundingLavaTiles.Add(tile);
					break;
				}
			}
		}

		StartCoroutine(UpdateRoutine());	// starting this the old way - it doesn't need to be terminated, and this way it will be stopped if the object disappears
	}

	protected IEnumerator UpdateRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(updateCheckSpeed);

			if (surroundingLavaTiles.Count >= 4)
				yield break;

			foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(parentTile))
			{
				if (tile == null || surroundingLavaTiles.Contains(tile))
					continue;

				if (tile.tileType != PacmanTile.TileType.Collide)
				{
					GameObject newLavaTileObject = (GameObject) Instantiate(lavaTrailPrefab);
					newLavaTileObject.transform.position = tile.GetWorldLocation().v3().z(this.transform.position.z);
					newLavaTileObject.name = "LavaTrail" + tile.ToString();
				//	newLavaTileObject.transform.parent = this.transform.parent;
					
					surroundingLavaTiles.Add(tile);
					tile.tileItems.Add(newLavaTileObject);
					
					PacmanLavaTileStart newLavaTile = newLavaTileObject.GetComponent<PacmanLavaTileStart>();
					newLavaTile.parentTile = tile;
					newLavaTile.originLavaTile = this;
					newLavaTile.Initialize();

				}
			}
		}
	}

    public override void OnTryEnter(PacmanCharacter character)
	{
	}

    public override void OnEnter(PacmanCharacter character)
	{
	}
}
