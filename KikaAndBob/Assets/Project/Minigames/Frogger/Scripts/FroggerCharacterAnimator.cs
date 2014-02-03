using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class FroggerCharacterAnimator : MonoBehaviour 
{
	protected BoneAnimation[] animationContainers;
	public BoneAnimation currentAnimationContainer = null;
	protected string currentAnimationClip = "";
	protected string currentAnimationPath = "";
	protected string lastAnimationPath = "";
	
	public string idleUp;
	public string idleDown;
	
	public string walkUp;
	public string walkDown;
	public string walkSide;

	public string hit;
	
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
		}

		if (lastAnimationPath == animationPath)
		{
			return;
		}

		lastAnimationPath = animationPath;
		
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