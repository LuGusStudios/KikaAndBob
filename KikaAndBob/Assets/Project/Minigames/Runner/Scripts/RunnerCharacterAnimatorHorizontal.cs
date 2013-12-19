using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class RunnerCharacterAnimatorHorizontal : MonoBehaviour 
{
	public BoneAnimation[] animationContainers;
	public BoneAnimation currentAnimationContainer = null;
	public string currentAnimationClip = "";
	public string currentAnimationPath = "";
	
	public RunnerCharacterControllerHorizontal character = null;

	public string runningAnimation = "RUNNING/KikaSide_RunningSprites";
	public string jumpAnimation = "OTHER/KikaSide_JumpShort";
	public string hitAnimation = "OTHER/KikaSide_Hit";

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
			character = RunnerCharacterControllerHorizontal.use;
		}
		
		if( character == null )
		{
			Debug.LogError(name + " : no character found!");
		}
		else
		{
			character.onJump += OnJump;
			character.onHit += OnHit;
		}
		
		PlayAnimation(runningAnimation);
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void OnJump(bool start)
	{
		//Debug.LogError(Time.frameCount + " JUMPING! " + start);

		if( start )
		{
			PlayAnimation( jumpAnimation ); 
			// TODO: this should be done in OnGrounded()
			//PlayAnimationDelayed( "RUNNING/KikaSide_RunningSprites", 1.05f );
		}
		else
		{
			PlayAnimation( runningAnimation );
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

	protected IEnumerator HitRoutine(RunnerPickup pickup)
	{
		hitRoutineBusy = true;
		
		PlayAnimation( hitAnimation );

		Color originalColor = Color.white;
		Color color = Color.red;

		float duration = 1.5f;
		int iterations = 5;
		float partDuration = duration / (float) iterations;

		for( int i = 0; i < iterations; ++i )
		{

			iTween.ColorTo(this.currentAnimationContainer.gameObject, color, partDuration / 2.0f );
			
			yield return new WaitForSeconds( partDuration / 2.0f );
			
			iTween.ColorTo(this.currentAnimationContainer.gameObject, originalColor, partDuration / 2.0f );

			yield return new WaitForSeconds( partDuration / 2.0f );
		}
		
		hitRoutineBusy = false;

		PlayAnimation( runningAnimation );
	}

	
	protected void Update () 
	{

		//if( !character.jump && currentAnimationClip == "KikaSide_Jump" )
		//{
		//	PlayAnimation( "OTHER/KikaSide_Jump" );
		//}
	}
}
