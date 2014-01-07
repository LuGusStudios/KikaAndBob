using UnityEngine;
using System.Collections;
using SmoothMoves;

public class PacmanPlayerCharacter : PacmanCharacter {

	public bool enemiesFlee = false;
	public float powerupDuration = 10;

	protected bool allowControl = true;
	protected bool cutScene = false;
	protected PacmanCharacter.CharacterDirections nextDirection = CharacterDirections.Undefined;
	protected ILugusAudioTrack walkTrack = null;
	protected LugusAudioTrackSettings walkTrackSettings = null;
	protected AudioClip walkSoundClip = null;


	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start()
	{
		SetupGlobal();
	}

	public void SetupLocal()
	{
		base.SetUpLocal();
	}

	public void SetupGlobal()
	{
		walkTrack = LugusAudio.use.SFX().GetTrack();
		walkTrack.Claim();
		walkTrackSettings = new LugusAudioTrackSettings().Loop(true);
	
		if (!string.IsNullOrEmpty(walkSoundKey))
			walkSoundClip = LugusResources.use.Shared.GetAudio(walkSoundKey);
	}

	private void Update () 
	{
		if (!PacmanGameManager.use.gameRunning)
			return;

		DetectCurrentTile();

		if (currentTile == null)
			return;

		if (allowControl == true)
		{
			if (PacmanInput.use.GetUp())
			{
				nextDirection = PacmanCharacter.CharacterDirections.Up;
				if (!moving)
					DestinationReached();
			}
			else if (PacmanInput.use.GetDown())
			{
				nextDirection = PacmanCharacter.CharacterDirections.Down;
				if (!moving)
					DestinationReached();
			}
			else if (PacmanInput.use.GetLeft())
			{
				nextDirection = PacmanCharacter.CharacterDirections.Left;
				if (!moving)
					DestinationReached();
			}
			else if (PacmanInput.use.GetRight())
			{
				nextDirection = PacmanCharacter.CharacterDirections.Right;
				if (!moving)
					DestinationReached();
			}
		}

		UpdateMovement();

		UpdateWalkSound();
	}

	protected void UpdateWalkSound()
	{
		if (moving)
		{
			if (!walkTrack.Playing)
				walkTrack.Play(walkSoundClip, walkTrackSettings);
		}
		else
		{
			if (walkTrack.Playing)
				walkTrack.Stop();
		}
	}

	public override void Reset()
	{
		enemiesFlee = false;
		characterAnimator.PlayAnimation("Idle");
		//PlayAnimationObject("Idle", PacmanCharacter.CharacterDirections.Undefined);
		DetectCurrentTile();
		ResetMovement();
		PlaceAtSpawnLocation();
	}

	public override void ChangeSpriteDirection(CharacterDirections direction)
	{
		CharacterDirections adjustedDirection = direction;
		
		// Right facing = left flipped on x axis
		if (direction == CharacterDirections.Undefined || direction == CharacterDirections.Right)
		{
			adjustedDirection = CharacterDirections.Left;
		}

		if (enemiesFlee)
		{
			if ( direction == CharacterDirections.Right || direction == CharacterDirections.Left)
			{
				characterAnimator.PlayAnimation(characterAnimator.poweredSide);
			}
			else if (direction == CharacterDirections.Up)
			{
				characterAnimator.PlayAnimation(characterAnimator.poweredUp);
			}
			else
			{
				characterAnimator.PlayAnimation(characterAnimator.poweredDown);
			}
		}
		else
		{
			characterAnimator.PlayAnimation("" + adjustedDirection.ToString());
		}
		
		if ( direction == CharacterDirections.Right )
		{
			// if going left, the scale.x needs to be negative
			if( characterAnimator.currentAnimationContainer.transform.localScale.x > 0 )
			{
				characterAnimator.currentAnimationContainer.transform.localScale = characterAnimator.currentAnimationContainer.transform.localScale.x( characterAnimator.currentAnimationContainer.transform.localScale.x * -1.0f );
			}
		}
		else if ( direction == CharacterDirections.Left )
		{
			// if going right, the scale.x needs to be positive 
			if( characterAnimator.currentAnimationContainer.transform.localScale.x < 0 )
			{
				characterAnimator.currentAnimationContainer.transform.localScale = characterAnimator.currentAnimationContainer.transform.localScale.x( Mathf.Abs(characterAnimator.currentAnimationContainer.transform.localScale.x) ); 
			}
		}
		//PlayAnimationObject("" + adjustedDirection.ToString(), direction);
	}
		
	public override void DestinationReached()
	{	
		if (cutScene)
			return;

		DoCurrentTileBehavior();
		
		moving = false;

		// if we can move in the next selected direction, go there
		PacmanTile nextTile = FindOpenTileInDirection(nextDirection);
		if (nextTile != null)
		{
			currentDirection = nextDirection;
			MoveTo(nextTile);
		}
		else // else continue in the current direction
		{
			nextTile = FindOpenTileInDirection(currentDirection);
			if (nextTile != null)
			{
				MoveTo(nextTile);
			}
		}

		ChangeSpriteDirection(currentDirection);
	}
	
	protected void TryMoveInDirection(CharacterDirections direction)
	{
		PacmanTile newTarget = FindOpenTileInDirection(direction);
		
		if (newTarget != null && newTarget != moveTargetTile)
		{
			MoveTo(newTarget);
			currentDirection = direction;
		}
	}

	// Effects per tile
	// Override for custom behavior
	protected virtual void DoCurrentTileBehavior()
	{
		if (currentTile.tileType == PacmanTile.TileType.Pickup)
		{
			currentTile.tileType = PacmanTile.TileType.Open;
			currentTile.rendered.SetActive(false);
			PacmanLevelManager.use.IncreasePickUpCount();
			PacmanLevelManager.use.CheckPickedUpItems();
		}
		else if (currentTile.tileType == PacmanTile.TileType.Upgrade)
		{
			currentTile.tileType = PacmanTile.TileType.Open;
			if (currentTile.rendered != null)
				currentTile.rendered.SetActive(false);
			LugusCoroutines.use.StartRoutine(PowerupRoutine());
		}
		else if (currentTile.tileType == PacmanTile.TileType.Lethal)
		{
			PacmanGameManager.use.LoseLife();
		}
		else if (currentTile.tileType == PacmanTile.TileType.LevelEnd && PacmanLevelManager.use.AllItemsPickedUp())
		{
			PacmanGameManager.use.WinGame();
			allowControl = false;
			return;
		}
		else if (allowControl && currentTile.tileType == PacmanTile.TileType.Teleport) // this is also under the scope of allowControl, 
			// because we don't want the player character to look for teleports when he's already moving in one
		{
			// TO DO: Make this more elegant and extensible
			if (currentTile.gridIndices.x < (float)PacmanLevelManager.use.width * 0.5f)
				StartCoroutine(TeleportRoutine(true));
			else
				StartCoroutine(TeleportRoutine(false));
			
			return;
		}
	}
	
	protected PacmanTile FindOpenTileInDirection(CharacterDirections direction)
	{
		int xIndex = (int)currentTile.gridIndices.x;	
		int yIndex = (int)currentTile.gridIndices.y;
		PacmanTile inspectedTile = null;
		
		if (direction == PacmanCharacter.CharacterDirections.Up)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex, yIndex+1);
			if (inspectedTile != null)
			{
				if (IsEnemyWalkable(inspectedTile))
					return inspectedTile;
			}
		}
		else if (direction == PacmanCharacter.CharacterDirections.Right)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex+1, yIndex);
			if (inspectedTile != null)
			{
				if (IsEnemyWalkable(inspectedTile))
					return inspectedTile;
			}
		}
		else if (direction == PacmanCharacter.CharacterDirections.Down)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex, yIndex-1);
			if (inspectedTile != null)
			{
				if (IsEnemyWalkable(inspectedTile))
					return inspectedTile;
			}
		}
		else if (direction == PacmanCharacter.CharacterDirections.Left)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex-1, yIndex);
			if (inspectedTile != null)
			{
				if (IsEnemyWalkable(inspectedTile))
					return inspectedTile;
			}
		}
		
		return null;
	}
	
	public bool IsEnemyWalkable(PacmanTile inspectedTile)
	{
		if (inspectedTile.tileType == PacmanTile.TileType.Collide ||
			inspectedTile.tileType == PacmanTile.TileType.Locked)
			return false;

		if (inspectedTile.tileType == PacmanTile.TileType.LevelEnd && !PacmanLevelManager.use.AllItemsPickedUp())
			return false;
		
		return true;
	}

	protected IEnumerator PowerupRoutine()
	{
		enemiesFlee = true;

		yield return new WaitForSeconds(powerupDuration);

		enemiesFlee = false;
	}

	protected IEnumerator TeleportRoutine(bool exitLeft)
	{				
		allowControl = false;
		
		if (exitLeft)
			MoveTo(PacmanLevelManager.use.GetTile(1, 7));			// 7 = y location on grid of teleporters. TO DO: Make cleaner/more extensible.
		else
			MoveTo(PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-2, 7));
		

		yield return new WaitForSeconds(movementDuration*2); // TO DO: This works. Figure out why.
		
		
		if (exitLeft)
		{
			transform.localPosition = PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-1, 7).location;			// put player on other side of level
			currentDirection = PacmanCharacter.CharacterDirections.Left;
			DetectCurrentTile();
			DestinationReached();
			//MoveTo(PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-4, 7));									// initiate movement to first tile next to teleporter
			
		}
		else // exiting on the right
		{
			transform.localPosition = PacmanLevelManager.use.GetTile(0, 7).location;								// put player on other side of level
			currentDirection = PacmanCharacter.CharacterDirections.Right;
			DetectCurrentTile();
			DestinationReached();
			//MoveTo(PacmanLevelManager.use.GetTile(3, 7));														// initiate movement to first tile next to teleporter
		}
		
		
		yield return new WaitForSeconds(movementDuration*3);
		
		
		allowControl = true;
	}
	
	public PacmanCharacter.CharacterDirections GetDirection()
	{
		return currentDirection;
	}

	private bool hitRoutineBusy = false;

	public void DoHitEffect()
	{
		LugusCoroutines.use.StartRoutine(HitRoutine());
	}

	protected IEnumerator HitRoutine()
	{
		hitRoutineBusy = true;

		PacmanGameManager.use.gameRunning = false;

		Color originalColor = Color.white;
		Color color = Color.red; 
		
		float duration = 1.5f; 
		int iterations = 5;
		float partDuration = duration / (float) iterations;

		BoneAnimation[] boneAnimations = GetComponentsInChildren<BoneAnimation>();
		
		for( int i = 0; i < iterations; ++i )
		{
			float percentage = 0.0f;
			float startTime = Time.time;
			bool rising = true;
			Color newColor = new Color();

			while( rising )
			{
				percentage = (Time.time - startTime) / (partDuration / 2.0f);
				newColor = originalColor.Lerp (color, percentage);
				
				foreach( BoneAnimation container in boneAnimations )
					container.SetMeshColor( newColor );

				if( percentage >= 1.0f )
					rising = false;
				
				yield return null;
			}
			
			percentage = 0.0f;
			startTime = Time.time;
			
			while( !rising )
			{
				percentage = (Time.time - startTime) / (partDuration / 2.0f);
				newColor = color.Lerp (originalColor,percentage);
				
				//currentAnimationContainer.SetMeshColor( newColor );
				
				foreach( BoneAnimation container in boneAnimations )
					container.SetMeshColor( newColor );
				
				if( percentage >= 1.0f )
					rising = true;
				
				yield return null;
			}
		}
		
		foreach( BoneAnimation container in boneAnimations )
			container.SetMeshColor( originalColor );

		PacmanGameManager.use.LoseLife();
	}
}
