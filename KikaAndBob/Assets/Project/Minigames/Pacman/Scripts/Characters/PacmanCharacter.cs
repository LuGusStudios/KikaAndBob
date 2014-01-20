using UnityEngine;
using System.Collections;
using SmoothMoves;
using System.Collections.Generic;

[RequireComponent(typeof(PacmanCharacterAnimator))]
public abstract class PacmanCharacter : MonoBehaviour {

	public float speed = 200f;		// TO DO: Convert this to tiles/second, instead of the world units it uses now.
	public string walkSoundKey = "";
	public float spawnDelay = 0;

	protected PacmanTile moveTargetTile;			// the tile we are immediately moving to
	protected Vector3 moveStartPosition = Vector3.zero;
	protected Vector2 spawnLocation = Vector2.zero;
	protected float movementTimer = 0;
	protected float movementDuration = 0;
	protected bool moving = false;
	protected bool horizontalMovement = false;
	protected bool alreadyTeleported = false;

	public PacmanTile currentTile = null;
	protected PacmanTile startTile;
	protected CharacterDirections currentDirection;
	protected CharacterDirections startDirection;
	protected BoneAnimation currentAnimation = null;

	protected PacmanCharacterAnimator characterAnimator = null;
		
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

	protected void Awake()
	{
		SetUpLocal();
	}

	protected void Start()
	{
		SetUpGlobal();
	}

	public virtual void SetUpLocal()
	{
		if (characterAnimator == null)
		{
			characterAnimator = GetComponent<PacmanCharacterAnimator>();
		}
		if (characterAnimator == null)
		{
			Debug.LogError("Missing character animator on: " + gameObject.name);
		}
	}

	public virtual void SetUpGlobal()
	{
	}
	
	// does actual moving and calls appropriate methods when destination was reached
	protected void UpdateMovement () 
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
				transform.localPosition = Vector3.Lerp(moveStartPosition, moveTargetTile.location, movementTimer/movementDuration);
			}
		}
	}
	
	protected virtual void MoveTo(PacmanTile target)
	{
		moving = true;
		
		ResetMovement();
		moveStartPosition = transform.localPosition;
		moveTargetTile = target;
		
		if (target == null)
		{
			Debug.LogWarning("Character move target tile was null!");
			return;
		}
		
		movementDuration = Vector3.Distance(moveStartPosition, new Vector3(moveTargetTile.location.x, moveTargetTile.location.y, 0)) * 1/speed;
		
		UpdateMovement();	// needs to be called again, or character will pause for one frame
	}

	// intermediary for changing sprite and its animation - i.e. transforms CharacterDirections.Right into Left, which is just the same one flipped, or limits choices to an object that actually exists
	// override for different rewrite rules
	public virtual void ChangeSpriteFacing(CharacterDirections direction)
	{
		CharacterDirections adjustedDirection = direction;

		// Right facing = left flipped on x axis
		if (direction == CharacterDirections.Undefined || direction == CharacterDirections.Right)
		{
			adjustedDirection = CharacterDirections.Left;
		}

		characterAnimator.PlayAnimation("" + adjustedDirection.ToString());

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
		//PlayAnimationObject("" + adjustedDirection.ToString(), direction);
	}

	public void SetStartDirection(CharacterDirections newDirection)
	{
		startDirection = newDirection;
	}

	protected virtual IEnumerator TeleportRoutine()
	{				
		alreadyTeleported = true;

		PacmanTile targetTile = null;	
		
		foreach(PacmanTile tile in PacmanLevelManager.use.teleportTiles)
		{
			if (currentTile != tile)
			{
				targetTile = tile;
				break;
			}
		}
		
		if (targetTile == null)
		{
			Debug.LogError("No other teleport tile found!");
			yield break;
		}
		
		transform.localPosition = targetTile.location.v3();
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

	public void EnableCharacter()
	{
		foreach (SpriteRenderer spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
		{
			spriteRenderer.enabled = true;
		}
		
		foreach (SkinnedMeshRenderer skinnedmeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			skinnedmeshRenderer.enabled = true;
		}
		
		this.enabled = true;
	}

	public void DisableCharacter()
	{
		foreach (SpriteRenderer spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
		{
			spriteRenderer.enabled = false;
		}
		
		foreach (SkinnedMeshRenderer skinnedmeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			skinnedmeshRenderer.enabled = false;
		}
		
		this.enabled = false;
	}

	public void SetSpawnLocation(Vector2 location)
	{
		spawnLocation = location;
	}

	public void PlaceAtSpawnLocation()
	{
		transform.localPosition = PacmanLevelManager.use.GetTile (spawnLocation).location;
	}
	
	public virtual void SetDefaultTargetTiles(Vector2[] defaultTargetTiles)
	{
	}

	public virtual void Reset()
	{
	}
}
