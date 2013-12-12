using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
using System;

namespace KikaAndBob
{
	public enum MovementQuadrant
	{
		NONE = 0,
		
		UP = 1, // 0001
		RIGHT = 2, // 0010
		DOWN = 4, // 0100
		LEFT = 8, // 1000
		
		UP_RIGHT = 3, // north east : 0011
		DOWN_RIGHT = 6, // south east : 0110
		UP_LEFT = 9, // north west : 1001
		DOWN_LEFT = 12 // south west : 1100
	}
}

public class ConsumableMoverAnimation : MonoBehaviour 
{
	public BoneAnimation[] animations;
	public BoneAnimation currentAnimation = null;

	public ConsumableMover mover = null;

	protected KikaAndBob.MovementQuadrant currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE;

	// convenience function
	protected KikaAndBob.MovementQuadrant DirectionToQuadrant(Vector3 movementDirection)
	{
		
		// 1. Figure out the quadrant for the movementDirection
		int quadrant = (int) KikaAndBob.MovementQuadrant.NONE;
		
		// movementDirection.x indicates left or right (and so also the sign of the localScale.x)
		// movementDirection.y indicates up or down
		
		// factor in epsilons (ex. going up is not exactly 0.0f, but between [-1.0f, and 1.0f])
		
		if( movementDirection.x < -0.1f ) 
		{ 
			quadrant = quadrant | (int) KikaAndBob.MovementQuadrant.LEFT;
		}
		else if( movementDirection.x > 0.1f )
		{
			quadrant = quadrant | (int) KikaAndBob.MovementQuadrant.RIGHT;
		}
		
		if( movementDirection.y > 0.3f )
		{
			quadrant = quadrant | (int) KikaAndBob.MovementQuadrant.UP;
		}
		else if( movementDirection.y < -0.3f )  
		{
			quadrant = quadrant | (int) KikaAndBob.MovementQuadrant.DOWN;
		}
		
		KikaAndBob.MovementQuadrant quadrantReal = (KikaAndBob.MovementQuadrant) Enum.ToObject(typeof(KikaAndBob.MovementQuadrant) , quadrant);
		
		if( quadrantReal == KikaAndBob.MovementQuadrant.NONE )
		{
			Debug.LogError(name + ": quadrant was NONE " + quadrant + "/" + movementDirection + " : defaulting to RIGHT");
			quadrantReal = KikaAndBob.MovementQuadrant.RIGHT;
		}
		
		return quadrantReal;
	}

	
	// Update is called once per frame
	void Update () 
	{
		AnimationLoop(); 
	}
	
	protected void AnimationLoop()
	{ 
		if( !mover.moving )
		{
			// TODO: possibly add an Idle animation routine or something here
			if( currentAnimation.name != "IDLE")
			{
				PlayAnimation("IDLE");
			}
			return;
		}
		
		Vector3 movementDirection = mover.movementDirection;
		
		KikaAndBob.MovementQuadrant newQuadrant = DirectionToQuadrant( movementDirection );


		if( (mover.moving) && (newQuadrant != currentMovementQuadrant) )
		{
			LoadQuadrantAnimation( newQuadrant );
		}
	}
	
	protected void OnMovementStopped()
	{
		currentMovementQuadrant = KikaAndBob.MovementQuadrant.NONE;
	
		PlayAnimation("IDLE");
	}

	protected void LoadQuadrantAnimation(KikaAndBob.MovementQuadrant quadrantReal)
	{
		// 1. Map the quadrant to the correct AnimationClip name
		// 2. Find the AnimationClip object and Play() it
		
		currentMovementQuadrant = quadrantReal;

		// 1. Map the quadrant to the correct AnimationClip name
		
		// we only have animations for the left side of the quadrants (up, left_up, left, left_down and down)
		// the other side is achieved by mirroring the animations by changing localScale.x
		KikaAndBob.MovementQuadrant quadrantAnimation = quadrantReal;
		bool movingLeft = false;
		if( (quadrantReal & KikaAndBob.MovementQuadrant.RIGHT) == KikaAndBob.MovementQuadrant.RIGHT )
		{
			movingLeft = true;
			
			// use bitwise NOT operator to remove RIGHT, then add LEFT with OR operator 
			// http://stackoverflow.com/questions/750240/how-do-i-set-or-clear-the-first-3-bits-using-bitwise-operations
			quadrantAnimation = quadrantAnimation & (~KikaAndBob.MovementQuadrant.RIGHT);
			quadrantAnimation = quadrantAnimation | KikaAndBob.MovementQuadrant.LEFT;
		}

		// we have no LEFT_UP and LEFT_DOWN here
		// so just use the LEFT for the diagonal movement by removing both UP and DOWN
		if( (quadrantAnimation & KikaAndBob.MovementQuadrant.LEFT) == KikaAndBob.MovementQuadrant.LEFT )
		{
			quadrantAnimation = quadrantAnimation & (~KikaAndBob.MovementQuadrant.UP);
			quadrantAnimation = quadrantAnimation & (~KikaAndBob.MovementQuadrant.DOWN);
		}

		string animationClipName = "" + quadrantAnimation.ToString(); 



		PlayAnimation( animationClipName, !movingLeft );
	}

	public void PlayAnimation(string clipName, bool moveRight = true)
	{
		currentAnimation = null;
		foreach( BoneAnimation animation in animations )
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
			currentAnimation = animations[0];
		}
		
		currentAnimation.Stop();
		//Debug.Log ("PLAYING ANIMATION " + currentAnimation.animation.clip.name + " ON " + currentAnimation.name );
		currentAnimation.Play( currentAnimation.animation.clip.name, PlayMode.StopAll );
		
		if( moveRight )
		{
			// if going right, the scale.x needs to be positive 
			if( currentAnimation.transform.localScale.x < 0 )
			{
				currentAnimation.transform.localScale = currentAnimation.transform.localScale.x( Mathf.Abs(currentAnimation.transform.localScale.x) ); 
			}
		}
		else // moving left
		{
			// if going left, the scale.x needs to be negative
			if( currentAnimation.transform.localScale.x > 0 )
			{
				currentAnimation.transform.localScale = currentAnimation.transform.localScale.x( currentAnimation.transform.localScale.x * -1.0f );
			}
		}
	}


	public void SetupLocal()
	{
		if( animations.Length == 0 )
		{
			animations = transform.GetComponentsInChildren<BoneAnimation>();
		}

		if( animations.Length == 0 )
		{
			Debug.LogError(name + " : no BoneAnimations found for this animator!");
		}
	}
	
	public void SetupGlobal()
	{
		if( mover == null )
		{
			mover = ConsumableMover.use;
		}
		
		if( mover == null )
		{
			Debug.LogError(name + " : no Mover found!");
		}

		mover.onStopped += OnMovementStopped;

		
		PlayAnimation("IDLE");
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
}
