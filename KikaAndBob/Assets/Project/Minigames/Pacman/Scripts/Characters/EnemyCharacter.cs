using UnityEngine;
using System.Collections;

public class EnemyCharacter : Character {

	public bool allowUTurns = true;
	public int forwardDetectDistance = 5;
	protected bool playerFound = false;
	protected bool runBehavior = true;			// if set to false, most behavior is disabled (for cutscenes, teleport etc.)
	protected float scatterModeDuration = 5;
	protected Vector3 originalScale = new Vector3(1f, 1f, 1f);
	protected GameTile defaultTargetTile;	// a default tile that the character moves if a target tile was not calculated
	protected GameTile lastTile;
	protected GameTile targetTile;			// the target tile is the tile we're trying to get to (i.e. different from moveTargetTile)
	protected PlayerCharacter player = null;
	protected iTweener playerDetectedItweener = null;
	protected iTweener frightenedItweener = null;
	private bool debugPathFinding = true;

	protected Transform targetMarker = null;

	[HideInInspector]
	public EnemyState enemyState = EnemyState.Neutral;
	public enum EnemyState
	{
		Neutral,
		Chasing,
		Frightened
	}
	
	void Start()
	{
		if (player == null)
			player = (PlayerCharacter) FindObjectOfType(typeof(PlayerCharacter));

		if (debugPathFinding)
		{
			if (targetMarker != null)
				Destroy(targetMarker.gameObject);

			targetMarker = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
			targetMarker.localScale = Vector3.one * (PacmanLevelManager.use.scale * 0.5f);
			targetMarker.parent = transform.parent;
		}
	}
		
	void Update() 
	{
		if (!PacmanGameManager.use.gameRunning)
			return;

		if (currentTile == player.currentTile)
		{
			if (enemyState != EnemyState.Frightened)
				PacmanGameManager.use.LoseLife();
			else
				DefeatedEffect();
		}

		// what tile is character currently on?
		DetectCurrentTile();

		if (!runBehavior)
			return;

		// is player near
		DetectPlayer();

		// move
		UpdatePosition();

		if (debugPathFinding)
		{
			targetMarker.localPosition = targetTile.location;
		}
	}
	
	public void Reset(Vector2 enemySpawnLocation)
	{
		playerFound = false;
		SetDefaultTargetTiles();
		targetTile = defaultTargetTile;
		transform.localPosition = PacmanLevelManager.use.GetTile(enemySpawnLocation).location;
		enemyState = EnemyState.Neutral;
		DetectCurrentTile();
		DestinationReached(); // calling DestinationReached will set enemies moving again
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
		foreach (GameTile tile in PacmanLevelManager.use.GetTilesInDirection(currentTile, forwardDetectDistance, currentDirection))
		{
			if (tile != null)
			{
				// if the tile is not open, line of sight is broken
				if (tile.tileType == GameTile.TileType.Collide)
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

		iTween.Stop(gameObject);
		transform.localScale = originalScale;

		renderer.material.color = Color.white;

		enemyState = EnemyState.Neutral;
	}

	// override for custom effect when the enemy finds the player
	protected virtual void PlayerDetectedEffect()
	{
		if (enemyState == EnemyState.Chasing)
			return;

		iTween.Stop(gameObject);
		transform.localScale = originalScale;
		
		// do detected effect
//		if (playerDetectedItweener == null)
//		{
//			// TO DO: Add updated iTweener
//			//playerDetectedItweener = gameObject.ScaleTo(Vector3.one * 0.7f).Time(0.5f).EaseType(iTween.EaseType.easeInOutQuad).LoopType(iTween.LoopType.pingPong);
//
//
//		}
//		playerDetectedItweener.Execute();

		iTween.ScaleTo(gameObject, iTween.Hash(
			"scale", Vector3.one * 0.7f,
			"time", 0.5f,
			"easetype", iTween.EaseType.easeInOutQuad,
			"looptype", iTween.LoopType.pingPong));

		renderer.material.color = Color.red;

		enemyState = EnemyState.Chasing;
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
			"scale", Vector3.one * 0.6f,
			"time", 0.5f,
			"easetype", iTween.EaseType.easeInOutQuad,
			"looptype", iTween.LoopType.pingPong));

		renderer.material.color = Color.blue;

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

	// override for custom behavior upon having reached a tile (e.g. picking the next tile to move to)
	public override void DestinationReached ()
	{
		if (runBehavior == true)
		{
			if (currentTile.tileType == GameTile.TileType.Teleport)
			{
				// TO DO: Make this more elegant and extensible
				if (currentTile.gridIndices.x < (float)PacmanLevelManager.use.width * 0.5f)
					StartCoroutine(TeleportRoutine(true));
				else
					StartCoroutine(TeleportRoutine(false));
				
				return;
			}
			else
			{
				MoveTo(FindTileClosestTo(targetTile));
			}
			
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
		// detect if it is more efficient to use a teleport than to find target tile directly
		// if target is more than half a level away
		if (Mathf.Abs(targetTile.gridIndices.x - currentTile.gridIndices.x) > (float)PacmanLevelManager.use.width *0.5f) // if player is (more than) half a level away in x distance
		{
			// and we're a quarter level or less way from a teleport
			foreach(GameTile tile in PacmanLevelManager.use.teleportTiles)
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
	protected virtual GameTile FindTileClosestTo(GameTile target)
	{
		GameTile closestTile = null;
		GameTile inspectedTile;
		float closestDistance = Mathf.Infinity;
		Character.CharacterDirections proposedDirection = Character.CharacterDirections.Undefined;
		
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
				if (distance < closestDistance && (allowUTurns || currentDirection != Character.CharacterDirections.Down || currentTile.exitCount <= 1))
				{
					closestDistance = distance;
					closestTile = inspectedTile;
					proposedDirection = Character.CharacterDirections.Up;
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
				if (distance < closestDistance && (allowUTurns || currentDirection != Character.CharacterDirections.Right || currentTile.exitCount <= 1))
				{
					closestDistance = distance;
					closestTile = inspectedTile;
					proposedDirection = Character.CharacterDirections.Left;
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
				if (distance < closestDistance && (allowUTurns || currentDirection != Character.CharacterDirections.Up || currentTile.exitCount <= 1))
				{	
					closestDistance = distance;
					closestTile = inspectedTile;
					proposedDirection = Character.CharacterDirections.Down;
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
				if (distance < closestDistance && (allowUTurns || currentDirection != Character.CharacterDirections.Left || currentTile.exitCount <= 1))
				{
					closestDistance = distance;
					closestTile = inspectedTile;
					proposedDirection = Character.CharacterDirections.Right;
				}
			}
		}
		
		if (proposedDirection != Character.CharacterDirections.Undefined)
			currentDirection = proposedDirection;
		
		return closestTile;
	}
	
	public static bool IsEnemyWalkable(GameTile inspectedTile)
	{
		if( inspectedTile.tileType == GameTile.TileType.Collide ||
		   	inspectedTile.tileType == GameTile.TileType.Locked ||
		   	inspectedTile.tileType == GameTile.TileType.LevelEnd ||
		   	inspectedTile.tileType == GameTile.TileType.EnemyAvoid ||
		  	inspectedTile.tileType == GameTile.TileType.Lethal
			)
			return false;
		
		return true;
	}
	
//	public void SetChasePlayer(bool chase)
//	{
//		chasePlayer = chase;
//		
//		if(chasePlayer == false)
//		{
//			if (Time.frameCount > 1)	// does nothing except to prevent this message from being shown in the first frame, before the main menu comes up
//				Debug.Log(gameObject.name + " went into scatter mode.");
//			targetTile = defaultTargetTile;
//			StartCoroutine(EndScatterMode());
//		}
//	}
	
	IEnumerator TeleportRoutine(bool exitLeft)
	{				
		runBehavior = false;
			
		if (exitLeft)
			MoveTo(PacmanLevelManager.use.GetTile(0, 7));			// 7 = y location on grid of teleporters. TO DO: Make cleaner/more extinsible.
		else
			MoveTo(PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-1, 7));
		
		yield return new WaitForSeconds(movementDuration);
		
		if (exitLeft)
		{
			transform.localPosition = PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-1, 7).location;			// put player on other side of level
			DetectCurrentTile();
			MoveTo(PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-4, 7));									// initiate movement to first tile next to teleporter
		}
		else // exiting on the right
		{
			transform.localPosition = PacmanLevelManager.use.GetTile(0, 7).location;								// put player on other side of level
			DetectCurrentTile();
			MoveTo(PacmanLevelManager.use.GetTile(3, 7));														// initiate movement to first tile next to teleporter
		}

		yield return new WaitForSeconds(movementDuration);
		
		runBehavior = true;
	}
	
//	IEnumerator EndScatterMode()
//	{
//		yield return new WaitForSeconds(scatterModeDuration);
//			
//		chasePlayer = true;
//		Debug.Log(gameObject.name + " stopped scattering.");
//	}
}
