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

	
	protected void Start()
	{
		SetupGlobal();
	}

	public override void SetUpLocal()
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
			if (moveTargetTile != null && moveTargetTile.tileType == PacmanTile.TileType.Teleport)
				return;

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
		if (currentTile.tileType != PacmanTile.TileType.Teleport & alreadyTeleported)
		{
			alreadyTeleported = false;
		}

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
		else if (currentTile.tileType == PacmanTile.TileType.Teleport && !alreadyTeleported)
		{
			LugusCoroutines.use.StartRoutine(TeleportRoutine());
		}
	}

	// TO DO: This doesn't really need to be a coroutine anymore
	protected override IEnumerator TeleportRoutine()
	{				
		alreadyTeleported = true;

		PacmanTile targetTile = null;

		if (PacmanLevelManager.use.teleportTiles.Count <= 1)
		{
			Debug.LogError("There's only one teleport tile in this level!");
			yield break;
		}

		// this idea is not what we want, because it links teleports in a circle (always to the next), but not in two directions (i.e. also to the previous one)
//		int indexCurrentTeleport = PacmanLevelManager.use.teleportTiles.IndexOf(currentTile);
//		int	indexCounterpart = indexCurrentTeleport  + 1;
//
//		if (indexCounterpart >= PacmanLevelManager.use.teleportTiles.Count)
//		{
//			indexCounterpart = 0;
//		}

		foreach(PacmanTile tile in PacmanLevelManager.use.teleportTiles)
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

		DestinationReached();
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
