using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour {
	
	protected GameTile moveTargetTile;			// the tile we are immediately moving to
	protected Vector3 startPosition = Vector3.zero;
	protected float movementTimer = 0;
	protected float movementDuration = 0;
	public float speed = 200f;		// TO DO: Convert this to tiles/second, instead of the world units it uses now.
	protected bool moving = false;
	protected bool horizontalMovement = false;
	public GameTile currentTile = null;
	protected GameTile startTile;
	protected CharacterDirections currentDirection;
		
	public enum CharacterDirections
	{
		Undefined,
		Up,
		Right,
		Down,
		Left
	}

	public CharacterDirections GetCurrentDirection()
	{
		return currentDirection;
	}

	// does actual moving and calls appropriate methods when destination was reached
	protected void UpdatePosition () 
	{		
		if (moveTargetTile != null)
		{
			if (movementTimer >= movementDuration)
			{
				ResetMovement();
				DestinationReached();
			}
			else
			{
				movementTimer += Time.deltaTime;
				transform.localPosition = Vector3.Lerp(startPosition, moveTargetTile.location, movementTimer/movementDuration);
			}
		}	
	}
	
	protected virtual void MoveTo(GameTile target)
	{
		moving = true;
		
		ResetMovement();
		startPosition = transform.localPosition;
		moveTargetTile = target;
		
		if (target == null)
		{
			Debug.LogWarning("Character move target tile was null!");
			return;
		}
		
		movementDuration = Vector3.Distance(startPosition, new Vector3(moveTargetTile.location.x, moveTargetTile.location.y, 0)) * 1/speed;
		
		UpdatePosition();	// needs to be called again, or character will pause for one frame
	}
	
	public void ChangeSpriteDirection(bool faceRight)
	{
		if (faceRight)
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		else
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
	}

	public void ChangeSpriteDirection(CharacterDirections direction)
	{
		if (direction == CharacterDirections.Down)
		{
			transform.localEulerAngles = Vector3.zero;
		}
		else if (direction == CharacterDirections.Up)
		{
			transform.localEulerAngles = new Vector3(0, 0, 180);
		}
		else if (direction == CharacterDirections.Right)
		{
			transform.localEulerAngles = new Vector3(0, 0, 90);
		}
		else if (direction == CharacterDirections.Left)
		{
			transform.localEulerAngles = new Vector3(0, 0, -90);
		}
	}


	public virtual void ResetMovement()
	{
		movementTimer = 0;
	 	movementDuration = 0;
	}
	
	// override for custom behavior upon having reached a tile (e.g. picking the next tile to move to)
	public virtual void DestinationReached()
	{
	}
	
	public void DetectCurrentTile()
	{		
		currentTile = PacmanLevelManager.use.GetTileByLocation(transform.localPosition.x, transform.localPosition.y);
	}
	
	public int GetCurrentXInt()
	{
		return Mathf.RoundToInt(transform.localPosition.x);
	}
	
	public int GetCurrentYInt()
	{
		return Mathf.RoundToInt(transform.localPosition.y);
	}

}
