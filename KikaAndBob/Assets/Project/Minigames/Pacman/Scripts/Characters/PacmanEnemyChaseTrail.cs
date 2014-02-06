using UnityEngine;
using System.Collections;

public class PacmanEnemyChaseTrail : PacmanEnemyCharacter {

	public int ambushDistance = 4;

	// inverted version of Ambush ai: find position x tiles behind player (not in standard Pacman) - since enemies can not normally turn around, it can still reach the player
	// makes cool, quite unpredictable behavior

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
					targetTile = player.currentTile;					// just pick player's tile
				}
				else
				{
					// inverse direction!
					if (playerDirection == CharacterDirections.Left)
						playerDirection = CharacterDirections.Right;
					else if (playerDirection == CharacterDirections.Right)
						playerDirection = CharacterDirections.Left;
					else if (playerDirection == CharacterDirections.Up)
						playerDirection = CharacterDirections.Down;
					else if (playerDirection == CharacterDirections.Down)
						playerDirection = CharacterDirections.Up;

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
	}

}
