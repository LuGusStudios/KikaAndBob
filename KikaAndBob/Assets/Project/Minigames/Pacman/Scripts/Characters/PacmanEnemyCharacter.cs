using UnityEngine;
using System.Collections;
using SmoothMoves;

public class PacmanEnemyCharacter : PacmanCharacter {

	public bool allowUTurns = true;
	public int forwardDetectDistance = 5;
	public string walkAnimation = "";
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
	private bool debugPathFinding = false;		// set true to mark targetTile in game (Debugging)

	protected Transform targetMarker = null;

	[HideInInspector]
	public EnemyState enemyState = EnemyState.Neutral;
	public enum EnemyState
	{
		Neutral,
		Chasing,
		Frightened
	}

	public override void SetUpLocal()
	{
		base.SetUpLocal();

		// TO DO replace
		if (player == null)
			player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));
		if (player == null)
			Debug.Log("Could not find player.");
		
		//		// used for visualizing enemy target tile
		//		if (debugPathFinding)
		//		{
		//			if (targetMarker != null)
		//				Destroy(targetMarker.gameObject);
		//
		//			targetMarker = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		//			targetMarker.localScale = Vector3.one * (PacmanLevelManager.use.scale * 0.5f);
		//			targetMarker.parent = transform.parent;
		//		}
	}

		
	void Update() 
	{
		if (!PacmanGameManager.use.gameRunning)
			return;

		foreach (PacmanPlayerCharacter p in PacmanGameManager.use.GetPlayerChars())
		{
			if (currentTile == p.currentTile)
			{
				if (enemyState != EnemyState.Frightened)
				{
					p.DoHitEffect();
				}
				else
					DefeatedEffect();
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

		if (debugPathFinding)
		{
			targetMarker.localPosition = targetTile.location;
		}
	}
	
	public override void Reset()
	{
		PlaceAtSpawnLocation();
		playerFound = false;
		SetDefaultTargetTiles();
		targetTile = defaultTargetTile;
		enemyState = EnemyState.Neutral;
		DetectCurrentTile();
		DestinationReached(); // calling DestinationReached will set enemies moving again

		//ChangeSpriteDirection(CharacterDirections.Left);
		//PlayAnimation(walkAnimation, CharacterDirections.Left);
		characterAnimator.PlayAnimation(walkAnimation);
	}

	// Set tile that enemy will originally try to find here.
	// Override for different default tile per enemy or setting other paths etc.
	protected virtual void SetDefaultTargetTiles()
	{
		defaultTargetTile = PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-1, PacmanLevelManager.use.height-1);
	}

	// set playerFound bool in this method and call any effects on the player
	// override to get a custom detection method (e.g. only in front of player, in a circle around the player, etc.)
	protected virtual void DetectPlayer()
	{
		foreach (PacmanTile tile in PacmanLevelManager.use.GetTilesInDirection(currentTile, forwardDetectDistance, currentDirection))
		{
			if (tile != null)
			{
				// if the tile is not open, line of sight is broken
				if (tile.tileType == PacmanTile.TileType.Collide)
				{
					playerFound = false;
					return;
				}
				// if player is in this tile, we have visual contact
				else if (tile == player.currentTile)
				{
					playerFound = true;
					return;
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

//		iTween.Stop(gameObject);
//		transform.localScale = originalScale;

		//renderer.material.color = Color.white;

		enemyState = EnemyState.Neutral;
	}

	// override for custom effect when the enemy finds the player
	protected virtual void PlayerDetectedEffect()
	{
		if (enemyState == EnemyState.Chasing)
			return;

//		iTween.Stop(gameObject);
//		transform.localScale = originalScale;
		
		// do detected effect
//		if (playerDetectedItweener == null)
//		{
//			// TO DO: Add updated iTweener
//			//playerDetectedItweener = gameObject.ScaleTo(Vector3.one * 0.7f).Time(0.5f).EaseType(iTween.EaseType.easeInOutQuad).LoopType(iTween.LoopType.pingPong);
//
//
//		}
//		playerDetectedItweener.Execute();

//		iTween.ScaleTo(gameObject, iTween.Hash(
//			"scale", Vector3.one * 1.1f,
//			"time", 0.5f,
//			"easetype", iTween.EaseType.easeInOutQuad,
//			"looptype", iTween.LoopType.pingPong));

		//renderer.material.color = Color.red;

		enemyState = EnemyState.Chasing;
	}

	public override void ChangeSpriteDirection (CharacterDirections direction)
	{
		// enemies probably only ever have one animation
		//PlayAnimationObject(CharacterDirections.Left.ToString(), direction);
		characterAnimator.PlayAnimation(CharacterDirections.Left.ToString());

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
	}

	// override for custom effect when the enemy runs away from the player
	protected virtual void FrightenedEffect()
	{
		if (enemyState == EnemyState.Frightened)
			return;

		iTween.Stop(gameObject);
		transform.localScale = originalScale;

		//TO DO: Add updated iTweener
		// do frightened effect
//		if (frightenedItweener == null)
//		{
//			
//			frightenedItweener = gameObject.ScaleTo(Vector3.one * 0.6f).Time(0.2f).EaseType(iTween.EaseType.easeInOutQuad).LoopType(iTween.LoopType.pingPong);
//		}
//		frightenedItweener.Execute();

		iTween.ScaleTo(gameObject, iTween.Hash(
			"scale", Vector3.one * 0.9f,
			"time", 0.5f,
			"easetype", iTween.EaseType.easeInOutQuad,
			"looptype", iTween.LoopType.pingPong));

		//renderer.material.color = Color.blue;

		enemyState = EnemyState.Frightened;
	}

	protected virtual void DefeatedEffect()
	{
		StartCoroutine(DefeatAnim());
	}

	private IEnumerator DefeatAnim()
	{
		runBehavior = false;
		gameObject.ScaleTo(Vector3.zero).Time(0.5f).Execute();

		yield return new WaitForSeconds(0.5f);

		transform.localScale = originalScale;
		gameObject.SetActive(false);
	}

//	public override void PlayAnimation(string clipName, CharacterDirections direction)
//	{
//		foreach (BoneAnimation ba in boneAnimations)
//		{
//			if (ba.AnimationClipExists(clipName))
//			{
//				ba.Play(clipName);
//				break;
//			}
//		}
//	}

	// override for custom behavior upon having reached a tile (e.g. picking the next tile to move to)
	public override void DestinationReached ()
	{
		if (runBehavior == true)
		{
			MoveTo(FindTileClosestTo(targetTile));
				
			if(moveTargetTile == null)
			{
				Debug.LogError("EnemyCharacterMover: Target tile was null!");
				return;
			}
			
			// figure out if target tile is to the right or to the left of the current position
			if (moveTargetTile.gridIndices.x > currentTile.gridIndices.x)
				ChangeSpriteDirection(CharacterDirections.Right);
			else if (moveTargetTile.gridIndices.x < currentTile.gridIndices.x)
				ChangeSpriteDirection(CharacterDirections.Left);
			else if (moveTargetTile.gridIndices.y < currentTile.gridIndices.y)
				ChangeSpriteDirection(CharacterDirections.Down);
			else if (moveTargetTile.gridIndices.y > currentTile.gridIndices.y)
				ChangeSpriteDirection(CharacterDirections.Up);
		}
	}

	
	// UpdateTargetTile runs every frame to decide where the enemy is trying to get to (random tile, player, etc.)
	// Override for custom behavior
	protected virtual void CheckTeleportProximity()
	{
		if (targetTile != null)
			return;

		// detect if it is more efficient to use a teleport than to find target tile directly
		// if target is more than half a level away
		if (Mathf.Abs(targetTile.gridIndices.x - currentTile.gridIndices.x) > (float)PacmanLevelManager.use.width *0.5f) // if player is (more than) half a level away in x distance
		{
			// and we're a quarter level or less way from a teleport
			foreach(PacmanTile tile in PacmanLevelManager.use.teleportTiles)
			{
				if (Vector2.Distance(currentTile.location, tile.location) <= PacmanLevelManager.use.width * 0.25f)
				{
					targetTile = tile;
					break;
				}
			}
		}	
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
		  	inspectedTile.tileType == PacmanTile.TileType.Lethal
			)
			return false;
		
		return true;
	}
}
