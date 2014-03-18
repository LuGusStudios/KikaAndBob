using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PacmanEnemyPatrolButler : PacmanEnemyPatrolGuard
{
    protected List<PacmanEnemyCharacter> _enemies;
    public PacmanTile _lastKnowTileOfPlayer;
    public override void SetUpLocal()
    {
        base.SetUpLocal();
       
    }

    public override void SetUpGlobal()
    {
        base.SetUpGlobal();
        _enemies = new List<PacmanEnemyCharacter>(PacmanGameManager.use.GetEnemyCharacters());
        Debug.Log("Enemy count " + _enemies.Count);
    }

    protected override void Update()
    {
        if (!PacmanGameManager.use.gameRunning || enemyState == EnemyState.Defeated || PacmanGameManager.use.Paused)
            return;

        // update player in case we've changed players (two character game etc.)
        player = PacmanGameManager.use.GetActivePlayer();

        // the player is in neutral state unless something else is happening
        //NeutralEffect();

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

    }
    protected override PacmanTile FindTileClosestTo(PacmanTile target)
    {
        PacmanTile closestTile = null;
        PacmanTile inspectedTile;
        float closestDistance = Mathf.Infinity;
        PacmanCharacter.CharacterDirections proposedDirection = PacmanCharacter.CharacterDirections.Undefined;

        int xCoord = (int)currentTile.gridIndices.x;
        int yCoord = (int)currentTile.gridIndices.y;

        // Apparently the direction decision making for Pacman ghosts values up > left > down in case of equivalent distances.
        // This order is maintained below.

        // check up
        inspectedTile = PacmanLevelManager.use.GetTile(xCoord, yCoord + 1);
        if (inspectedTile != null)
        {
            // first we run OnTryEnter(), because this might still alter things about the tile (e.g. changing it from Collide to Open if the player has a key for a door)
            foreach (GameObject go in inspectedTile.tileItems)
            {
                if (go.GetComponent<PacmanTileItem>() != null)
                {
                    go.GetComponent<PacmanTileItem>().OnTryEnter(this);
                }
            }
            if (IsEnemyWalkable(inspectedTile))
            {
                //print (targetTile);
                float distance = Vector2.Distance(inspectedTile.location, targetTile.location);
                if (distance < closestDistance && (allowUTurns || currentDirection != PacmanCharacter.CharacterDirections.Down || currentTile.exitCount <= 1))
                {
                    closestDistance = distance;
                    closestTile = inspectedTile;
                    proposedDirection = PacmanCharacter.CharacterDirections.Up;
                }
            }
        }

        // check left
        inspectedTile = PacmanLevelManager.use.GetTile(xCoord - 1, yCoord);
        if (inspectedTile != null)
        {
            // first we run OnTryEnter(), because this might still alter things about the tile (e.g. changing it from Collide to Open if the player has a key for a door)
            foreach (GameObject go in inspectedTile.tileItems)
            {
                if (go.GetComponent<PacmanTileItem>() != null)
                {
                    go.GetComponent<PacmanTileItem>().OnTryEnter(this);
                }
            }
            if (IsEnemyWalkable(inspectedTile))
            {
                float distance = Vector2.Distance(inspectedTile.location, targetTile.location);
                if (distance < closestDistance && (allowUTurns || currentDirection != PacmanCharacter.CharacterDirections.Right || currentTile.exitCount <= 1))
                {
                    closestDistance = distance;
                    closestTile = inspectedTile;
                    proposedDirection = PacmanCharacter.CharacterDirections.Left;
                }
            }
        }

        // check down
        inspectedTile = PacmanLevelManager.use.GetTile(xCoord, yCoord - 1);
        if (inspectedTile != null)
        {
            // first we run OnTryEnter(), because this might still alter things about the tile (e.g. changing it from Collide to Open if the player has a key for a door)
            foreach (GameObject go in inspectedTile.tileItems)
            {
                if (go.GetComponent<PacmanTileItem>() != null)
                {
                    go.GetComponent<PacmanTileItem>().OnTryEnter(this);
                }
            }
            if (IsEnemyWalkable(inspectedTile))
            {
                float distance = Vector2.Distance(inspectedTile.location, targetTile.location);
                if (distance < closestDistance && (allowUTurns || currentDirection != PacmanCharacter.CharacterDirections.Up || currentTile.exitCount <= 1))
                {
                    closestDistance = distance;
                    closestTile = inspectedTile;
                    proposedDirection = PacmanCharacter.CharacterDirections.Down;
                }
            }
        }

        // check right
        inspectedTile = PacmanLevelManager.use.GetTile(xCoord + 1, yCoord);
        if (inspectedTile != null)
        {
            // first we run OnTryEnter(), because this might still alter things about the tile (e.g. changing it from Collide to Open if the player has a key for a door)
            foreach (GameObject go in inspectedTile.tileItems)
            {
                if (go.GetComponent<PacmanTileItem>() != null)
                {
                    go.GetComponent<PacmanTileItem>().OnTryEnter(this);
                }
            }
            if (IsEnemyWalkable(inspectedTile))
            {
                float distance = Vector2.Distance(inspectedTile.location, targetTile.location);
                if (distance < closestDistance && (allowUTurns || currentDirection != PacmanCharacter.CharacterDirections.Left || currentTile.exitCount <= 1))
                {
                    closestDistance = distance;
                    closestTile = inspectedTile;
                    proposedDirection = PacmanCharacter.CharacterDirections.Right;
                }
            }
        }

        if (proposedDirection != PacmanCharacter.CharacterDirections.Undefined)
            currentDirection = proposedDirection;

        return closestTile;
    }
    public override void DestinationReached()
    {


        // if the player was detected and the player has no power up, flee from him
        if (playerFound && !player.poweredUp || enemyState == EnemyState.Frightened)
        {
            PlayerSeenEffect();

            //Go to your door


            targetTile = player.currentTile;
            //when it reaches the enemy go back to patrol
            if (targetTile ==  this.currentTile)
            {
                Debug.Log("Reached!!");
                NeutralEffect();
            }
        }
        else	// if player isn't close
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


        CheckTeleportProximity();

        MoveTo(FindTileClosestTo(targetTile));

        if (moveTargetTile == null)
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

        //TODO: animations
        //if (currentTile.tileType == PacmanTile.TileType.Lethal)
        //{
        //    foreach (GameObject go in currentTile.tileItems)
        //    {
        //        if (go.GetComponent<PacmanTileItem>() != null)
        //        {
        //            go.GetComponent<PacmanTileItem>().OnEnter(this);
        //        }
        //    }
        //    DefeatedEffect();

        //}
    }

    protected override void PlayerSeenEffect()
    {
        if (enemyState == EnemyState.Frightened)
            return;

        enemyState = EnemyState.Frightened;

        _lastKnowTileOfPlayer = player.currentTile;

        LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(discoveredSound));
        iTween.PunchScale(this.gameObject, Vector3.one, 0.5f);
    }
}
