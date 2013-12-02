using UnityEngine;
using System.Collections;

public class PlayerCharacterClick : PlayerCharacter {

	protected GameTile clickedTile = null;

	private void Update () 
	{
		if (!PacmanGameManager.use.gameRunning)
			return;
		
		DetectCurrentTile();
		
		if (currentTile == null)
			return;
		
		if (allowControl == true)
		{
			if (LugusInput.use.down)
			{
				clickedTile = PacmanLevelManager.use.GetTileByClick(LugusInput.use.lastPoint);

				if (clickedTile != null)
				{
					if (clickedTile.gridIndices.x > currentTile.gridIndices.x)
						nextDirection =  CharacterDirections.Right;
					else if (clickedTile.gridIndices.x < currentTile.gridIndices.x)
						nextDirection =  CharacterDirections.Left;
					else if (clickedTile.gridIndices.y < currentTile.gridIndices.y)
						nextDirection =  CharacterDirections.Down;
					else if (clickedTile.gridIndices.y > currentTile.gridIndices.y)
						nextDirection =  CharacterDirections.Up;

					// if we're mot moving, start moving again
					if (!moving)
						DestinationReached();
				}
			}
		}
		
		UpdatePosition();
		
		ChangeSpriteDirection (nextDirection);
	}

	public override void DestinationReached ()
	{
		if (cutScene)
			return;
		
		DoCurrentTileBehavior();

		// if there is no tile that was clicked, don't move any more
		if (clickedTile == null)
		{
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
}
