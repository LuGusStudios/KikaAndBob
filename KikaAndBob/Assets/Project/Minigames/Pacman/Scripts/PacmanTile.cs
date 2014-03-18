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
	public List<GameObject> tileItems = new List<GameObject>();
	
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

	// will only leave behind non-null tileitems (e.g. temporary items that are cleared each round)
	public void PruneTileItems()
	{
		List<GameObject> oldList = new List<GameObject>(tileItems);

		tileItems.Clear();

		foreach(GameObject tileItem in oldList)
		{
			if (tileItem != null)
				tileItems.Add(tileItem);
		}
	}
}
