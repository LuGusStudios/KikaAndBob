using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanPlayerArrowAndClickSteps : PacmanPlayerCharacter 
{
	public bool allowClickControl = false;

	protected PacmanTile clickedTile = null;
	protected bool movingwithArrows = true;

	private void Update () 
	{
		if (!PacmanGameManager.use.gameRunning || PacmanGameManager.use.Paused)
			return;
		
		DetectCurrentTile();
		
		if (currentTile == null)
			return;
		
		if (allowControl == true)
		{
            
			CheckArrows();
			CheckClick();
		}

		UpdateMovement();

		UpdateWalkSound();

		PacmanCameraFollower.use.FollowCamera();	// the camera does not update automatically anymore - an unpredictable order of Update calls between this and the characters can cause jitter	
													// instead, it's called from the character scripts
	}

	private void CheckClick()
	{
		if (!allowClickControl)
			return;

		if (LugusInput.use.down && PacmanGameManager.use.GameRunning)
		{
			if (moveTargetTile != null)// && moveTargetTile.tileType == PacmanTile.TileType.Teleport)
				return;

			clickedTile = PacmanLevelManager.use.GetTileByClick(LugusInput.use.lastPoint);
			
			if (clickedTile != null)
			{
				movingwithArrows = false;

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
				else if (clickedTile.gridIndices.y > currentTile.gridIndices.y)
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
				
				ChangeSpriteFacing (nextDirection);
			}
		}
	}

	private void CheckArrows()
	{
	    if (moveTargetTile != null && moveTargetTile.tileType == PacmanTile.TileType.Teleport)
            return;
	    
		if (moving)
			return;

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

	private void CheckArrowsContinuous()
	{
        if (moveTargetTile != null && moveTargetTile.tileType == PacmanTile.TileType.Teleport)// || alreadyTeleported)
            return;

	    if (currentTile.tileType == PacmanTile.TileType.Hide)
	    {
	        HideCharacter();
	    }

		if (moving)
			return;
		
		if (PacmanInput.use.GetUpContinuous())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Up;
		}
		else if (PacmanInput.use.GetDownContinuous())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Down;
		}
		else if (PacmanInput.use.GetLeftContinuous())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Left;
		}
		else if (PacmanInput.use.GetRightContinuous())
		{
			movingwithArrows = true;
			clickedTile = null;
			nextDirection = PacmanCharacter.CharacterDirections.Right;
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

		moving = false;

		moveTargetTile = null;
        		
        // if there is no tile that was clicked, don't move any more
        if (clickedTile == null)
        {
            characterAnimator.PlayAnimation(characterAnimator.idle);
            return;
        }
		// if clicked tile was reached, success
		if (currentTile == clickedTile)// || (currentTile.tileType == PacmanTile.TileType.Teleport && !alreadyTeleported)) 
		{
            Debug.Log("Current clicked Tile has been reached");
			ResetMovement();
			currentDirection = CharacterDirections.Undefined;
			clickedTile = null;
            characterAnimator.PlayAnimation(characterAnimator.idle);
			return;
		}
		// if x coords are the same, close enough
		else if  ((currentDirection == CharacterDirections.Left || currentDirection == CharacterDirections.Right) && currentTile.gridIndices.x == clickedTile.gridIndices.x)
		{
			ResetMovement();
			currentDirection = CharacterDirections.Undefined;
			clickedTile = null;
            characterAnimator.PlayAnimation(characterAnimator.idle);
			return;
		}
		// if y coords are the same, close enough
		else if  ((currentDirection == CharacterDirections.Up || currentDirection == CharacterDirections.Down) && currentTile.gridIndices.y == clickedTile.gridIndices.y)
		{
			ResetMovement();
			currentDirection = CharacterDirections.Undefined;
			clickedTile = null;
            characterAnimator.PlayAnimation(characterAnimator.idle);
			return;
		}
		
		// if we can move in the next selected direction, go there
		PacmanTile nextTile = FindOpenTileInDirection(nextDirection);
		if (nextTile != null)
		{
			currentDirection = nextDirection;

            DoCurrentTileLeaveBehavior();

			MoveTo(nextTile);
		}
		else // else continue in the current direction
		{
			nextTile = FindOpenTileInDirection(currentDirection);
			if (nextTile != null)
			{
			    DoCurrentTileLeaveBehavior();
				MoveTo(nextTile);
			}
			else
			{
                characterAnimator.PlayAnimation(characterAnimator.idle);
			}
		}
        
	}

	public void DestinationReachedArrows()
	{	
		DoCurrentTileBehavior();
		
		moving = false;

		// in this player controller, it makes sense to check arrows again upon arriving
		if (nextDirection == CharacterDirections.Undefined)
		{
			CheckArrowsContinuous();
		}

		// if arrows weren't being held down, stop
        if (nextDirection == CharacterDirections.Undefined )
        {
            characterAnimator.PlayAnimation(characterAnimator.idle);
		    moveTargetTile = null;
			return;
		}

		// if we can move in the next selected direction, go there
		PacmanTile nextTile = FindOpenTileInDirection(nextDirection);
		if (nextTile != null)
		{
			currentDirection = nextDirection;
			nextDirection = CharacterDirections.Undefined;
            DoCurrentTileLeaveBehavior();
			MoveTo(nextTile);
			ChangeSpriteFacing(currentDirection);
		}
		else
		{
            characterAnimator.PlayAnimation(characterAnimator.idle);
		}
//		else // else continue in the current direction
//		{
//			ChangeSpriteFacing(currentDirection);
//			nextTile = FindOpenTileInDirection(currentDirection);
//			if (nextTile != null)
//			{
//				MoveTo(nextTile);
//			}
//			else
//			{
//				//PlayAnimationObject("Idle", CharacterDirections.Undefined);
//				if (enemiesFlee)
//					characterAnimator.PlayAnimation(characterAnimator.poweredUpIdle);
//				else
//					characterAnimator.PlayAnimation(characterAnimator.idle);
//			}
//		}
//		

	}

 
   protected override void MoveTo(PacmanTile target)
    {
        base.MoveTo(target);
        PacmanTile teleportTile = null;
        //check targettile linked tile so destination has been reached when entering the teleport
		// linked tile
//        foreach (GameObject go in target.tileItems)
//        {
//            if (go.GetComponent<PacmanTileItemTeleport>() != null)
//            { 
//                teleportTile = go.GetComponent<PacmanTileItemTeleport>().linkedTile.parentTile;
//            }
//        }

		foreach(PacmanTileItem tileItem in target.tileItems)
		{
			if (tileItem.GetComponent<PacmanTileItemTeleport>() != null)
			{ 
				teleportTile = tileItem.GetComponent<PacmanTileItemTeleport>().linkedTile.parentTile;
			}
		}

        if (teleportTile != null)
        {
            clickedTile = teleportTile;
        }
    }

    public void DoCurrentTileLeaveBehavior()
    {
		// altered by Kasper
//        foreach (GameObject go in currentTile.tileItems)
//        {
//            if (go.GetComponent<PacmanTileItem>() != null)
//            {
//                go.GetComponent<PacmanTileItem>().OnLeave(this);
//                //only if it's a hide tile turn on character again
//                if (go.GetComponent<PacmanTileItemHide>() != null)
//                {
//                    ShowCharacter();
//                }
//            }
//        }

		foreach(PacmanTileItem tileItem in currentTile.tileItems)
		{
			tileItem.OnLeave(this);
			//only if it's a hide tile turn on character again
			if (tileItem.GetComponent<PacmanTileItemHide>() != null)
			{
				ShowCharacter();
			}
		}
    }
}
