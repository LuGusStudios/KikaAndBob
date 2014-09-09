using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
using System;

namespace KikaAndBob
{
    public enum CMMovementQuadrant
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

public class CatchingMiceCharacterAnimation : MonoBehaviour 
{
    public BoneAnimation[] animationContainers;
    public BoneAnimation currentAnimationContainer = null;

    public ICatchingMiceCharacter character = null;

    public string currentAnimationClip = "";
    public string currentAnimationPath = "";

    protected string _lastAnimationClip = "";

    //String example DOWN/Cat01Front_Jump --> 
    //"_currentMovementQuadrant  + / + characterNameAnimation + frontAnimationClip + jumpAnimationClip
    public string characterNameAnimation = "Cat01";
    public string jumpAnimationClip = "_Jump";
    public string walkAnimationClip = "_Walk";
    public string eatingAnimationClip = "_Attack";
    public string idleAnimationClip = "_Idle";

    public string _sideAnimationClip = "Side";
    public string _frontAnimationClip = "Front";
    public string _backAnimationClip = "Back";

    protected KikaAndBob.CMMovementQuadrant _currentMovementQuadrant = KikaAndBob.CMMovementQuadrant.NONE;
    protected bool _jumped = false;

    // convenience function
    protected KikaAndBob.CMMovementQuadrant DirectionToQuadrant(Vector3 movementDirection)
    {

        // 1. Figure out the quadrant for the movementDirection
        int quadrant = (int)KikaAndBob.CMMovementQuadrant.NONE;

        // movementDirection.x indicates left or right (and so also the sign of the localScale.x)
        // movementDirection.y indicates up or down

        // factor in epsilons (ex. going up is not exactly 0.0f, but between [-1.0f, and 1.0f])

        if (movementDirection.x < -0.2f)
        {
            quadrant = quadrant | (int)KikaAndBob.CMMovementQuadrant.LEFT;
        }
        else if (movementDirection.x > 0.2f)
        {
            quadrant = quadrant | (int)KikaAndBob.CMMovementQuadrant.RIGHT;
        }

        if (movementDirection.y > 0.3f)
        {
            quadrant = quadrant | (int)KikaAndBob.CMMovementQuadrant.UP;
        }
        else if (movementDirection.y < -0.3f)
        {
            quadrant = quadrant | (int)KikaAndBob.CMMovementQuadrant.DOWN; 
        }

        KikaAndBob.CMMovementQuadrant quadrantReal = (KikaAndBob.CMMovementQuadrant)Enum.ToObject(typeof(KikaAndBob.CMMovementQuadrant), quadrant);

        if (quadrantReal == KikaAndBob.CMMovementQuadrant.NONE)
        {
            //CatchingMiceLogVisualizer.use.LogError(name + ": quadrant was NONE " + quadrant + "/" + movementDirection + " : defaulting to RIGHT");
            quadrantReal = KikaAndBob.CMMovementQuadrant.RIGHT;
        }

        return quadrantReal;
    }

	// Use this for initialization
	void Start () 
    {
		InitializeAnimations();
	}

	// EXPERIMENTAL: SmoothMoves seems to sometimes build/cache something when an animation is played the first time
	// This means the correct animation/texture can sometimes be delayed quite a bit. Hopefully, calling all the right animations once at Start helps with this.
	protected void InitializeAnimations()
	{
		foreach(BoneAnimation ba in GetComponentsInChildren<BoneAnimation>(true)) 
		{
			string currentAnimation = ba.animation.name; 


			foreach(SmoothMoves.AnimationClipSM_Lite clip in ba.mAnimationClips)
			{
				if (clip.animationName.Contains(characterNameAnimation))
				{
					ba.Play(clip.animationName);
				}
				//else
					//print (currentAnimation + " " + clip.animationName);
			}

		}
	}

	// Update is called once per frame
	void Update () 
    {
        AnimationLoop();
	}

    protected virtual void AnimationLoop()
    {
        if (!character.moving && !character.attacking)
        {
            IdleLoop();
            return;
        }

        Vector3 movementDirection = character.movementDirection;
        
        KikaAndBob.CMMovementQuadrant newQuadrant = DirectionToQuadrant(movementDirection);
        
        if ((character.moving) && (newQuadrant != _currentMovementQuadrant) && (!character.jumping)) 
        {
            LoadQuadrantAnimation(newQuadrant,walkAnimationClip);
        }
    }

    protected virtual void IdleLoop()
    {
		bool mirror = false;

		KikaAndBob.CMMovementQuadrant heading = DirectionToQuadrant(character.movementDirection);

		if (heading == KikaAndBob.CMMovementQuadrant.RIGHT)
		{
			mirror = true;
			heading = KikaAndBob.CMMovementQuadrant.LEFT;
		}

		string facing = CheckFacing(heading);
		string targetAnimation = characterNameAnimation + facing + idleAnimationClip;


		if (currentAnimationClip != targetAnimation)
        {
			PlayAnimation(heading.ToString() + "/" + targetAnimation, !mirror);
			_currentMovementQuadrant = KikaAndBob.CMMovementQuadrant.NONE; 

//			if (mirror) // mirror for facing right
//			{
//				transform.localScale = transform.localScale.x( - Mathf.Abs(transform.localScale.x));
//			}
//			else  // dont forget to always reset the scale in case we were facing right earlier
//			{
//				transform.localScale = transform.localScale.x(Mathf.Abs(transform.localScale.x));
//			}
		}
    }
    
	protected void LoadQuadrantAnimation(KikaAndBob.CMMovementQuadrant quadrantReal, string animationType)
    {
        // 1. Map the quadrant to the correct AnimationClip name
        // 2. Find the AnimationClip object and Play() it

        _currentMovementQuadrant = quadrantReal;

        // 1. Map the quadrant to the correct AnimationClip name

        // we only have animations for the left side of the quadrants (up, left_up, left, left_down and down)
        // the other side is achieved by mirroring the animations by changing localScale.x
        KikaAndBob.CMMovementQuadrant quadrantAnimation = quadrantReal;
        bool movingRight = false;
        if ((quadrantReal & KikaAndBob.CMMovementQuadrant.RIGHT) == KikaAndBob.CMMovementQuadrant.RIGHT)
        {
            movingRight = true;

            // use bitwise NOT operator to remove RIGHT, then add LEFT with OR operator 
            // http://stackoverflow.com/questions/750240/how-do-i-set-or-clear-the-first-3-bits-using-bitwise-operations
            quadrantAnimation = quadrantAnimation & (~KikaAndBob.CMMovementQuadrant.RIGHT);
            quadrantAnimation = quadrantAnimation | KikaAndBob.CMMovementQuadrant.LEFT;
        }

        // we have no LEFT_UP and LEFT_DOWN here
        // so just use the LEFT for the diagonal movement by removing both UP and DOWN
        if ((quadrantAnimation & KikaAndBob.CMMovementQuadrant.LEFT) == KikaAndBob.CMMovementQuadrant.LEFT)
        {
            quadrantAnimation = quadrantAnimation & (~KikaAndBob.CMMovementQuadrant.UP);
            quadrantAnimation = quadrantAnimation & (~KikaAndBob.CMMovementQuadrant.DOWN);
        }

        string animationClipName = "" + quadrantAnimation.ToString() + "/" + characterNameAnimation;

        animationClipName += CheckFacing(quadrantAnimation) + animationType;
        

        PlayAnimation(animationClipName, !movingRight);
    }
    
	public virtual void PlayAnimation(string animationPath, bool moveRight = true, float fadeTime = 0.3f)
    {

        string[] parts = animationPath.Split('/');
        if (parts.Length != 2)
        {
            CatchingMiceLogVisualizer.use.LogError(name + " : AnimationPath should be a string with a single / as separator! " + animationPath);
            return;
        }

        string containerName = parts[0];
        string clipName = parts[1];


        currentAnimationContainer = null;
        foreach (BoneAnimation container in animationContainers)
        {
            if (container.name == containerName)
            {
                currentAnimationContainer = container;
                currentAnimationContainer.gameObject.SetActive(true);
                currentAnimationContainer.animation.enabled = true;
            }
            else
            {
                container.gameObject.SetActive(false);
                //currentAnimationContainer.animation.enabled = false;
            }
        }

        if (currentAnimationContainer == null)
        {
            CatchingMiceLogVisualizer.use.LogError(name + " : No animation found for name " + animationPath);
            currentAnimationContainer = animationContainers[0];
        }
		else
		{
			string direction = "";


				

		}

        currentAnimationPath = animationPath;
        currentAnimationClip = clipName;

        
        //CatchingMiceLogVisualizer.use.Log("PLAYING ANIMATION " + clipName + " ON " + currentAnimationContainer.name);
        if (currentAnimationClip.Contains("Jump"))
        {
            currentAnimationContainer.Stop();
            currentAnimationContainer.Play(clipName);

            //CatchingMiceLogVisualizer.use.LogError("PLAYING CLIP " + clipName);
        }
        else
        {
			currentAnimationContainer.CrossFade(clipName,fadeTime); 
        }


        if (moveRight)
        {
            // if going right, the scale.x needs to be positive 
            if (currentAnimationContainer.transform.localScale.x < 0)
            {
                currentAnimationContainer.transform.localScale = currentAnimationContainer.transform.localScale.x(Mathf.Abs(currentAnimationContainer.transform.localScale.x));
            }
        }
        else // moving left
        {
            // if going left, the scale.x needs to be negative
            if (currentAnimationContainer.transform.localScale.x > 0)
            {
                currentAnimationContainer.transform.localScale = currentAnimationContainer.transform.localScale.x(currentAnimationContainer.transform.localScale.x * -1.0f);
            }
        }
    }

    

    protected String CheckFacing(KikaAndBob.CMMovementQuadrant quadrantAnimation)
    {
        string facing = "";
        switch (quadrantAnimation)
        {
            case KikaAndBob.CMMovementQuadrant.UP:
                facing = _backAnimationClip;
                break;
            case KikaAndBob.CMMovementQuadrant.RIGHT:
//                //because of the naming of cat no definition of Side needs to be added
//                if (character is CatchingMiceCharacterPlayer && !character.jumping)
//                {
//                    break;
//                }
                facing = _sideAnimationClip;
                break;
            case KikaAndBob.CMMovementQuadrant.DOWN:
                facing = _frontAnimationClip;
                break;
            case KikaAndBob.CMMovementQuadrant.LEFT:
//                if (character is CatchingMiceCharacterPlayer && !character.jumping)
//                {
//                    break;
//                }
                facing =_sideAnimationClip;
                break;
            default:
                CatchingMiceLogVisualizer.use.LogError("No correct movement quadrant was chosen.");
                break;
        }
        return facing;
    }
    
	public virtual void SetupLocal()
    {
        if (animationContainers.Length == 0)
        {
            animationContainers = transform.GetComponentsInChildren<BoneAnimation>();
        }

        if (animationContainers.Length == 0)
        {
            CatchingMiceLogVisualizer.use.LogError(name + " : no BoneAnimations found for this animator!");
        }

        SetCharacter();

        //PlayAnimation("DOWN/" + characterNameAnimation + _frontAnimationClip + idleAnimationClip);
    }
    
	public void OnJump()
    {
        KikaAndBob.CMMovementQuadrant quadrant = DirectionToQuadrant(character.movementDirection);
        LoadQuadrantAnimation(quadrant,jumpAnimationClip);
        _currentMovementQuadrant = KikaAndBob.CMMovementQuadrant.NONE; 
    }
    
	protected virtual void SetCharacter()
    {
        if (character == null)
        {
            character = transform.GetComponent<ICatchingMiceCharacter>();
        }

        if (character == null)
        {
            CatchingMiceLogVisualizer.use.LogError(name + " : no character found!");
        }
        else
        {
            character.onJump += OnJump;
            character.onHit += OnHit;
        }
    }
    
	public virtual void OnHit()
    {
		bool mirror = false;
		KikaAndBob.CMMovementQuadrant heading = DirectionToQuadrant(character.movementDirection);

		// there is no idle animation for the cat facing away from the camera, so we just use the idle animation
		// the side attack animations are named differently, but even though they look cool they don't loop well and don't align with the idle animation, so we decided to not use them
		// final result UP = back idle, everything else = front attack
		if (heading != KikaAndBob.CMMovementQuadrant.UP)
		{
			heading = KikaAndBob.CMMovementQuadrant.DOWN;
		}

		string facing = CheckFacing(heading);
		string targetAnimation = characterNameAnimation + facing;

		if (heading == KikaAndBob.CMMovementQuadrant.UP)
			targetAnimation += idleAnimationClip;
		else
			targetAnimation += eatingAnimationClip;

		if (currentAnimationClip != targetAnimation)
		{
			PlayAnimation(heading.ToString() + "/" + targetAnimation, !mirror);
		}
    }

    protected void Awake()
    {
        SetupLocal();
    }
    
	protected void OnDisable()
    {
        character.onJump -= OnJump;
        character.onHit -= OnHit;
    }
}
