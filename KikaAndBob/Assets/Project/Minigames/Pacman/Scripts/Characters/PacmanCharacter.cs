using UnityEngine;
using System.Collections;
using SmoothMoves;
using System.Collections.Generic;

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
		// this only needs to be done once, but it's handy to be able to call from various places to ensure these are always assigned
		if (boneAnimations == null || boneAnimations.Length <= 0)
		{
			// since switching between animations relies on setting child objects non-active, we can't rely on getcomponentsinchildren or something similar
			// instead, make sure they're all active and and turn them off again if needed
			List<BoneAnimation> foundAnimations = new List<BoneAnimation>();
			foreach(Transform t in transform)
			{
				bool objectActive = t.gameObject.activeSelf;
				if (!objectActive)
					t.gameObject.SetActive(true);

				BoneAnimation found = t.gameObject.GetComponent<BoneAnimation>();

				if (found != null)
					foundAnimations.Add(found);

				if (objectActive != t.gameObject.activeSelf)
					t.gameObject.SetActive(objectActive);
			}

			boneAnimations = foundAnimations.ToArray();
		}
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
	
	public virtual void ChangeSpriteDirection(bool faceRight)
	{
		if (faceRight)
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		else
			transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
	}

	public virtual void ChangeSpriteDirection(CharacterDirections direction)
	{
		CharacterDirections adjustedDirection = direction;

		// Right facing = left flipped on x axis
		if (direction == CharacterDirections.Undefined || direction == CharacterDirections.Right)
		{
			adjustedDirection = CharacterDirections.Left;
		}

		PlayAnimation("" + adjustedDirection.ToString(), direction);
	}

	public void PlayAnimation(string clipName, CharacterDirections direction)
	{
		FindAnimations();

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