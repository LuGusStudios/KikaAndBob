using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class PacmanEnemyPatrolGuardChase : PacmanEnemyPatrolGuard
{ 
    protected List<PacmanTile> FollowTiles = new List<PacmanTile>(); 
    protected bool useTeleport = false;
    protected bool canRecieveTeleportTile = false;
    protected int fallbackAfterSteps = 10;
    protected int lockedSteps = 0;
    protected float chaseSpeedMultiplier = 0.5f;
    protected float maxChaseSpeedInPercentage = 0.90f;
    public override void SetUpLocal()
    {
        base.SetUpLocal();
        //when the no player has been found on awake it won't do the chase tiles
        playerChaseCounter = int.MaxValue;
        canRecieveTeleportTile = false;
        useTeleport = false;
        //allowUTurns = false;
    }

    protected void OnEnable()
    {
        if (player != null)
        {
            player.OnTeleported += OnPlayerTeleported;
        }
    }

    protected void OnDisable()
    {
        if (player != null)
        {
            player.OnTeleported -= OnPlayerTeleported;
        }
    }
    private void OnPlayerTeleported(PacmanTile teleportTile)
    {
        //when the player send its event out that it has teleported go to the teleport tile
        if (canRecieveTeleportTile)
        {
            useTeleport = true;
            targetTile = teleportTile;

            lockedSteps = 0;
            canRecieveTeleportTile = false;
        }
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
        
        // move
        UpdateMovement();

        TryOpenDoor();

        //when player hasn't teleported
        if(!useTeleport)
            CheckTeleportProximity();

        //when player is found and not disguised and hasn't been already seen
        if (playerFound && !player.poweredUp && !detectedRoutineRunning)
        {
            PlayerSeenEffect();
            DestinationReached();
            
            //UpdateMovement();
            //ResetMovement();
            

        } 
    }

    public override void Reset()
    {
        base.Reset();
        FollowTiles.Clear();
        playerChaseCounter = int.MaxValue;
        patrolIndex = 0;
        lockedSteps = 0;
    }

    public override void DestinationReached()
    {
        DoCurrentTileBehavior();
        //when you are not chasing and have seen the enemy do normal behaviour
        if (!useTeleport)
        {
            // if the player was detected, chase him, and the player has no power up
            if (playerFound && !player.poweredUp)
            {
                lockedSteps = 0;
                PlayerSeenEffect();
                playerChaseCounter = 0;
                targetTile = player.currentTile;
                //Debug.Log("Player detected! Giving chase.");
            }
            else	// if player isn't close
            {
                //// if enemy has only just lost sight of the player, try chasing him for a number of tiles (playerChaseCount)
                if (playerChaseCounter < playerChaseCount && !player.poweredUp)
                {
                    Debug.Log("Player lost! Trying to find them back. playerCounter " + playerChaseCounter);
                    PlayerSeenEffect();
                    //if player is hiding, randomize search
                    if (player.currentTile.tileType == PacmanTile.TileType.Hide)
                    {
                        PacmanTile[] tiles = PacmanLevelManager.use.GetTilesForQuadrant(
                                             PacmanLevelManager.use.GetQuadrantOfTile(player.currentTile));
                        targetTile = tiles[Random.Range(0, tiles.Length - 1)];

                    }
                    else //chase the player
                    {
                        targetTile = player.currentTile;
                    }
                    playerChaseCounter++;
                    lockedSteps = 0;

                }
                else // if enemy has lost sight of the player for a while, resume patrol
                {
                    NeutralEffect();
                    detectedRoutineRunning = false;
                    // turn off player sighted effect TO DO: Remove
                    transform.localScale = originalScale;
                    canRecieveTeleportTile = false;

                    //check if patrol Path is in sight, if it is clear follow tiles list
                    DetectPatrolPath();

                    //Follow the player followed path back
                    if (FollowTiles.Count > 0)
                    {
                        Debug.Log("following back path " + FollowTiles.Count + FollowTiles[FollowTiles.Count - 1].gridIndices );
                        if (moveTargetTile == FollowTiles[FollowTiles.Count - 1])
                        {
                            FollowTiles.RemoveAt(FollowTiles.Count - 1);
                            if (FollowTiles.Count>0)
                            {
                                targetTile = FollowTiles[Math.Max(FollowTiles.Count - 1, 0)];
                                lockedSteps = 0;
                            }
                        }
                        else
                        {
                            targetTile = FollowTiles[FollowTiles.Count - 1];
                            lockedSteps ++;
                        }
                    }
                    else if (patrolPath.Count > 0)
                    {
                        //Debug.Log("Player not detected! Continuing patrol.");
                        //foreach (PacmanTile patrolTile in patrolPath)
                        //{
                        //    // if patrol waypoint was reached, find next
                        //    if (currentTile == patrolTile)
                        //    {
                        //        //Debug.Log("Patrol point reached: " + patrolTile);
                        //        patrolIndex++;
                        //        lockedSteps = 0;
                        //        if (patrolIndex >= patrolPath.Count)
                        //        {
                        //            patrolIndex = 0;
                        //        }
                        //    }
                        //}
                        if (currentTile == patrolPath[patrolIndex])
                        {
                            patrolIndex++;
                            lockedSteps = 0;
                            if (patrolIndex >= patrolPath.Count)
                            {
                                patrolIndex = 0;
                            }
                        }
                        //if the target tile hasn't reached its position add lockstep
                        else
                        {
                            lockedSteps++;
                        }
                        targetTile = patrolPath[patrolIndex];
                    }
                }
            }
        }
        else
        {
            //Has a targetTile set because player has teleported
            Debug.Log("Teleport found");
            //FollowTiles.Add(targetTile);
            lockedSteps++;
        }
        //if(FollowTiles.Count>0)
        //    Debug.Log(FollowTiles.Count + " " +  FollowTiles[FollowTiles.Count - 1]);

        //if guard hasn't found his way to the teleport/patrolpath fall back to random tile in quadrant
        if (lockedSteps >= fallbackAfterSteps)
        {
            lockedSteps++;
            
            Debug.Log(this.name + " got stuck, fall back to random direction");
            useTeleport = false;
            canRecieveTeleportTile = false;

            PacmanTile[] tiles = PacmanLevelManager.use.GetTilesForQuadrant(
                PacmanLevelManager.use.GetQuadrantOfTile(currentTile));
            targetTile = tiles[Random.Range(0, tiles.Length - 1)];


            //when its got locked while trying to follow back his follow tiles use next one
            if (FollowTiles.Count > 0)
            {
                FollowTiles.RemoveAt(FollowTiles.Count - 1);
                lockedSteps = 0;
            }

            //After two time the fallback, try to search patrol path again
            if (lockedSteps%(fallbackAfterSteps*2) == 0)
            {
                lockedSteps = 0;
            }  
        }

		MoveTo(FindTileClosestTo(targetTile));

		if(moveTargetTile == null)
		{
			Debug.LogError("EnemyCharacterMover: Target tile was null!");
			return;
		}
		else
		{
		    DoCurrentTileLeaveBehavior();
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
    protected override void MoveTo(PacmanTile target)
    {
        moving = true;

        ResetMovement();
        moveStartPosition = transform.localPosition;
        moveTargetTile = target;

        if (target == null)
        {
            Debug.LogWarning("PacmanCharacter: Character move target tile was null!");
            return;
        }
        if (enemyState == EnemyState.Chasing)
        {
            //will chase the player faster by chaseSpeedMultiplier amount from base speed
            //but clamp is at maxChaseSpeed of player so it never runs faster/equaly as the player 
            movementDuration = Vector3.Distance(moveStartPosition, new Vector3(moveTargetTile.location.x, moveTargetTile.location.y, 0)) * 1 / Mathf.Min(speed + (speed * chaseSpeedMultiplier), player.speed * maxChaseSpeedInPercentage);
           
            FollowTiles.Add(moveTargetTile); 
            Debug.Log("Adding Follow Tile " + FollowTiles.Count + " with 2DVector " + FollowTiles[FollowTiles.Count-1]);
            
        }
        else
        {
            movementDuration = Vector3.Distance(moveStartPosition, new Vector3(moveTargetTile.location.x, moveTargetTile.location.y, 0)) * 1 / speed;
        }
        UpdateMovement();	// needs to be called again, or character will pause for one frame
    }
    protected override void DoCurrentTileBehavior()
    {
        foreach (GameObject go in currentTile.tileItems)
        {
            //if the tile has a teleport tile item teleport and set to false
            if (go.GetComponent<PacmanTileItemTeleport>() != null)
            {
                go.GetComponent<PacmanTileItemTeleport>().OnEnter(this);
                if (useTeleport)
                {
                    FollowTiles.Add(currentTile);
                }
                useTeleport = false;
                lockedSteps = 0;
            }
        }
        // if we just teleported and hit the next non-teleport tile, we're done teleporting
        if (currentTile.tileType != PacmanTile.TileType.Teleport & alreadyTeleported)
        {
            alreadyTeleported = false;
        }
        if (currentTile.tileType == PacmanTile.TileType.Teleport && !alreadyTeleported)
        {
            LugusCoroutines.use.StartRoutine(TeleportRoutine());
        }
    }
    protected override IEnumerator TeleportRoutine()
    {
        alreadyTeleported = true;

        PacmanTile targetTile = null;

        foreach (PacmanTile tile in PacmanLevelManager.use.teleportTiles)
        {
            if (currentTile != tile)
            {
                targetTile = tile;
                break;
            }
        }

        if (targetTile == null)
        {
            Debug.LogError("No other teleport tile found!");
            yield break;
        }

        transform.localPosition = targetTile.location.v3();

        currentTile = targetTile; 

        DestinationReached();
    }
    protected override void DetectPlayer()
    {
        
        base.DetectPlayer();
        //check further from base when no player has found in direction
        if (!playerFound)
        {
            // this default implementation looks around itself and detects player if the player is moving
            foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(this.currentTile))
            {
                if (tile != null)
                {
                    // check if any players are on the currently inspected tile and moving
                    foreach (PacmanPlayerCharacter playerChar in PacmanGameManager.use.GetPlayerChars())
                    {
                        if (tile == playerChar.currentTile && playerChar.moving && tile.tileType != PacmanTile.TileType.Hide )
                        {
                            playerFound = true;
                            //useTeleport = false;
                            return;
                        }
                    }
                }
            }
            playerFound = false;
        }
    }

    protected void DetectPatrolPath()
    {
        //check each tile in direction and clear followback path if a patrol tile is insight
        foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesInDirection(currentTile, forwardDetectDistance, currentDirection))
        {
            foreach (PacmanTile PatrolTile in patrolPath)
            {
                if (tile != null)
                {
                    // if the tile is not open, line of sight is broken
                    if (tile.tileType == PacmanTile.TileType.Collide || tile.tileType == PacmanTile.TileType.Hide)
                    {
                        return;
                    }
                    else if (tile.location == PatrolTile.location)
                    {
                        FollowTiles.Clear();
                        lockedSteps = 0;
                    }
                }
            }
        }
    }
    protected virtual void TryOpenDoor()
    {
        foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesAroundStraight(this.currentTile))
        {
            if (tile != null)
            {
                
                // check if any players are on the currently inspected tile and moving
                foreach (GameObject tileItem in tile.tileItems)
                {
                    tileItem.GetComponent<PacmanTileItem>().OnTryEnter(this);
                }
            }
        }
    }
    protected override void PlayerSeenEffect()
    {
        if (enemyState == EnemyState.Chasing)
            return;

        enemyState = EnemyState.Chasing;
        lockedSteps = 0;
        canRecieveTeleportTile = true;
        if (!detectedRoutineRunning)
        {
            LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(discoveredSound));
            iTween.PunchScale(this.gameObject, Vector3.one, 0.5f);
        }
        detectedRoutineRunning = true;
    }
    public void DoCurrentTileLeaveBehavior()
    {
        foreach (GameObject go in currentTile.tileItems)
        {
            if (go.GetComponent<PacmanTileItem>() != null)
            {
                go.GetComponent<PacmanTileItem>().OnLeave(this);
            }
        }
    }
}
