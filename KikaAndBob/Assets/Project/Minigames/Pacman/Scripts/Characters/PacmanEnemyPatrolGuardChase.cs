using UnityEngine;
using System.Collections;

public class PacmanEnemyPatrolGuardChase : EnemyPatrol
{
    protected override void Update()
    {
        base.Update();

        DetectPlayer();
        if (playerFound)
        {
            PlayerSeenEffect();
        }
           
    }

    public override void DestinationReached()
    {
        //		// TO DO replace
//		if (player == null)
//			player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));

		if (player.poweredUp && playerFound)
		{
			FrightenedEffect();

			PacmanTile[] tiles = PacmanLevelManager.use.GetTilesForQuadrant(
				PacmanLevelManager.use.GetOppositeQuadrant(
					PacmanLevelManager.use.GetQuadrantOfTile(player.currentTile)));

			targetTile = tiles[Random.Range(0, tiles.Length - 1)];

		//	allowUTurns = false;		// don't allow u-turns now; could run head first into the player
		}
		else
		{
//			// TO DO: Turn on or not? Changes behavior quite a lot.
//			allowUTurns = false;	

			// if the player was detected, chase him
			if (playerFound)
			{
				PlayerSeenEffect();
				playerChaseCounter = 0;
				targetTile = player.currentTile;
				//Debug.Log("Player detected! Giving chase.");
			}
			else	// if player isn't close
			{
                //// if enemy has only just lost sight of the player, try chasing him for a number of tiles (playerChaseCount)
                //if (playerChaseCounter < playerChaseCount)
                //{
                //    PlayerSeenEffect();
                //    playerChaseCounter++;
                //    targetTile = player.currentTile;
                //    //Debug.Log("Player lost! Trying to find them back.");
                //}
                //else // if enemy has lost sight of the player for a while, resume patrol
				{
					NeutralEffect();

					// turn off player sighted effect TO DO: Remove
					transform.localScale = originalScale;

					if (patrolPath.Count > 0)
					{
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
			ChangeSpriteFacing(CharacterDirections.Right);
		else if (moveTargetTile.gridIndices.x < currentTile.gridIndices.x)
			ChangeSpriteFacing(CharacterDirections.Left);
		else if (moveTargetTile.gridIndices.y < currentTile.gridIndices.y)
			ChangeSpriteFacing(CharacterDirections.Down);
		else if (moveTargetTile.gridIndices.y > currentTile.gridIndices.y)
			ChangeSpriteFacing(CharacterDirections.Up);
	

        if (currentTile.tileType == PacmanTile.TileType.Lethal)
        {
            foreach (GameObject go in currentTile.tileItems)
            {
                if (go.GetComponent<PacmanTileItem>() != null)
                {
                    go.GetComponent<PacmanTileItem>().OnEnter(this);
                }
            }
            DefeatedEffect();

        }   
    }

    protected override void PlayerSeenEffect()
    {
      if (enemyState == EnemyState.Chasing)
            return;

        enemyState = EnemyState.Chasing;
    }

    public override void ChangeSpriteFacing(CharacterDirections direction)
    {
        CharacterDirections adjustedDirection = direction;

        if (direction == CharacterDirections.Right)
        {
            adjustedDirection = CharacterDirections.Left;
        }

        characterAnimator.PlayAnimation(adjustedDirection.ToString());

        if (characterAnimator.GetComponent<FlippedIncorrectly>() == null)
        {
            if (direction == CharacterDirections.Right)
            {
                // if going left, the scale.x needs to be negative
                if (characterAnimator.currentAnimationContainer.transform.localScale.x > 0)
                {
                    characterAnimator.currentAnimationContainer.transform.localScale =
                        characterAnimator.currentAnimationContainer.transform.localScale.x(
                                                                                           characterAnimator.currentAnimationContainer.transform.localScale.x * -1.0f);
                }
            }
            else if (direction == CharacterDirections.Left)
            {
                // if going right, the scale.x needs to be positive 
                if (characterAnimator.currentAnimationContainer.transform.localScale.x < 0)
                {
                    characterAnimator.currentAnimationContainer.transform.localScale =
                        characterAnimator.currentAnimationContainer.transform.localScale.x(
                                                                                           Mathf.Abs(characterAnimator.currentAnimationContainer.transform.localScale.x));
                }
            }
        }
        else
        {
            if (direction == CharacterDirections.Left)
            {
                // if going left, the scale.x needs to be negative
                if (characterAnimator.currentAnimationContainer.transform.localScale.x > 0)
                {
                    characterAnimator.currentAnimationContainer.transform.localScale =
                        characterAnimator.currentAnimationContainer.transform.localScale.x(
                                                                                           characterAnimator.currentAnimationContainer.transform.localScale.x * -1.0f);
                }
            }
            else if (direction == CharacterDirections.Right)
            {
                // if going right, the scale.x needs to be positive 
                if (characterAnimator.currentAnimationContainer.transform.localScale.x < 0)
                {
                    characterAnimator.currentAnimationContainer.transform.localScale =
                        characterAnimator.currentAnimationContainer.transform.localScale.x(
                                                                                           Mathf.Abs(characterAnimator.currentAnimationContainer.transform.localScale.x));
                }
            }
        }
    }
}
