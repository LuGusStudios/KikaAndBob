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
		Lethal
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
}
