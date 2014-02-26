using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PacmanEnemyPatrolGuardChase : PacmanEnemyPatrolGuard
{
    protected bool butlerFound = false;
    protected List<PacmanTile> FollowTiles = new List<PacmanTile>(); 
    public override void SetUpLocal()
    {
        base.SetUpLocal();
        //when the no player has been found on awake it won't do the chase tiles
        playerChaseCounter = int.MaxValue;

        //allowUTurns = false;
    }

    protected override void Update()
    {
        if (!PacmanGameManager.use.gameRunning || enemyState == EnemyState.Defeated || PacmanGameManager.use.Paused)
            return;

        // update player in case we've changed players (two character game etc.)
        player = PacmanGameManager.use.GetActivePlayer();

        // the player is in neutral state unless something else is happening
        NeutralEffect();

        // iterate over players to see if we're on the same tile as any of them
        foreach (PacmanPlayerCharacter p in PacmanGameManager.use.GetPlayerChars())
        {
            
            // if we're on the same tile as a player, determine behavior
            if (currentTile == p.currentTile)
            {
                // if player is not powered up, player loses life
                if (!p.poweredUp)
                {
                    if (!string.IsNullOrEmpty(attackSoundKey))
                    {
                        LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(attackSoundKey));
                    }
                    p.DoHitEffect();
                }
            }
        }

        // what tile is character currently on?
        DetectCurrentTile();

        if (!runBehavior)
            return;

        // is player near
        DetectPlayer();

       

        // is a butler
        //DetectButler();

        // move
        UpdateMovement();

        if(playerFound && !player.poweredUp && !detectedRoutineRunning) 
        {
            PlayerSeenEffect();
            ResetMovement();
            targetTile = currentTile;
            MoveTo(FindTileClosestTo(targetTile));
            DestinationReached();
            
        }
    }

    public override void DestinationReached()
    {

		// if the player was detected, chase him, and the player has no power up
		if (playerFound && !player.poweredUp)
		{
			PlayerSeenEffect();
			playerChaseCounter = 0;
			targetTile = player.currentTile;
		    butlerFound = false;
            FollowTiles.Add(currentTile);
		    //Debug.Log("Player detected! Giving chase.");
		}
		else	// if player isn't close
		{
            //// if enemy has only just lost sight of the player, try chasing him for a number of tiles (playerChaseCount)
           
            if (playerChaseCounter < playerChaseCount)
            {
                PlayerSeenEffect();
                playerChaseCounter++;
                targetTile = player.currentTile;
                FollowTiles.Add(currentTile);
                //Debug.Log("Player lost! Trying to find them back.");
            }
            //else if (butlerFound)
            //{
            //    PlayerSeenEffect();
            //    targetTile = player.currentTile;
            //    if (lastKnowPlayerTile == currentTile)
            //    {
            //        NeutralEffect();
            //        butlerFound = false;
            //    }
            //}
            else // if enemy has lost sight of the player for a while, resume patrol
			{
				NeutralEffect();
			    detectedRoutineRunning = false;
				// turn off player sighted effect TO DO: Remove
				transform.localScale = originalScale;
			    if (FollowTiles.Count > 0)
			    {
			        targetTile = FollowTiles[FollowTiles.Count - 1];
                    FollowTiles.RemoveAt(FollowTiles.Count-1);
			    }
			    else if (patrolPath.Count > 0)
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
	
        //When the patrol guard enters a trap kill itself
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
        if (!detectedRoutineRunning)
        {
            LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(discoveredSound));
            iTween.PunchScale(this.gameObject, Vector3.one, 0.5f);
        }
        detectedRoutineRunning = true;
    }

    protected virtual void DetectButler()
    {
        // this while look at all the tiles around the current tile
        foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(currentTile))
        {
            if (tile != null)
            {
               
                // check if any players are on the currently inspected tile
                foreach (PacmanEnemyCharacter enemy in PacmanGameManager.use.GetEnemyCharacters())
                {
                    if (tile == enemy.currentTile && enemy.enemyState == EnemyState.Frightened)
                    {
                        butlerFound = true;
                        //lastKnowPlayerTile = enemy._lastKnowTileOfPlayer;
                        return;
                    }
                }
                
            }
        }

        playerFound = false;
    }
}
