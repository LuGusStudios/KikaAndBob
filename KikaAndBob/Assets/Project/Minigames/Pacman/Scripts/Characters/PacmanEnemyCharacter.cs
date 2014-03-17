using UnityEngine;
using System.Collections;
using SmoothMoves;

public class PacmanEnemyCharacter : PacmanCharacter {

	public bool allowUTurns = true;
	public int forwardDetectDistance = 5;
	public string walkAnimation = "";
	public string scaredWalkAnimation = "";
	public string defeatSoundKey = "";
	public string attackSoundKey = "";

	protected bool playerFound = false;
	protected bool runBehavior = true;			// if set to false, most behavior is disabled (for cutscenes, teleport etc.)
	protected float scatterModeDuration = 5;
	protected Vector3 originalScale = new Vector3(1f, 1f, 1f);
	protected PacmanTile defaultTargetTile;	// a default tile that the character moves if a target tile was not calculated
	protected PacmanTile lastTile;
	protected PacmanTile targetTile;			// the target tile is the tile we're trying to get to (i.e. different from moveTargetTile)
	protected PacmanPlayerCharacter player = null;
	protected iTweener playerDetectedItweener = null;
	protected iTweener frightenedItweener = null;
	protected ParticleSystem defeatParticles = null;
	protected ParticleSystem frightenedParticles = null;

	private bool debugPathFinding = false;		// set true to mark targetTile in game (Debugging)

	protected Transform targetMarker = null;

	[HideInInspector]
	public EnemyState enemyState = EnemyState.Neutral;
	public enum EnemyState
	{
		Neutral,
		Chasing,
		Frightened,
		Defeated,
	}

	public override void SetUpLocal()
	{
		base.SetUpLocal();

		// TO DO replace
		if (player == null)
			player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));
		if (player == null)
			Debug.Log("Could not find player.");

		if (defeatParticles == null)
		{
			Transform child = transform.FindChild("DefeatParticles");
			if (child != null)
			{
				defeatParticles = child.GetComponent<ParticleSystem>();
				
				if (defeatParticles == null)
				{
					Debug.LogError("PacmanEnemyCharacter: Missing defeat particles!");
				}
			}
		}

		if (frightenedParticles == null)
		{
			Transform child = transform.FindChild("FrightenedParticles");
			if (child != null)
			{
				frightenedParticles = child.GetComponent<ParticleSystem>();
				
				if (frightenedParticles == null)
				{
					Debug.LogError("PacmanEnemyCharacter: Missing frightened particles!");
				}
			}
		}

		#if UNITY_EDITOR	// handy for debugging - not to be included in build
		// used for visualizing enemy target tile
		if (debugPathFinding)
		{
			if (targetMarker != null)
				Destroy(targetMarker.gameObject);

			targetMarker = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
			targetMarker.localScale = Vector3.one * (PacmanLevelManager.use.scale * 0.5f);
			targetMarker.parent = transform.parent;
		}
		#endif	
	}

		
	protected virtual void Update() 
	{
		if (!PacmanGameManager.use.gameRunning || enemyState == EnemyState.Defeated || PacmanGameManager.use.Paused)
			return;

		// update player in case we've changed players (two character game etc.)
		player = PacmanGameManager.use.GetActivePlayer();	

		// the player is in neutral state unless something else is happening

		EnemyState nextState = EnemyState.Neutral;

		// iterate over players to see if we're on the same tile as any of them
		foreach (PacmanPlayerCharacter p in PacmanGameManager.use.GetPlayerChars())
		{
			if (p.poweredUp) 
			{
				nextState = EnemyState.Frightened;
			}

			// if we're on the same tile as a player, determine behavior
			if (currentTile == p.currentTile)
			{
				// if player is powered up, defeat this enemy
				if (p.poweredUp)
				{
					nextState = EnemyState.Defeated;
				}
				// else, player loses life
				else
				{
					if (!string.IsNullOrEmpty(attackSoundKey))
					{
						LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(attackSoundKey));
					}
					p.DoHitEffect();
				}
			}
		}

		if (nextState == EnemyState.Neutral)
		{
			NeutralEffect();
		}
		else if (nextState == EnemyState.Frightened)
		{
			FrightenedEffect();
		}
		else if (nextState == EnemyState.Defeated)
		{
			DefeatedEffect();
		}
		else if (nextState == EnemyState.Chasing)
		{
			PlayerSeenEffect();
		}


		// what tile is character currently on?
		DetectCurrentTile();

		if (!runBehavior)
			return;

		// is player near
		DetectPlayer();

		// move
		UpdateMovement();

		#if UNITY_EDITOR	// only handy for debugging
		if (debugPathFinding)
		{
			targetMarker.position = targetTile.GetWorldLocation().v3();
		}
		#endif
	}
	
	// reset enemy to default values, location, etc.
	public override void Reset()
	{
		PlaceAtSpawnLocation();

		targetTile = defaultTargetTile;

		// set the sprite to face the start direction if provided
		if (startDirection != CharacterDirections.Undefined)
		{
			currentDirection = startDirection;
			ChangeSpriteFacing(startDirection);
		}
		else // else, default to left
		{
			currentDirection = CharacterDirections.Left;
			ChangeSpriteFacing(CharacterDirections.Left);
		}

		DetectCurrentTile();	// figure out what tile this enemy is on after having been moved to start position
		DestinationReached(); 	// calling DestinationReached will set enemies moving again
		
		NeutralEffect();

		playerFound = false;

		characterAnimator.PlayAnimation(walkAnimation);
	}

	// Sets a number of default target tiles. How these are used exactly is up to the character type itself.
	// Override for different default tile per enemy or setting different paths etc.
	public override void SetDefaultTargetTiles(Vector2[] defaultTargetTiles)
	{
		// this default implementation just takes the first valid tile provided and ignores the rest
		// others might for instance use several to construct a patrol path

		PacmanTile tile = null;

		// pick the first valid tile from a patrol path if given
		foreach(Vector2 indices in defaultTargetTiles)
		{
			tile = PacmanLevelManager.use.GetTile(indices);
			
			if (tile != null)
				break;
		}

		if (tile == null)
		{
			// this can be desired behavior, so it shouldn't register as an error; e.g. the standard chasing enemy has no use for its
			// default target tile, since it always finds the player anyway. Other enemy types might, though.
			// Debug.LogError(this.gameObject.name + ": No valid default target tile found. Defaulting to (0,0).");
			defaultTargetTile = PacmanLevelManager.use.GetTile(0,0);
		}
		else
		{
			defaultTargetTile = tile;
		}
	}

	// Set playerFound bool in this method and call any effects on the player
	// Override to get a custom player detection method (e.g. only in front of player, in a circle around the player, etc.)
	protected virtual void DetectPlayer()
	{
		if (currentTile == null)
			return;

		// this default implementation looks forwardDetectDistance tiles in the current direction and checks for a player there
		foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesInDirection(currentTile, forwardDetectDistance, currentDirection))
		{
			if (tile != null)
			{
				// if the tile is not open, line of sight is broken
				if (tile.tileType == PacmanTile.TileType.Collide || tile.tileType == PacmanTile.TileType.Hide)
				{
					playerFound = false;
					return;
				}
				else 
				{
					// check if any players are on the currently inspected tile
					foreach(PacmanPlayerCharacter playerChar in PacmanGameManager.use.GetPlayerChars())
					{
						if (tile == playerChar.currentTile)
						{
							playerFound = true;
							return;
						}
					}
				}
			}
		}

		playerFound = false;
	}

	// override for custom effect when the enemy loses track of the player
	protected virtual void NeutralEffect()
	{
		if (enemyState == EnemyState.Neutral)
			return;

		characterAnimator.PlayAnimation(walkAnimation);

		enemyState = EnemyState.Neutral;
	}

	// override for custom effect when the enemy finds the player
	protected virtual void PlayerSeenEffect()
	{
	}

	// override for custom effect when the enemy runs away from the player
	protected virtual void FrightenedEffect()
	{
		if (enemyState == EnemyState.Frightened)
			return;

		// flip their direction to make the scorpions run in fear
		currentDirection = PacmanLevelManager.use.GetOppositeDirection(currentDirection);

		frightenedParticles.Play();

		characterAnimator.PlayAnimation(characterAnimator.runScared);

		enemyState = EnemyState.Frightened;
	}

	// override for custom death anim	
	protected virtual void DefeatedEffect()
	{
		if (enemyState == EnemyState.Defeated)
			return;

		StartCoroutine(DefeatAnim());

		enemyState = EnemyState.Defeated;
	}
	
	private IEnumerator DefeatAnim()
	{
		runBehavior = false;
		gameObject.ScaleTo(Vector3.zero).Time(0.5f).Execute();

		if (!string.IsNullOrEmpty(defeatSoundKey))
		{
			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(defeatSoundKey));
		}

		// instatiate the particles, because we don't want them to scale or be affected by anything similar taking place on the enemy proper
		if (defeatParticles != null)
		{
			ParticleSystem particlesSpawn = (ParticleSystem)Instantiate(defeatParticles);
			particlesSpawn.transform.position = defeatParticles.transform.position;
			particlesSpawn.Play();

			while (particlesSpawn.isPlaying)
			{
				yield return new WaitForEndOfFrame();
			}

			Destroy(particlesSpawn.gameObject);
		}
		else
		{
			// default wait time
			yield return new WaitForSeconds(0.5f);
		}
		
		transform.localScale = originalScale;
		gameObject.SetActive(false);
	}

	public override void ChangeSpriteFacing (CharacterDirections direction)
	{
		// enemies probably only ever have one animation
		//characterAnimator.PlayAnimation(walkAnimation);

		if (characterAnimator.currentAnimationTransform == null)
			return;

		if ( direction == CharacterDirections.Right )
		{
			// if going left, the scale.x needs to be negative
			if( characterAnimator.currentAnimationTransform.transform.localScale.x > 0 )
			{
				characterAnimator.currentAnimationTransform.transform.localScale = characterAnimator.currentAnimationTransform.transform.localScale.x( characterAnimator.currentBoneAnimation.transform.localScale.x * -1.0f );
			}
		}
		else if ( direction == CharacterDirections.Left )
		{
			// if going right, the scale.x needs to be positive 
			if( characterAnimator.currentAnimationTransform.transform.localScale.x < 0 )
			{
				characterAnimator.currentAnimationTransform.transform.localScale = characterAnimator.currentAnimationTransform.transform.localScale.x( Mathf.Abs(characterAnimator.currentBoneAnimation.transform.localScale.x) ); 
			}
		}
	}

	
	// override for custom behavior upon having reached a tile (e.g. picking the next tile to move to)
	public override void DestinationReached ()
	{
		if (runBehavior == true)
		{
			DoCurrentTileBehavior();

			if (enemyState == EnemyState.Frightened)
			{
				// run away to faraway tile
				PacmanTile[] tiles = PacmanLevelManager.use.GetTilesForQuadrant(
					PacmanLevelManager.use.GetOppositeQuadrant(
					PacmanLevelManager.use.GetQuadrantOfTile(player.currentTile)));
				
				targetTile = tiles[Random.Range(0, tiles.Length - 1)];
			}
			else
			{
				// head for player
				targetTile = player.currentTile;
			}

			// before finding the next tile to move to, check if target tile cannot be reached more easily through a teleport
			CheckTeleportProximity(); 

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
	}

	protected override void DoCurrentTileBehavior()
	{
		// TO DO: Enemy teleport is highly unpredictable.
		// if we just teleported and hit the next non-teleport tile, we're done teleporting
//		if (currentTile.tileType != PacmanTile.TileType.Teleport & alreadyTeleported)
//		{
//			alreadyTeleported = false;
//		}
//
//		if (currentTile.tileType == PacmanTile.TileType.Teleport && !alreadyTeleported)
//		{
//			LugusCoroutines.use.StartRoutine(TeleportRoutine());
//		}
	}

	// Override for custom behavior
	protected virtual void CheckTeleportProximity()
	{
		// TO DO: Too unpredictable to test in the short term.


		/*
		if (targetTile == null)
			return;

		float distanceFromMeToClosestTeleport = Mathf.Infinity;
		PacmanTile tileClosestToMe = null;

		float distanceFromTargetToClosestTeleport  = Mathf.Infinity;
		PacmanTile tileClosestToTarget = null;
		
		foreach(PacmanTile tile in PacmanLevelManager.use.teleportTiles)
		{
			if (Vector2.Distance(currentTile.location, tile.location) <= distanceFromMeToClosestTeleport)
			{
				distanceFromMeToClosestTeleport = Vector2.Distance(currentTile.location, tile.location);
				tileClosestToMe = tile;
			}

			if (Vector2.Distance(targetTile.location, tile.location) <= distanceFromTargetToClosestTeleport)
			{
				distanceFromTargetToClosestTeleport = Vector2.Distance(targetTile.location, tile.location);
				tileClosestToTarget = tile;
			}
		}

		if (tileClosestToMe == null || tileClosestToTarget == null || tileClosestToMe == tileClosestToTarget)
			return;

		if (distanceFromMeToClosestTeleport + distanceFromTargetToClosestTeleport < Vector2.Distance(currentTile.location, targetTile.location))
		{
			print (distanceFromMeToClosestTeleport + distanceFromTargetToClosestTeleport);
			print (Vector2.Distance(currentTile.location, targetTile.location));

			targetTile = tileClosestToMe;
		}
		*/
	}

	// Returns a tile that is the best bet for getting from the tile where the enemy is now to the target tile.
	// Override for custom behavior
	protected virtual PacmanTile FindTileClosestTo(PacmanTile target)
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
		inspectedTile = PacmanLevelManager.use.GetTile(xCoord-1 , yCoord);
		if (inspectedTile != null)
		{
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
		inspectedTile = PacmanLevelManager.use.GetTile(xCoord, yCoord-1);
		if (inspectedTile != null)
		{
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
		inspectedTile = PacmanLevelManager.use.GetTile(xCoord+1, yCoord);
		if (inspectedTile != null)
		{
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
	
	public static bool IsEnemyWalkable(PacmanTile inspectedTile)
	{
		if( inspectedTile.tileType == PacmanTile.TileType.Collide ||
		   	inspectedTile.tileType == PacmanTile.TileType.Locked ||
		   	inspectedTile.tileType == PacmanTile.TileType.LevelEnd ||
		   	inspectedTile.tileType == PacmanTile.TileType.EnemyAvoid ||
		  	inspectedTile.tileType == PacmanTile.TileType.Lethal ||
		    inspectedTile.tileType == PacmanTile.TileType.Teleport ||
            inspectedTile.tileType == PacmanTile.TileType.Hide
		   )
			return false;
		
		return true;
	}
}
