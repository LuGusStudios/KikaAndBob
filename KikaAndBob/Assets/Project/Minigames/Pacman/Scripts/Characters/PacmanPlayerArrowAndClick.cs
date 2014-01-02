using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanPlayerArrowAndClick : PacmanPlayerCharacter 
{
	protected GameTile clickedTile = null;
	protected bool movingwithArrows = true;

	private void Update () 
	{
		if (!PacmanGameManager.use.gameRunning)
			return;
		
		DetectCurrentTile();
		
		if (currentTile == null)
			return;
		
		if (allowControl == true)
		{
			CheckArrows();
			CheckClick();
		}

		UpdatePosition();
	}

	private void CheckClick()
	{
		if (LugusInput.use.down)
		{
			movingwithArrows = false;

			clickedTile = PacmanLevelManager.use.GetTileByClick(LugusInput.use.lastPoint);
			
			if (clickedTile != null)
			{
				// player can click tiles that are not directly reachable 
				// in this case, the player will move in a generally right direction
				// the direction is selected based on distance: the direction with the largest distance wins out
				
				float largestDistance = 0;
				float currentDistance = 0;
				Vector2 tileDifference = PacmanLevelManager.use.GetTileDistanceBetweenTiles(currentTile, clickedTile);
				
				if (clickedTile.gridIndices.x > currentTile.gridIndices.x)
				{
					currentDistance = tileDifference.x;
					if (currentDistance > largestDistance)
					{
						nextDirection = CharacterDirections.Right;
						largestDistance = currentDistance;
					}
				}
				else if (clickedTile.gridIndices.x < currentTile.gridIndices.x)
				{
					currentDistance = tileDifference.x;
					if (currentDistance > largestDistance)
					{
						nextDirection = CharacterDirections.Left;
						largestDistance = currentDistance;
					}
				}
				
				if (clickedTile.gridIndices.y < currentTile.gridIndices.y)
				{
					currentDistance = tileDifference.y;
					if (currentDistance > largestDistance)
					{
						nextDirection = CharacterDirections.Down;
						largestDistance = currentDistance;
					}
				}
				
				if (clickedTile.gridIndices.y > currentTile.gridIndices.y)
				{
					currentDistance = tileDifference.y;
					if (currentDistance > largestDistance)
					{
						nextDirection = CharacterDirections.Up;
						largestDistance = currentDistance;
					}
				}
				
				// if we're mot moving, start moving again
				if (!moving)
					DestinationReachedClick();
				
				ChangeSpriteDirection (nextDirection);
			}
		}
	}

	private void CheckArrows()
	{
		if (PacmanInput.use.GetUp())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Up;
			if (!moving)
				DestinationReachedArrows();
		}
		else if (PacmanInput.use.GetDown())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Down;
			if (!moving)
				DestinationReachedArrows();
		}
		else if (PacmanInput.use.GetLeft())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Left;
			if (!moving)
				DestinationReachedArrows();
		}
		else if (PacmanInput.use.GetRight())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Right;
			if (!moving)
				DestinationReachedArrows();
		}
	}


	public override void DestinationReached()
	{
		if (movingwithArrows)
			DestinationReachedArrows();
		else
			DestinationReachedClick();
	}

	public void DestinationReachedClick ()
	{
		if (cutScene)
			return;
		
		DoCurrentTileBehavior();
		
		// if there is no tile that was clicked, don't move any more
		if (clickedTile == null)
		{
			PlayAnimation("Idle", CharacterDirections.Undefined);
			return;
		}
		// if clicked tile was reached, success
		else if (currentTile == clickedTile)
		{
			ResetMovement();
			currentDirection = CharacterDirections.Undefined;
			clickedTile = null;
			return;
		}
		// if x coords are the same, close enough
		else if  ((currentDirection == CharacterDirections.Left || currentDirection == CharacterDirections.Right) && currentTile.gridIndices.x == clickedTile.gridIndices.x)
		{
			ResetMovement();
			currentDirection = CharacterDirections.Undefined;
			clickedTile = null;
			return;
		}
		// if y coords are the same, close enough
		else if  ((currentDirection == CharacterDirections.Up || currentDirection == CharacterDirections.Down) && currentTile.gridIndices.y == clickedTile.gridIndices.y)
		{
			ResetMovement();
			currentDirection = CharacterDirections.Undefined;
			clickedTile = null;
			return;
		}
		
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

	public void DestinationReachedArrows()
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
		
		ChangeSpriteDirection(currentDirection);
	}
}
