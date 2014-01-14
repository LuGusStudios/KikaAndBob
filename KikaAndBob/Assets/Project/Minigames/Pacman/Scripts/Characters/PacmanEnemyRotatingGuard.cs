using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanEnemyRotatingGuard : PacmanEnemyCharacter 
{
	public float directionChangeInterval = 4.0f;
	public string discoveredSound = "Discovered01";
	protected float directionChangeTimer = 0;
	protected bool detectedRoutineRunning = false;

	
	public override void SetUpGlobal ()
	{
		base.SetUpGlobal ();

//		// randomize start direction
//		currentDirection = (CharacterDirections) Mathf.Pow(2, Random.Range(0, 4)); // CharacterDirections enum has binary values
//		ChangeSpriteDirection(currentDirection);
	}

	protected override void Update () 
	{	
		if (!PacmanGameManager.use.gameRunning)
			return;

		DetectCurrentTile();

		if (!runBehavior)
			return;

		directionChangeTimer += Time.deltaTime;
		if (directionChangeTimer >= 1/speed)
		{
			directionChangeTimer = 0;
			FaceNextDirection();
		}

		DetectPlayer();

		if (playerFound && !detectedRoutineRunning)
		{
			PlayerSeenEffect();
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

	protected void PlayerSeenEffect ()
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
}
