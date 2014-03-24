using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanEnemyRotatingGuard : PacmanEnemyCharacter 
{
	public float directionChangeInterval = 4.0f;
	public string discoveredSound = "Discovered01";
	protected float directionChangeTimer = 0;
	protected bool detectedRoutineRunning = false;
	protected ParticleSystem angryParticles = null;
	protected LineRenderer fovRenderer = null;
	protected Material lineMaterial = null;

	public override void SetUpGlobal ()
	{
		base.SetUpGlobal ();

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

//		// randomize start direction
//		currentDirection = (CharacterDirections) Mathf.Pow(2, Random.Range(0, 4)); // CharacterDirections enum has binary values
//		ChangeSpriteDirection(currentDirection);
	}

	protected override void Update () 
	{	
		if (!PacmanGameManager.use.gameRunning || PacmanGameManager.use.Paused)
		{
			return;
		}

		DetectCurrentTile();

		if (!runBehavior)
			return;

		directionChangeTimer += Time.deltaTime;
		if (directionChangeTimer >= 1/speed)
		{
			directionChangeTimer = 0;
			FaceNextDirection();
		}

		ScaleFOV();

		DetectPlayer();

		if (playerFound && !detectedRoutineRunning)
		{
			PlayerSeenEffect();
		}
	}

	protected void ScaleFOV()
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

	protected void FaceNextDirection()
	{
		int nextDirectionStep = (int) currentDirection;
		nextDirectionStep *= 2;  //each value in the direction array is a different bit value

		if (nextDirectionStep > 8)		// wrap around; Undefined direction = 0
		{
			nextDirectionStep = 1;	
		}

		currentDirection = (PacmanCharacter.CharacterDirections)nextDirectionStep;

		ChangeSpriteFacing(currentDirection);
	}

	public override void Reset()
	{
		PlaceAtSpawnLocation();
		
	//	SetDefaultTargetTiles();
		targetTile = defaultTargetTile;
		
		// set the sprite to face the start direction if provided
		if (startDirection != CharacterDirections.Undefined)
		{
			currentDirection = startDirection;
			ChangeSpriteFacing(startDirection);
		}
		else
		{
			currentDirection = CharacterDirections.Left;
			ChangeSpriteFacing(CharacterDirections.Left);
		}
		
		DetectCurrentTile();
		
		enemyState = EnemyState.Neutral;
		
		playerFound = false;
	}

	public override void ChangeSpriteFacing (CharacterDirections direction)
	{
		CharacterDirections adjustedDirection = direction;
		if (direction == CharacterDirections.Right)
		{
			adjustedDirection = CharacterDirections.Left;
		}

		characterAnimator.PlayAnimation(adjustedDirection.ToString());

	
		if (characterAnimator.GetComponent<FlippedIncorrectly>() == null)
		{
			if ( direction == CharacterDirections.Right )
			{
				// if going left, the scale.x needs to be negative
				if( characterAnimator.currentBoneAnimation.transform.localScale.x > 0 )
				{
					characterAnimator.currentBoneAnimation.transform.localScale = 
						characterAnimator.currentBoneAnimation.transform.localScale.x( 
							characterAnimator.currentBoneAnimation.transform.localScale.x * -1.0f );
				}
			}
			else if ( direction == CharacterDirections.Left )
			{
				// if going right, the scale.x needs to be positive 
				if( characterAnimator.currentBoneAnimation.transform.localScale.x < 0 )
				{
					characterAnimator.currentBoneAnimation.transform.localScale = 
						characterAnimator.currentBoneAnimation.transform.localScale.x( 
							Mathf.Abs(characterAnimator.currentBoneAnimation.transform.localScale.x)); 
				}
			}
		}
		else
		{
			if ( direction == CharacterDirections.Left )
			{
				// if going left, the scale.x needs to be negative
				if( characterAnimator.currentBoneAnimation.transform.localScale.x > 0 )
				{
					characterAnimator.currentBoneAnimation.transform.localScale = 
						characterAnimator.currentBoneAnimation.transform.localScale.x( 
						                                                                   characterAnimator.currentBoneAnimation.transform.localScale.x * -1.0f );
				}
			}
			else if ( direction == CharacterDirections.Right )
			{
				// if going right, the scale.x needs to be positive 
				if( characterAnimator.currentBoneAnimation.transform.localScale.x < 0 )
				{
					characterAnimator.currentBoneAnimation.transform.localScale = 
						characterAnimator.currentBoneAnimation.transform.localScale.x( 
						                                                                   Mathf.Abs(characterAnimator.currentBoneAnimation.transform.localScale.x)); 
				}
			}
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
}
