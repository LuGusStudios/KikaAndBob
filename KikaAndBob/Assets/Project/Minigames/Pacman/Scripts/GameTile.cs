using UnityEngine;
using System.Collections;

public class GameTile
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

	public GameObject sprite;
	public TileType tileType;
	public Vector2 location;
	public Vector2 gridIndices;
	public int exitCount;
	
	public GameTile()
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
}
