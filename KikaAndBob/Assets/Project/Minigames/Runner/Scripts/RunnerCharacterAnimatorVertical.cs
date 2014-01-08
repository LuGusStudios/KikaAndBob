using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class RunnerCharacterAnimatorVertical : MonoBehaviour 
{
	public BoneAnimation[] animationContainers;
	public BoneAnimation currentAnimationContainer = null;
	public string currentAnimationClip = "";
	public string currentAnimationPath = "";
	
	public RunnerCharacterControllerVertical character = null;

	public string normalAnimation = "ALL/KikaParachute_FallingLeft";
	public string slowAnimation = "ALL/KikaParachute_Slower";
	public string fastAnimation = "ALL/KikaParachute_Faster";

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
			}
			else
			{
				container.gameObject.SetActive(false);
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

	public void SetupLocal()
	{
		if( animationContainers.Length == 0 )
		{
			animationContainers = transform.GetComponentsInChildren<BoneAnimation>();
		}
		
		if( animationContainers.Length == 0 )
		{
			Debug.LogError(name + " : no BoneAnimations found for this animator!");
		}
	}
	
	public void SetupGlobal()
	{
		if( character == null )
		{
			character = RunnerCharacterControllerVertical.use;
		}
		
		if( character == null )
		{
			Debug.LogError(name + " : no character found!");
		}
		else
		{
			character.onSpeedTypeChange += OnSpeedTypeChange;
			character.onHit  += OnHit;
		}
		
		PlayAnimation(normalAnimation);
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void OnSpeedTypeChange(RunnerCharacterControllerVertical.SpeedType oldType, RunnerCharacterControllerVertical.SpeedType newType)
	{
		if( newType == RunnerCharacterControllerVertical.SpeedType.NORMAL )
		{
			PlayAnimation( normalAnimation );
		}
		else if( newType == RunnerCharacterControllerVertical.SpeedType.SLOW )
		{
			PlayAnimation( slowAnimation );
		}
		else if( newType == RunnerCharacterControllerVertical.SpeedType.FAST )
		{
			PlayAnimation( fastAnimation );
		}
	}

	protected bool hitRoutineBusy = false;

	public void OnHit(RunnerPickup pickup)
	{
		if( pickup.negative )
		{
			LugusCoroutines.use.StartRoutine( HitRoutine(pickup) );
		}
	}

	/*
	protected IEnumerator HitAnimationRoutine(float delay)
	{
		yield return new WaitForSeconds(delay);

		hitRoutineBusy = false;
		
		PlayAnimation( runningAnimation );  
	}
	*/

	protected IEnumerator HitRoutine(RunnerPickup pickup)
	{
		//PlayAnimation( hitAnimation );

		//hitRoutineBusy = true;

		Color originalColor = Color.white;
		Color color = Color.red; 

		// we want the running animation to start playing before the invulnerability (red blinking) is done
		//LugusCoroutines.use.StartRoutine( HitAnimationRoutine (0.3f) );

		float duration = 1.5f; 
		int iterations = 5;
		float partDuration = duration / (float) iterations;

		for( int i = 0; i < iterations; ++i )
		{

			float percentage = 0.0f;
			float startTime = Time.time;
			bool rising = true;
			Color newColor = new Color();

			while( rising )
			{
				percentage = (Time.time - startTime) / (partDuration / 2.0f);
				newColor = originalColor.Lerp (color, percentage);

				foreach( BoneAnimation container in animationContainers )
					container.SetMeshColor( newColor );

				//currentAnimationContainer.SetMeshColor(newColor );

				if( percentage >= 1.0f )
					rising = false;

				yield return null;
			}

			percentage = 0.0f;
			startTime = Time.time;

			while( !rising )
			{
				percentage = (Time.time - startTime) / (partDuration / 2.0f);
				newColor = color.Lerp (originalColor,percentage);

				//currentAnimationContainer.SetMeshColor( newColor );
				
				foreach( BoneAnimation container in animationContainers )
					container.SetMeshColor( newColor );
				
				if( percentage >= 1.0f )
					rising = true;
				
				yield return null;
			}
		}


		//yield return new WaitForSeconds( duration );

		foreach( BoneAnimation container in animationContainers )
			container.SetMeshColor( originalColor );
	}

	
	protected void Update () 
	{

		//if( !character.jump && currentAnimationClip == "KikaSide_Jump" )
		//{
		//	PlayAnimation( "OTHER/KikaSide_Jump" );
		//}
	}
}
