using UnityEngine;
using System.Collections;

public class PacmanEnemyChaseProximity : PacmanEnemyCharacter {

	// 	this ai imitates the orange pacman ghost. Quoting from http://gameinternals.com/post/2072558330/understanding-pac-man-ghost-behavior:
	//	"Whenever Clyde needs to determine his target tile, he first calculates his distance from Pac-Man. 
	//	If he is farther than eight tiles away, his targeting is identical to Blinky’s, using Pac-Man’s current tile as his target. 
	//	However, as soon as his distance to Pac-Man becomes less than eight tiles, Clyde’s target is set to the same tile as his 
	//	fixed one in Scatter mode, just outside the bottom-left corner of the maze."
	// 	I.e. The general behavior of this AI is to head for the player if the player is far away, then change its mind and head
	// 	back to its default target tile.


	public int detectDistance = 8;	// tile radius within which enemy will chase player
	

	public override void DestinationReached()
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
				// if player is withing a certain radius, chase them
				if (Vector2.Distance(player.currentTile.gridIndices, currentTile.gridIndices) <= detectDistance)
				{
					targetTile = defaultTargetTile;
				
				}
				// if not, stop caring and go find default target tile
				else
				{
					targetTile = player.currentTile;
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
