using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
using KikaAndBob.Runner;

public class RunnerCharacterAnimatorFasterSlower : RunnerCharacterAnimator 
{
	public IRunnerCharacterController_FasterSlower character = null;

	public string normalAnimation = "ALL/KikaParachute_FallingLeft";
	public string slowAnimation = "ALL/KikaParachute_Slower";
	public string fastAnimation = "ALL/KikaParachute_Faster";
	public string stillAnimation = "ALL/KikaParachute_Slower";

	
	public override void SetupGlobal()
	{
		base.SetupGlobal();

		if( character == null )
		{
			character = RunnerCharacterController.fasterSlower;
		}
		
		if( character == null )
		{
			Debug.LogError(name + " : no character found!");
		}
		else
		{
			character.onSpeedTypeChange += OnSpeedTypeChange;
			( (IRunnerCharacterController) character).onHit  += OnHit;
		}
		
		PlayAnimation(normalAnimation);
	} 
	
	protected override void Awake()
	{
		SetupLocal(); 
	}

	protected override void Start () 
	{
		SetupGlobal();
	}

	public void OnSpeedTypeChange(SpeedType oldType, SpeedType newType)
	{
		if( !this.enabled )
			return;

		if( newType == SpeedType.NORMAL )
		{
			PlayAnimation( normalAnimation );
		}
		else if( newType == SpeedType.SLOW )
		{
			PlayAnimation( slowAnimation );
		}
		else if( newType == SpeedType.FAST )
		{
			PlayAnimation( fastAnimation );
		}
		else if( newType == SpeedType.STILL )
		{
			PlayAnimation( stillAnimation );
		}
	}


	public void OnHit(RunnerPickup pickup)
	{
		if( !this.enabled )
			return;

		if( pickup == null || pickup.negative )
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
		if( !this.enabled )
			yield break;

		//Debug.LogError("HITROUTINE FASTERSLOWER");
		LugusCoroutines.use.StartRoutine( SmoothMovesUtil.Blink(animationContainers, Color.red, 1.5f, 5) );
		yield break;
	}

	
	protected void Update () 
	{

		//if( !character.jump && currentAnimationClip == "KikaSide_Jump" )
		//{
		//	PlayAnimation( "OTHER/KikaSide_Jump" );
		//}
	}
}
