using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanEnemyPatrolGuard : EnemyPatrol 
{
	public string discoveredSound = "Discovered01";
	protected bool detectedRoutineRunning = false;

	protected override void Update() 
	{
		if (!PacmanGameManager.use.gameRunning || PacmanGameManager.use.Paused)
			return;
		
		foreach (PacmanPlayerCharacter p in PacmanGameManager.use.GetPlayerChars())
		{
			if (currentTile == p.currentTile)
			{
				if (enemyState == EnemyState.Frightened)
					DefeatedEffect();
			}
		}
		
		// what tile is character currently on?
		DetectCurrentTile();
		
		if (!runBehavior)
			return;

		// move
		UpdateMovement();

		// is player near
		DetectPlayer();

		if (playerFound && !detectedRoutineRunning)
		{
			PlayerSeenEffect();
		}
	}

	protected override void PlayerSeenEffect ()
	{
		LugusCoroutines.use.StartRoutine(PlayerSeenRoutine());
	}
	
	protected IEnumerator PlayerSeenRoutine()
	{
		detectedRoutineRunning = true;
		
		LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(discoveredSound));
		
		iTween.PunchScale(this.gameObject, Vector3.one, 0.5f);
		
		yield return new WaitForSeconds(0.5f);
		
		PacmanGameManager.use.LoseLife();
		detectedRoutineRunning = false;
	}

	public override void DestinationReached ()
	{
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

		//CheckTeleportProximity();
		
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

    

    public override void ChangeSpriteFacing (CharacterDirections direction)
	{
		CharacterDirections adjustedDirection = direction;

		if ( direction == CharacterDirections.Right )
		{
			adjustedDirection = CharacterDirections.Left;
		}

		characterAnimator.PlayAnimation(adjustedDirection.ToString());

		if (characterAnimator.GetComponent<FlippedIncorrectly>() == null)
		{
			if ( direction == CharacterDirections.Right )
			{
				// if going left, the scale.x needs to be negative
				if( characterAnimator.currentAnimationContainer.transform.localScale.x > 0 )
				{
					characterAnimator.currentAnimationContainer.transform.localScale = 
						characterAnimator.currentAnimationContainer.transform.localScale.x( 
						                                                                   characterAnimator.currentAnimationContainer.transform.localScale.x * -1.0f );
				}
			}
			else if ( direction == CharacterDirections.Left )
			{
				// if going right, the scale.x needs to be positive 
				if( characterAnimator.currentAnimationContainer.transform.localScale.x < 0 )
				{
					characterAnimator.currentAnimationContainer.transform.localScale = 
						characterAnimator.currentAnimationContainer.transform.localScale.x( 
						                                                                   Mathf.Abs(characterAnimator.currentAnimationContainer.transform.localScale.x)); 
				}
			}
		}
		else
		{
			if ( direction == CharacterDirections.Left )
			{
				// if going left, the scale.x needs to be negative
				if( characterAnimator.currentAnimationContainer.transform.localScale.x > 0 )
				{
					characterAnimator.currentAnimationContainer.transform.localScale = 
						characterAnimator.currentAnimationContainer.transform.localScale.x( 
						                                                                   characterAnimator.currentAnimationContainer.transform.localScale.x * -1.0f );
				}
			}
			else if ( direction == CharacterDirections.Right )
			{
				// if going right, the scale.x needs to be positive 
				if( characterAnimator.currentAnimationContainer.transform.localScale.x < 0 )
				{
					characterAnimator.currentAnimationContainer.transform.localScale = 
						characterAnimator.currentAnimationContainer.transform.localScale.x( 
						                                                                   Mathf.Abs(characterAnimator.currentAnimationContainer.transform.localScale.x)); 
				}
			}
		}
	}

}
