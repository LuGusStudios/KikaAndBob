using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class PlayCatAnimation : MonoBehaviour 
{
	public string catName = "Cat01";
	public string jumpAnimationClip = "_Jump";
	public string walkAnimationClip = "_Walk";
	public string eatingAnimationClip = "_Attack";
	public string idleAnimationClip = "_Idle";
	
	public string _sideAnimationClip = "Side";
	public string _frontAnimationClip = "Front";
	public string _backAnimationClip = "Back";

	protected BoneAnimation[] animationContainers;
	protected string currentAnimationPath = "";

	public void SetupLocal()
	{
		if (animationContainers == null || animationContainers.Length == 0)
		{
			animationContainers = transform.GetComponentsInChildren<BoneAnimation>(true);
		}
		
		if (animationContainers.Length == 0)
		{
			Debug.LogError(name + " : no BoneAnimations found for this animator!");
		}
	}
	
	public void SetupGlobal()
	{
		catName = "Cat0" + LugusConfig.use.User.GetInt("CatIndex", 1).ToString();
		InitializeAnimations();
		PlayAnimation("LEFT/@Side_Idle");
	}

	// EXPERIMENTAL: SmoothMoves seems to sometimes build/cache something when an animation is played the first time...
	// This means the correct animation/texture can sometimes be delayed quite a bit. Hopefully, calling all the right animations once at Start helps with this.
	protected void InitializeAnimations()
	{
		foreach(BoneAnimation ba in GetComponentsInChildren<BoneAnimation>(true)) 
		{
			foreach(SmoothMoves.AnimationClipSM_Lite clip in ba.mAnimationClips)
			{
				if (clip.animationName.Contains(catName))
				{
					ba.Play(clip.animationName);
				}
			}
			
		}
	}

	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start() 
	{
		SetupGlobal();
	}
	
	protected void Update() 
	{
	
	}

	public void PlayAnimation(string animationPath, float fadeTime = 0)
	{
		string[] parts = animationPath.Split('/');
		if (parts.Length != 2)
		{
			Debug.LogError(name + " : AnimationPath should be a string with a single / as separator! " + animationPath);
			return;
		}

		string containerName = parts[0];
		string clipName = parts[1].Replace("@", catName);	// make sure correct cat is being displayed


		BoneAnimation targetAnimationContainer = null;
		foreach (BoneAnimation container in animationContainers)
		{
			if (container.name == containerName)
			{
				targetAnimationContainer = container;
				targetAnimationContainer.gameObject.SetActive(true);
				targetAnimationContainer.animation.enabled = true;
			}
			else
			{
				container.gameObject.SetActive(false);
			}
		}
		
		if (targetAnimationContainer == null)
		{
			Debug.LogError(name + " : No animation container found with name " + containerName + ". Defaulting to first container.");
			targetAnimationContainer = animationContainers[0];
		}

		string newAnimationPath = containerName + "/" + clipName;

		if (currentAnimationPath != newAnimationPath)
		{
			currentAnimationPath = newAnimationPath;
		}
		else
		{
			return;
		}

		targetAnimationContainer.Stop();

		if (fadeTime <= 0)
			targetAnimationContainer.Play(clipName);
		else
		{
			targetAnimationContainer.CrossFade(clipName, fadeTime);
		}
			
//		if (moveRight)
//		{
//			// if going right, the scale.x needs to be positive 
//			if (targetAnimationContainer.transform.localScale.x < 0)
//			{
//				targetAnimationContainer.transform.localScale = targetAnimationContainer.transform.localScale.x(Mathf.Abs(targetAnimationContainer.transform.localScale.x));
//			}
//		}
//		else // moving left
//		{
//			// if going left, the scale.x needs to be negative
//			if (targetAnimationContainer.transform.localScale.x > 0)
//			{
//				targetAnimationContainer.transform.localScale = targetAnimationContainer.transform.localScale.x(targetAnimationContainer.transform.localScale.x * -1.0f);
//			}
//		}
	}
}
