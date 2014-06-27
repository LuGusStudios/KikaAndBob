using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceCage : CatchingMiceObstacle
{
	public List<CatchingMiceCharacterPlayer> PlayerHold
	{
		get
		{
			return playerHold;
		}
		set
		{
			playerHold = value;
		}
	}

	protected List<CatchingMiceCharacterPlayer> playerHold = new List<CatchingMiceCharacterPlayer>();

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		CatchingMiceLevelManager.use.Cage = this;
	}

	public override void SetTileType(List<CatchingMiceTile> tiles)
	{
		foreach (CatchingMiceTile tile in tiles)
		{
			tile.tileType = tile.tileType | tileType;
			tile.obstacle = this;
		}

		transform.position = transform.position.yAdd(yOffset).zAdd(-zOffset);
	}

	public override bool ValidateTile(CatchingMiceTile tile)
	{
		if (!base.ValidateTile(tile))
		{
			return false;
		}

		if ((tile.obstacle != null) || ((tile.tileType & CatchingMiceTile.TileType.Obstacle) == CatchingMiceTile.TileType.Obstacle))
		{
			CatchingMiceLogVisualizer.use.LogError("Obstacle " + transform.name + " cannot be placed because another obstacle is already present.");
			return false;
		}

		return true;
	}

	public override void FromXMLObstacleDefinition(string configuration)
	{
		
	}

	public void PlayerDetected(CatchingMiceCharacterPlayer player)
	{
		if (playerHold.Contains(player))
		{
			return;
		}

		if (player.jumping)
		{
			player.interrupt = true;
		}

		player.StopCurrentBehaviour();

		player.transform.position = transform.position.zAdd(zOffset * 0.5f);
		player.currentTile = parentTile;

		Collider2D[] colliders = player.GetComponentsInChildren<Collider2D>();
		foreach(Collider2D coll2D in colliders)
		{
			coll2D.enabled = false;
		}

		playerHold.Add(player);
	}

	public void PlayerInteraction()
	{
		if (playerHold.Count <= 0)
		{
			return;
		}

		foreach(CatchingMiceCharacterPlayer player in playerHold)
		{
			Collider2D[] colliders = player.GetComponentsInChildren<Collider2D>(true);
			foreach(Collider2D coll2D in colliders)
			{
				coll2D.enabled = true;
			}

			float zOffset = player.zOffset;
			if (parentTile.furniture != null)
			{
				zOffset += parentTile.furniture.zOffset;
			}

			player.transform.position = player.transform.position.z(player.currentTile.location.z - zOffset);
		}

		playerHold.Clear();
	}
}
