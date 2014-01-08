using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPatrol : PacmanEnemyCharacter {

	protected List<PacmanTile> patrolPath = new List<PacmanTile>();
	protected int patrolIndex = 0;
	protected int playerChaseCount = 5;		// how many tiles will the enemy chase the player without seeing them directly
	protected int playerChaseCounter = 0; 	// how many tiles the enemy has chased the player without seeing them directly

	protected override void SetDefaultTargetTiles()
	{
		patrolIndex = 0;
		patrolPath.Add(PacmanLevelManager.use.GetTile(1,1));
		patrolPath.Add(PacmanLevelManager.use.GetTile(1,11));
		patrolPath.Add(PacmanLevelManager.use.GetTile(11,11));
		patrolPath.Add(PacmanLevelManager.use.GetTile(11,1));

		defaultTargetTile = patrolPath[patrolIndex];
	}

	public override void DestinationReached()
	{
		// TO DO replace
		if (player == null)
			player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));

		if (player.enemiesFlee)
		{
			FrightenedEffect();

			PacmanTile[] tiles = PacmanLevelManager.use.GetTilesForQuadrant(
				PacmanLevelManager.use.GetOppositeQuadrant(
					PacmanLevelManager.use.GetQuadrantOfTile(player.currentTile)));

			targetTile = tiles[Random.Range(0, tiles.Length - 1)];

			allowUTurns = false;		// don't allow u-turns now; could run head first into the player
		}
		else
		{
			// TO DO: Turn on or not? Changes behavior quite a lot.
			allowUTurns = false;	

			// if the player was detected, chase him
			if (playerFound)
			{
				PlayerDetectedEffect();
				playerChaseCounter = 0;
				targetTile = player.currentTile;
				//Debug.Log("Player detected! Giving chase.");
			}
			else	// if player isn't close
			{
				// if enemy has only just lost sight of the player, try chasing him for a number of tiles (playerChaseCount)
				if (playerChaseCounter < playerChaseCount)
				{
					PlayerDetectedEffect();
					playerChaseCounter++;
					targetTile = player.currentTile;
					//Debug.Log("Player lost! Trying to find them back.");
				}
				else // if enemy has lost sight of the player for a while, resume patrol
				{
					NeutralEffect();

					// turn off player sighted effect
					transform.localScale = originalScale;
					//Debug.Log("Player not detected! Continuing patrol.");
					foreach (PacmanTile patrolTile in patrolPath)
					{
						// if patrol waypoint was reached, find next
						if (currentTile == patrolTile)
						{
							//Debug.Log("Patrol point reached: " + patrolTile);
							patrolIndex++;
							if (patrolIndex >= patrolPath.Count)
							{
								patrolIndex = 0;
							}
						}
					}
					targetTile = patrolPath[patrolIndex];
				}
			}
		}

		CheckTeleportProximity();

		MoveTo(FindTileClosestTo(targetTile));

		if(moveTargetTile == null)
		{
			Debug.LogError("EnemyCharacterMover: Target tile was null!");
			return;
		}

		// figure out if target tile is to the right or to the left of the current position
		if (moveTargetTile.gridIndices.x > currentTile.gridIndices.x)
			ChangeSpriteDirection(CharacterDirections.Right);
		else if (moveTargetTile.gridIndices.x < currentTile.gridIndices.x)
			ChangeSpriteDirection(CharacterDirections.Left);
		else if (moveTargetTile.gridIndices.y < currentTile.gridIndices.y)
			ChangeSpriteDirection(CharacterDirections.Down);
		else if (moveTargetTile.gridIndices.y > currentTile.gridIndices.y)
			ChangeSpriteDirection(CharacterDirections.Up);
	}
}
