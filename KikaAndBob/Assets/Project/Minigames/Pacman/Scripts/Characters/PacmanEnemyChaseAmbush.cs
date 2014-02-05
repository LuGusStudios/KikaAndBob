using UnityEngine;
using System.Collections;

public class PacmanEnemyChaseAmbush : PacmanEnemyCharacter {

	public int ambushDistance = 4;

	// 	this ai imitates the pink pacman ghost. Quoting from http://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior:
	//	"Pinky’s target tile in Chase mode is determined by looking at Pac-Man’s current position and orientation, 
	//	and selecting the location four tiles straight ahead of Pac-Man."
	// 	I.e. this AI will seem to be "ambushing" the player by trying to get ahead of them.
	
	public override void DestinationReached ()
	{
		if (runBehavior == true)
		{
			if (enemyState == EnemyState.Frightened)
			{
				PacmanTile[] tiles = PacmanLevelManager.use.GetTilesForQuadrant(
					PacmanLevelManager.use.GetOppositeQuadrant(
					PacmanLevelManager.use.GetQuadrantOfTile(player.currentTile)));
				
				targetTile = tiles[Random.Range(0, tiles.Length - 1)];
			}
			else
			{
				CharacterDirections playerDirection = player.GetDirection();

				if (playerDirection == CharacterDirections.Undefined)	// this can happen if the player hasn't moved yet
				{
					print("undefined");
					targetTile = player.currentTile;					// just pick player's tile
				}
				else
				{
					print(playerDirection);
					// check x tiles ahead of player
					foreach(PacmanTile tile in PacmanLevelManager.use.GetTilesInDirection(player.currentTile, ambushDistance, playerDirection, true, true))
					{
						// if this a valid tile, go there, else try one's thats closer to the player
						if (tile != null) // no need to check for tile type, it doesn't really matter if the target tile is a collider, we only need the general direction
						{
							targetTile = tile;
							break;
						}
					}
				}
			}
			
			// before finding the next tile to move to, check if target tile cannot be reached more easily through a teleport
			CheckTeleportProximity();
			
			// find immediately surrounding tile that's the best bet for getting to the target tile
			MoveTo(FindTileClosestTo(targetTile));
			
			if(moveTargetTile == null)
			{
				Debug.LogError("EnemyCharacterMover: Target tile was null!");
				return;
			}
			
			// figure out if target tile is to the right or to the left of the current position
			if (moveTargetTile.gridIndices.x > currentTile.gridIndices.x)
				ChangeSpriteFacing(CharacterDirections.Right);
			else if (moveTargetTile.gridIndices.x < currentTile.gridIndices.x)
				ChangeSpriteFacing(CharacterDirections.Left);
			else if (moveTargetTile.gridIndices.y < currentTile.gridIndices.y)
				ChangeSpriteFacing(CharacterDirections.Down);
			else if (moveTargetTile.gridIndices.y > currentTile.gridIndices.y)
				ChangeSpriteFacing(CharacterDirections.Up);
		}


		
//		if (player.currentTile != null)
//		{
//			PacmanTile favoredTile = null;
//			PacmanCharacter.CharacterDirections heading = player.GetDirection();
//			
//			int playerX = (int)player.currentTile.gridIndices.x;
//			int playerY = (int)player.currentTile.gridIndices.y;
//			
//			// check four tiles ahead in player's current direction; try fewer tiles if this is not a valid tile; find player if all else fails
//			if (heading == PacmanCharacter.CharacterDirections.Up)
//			{
//				for (int i = 4; i >= 0; i--)
//				{
//					favoredTile = PacmanLevelManager.use.GetTile(playerX, playerY+i);
//					if(favoredTile != null)
//						break;
//				}
//			}
//			else if (heading == PacmanCharacter.CharacterDirections.Right)
//			{
//				for (int i = 4; i >= 0; i--)
//				{
//					favoredTile = PacmanLevelManager.use.GetTile(playerX+i, playerY);
//					if(favoredTile != null)
//						break;
//				}
//			}
//			else if (heading == PacmanCharacter.CharacterDirections.Down)
//			{
//				for (int i = 4; i >= 0; i--)
//				{
//					favoredTile = PacmanLevelManager.use.GetTile(playerX, playerY-i);
//					if(favoredTile != null)
//						break;
//				}
//			}
//			else if (heading == PacmanCharacter.CharacterDirections.Left)
//			{
//				for (int i = 4; i >= 0; i--)
//				{
//					favoredTile = PacmanLevelManager.use.GetTile(playerX-i, playerY);
//					if(favoredTile != null)
//						break;
//				}
//			}
//			
//			// Given the above, this should technically never happen. Better safe than sorry.
//			if (favoredTile == null)
//				favoredTile = player.currentTile;
//		
//			// detect if it is more efficient to use a teleport than to move to target tile
//			if (Mathf.Abs(favoredTile.gridIndices.x - currentTile.gridIndices.x) > (float)PacmanLevelManager.use.width *0.5f) // if target tile is (more than) half a level away in x distance
//			{
//				// if reasonably close to teleport, go there
//				foreach(PacmanTile tile in PacmanLevelManager.use.teleportTiles)
//				{
//					if (Vector2.Distance(currentTile.location, tile.location) <= PacmanLevelManager.use.width *0.25f)
//					{
//						targetTile = tile;
//						break;
//					}
//				}
//			}
//			else
//				targetTile = favoredTile;		
//		}		
	}
}
