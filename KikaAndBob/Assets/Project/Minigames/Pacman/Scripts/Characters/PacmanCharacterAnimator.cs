using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class PacmanCharacterAnimator : MonoBehaviour 
{
	protected BoneAnimation[] animationContainers;
	public BoneAnimation currentAnimationContainer = null;
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
		if(animationContainers == null || animationContainers.Length == 0 )
		{
			animationContainers = transform.GetComponentsInChildren<BoneAnimation>(true);
		}
		
		if( animationContainers.Length == 0 )
		{
			Debug.LogError(name + " : no BoneAnimations found for this animator!");
		}
		else
		{
			currentAnimationContainer = animationContainers[0];
		}
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

		currentAnimationContainer = null;
		foreach( BoneAnimation container in animationContainers )
		{
			if( container.name == containerName )
			{
				currentAnimationContainer = container;
				currentAnimationContainer.gameObject.SetActive(true);
			}
			else
			{
				container.gameObject.SetActive(false);
			}
		}
		
		if( currentAnimationContainer == null )
		{
			Debug.LogError(name + " : No animationContainer found for name: " + containerName);
			currentAnimationContainer = animationContainers[0];
		}

		currentAnimationPath = animationPath;
	
		if ( parts.Length == 2 )
		{
			currentAnimationClip = clipName;
			currentAnimationContainer.Stop();
			currentAnimationContainer.Play(clipName);
		}
	}
}
