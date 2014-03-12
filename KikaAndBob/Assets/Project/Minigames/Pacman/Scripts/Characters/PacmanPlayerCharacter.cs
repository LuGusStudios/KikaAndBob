using UnityEngine;
using System.Collections;
using SmoothMoves;

public class PacmanPlayerCharacter : PacmanCharacter {

	public bool poweredUp = false;
	public float powerupDuration = 10.0f;

	protected bool allowControl = true;
	protected bool cutScene = false;
	protected PacmanCharacter.CharacterDirections nextDirection = CharacterDirections.Undefined;
	protected ILugusAudioTrack walkTrack = null;
	protected LugusAudioTrackSettings walkTrackSettings = null;
	protected AudioClip walkSoundClip = null;
	protected BoneAnimation[] boneAnimations = null;
	protected ParticleSystem powerUpParticles = null;
	protected ILugusCoroutineHandle powerUpRoutine = null;
	protected ILugusCoroutineHandle powerUpBlinkRoutine = null;
	protected float powerUpDurationLeft = 0.0f;


	public override void SetUpLocal()
	{
		base.SetUpLocal();
	}

	public override void SetUpGlobal()
	{
		base.SetUpGlobal();

		walkTrack = LugusAudio.use.SFX().GetTrack();
		walkTrack.Claim();
		walkTrackSettings = new LugusAudioTrackSettings().Loop(true);
	
		if (!string.IsNullOrEmpty(walkSoundKey))
			walkSoundClip = LugusResources.use.Shared.GetAudio(walkSoundKey);

		boneAnimations = (BoneAnimation[])GetComponentsInChildren<BoneAnimation>(true);

		if (powerUpParticles == null)
		{
			GameObject powerUpParticlesObject = PacmanLevelManager.use.GetPrefab("PowerUpParticles");

			if (powerUpParticlesObject != null)
			{
				powerUpParticles = powerUpParticlesObject.GetComponent<ParticleSystem>();

				if (powerUpParticles == null)
				{
					Debug.LogError("PacmanCharacter: Missing power up particles!");
				}
			}
		}
	}

	private void Update () 
	{
		if (!PacmanGameManager.use.gameRunning || PacmanGameManager.use.Paused)
			return;

		DetectCurrentTile();

		if (currentTile == null)
			return;

		if (allowControl == true)
		{
			// once we're heading for a teleport tile, disable further input
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

		PacmanCameraFollower.use.FollowCamera();	// the camera does not update automatically anymore - an unpredictable order of Update calls between this and the characters can cause jitter	
													// instead, it's called from the character scripts
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
		moving = false;
		poweredUp = false;
		characterAnimator.PlayAnimation("Idle");
		//PlayAnimationObject("Idle", PacmanCharacter.CharacterDirections.Undefined);
		DetectCurrentTile();
		ResetMovement();
		moveTargetTile = null;
		PlaceAtSpawnLocation();
	}

	public override void ChangeSpriteFacing(CharacterDirections direction)
	{
		CharacterDirections adjustedDirection = direction;
		
		// Right facing = left flipped on x axis
		if (direction == CharacterDirections.Undefined || direction == CharacterDirections.Right)
		{
			adjustedDirection = CharacterDirections.Left;
		}

		if (poweredUp)
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
			// if going right, the scale.x needs to be negative
			if( characterAnimator.currentAnimationTransform.localScale.x > 0 )
			{
				characterAnimator.currentAnimationTransform.localScale = characterAnimator.currentAnimationTransform.localScale.x( characterAnimator.currentAnimationTransform.localScale.x * -1.0f );
			}
		}
		else if ( direction == CharacterDirections.Left )
		{
			// if going left, the scale.x needs to be positive 
			if( characterAnimator.currentAnimationTransform.localScale.x < 0 )
			{
				characterAnimator.currentAnimationTransform.localScale = characterAnimator.currentAnimationTransform.localScale.x( Mathf.Abs(characterAnimator.currentAnimationTransform.localScale.x) ); 
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

		ChangeSpriteFacing(currentDirection);
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
	protected override void DoCurrentTileBehavior()
	{
		// if we just teleported and hit the next non-teleport tile, we're done teleporting
		if (currentTile.tileType != PacmanTile.TileType.Teleport & alreadyTeleported)
		{
			alreadyTeleported = false;
		}

		// check all sorts things placed on this tile
		foreach(GameObject go in currentTile.tileItems)
		{
			if (go.GetComponent<PacmanTileItem>() != null)
			{
				go.GetComponent<PacmanTileItem>().OnEnter();
			}
		}

		if (currentTile.tileType == PacmanTile.TileType.Pickup)
		{
			currentTile.tileType = PacmanTile.TileType.Open;
			currentTile.rendered.SetActive(false);
			PacmanLevelManager.use.IncreasePickUpCount();
		}
		else if (currentTile.tileType == PacmanTile.TileType.Upgrade)
		{
			currentTile.tileType = PacmanTile.TileType.Open;
			if (currentTile.rendered != null)
				currentTile.rendered.SetActive(false);

			powerUpDurationLeft += powerupDuration;

			if (powerUpRoutine == null || !powerUpRoutine.Running)
			{
				powerUpRoutine = LugusCoroutines.use.StartRoutine(PowerupRoutine());
			}

			// if this character is still blinking, stop
			if (powerUpBlinkRoutine != null && powerUpBlinkRoutine.Running)
			{
				powerUpBlinkRoutine.StopRoutine();
				SmoothMovesUtil.SetColor(boneAnimations, Color.white);
			}
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
//	protected override IEnumerator TeleportRoutine()
//	{				
//		alreadyTeleported = true;
//
//
//
//		if (PacmanLevelManager.use.teleportTiles.Count <= 1)
//		{
//			Debug.LogError("There's only one teleport tile in this level!");
//			yield break;
//		}
//
//		if (teleportParticles != null)
//		{
//			ParticleSystem spawnedParticles = (ParticleSystem)Instantiate(teleportParticles);
//			spawnedParticles.transform.position = this.transform.position;
//			
//			spawnedParticles.Play();
//			Destroy(spawnedParticles.gameObject, 2.0f);
//		}
//
//
//
//		// this idea is not what we want, because it links teleports in a circle (always to the next), but not in two directions (i.e. also to the previous one)
////		int indexCurrentTeleport = PacmanLevelManager.use.teleportTiles.IndexOf(currentTile);
////		int	indexCounterpart = indexCurrentTeleport  + 1;
////
////		if (indexCounterpart >= PacmanLevelManager.use.teleportTiles.Count)
////		{
////			indexCounterpart = 0;
////		}
//
//		PacmanTile targetTile = null;
//
//		foreach(PacmanTile tile in PacmanLevelManager.use.teleportTiles)
//		{
//			if (currentTile != tile)
//			{
//				targetTile = tile;
//				break;
//			}
//		}
//		
//		if (targetTile == null)
//		{
//			Debug.LogError("No other teleport tile found!");
//			yield break;
//		}
//		
//		transform.localPosition = targetTile.location.v3();
//
//		if (teleportParticles != null)
//		{
//			ParticleSystem spawnedParticles = (ParticleSystem)Instantiate(teleportParticles);
//			spawnedParticles.transform.position = this.transform.position;
//			
//			spawnedParticles.Play();
//			Destroy(spawnedParticles.gameObject, 2.0f);
//		}
//		
//		DestinationReached();
//	}

	
	protected PacmanTile FindOpenTileInDirection(CharacterDirections direction)
	{
		int xIndex = (int)currentTile.gridIndices.x;	
		int yIndex = (int)currentTile.gridIndices.y;
		PacmanTile inspectedTile = null;
		
		if (direction == PacmanCharacter.CharacterDirections.Up)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex, yIndex+1);
		}
		else if (direction == PacmanCharacter.CharacterDirections.Right)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex+1, yIndex);
		}
		else if (direction == PacmanCharacter.CharacterDirections.Down)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex, yIndex-1);
		}
		else if (direction == PacmanCharacter.CharacterDirections.Left)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex-1, yIndex);
		}

		if (inspectedTile != null)
		{
			// first we run OnTryEnter(), because this might still alter things about the tile (e.g. changing it from Collide to Open if the player has a key for a door)
			foreach(GameObject go in inspectedTile.tileItems)
			{
				if (go.GetComponent<PacmanTileItem>() != null)
				{
					go.GetComponent<PacmanTileItem>().OnTryEnter();
				}
			}

			if (IsEnemyWalkable(inspectedTile))
				return inspectedTile;
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
		poweredUp = true;

		if (powerUpParticles != null)
		{
			ParticleSystem spawnedParticles = (ParticleSystem)Instantiate(powerUpParticles);
			spawnedParticles.transform.position = this.transform.position;

			spawnedParticles.Play();
			Destroy(spawnedParticles.gameObject, 2.0f);
		}

		// this way, we can add on to powerUpDurationLeft at will to keep it going
		while (powerUpDurationLeft > 0.0f)
		{
			yield return new WaitForEndOfFrame();

			powerUpDurationLeft -= Time.deltaTime;

			if (powerUpDurationLeft <= 3.0f)
			{
				if (powerUpBlinkRoutine == null || !powerUpBlinkRoutine.Running)
				{
					powerUpBlinkRoutine = LugusCoroutines.use.StartRoutine(SmoothMovesUtil.Blink(boneAnimations, Color.blue, 3.0f, 5));
				}
			}
		}

		if (powerUpParticles != null)
		{
			ParticleSystem spawnedParticles = (ParticleSystem)Instantiate(powerUpParticles);
			spawnedParticles.transform.position = this.transform.position;
			
			spawnedParticles.Play();
			Destroy(spawnedParticles.gameObject, 2.0f);
		}

		poweredUp = false;
	}
	
	public PacmanCharacter.CharacterDirections GetDirection()
	{
		return currentDirection;
	}

	public void DoHitEffect()
	{
		if (hitRoutineBusy || PacmanGameManager.use.Paused)
			return;

		float duration = 1.5f;

		LugusCoroutines.use.StartRoutine(HitRoutine(duration));
		LugusCoroutines.use.StartRoutine(SmoothMovesUtil.Blink(boneAnimations, Color.red, duration, 4));
	}

	private bool hitRoutineBusy = false;

	protected IEnumerator HitRoutine(float duration)
	{
		PacmanGameManager.use.LoseLife();

		hitRoutineBusy = true;
		PacmanGameManager.use.gameRunning = false;

		characterAnimator.PlayAnimation(characterAnimator.hitAnimation);

		yield return new WaitForSeconds(duration);

		hitRoutineBusy = false;
	}
}
