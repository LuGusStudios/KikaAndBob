using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class RunnerCharacterAnimator : MonoBehaviour 
{
	public BoneAnimation[] animationContainers;
	public BoneAnimation currentAnimationContainer = null;
	public string currentAnimationClip = "";
	public string currentAnimationPath = ""; 
	
	protected bool hitRoutineBusy = false;

	public void PlayAnimation(string animationPath)
	{
		string[] parts = animationPath.Split('/');
		if( parts.Length != 2 )
		{
			Debug.LogError(name + " : AnimationPath should be a string with a single / as separator! " + animationPath );
			return;
		}
		
		string containerName = parts[0];
		string clipName = parts[1];
		
		currentAnimationContainer = null;
		foreach( BoneAnimation container in animationContainers )
		{
			if( container.name == containerName )
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
		
		if( currentAnimationContainer == null )
		{
			Debug.LogError(name + " : No animationContainer found for name " + containerName);
			currentAnimationContainer = animationContainers[0];
		}
		
		currentAnimationPath = animationPath;
		currentAnimationClip = clipName;
		
		if( !hitRoutineBusy )
		{
			//Debug.LogError("PLAY ANIMATION " + currentAnimationContainer.name + "/" + clipName);
			currentAnimationContainer.Stop();
			//Debug.Log ("PLAYING ANIMATION " + currentAnimation.animation.clip.name + " ON " + currentAnimation.name );
			currentAnimationContainer.Play(clipName);//CrossFade( clipName, 0.5f );
		}
	}
	
	public void PlayAnimationDelayed(string animationPath, float delay)
	{
		LugusCoroutines.use.StartRoutine( PlayAnimationDelayedRoutine(animationPath, delay) );
	}
	
	protected IEnumerator PlayAnimationDelayedRoutine(string animationPath, float delay)
	{
		yield return new WaitForSeconds(delay);
		
		PlayAnimation( animationPath );
	}
	
	public virtual void SetupLocal()
	{
		if( animationContainers.Length == 0 )
		{
			animationContainers = transform.GetComponentsInChildren<BoneAnimation>();
		}
		
		if( animationContainers.Length == 0 )
		{
			Debug.LogError(transform.Path () + " : no BoneAnimations found for this animator!");
		}
	}

	protected virtual void Awake()
	{
		SetupLocal();
	}

	public virtual void SetupGlobal()
	{

	}
	
	protected virtual void Start()
	{
		SetupGlobal();
	}
}
