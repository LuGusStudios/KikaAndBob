using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanTile
{
	public enum TileType
	{
		None,
		Collide,
		Open,
		Pickup,
		Teleport,
		Locked,
		LevelEnd,
		EnemyAvoid,
		Door,
		Upgrade,
		Lethal,
        Hide
	}

	public GameObject rendered;
	public TileType tileType;
	public Vector2 location;
	public Vector2 gridIndices;
	public int exitCount;
	public List<PacmanTileItem> tileItems = new List<PacmanTileItem>();
	//public List<GameObject> tileItems = new List<GameObject>();

	public PacmanTile()
	{
		tileType = TileType.Open;
		location = Vector2.zero;
		gridIndices = Vector2.zero;
		exitCount = 0;
	}

	public override string ToString ()
	{
		return "GameTile: " + gridIndices;
	}

	public Vector2 GetWorldLocation()
	{
		return PacmanLevelManager.use.GetLevelRoot().position.v2() + location;
	}

	public void ResetTile()
	{
		PruneTileItems();
		ResetTileItems();
	}

	// will only leave behind non-null tileitems (e.g. temporary items that are cleared each round)
	public void PruneTileItems()
	{
		if (PacmanLevelManager.use.temporaryParent == null)
			return;

		List<PacmanTileItem> oldList = new List<PacmanTileItem>(tileItems);

		tileItems.Clear();

		foreach(PacmanTileItem tileItem in oldList)
		{
			if (tileItem != null && tileItem.transform.parent != PacmanLevelManager.use.temporaryParent)
			{
				tileItems.Add(tileItem);
			}
			else
			{
				GameObject.Destroy(tileItem.gameObject);
			}
		}
	}

	public void ResetTileItems()
	{
		foreach(PacmanTileItem tileItem in tileItems)
		{
			tileItem.Reset();
		}
	}
}
