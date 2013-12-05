using UnityEngine;
using System.Collections;
using SmoothMoves;

public abstract class PacmanCharacter : MonoBehaviour {
	
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
	protected BoneAnimation currentAnimation = null;
	protected BoneAnimation[] boneAnimations = null;
		
	public enum CharacterDirections
	{
		Undefined = 0,
		Up = 1,			// 0001
		Right = 2,		// 0010
		Down = 4,		// 0100
		Left = 8		// 1000
	}

	public CharacterDirections GetCurrentDirection()
	{
		return currentDirection;
	}

	void Awake()
	{

	}

	void FindAnimations()
	{
		if (boneAnimations == null)
			boneAnimations = (BoneAnimation[])FindObjectsOfType(typeof(BoneAnimation));
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
		FindAnimations();

		CharacterDirections adjustedDirection = direction;

		if (direction == CharacterDirections.Undefined || direction == CharacterDirections.Right)
		{
			adjustedDirection = CharacterDirections.Left;
		}

		string clipName = "" + adjustedDirection.ToString(); 

		currentAnimation = null;
		foreach( BoneAnimation animation in boneAnimations )
		{
			animation.gameObject.SetActive(false);
			if( animation.name == clipName )
			{
				currentAnimation = animation;
				animation.gameObject.SetActive(true);
			}
		}
		
		if( currentAnimation == null )
		{
			Debug.LogError(name + " : No animation found for name " + clipName);
			currentAnimation = boneAnimations[0];
		}
		
		currentAnimation.Stop();
	//	Debug.Log ("PLAYING ANIMATION " + currentAnimation.animation.clip.name + " ON " + currentAnimation.name );

		currentAnimation.Play( currentAnimation.animation.clip.name, PlayMode.StopAll );
		
		if( direction == CharacterDirections.Right )
		{
			// if going left, the scale.x needs to be negative
			if( currentAnimation.transform.localScale.x > 0 )
			{
				currentAnimation.transform.localScale = currentAnimation.transform.localScale.x( currentAnimation.transform.localScale.x * -1.0f );
			}
		}
		else // moving left
		{
			// if going right, the scale.x needs to be positive 
			if( currentAnimation.transform.localScale.x < 0 )
			{
				currentAnimation.transform.localScale = currentAnimation.transform.localScale.x( Mathf.Abs(currentAnimation.transform.localScale.x) ); 
			}
		}

	}


	public virtual void ResetMovement()
	{
		ChangeSpriteDirection (CharacterDirections.Right);
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
