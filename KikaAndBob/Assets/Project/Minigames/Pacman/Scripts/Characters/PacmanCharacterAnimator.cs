using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class PacmanCharacterAnimator : MonoBehaviour 
{
	protected List<BoneAnimation> boneAnimations = new List<BoneAnimation>();
	protected List<Animator> spriteAnimators = new List<Animator>();
	public BoneAnimation currentBoneAnimation = null;
	public Transform currentAnimationTransform = null;
	public string currentAnimationClip = "";
	public string currentAnimationPath = "";

	public string idle;

	public string up;
	public string down;
	public string side;

	public string poweredUpIdle;
		
	public string poweredUp;
	public string poweredDown;
	public string poweredSide;

	public string hitAnimation;

	public string runScared;

	
	public void SetupLocal()
	{
		if(boneAnimations == null || boneAnimations.Count == 0 )
		{
			boneAnimations.AddRange(transform.GetComponentsInChildren<BoneAnimation>(true));
		}

		// no need to check if count of boneAnimations <= 0; it's perfectly valid for it to be empty


		if(spriteAnimators == null || spriteAnimators.Count == 0 )
		{
			spriteAnimators.AddRange(transform.GetComponentsInChildren<Animator>(true));
		}
		
		// no need to check if count of spriteAnimators <= 0; it's perfectly valid for it to be empty
	}
	
	public void SetupGlobal()
	{
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}

	public void PlayAnimation(string animationPath)
	{
		if (string.IsNullOrEmpty(animationPath))
		{
			Debug.LogError("Animation path was empty!");
			return;
		}

		// don't update if we're not changing anything
		if (currentAnimationPath == animationPath)
			return;

		string[] parts = animationPath.Split('/');
		string containerName = "";
		string clipName = "";

		if ( parts.Length == 1 )
		{
			containerName = parts[0];
			clipName = parts[0];
		}
		else if ( parts.Length == 2 )
		{
			containerName = parts[0];
			clipName = parts[1];
		}

		// this will contain either a bone animation or a sprite animation
		currentBoneAnimation = null;


		currentAnimationTransform = null;
		foreach( BoneAnimation boneAnimation in boneAnimations )
		{
			if( boneAnimation.name == containerName )
			{
				currentAnimationTransform = boneAnimation.transform;
				currentBoneAnimation = boneAnimation;
				currentBoneAnimation.gameObject.SetActive(true);
			}
			else
			{
				boneAnimation.gameObject.SetActive(false);
			}
		}


		Animator currentSpriteAnimation = null;
		foreach( Animator spriteAnimation in spriteAnimators )
		{
			if( spriteAnimation.name == containerName )
			{
				if (currentAnimationTransform != null)
				{
					Debug.LogError("PacmanCharacterAnimator: GameObject: " + this.name + " Contains both a sprite animator and a bone animation with the same name: " + containerName);
				}

				currentAnimationTransform = spriteAnimation.transform;
				currentSpriteAnimation = spriteAnimation;
				spriteAnimation.gameObject.SetActive(true);
			}
			else
			{
				spriteAnimation.gameObject.SetActive(false);
			}
		}
		
		if( currentAnimationTransform == null )
		{
			Debug.LogError(name + " : No bone or sprite animation found for name: " + containerName);
		//	currentAnimationContainer = animationContainers[0];
			return;
		}

		// if a sprite animation was found, no more work has to be done
		if (currentSpriteAnimation != null)
			return;


		currentAnimationPath = animationPath;
	
		if ( parts.Length == 2 )
		{
			currentAnimationClip = clipName;
			currentBoneAnimation.Stop();
			currentBoneAnimation.Play(clipName);
		}
	}
}
