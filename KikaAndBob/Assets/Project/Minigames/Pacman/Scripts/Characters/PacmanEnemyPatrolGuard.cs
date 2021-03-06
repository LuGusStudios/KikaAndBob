﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanEnemyPatrolGuard : EnemyPatrol 
{
	public string discoveredSound = "Discovered01";
	protected bool detectedRoutineRunning = false;
	protected ParticleSystem angryParticles = null;

	protected LineRenderer fovRenderer = null;
	protected Material lineMaterial = null;

	public override void SetUpGlobal()
	{
		base.SetUpGlobal();

		if (angryParticles == null)
		{
			angryParticles = GetComponentInChildren<ParticleSystem>();
		}
		
		if (angryParticles == null)
		{
			Debug.LogError("PacmanEnemyRotatingGuard: Missing angry particles!");
		}

		if (fovRenderer == null)
		{
			fovRenderer = GetComponentInChildren<LineRenderer>();
		}
		
		if (fovRenderer == null)
		{
			Debug.LogError("PacmanEnemyRotatingGuard: Missing FOV!");
		}

		lineMaterial = fovRenderer.material;

		ChangeSpriteFacing(startDirection);
	}

	protected override void Update() 
	{
		if (!PacmanGameManager.use.gameRunning || PacmanGameManager.use.Paused)
		{
			// this just pauses the animation if the game is paused
			if (characterAnimator.currentSpriteAnimation != null && characterAnimator.currentSpriteAnimation.speed > 0)
			{
				characterAnimator.currentSpriteAnimation.speed = 0;
			}

			return;
		}

		// unpause the animation if the game is not paused
		if (characterAnimator.currentSpriteAnimation != null && characterAnimator.currentSpriteAnimation.speed <= 0)
		{
			characterAnimator.currentSpriteAnimation.speed = 1;
		}
		
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

		ScaleFOV();

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
		PacmanGameManager.use.gameRunning = false;	// need to set this already so other guards can't detect the player anymore

		PacmanPlayerCharacter player = PacmanGameManager.use.GetActivePlayer();
		player.characterAnimator.PlayAnimation(player.characterAnimator.idle);

		detectedRoutineRunning = true;

		angryParticles.Play();

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

    protected virtual void ScaleFOV()
	{
		float playerDistance = Vector2.Distance(transform.position.v2(), PacmanGameManager.use.GetActivePlayer().transform.position.v2());
		float maxDistance = (forwardDetectDistance) * PacmanLevelManager.use.scale;
		
		if (playerDistance >= maxDistance)
		{
			return;
		}
		else
		{
			lineMaterial.SetColor("_TintColor", lineMaterial.GetColor("_TintColor").a(1.0f - Mathf.Clamp(playerDistance / maxDistance, 0.0f, 1.0f ) ));
		}


		PacmanTile[] view = PacmanLevelManager.use.GetTilesInDirection(currentTile, forwardDetectDistance, currentDirection);
		PacmanTile lastTile = null;

		foreach (PacmanTile tile in view)
		{
			if (tile == null || tile.tileType == PacmanTile.TileType.Collide)
				break;

			lastTile = tile;
		}

		if (lastTile != null)
		{
			if (!fovRenderer.enabled)
				fovRenderer.enabled = true;

			fovRenderer.SetPosition(0, transform.position);

			Vector3 targetLocation = lastTile.GetWorldLocation().v3().z(transform.position.z);

			targetLocation += (targetLocation - transform.position).normalized * (PacmanLevelManager.use.scale * 0.5f);

			fovRenderer.SetPosition(1, targetLocation);
		}
		else
		{
			if (fovRenderer.enabled)
				fovRenderer.enabled = false;
		}
	}

    public override void ChangeSpriteFacing (CharacterDirections direction)
	{
		CharacterDirections adjustedDirection = direction;

		if ( direction == CharacterDirections.Right )
		{
			adjustedDirection = CharacterDirections.Left;
		}

		characterAnimator.PlayAnimation(adjustedDirection.ToString());

		if (characterAnimator.currentAnimationTransform.GetComponent<FlippedIncorrectly>() == null)
		{
			if ( direction == CharacterDirections.Right )
			{
				// if going right, the scale.x needs to be negative
				if( characterAnimator.currentAnimationTransform.localScale.x > 0 )
				{
					characterAnimator.currentAnimationTransform.localScale = 
						characterAnimator.currentAnimationTransform.localScale.x( Mathf.Abs(characterAnimator.currentAnimationTransform.localScale.x) * -1.0f );
				}
			}
			else if ( direction == CharacterDirections.Left )
			{
				// if going left, the scale.x needs to be positive 
				if( characterAnimator.currentAnimationTransform.localScale.x < 0 )
				{
					characterAnimator.currentAnimationTransform.localScale = 
						characterAnimator.currentAnimationTransform.localScale.x( Mathf.Abs(characterAnimator.currentAnimationTransform.localScale.x) ); 
				}
			}
		}
		else
		{
			if ( direction == CharacterDirections.Left )
			{
				// if going left, the scale.x needs to be negative
				if( characterAnimator.currentAnimationTransform.localScale.x > 0 )
				{
					characterAnimator.currentAnimationTransform.localScale = 
						characterAnimator.currentAnimationTransform.localScale.x( 
						                                                                   characterAnimator.currentAnimationTransform.localScale.x * -1.0f );
				}
			}
			else if ( direction == CharacterDirections.Right )
			{
				// if going right, the scale.x needs to be positive 
				if( characterAnimator.currentAnimationTransform.localScale.x < 0 )
				{
					characterAnimator.currentAnimationTransform.localScale = 
						characterAnimator.currentAnimationTransform.localScale.x( 
						                                                                   Mathf.Abs(characterAnimator.currentAnimationTransform.localScale.x)); 
				}
			}
		}
	}

}
