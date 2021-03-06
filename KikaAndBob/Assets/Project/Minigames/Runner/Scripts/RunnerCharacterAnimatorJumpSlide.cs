using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
using KikaAndBob.Runner;

public class RunnerCharacterAnimatorJumpSlide : RunnerCharacterAnimator 
{	
	public IRunnerCharacterController_JumpSlide character = null;

	public string runningAnimation = "RUNNING/KikaSide_RunningSprites";
	public string jumpAnimation = "OTHER/KikaSide_JumpShort";
	public string hitAnimation = "OTHER/KikaSide_Hit";
	public string slideAnimation = "OTHER/KikaSide_Slide";
	public string deathAnimation = "DEATH/KikaSide_Death";

	public ParticleSystem dust = null;

	public bool characterDead = false;

	public override void StopAll()
	{
		dust.gameObject.SetActive(false);

		base.StopAll();
	}

	public override void SetupGlobal()
	{
		base.SetupGlobal();

		if( character == null )
		{
			character = RunnerCharacterController.jumpSlide;
		}
		
		if( character == null )
		{
			Debug.LogError(name + " : no character found!");
		}
		else
		{
			character.onJump += OnJump;
			character.onSlide += OnSlide;
			( (IRunnerCharacterController) character).onHit  += OnHit;
		}
		
		PlayAnimation(runningAnimation);
	}

	public override void SetupLocal()
	{
		base.SetupLocal();

		if( dust == null )
		{
			dust = transform.FindChild("Dust").GetComponent<ParticleSystem>();

			dust.enableEmission = true;
		}
	}
	
	protected override void Awake()
	{
		SetupLocal();
	}

	protected override void Start () 
	{
		SetupGlobal();
	}

	public void OnJump(bool start)
	{
		//Debug.LogError(Time.frameCount + " JUMPING! " + start);

		if( start )
		{
			dust.enableEmission = false;
			PlayAnimation( jumpAnimation );
		}
		else
		{
			dust.enableEmission = true;
			PlayAnimation( runningAnimation );
		}
	}

	public void OnSlide(bool start)
	{
		if( start )
		{
			dust.enableEmission = true;
			PlayAnimation( slideAnimation ); 
		}
		else
		{
			dust.enableEmission = true;
			PlayAnimation( runningAnimation );
		}
	}

	public void OnHit(RunnerPickup pickup)
	{
		if( pickup.negative )
		{
			LugusCoroutines.use.StartRoutine( HitRoutine(pickup) );
		}
	}

	protected IEnumerator HitAnimationRoutine(float delay)
	{
		yield return new WaitForSeconds(delay);

		hitRoutineBusy = false;

		if( !characterDead )
		{
			dust.enableEmission = true; 
			PlayAnimation( runningAnimation );  
		}
	}

	protected IEnumerator HitRoutine(RunnerPickup pickup)
	{
		//Debug.LogError("HIT ANIMATION PLAYING NOW");

		if( !characterDead )
		{
			PlayAnimation( hitAnimation );
		}

		hitRoutineBusy = true;

		// we want the running animation to start playing before the invulnerability (red blinking) is done
		LugusCoroutines.use.StartRoutine( HitAnimationRoutine (0.3f) );

		LugusCoroutines.use.StartRoutine( SmoothMovesUtil.Blink(animationContainers, Color.red, 1.5f, 5) );

		yield break;

		/*
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
		*/
	}

	
	protected void Update () 
	{

		//if( !character.jump && currentAnimationClip == "KikaSide_Jump" )
		//{
		//	PlayAnimation( "OTHER/KikaSide_Jump" );
		//}
	}
}
