using UnityEngine;
using System.Collections;

public class Enemy1 : EnemyCharacter {
	
	public int detectDistance = 8;	// tile radius within which enemy will chase player

	protected override void SetDefaultTargetTiles()
	{
		defaultTargetTile = PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-1, PacmanLevelManager.use.height-1);
	}
	
	
	// blue cat: find player directly if close, find default target tile if not
	protected override void CheckTeleportProximity()
	{
		if (player == null)
		{
			Debug.LogError(gameObject.name + " Character not could find player.");
			return;
		}
		
		if (player.currentTile != null)
		{
			GameTile favoredTile;
			
			if (Vector2.Distance(player.currentTile.gridIndices, currentTile.gridIndices) <= detectDistance)
				favoredTile = player.currentTile;
			else
				favoredTile = defaultTargetTile;
			
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