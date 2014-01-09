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

	protected void Update () 
	{	
		if (!PacmanGameManager.use.gameRunning)
			return;

		DetectCurrentTile();

		if (!runBehavior)
			return;

		directionChangeTimer += Time.deltaTime;
		if (directionChangeTimer >= directionChangeInterval)
		{
			directionChangeTimer = 0;
			FaceNextDirection();
		}

		DetectPlayer();

		if (playerFound && !detectedRoutineRunning)
		{
			PlayerDetectedEffect();
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

		Debug.Log(currentDirection, gameObject);

		ChangeSpriteFacing(currentDirection);
	}

	public override void Reset()
	{
		PlaceAtSpawnLocation();
		
		SetDefaultTargetTiles();
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

		// TO DO: FIX THIS AGAIN TO USE THE X DIRECTION!!!
		// USING A STATIC (ROTATED) SPRITE UNTIL THERE'S A PROPER ANIMATED CHARACTER

		if ( direction == CharacterDirections.Right )
		{
			// if going left, the scale.x needs to be negative
			if( characterAnimator.currentAnimationContainer.transform.localScale.y > 0 )
			{
				characterAnimator.currentAnimationContainer.transform.localScale = characterAnimator.currentAnimationContainer.transform.localScale.y( characterAnimator.currentAnimationContainer.transform.localScale.y * -1.0f );
			}
		}
		else if ( direction == CharacterDirections.Left )
		{
			// if going right, the scale.x needs to be positive 
			if( characterAnimator.currentAnimationContainer.transform.localScale.y < 0 )
			{
				characterAnimator.currentAnimationContainer.transform.localScale = characterAnimator.currentAnimationContainer.transform.localScale.y( Mathf.Abs(characterAnimator.currentAnimationContainer.transform.localScale.y)); 
			}
		}
	}

	protected override void PlayerDetectedEffect ()
	{
		LugusCoroutines.use.StartRoutine(PlayerSeenRoutine());
	}

	protected IEnumerator PlayerSeenRoutine()
	{
		detectedRoutineRunning = true;
		iTween.PunchScale(this.gameObject, Vector3.one, 0.5f);

		yield return new WaitForSeconds(0.5f);

		PacmanGameManager.use.LoseLife();
		detectedRoutineRunning = false;
	}
}
