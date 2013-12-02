using UnityEngine;
using System.Collections;

public class Enemy3 : EnemyCharacter {

	protected override void SetDefaultTargetTiles()
	{
		defaultTargetTile = PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-1, 0);
	}
	
	// green cat : find position four tiles behind player - since enemies can not normally turn around, it can still reach the player
	protected override void CheckTeleportProximity()
	{
		if (player == null)
		{
			Debug.LogError(gameObject.name + " Character not could find player.");
			return;
		}
		
		if (player.currentTile != null)
		{
			GameTile favoredTile = null;
			Character.CharacterDirections heading = player.GetDirection();
			
			int playerX = (int)player.currentTile.gridIndices.x;
			int playerY = (int)player.currentTile.gridIndices.y;
			
			
			// check four tiles backwards in player's current direction; try fewer tiles if this is not a valid tile; find player if all else fails
			if (heading == Character.CharacterDirections.Up)
			{
				for (int i = 4; i >= 0; i--)
				{
					favoredTile = PacmanLevelManager.use.GetTile(playerX, playerY-i);
					if(favoredTile != null)
						break;
				}
			}
			else if (heading == Character.CharacterDirections.Right)
			{
				for (int i = 4; i >= 0; i--)
				{
					favoredTile = PacmanLevelManager.use.GetTile(playerX-i, playerY);
					if(favoredTile != null)
						break;
				}
			}
			else if (heading == Character.CharacterDirections.Down)
			{
				for (int i = 4; i >= 0; i--)
				{
					favoredTile = PacmanLevelManager.use.GetTile(playerX, playerY+i);
					if(favoredTile != null)
						break;
				}
			}
			else if (heading == Character.CharacterDirections.Left)
			{
				for (int i = 4; i >= 0; i--)
				{
					favoredTile = PacmanLevelManager.use.GetTile(playerX+i, playerY);
					if(favoredTile != null)
						break;
				}
			}
			
			// Given the above, this should technically never happen. Better safe than sorry.
			if (favoredTile == null)
				favoredTile = player.currentTile;
		
			// detect if it is more efficient to use a teleport than to move to target tile
			if (Mathf.Abs(favoredTile.gridIndices.x - currentTile.gridIndices.x) > (float)PacmanLevelManager.use.width *0.5f) // if target tile is (more than) half a level away in x distance
			{
				// if reasonably close to teleport, go there
				foreach(GameTile tile in PacmanLevelManager.use.teleportTiles)
				{
					if (Vector2.Distance(currentTile.location, tile.location) <= PacmanLevelManager.use.width *0.25f)
					{
						targetTile = tile;
						break;
					}
				}
			}
			else
				targetTile = favoredTile;		
		}		
	}
	
}
