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

    public override void DestinationReached()
    {


        // if the player was detected, chase him, and the player has no power up
        if (playerFound && !player.poweredUp || enemyState == EnemyState.Frightened)
        {
            PlayerSeenEffect();
            PacmanEnemyCharacter enemyCharacter = _enemies[0] ;
            //if the enemy in the list is himself change enemy from the list
            if (enemyCharacter.currentTile == _enemies[0].currentTile)
            {
                enemyCharacter = _enemies[1];
            }
            foreach (PacmanEnemyCharacter enemy in _enemies)
            {
                //search the closest tile to go to and which is not his own tile
                if (PacmanLevelManager.use.GetDistanceBetweenTiles(this.currentTile,enemy.currentTile) < PacmanLevelManager.use.GetDistanceBetweenTiles(this.currentTile,enemyCharacter.currentTile)
                    && this.currentTile != enemy.currentTile
                    && enemy.gameObject.activeSelf)
                {
                    enemyCharacter = enemy;
                }
            }
            targetTile = enemyCharacter.currentTile;
            //when it reaches the enemy go back to patrol
            if (targetTile ==  this.currentTile)
            {
                Debug.Log("Reached!!");
                NeutralEffect();
            }
            Debug.Log("Player detected! Giving flee. to " + targetTile + enemyCharacter.transform.name);
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
