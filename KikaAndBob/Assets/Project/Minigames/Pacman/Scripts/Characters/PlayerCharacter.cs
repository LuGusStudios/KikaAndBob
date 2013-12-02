using UnityEngine;
using System.Collections;

public class PlayerCharacter : Character {

	public bool enemiesFlee = false;
	public float powerupDuration = 10;
	protected bool allowControl = true;
	protected bool cutScene = false;
	protected Character.CharacterDirections nextDirection = CharacterDirections.Undefined;

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
				nextDirection = Character.CharacterDirections.Up;
				if (!moving)
					DestinationReached();
			}
			else if (PacmanInput.use.GetDown())
			{
				nextDirection = Character.CharacterDirections.Down;
				if (!moving)
					DestinationReached();
			}
			else if (PacmanInput.use.GetLeft())
			{
				nextDirection = Character.CharacterDirections.Left;
				if (!moving)
					DestinationReached();
			}
			else if (PacmanInput.use.GetRight())
			{
				nextDirection = Character.CharacterDirections.Right;
				if (!moving)
					DestinationReached();
			}
		}

		UpdatePosition();

		ChangeSpriteDirection (nextDirection);
	}
	
	public override void DestinationReached()
	{	
		if (cutScene)
			return;

		DoCurrentTileBehavior();
		
		moving = false;

		// if we can move in the next selected direction, go there
		GameTile nextTile = FindOpenTileInDirection(nextDirection);
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
	}
	
	protected void TryMoveInDirection(CharacterDirections direction)
	{
		GameTile newTarget = FindOpenTileInDirection(direction);
		
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
		if (currentTile.tileType == GameTile.TileType.Pickup)
		{
			currentTile.tileType = GameTile.TileType.Open;
			currentTile.sprite.SetActive(false);
			PacmanLevelManager.use.IncreasePickUpCount();
			PacmanLevelManager.use.CheckPickedUpItems();
		}
		else if (currentTile.tileType == GameTile.TileType.Upgrade)
		{
			currentTile.tileType = GameTile.TileType.Open;
			currentTile.sprite.SetActive(false);
			LugusCoroutines.use.StartRoutine(PowerupRoutine());
		}
		else if (currentTile.tileType == GameTile.TileType.Lethal)
		{
			PacmanGameManager.use.LoseLife();
		}
		else if (currentTile.tileType == GameTile.TileType.LevelEnd && PacmanLevelManager.use.AllItemsPickedUp())
		{
			PacmanGameManager.use.WinGame();
			allowControl = false;
			return;
		}
		else if (allowControl && currentTile.tileType == GameTile.TileType.Teleport) // this is also under the scope of allowControl, 
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
	
	protected GameTile FindOpenTileInDirection(CharacterDirections direction)
	{
		int xIndex = (int)currentTile.gridIndices.x;	
		int yIndex = (int)currentTile.gridIndices.y;
		GameTile inspectedTile = null;
		
		if (direction == Character.CharacterDirections.Up)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex, yIndex+1);
			if (inspectedTile != null)
			{
				if (IsEnemyWalkable(inspectedTile))
					return inspectedTile;
			}
		}
		else if (direction == Character.CharacterDirections.Right)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex+1, yIndex);
			if (inspectedTile != null)
			{
				if (IsEnemyWalkable(inspectedTile))
					return inspectedTile;
			}
		}
		else if (direction == Character.CharacterDirections.Down)
		{
			inspectedTile = PacmanLevelManager.use.GetTile(xIndex, yIndex-1);
			if (inspectedTile != null)
			{
				if (IsEnemyWalkable(inspectedTile))
					return inspectedTile;
			}
		}
		else if (direction == Character.CharacterDirections.Left)
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
	
	public bool IsEnemyWalkable(GameTile inspectedTile)
	{
		if (inspectedTile.tileType == GameTile.TileType.Collide ||
			inspectedTile.tileType == GameTile.TileType.Locked)
			return false;

		if (inspectedTile.tileType == GameTile.TileType.LevelEnd && !PacmanLevelManager.use.AllItemsPickedUp())
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
			currentDirection = Character.CharacterDirections.Left;
			DetectCurrentTile();
			DestinationReached();
			//MoveTo(PacmanLevelManager.use.GetTile(PacmanLevelManager.use.width-4, 7));									// initiate movement to first tile next to teleporter
			
		}
		else // exiting on the right
		{
			transform.localPosition = PacmanLevelManager.use.GetTile(0, 7).location;								// put player on other side of level
			currentDirection = Character.CharacterDirections.Right;
			DetectCurrentTile();
			DestinationReached();
			//MoveTo(PacmanLevelManager.use.GetTile(3, 7));														// initiate movement to first tile next to teleporter
		}
		
		
		yield return new WaitForSeconds(movementDuration*3);
		
		
		allowControl = true;
	}
	
	public Character.CharacterDirections GetDirection()
	{
		return currentDirection;
	}
}
