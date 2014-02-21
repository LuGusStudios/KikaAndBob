using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KikaAndBob
{
	namespace Runner
	{
		public enum SpeedType
		{
			NONE = -1,

			STILL = 0,
			SLOW = 1, // modifierPercentage = 0.5
			NORMAL = 2, // modifierPercentage = 1
			FAST = 3 // modifierPercentage = 1.2f
		}

		public delegate void OnJump(bool start);
		public delegate void OnHit(RunnerPickup pickup);
		public delegate void OnSlide(bool start);
		public delegate void OnSpeedTypeChange(SpeedType oldType, SpeedType newType);
	}
}

public interface IRunnerCharacterController
{
	void OnPickupHit(RunnerPickup pickup);
	
	DataRange SpeedRange();
	Vector2 Velocity();
	
	Vector3 SpeedScale(); // to be used in ParallaxMover

	
	event KikaAndBob.Runner.OnHit onHit;
}

public interface IRunnerCharacterController_JumpSlide
{
	event KikaAndBob.Runner.OnJump onJump;
	event KikaAndBob.Runner.OnSlide onSlide;
}

public interface IRunnerCharacterController_FasterSlower
{
	event KikaAndBob.Runner.OnSpeedTypeChange onSpeedTypeChange;
}

public class RunnerCharacterController : MonoBehaviour 
{
	private static IRunnerCharacterController _use = null;
	
	public static void Reset()
	{
		_use = null;
		_jumpSlide = null;
		_fasterSlower = null;
	}

	public static IRunnerCharacterController use 
	{ 
		get 
		{
			if ( _use == null )
			{
				if( RunnerCharacterControllerJumpSlide.Exists() )
					_use = RunnerCharacterControllerJumpSlide.use;
				else if( RunnerCharacterControllerFasterSlower.Exists() )
					_use = RunnerCharacterControllerFasterSlower.use;
				else if( RunnerCharacterControllerClimbing.Exists() )
					_use = RunnerCharacterControllerClimbing.use;
			}
			
			
			return _use; 
		}
	}

	public static MonoBehaviour useBehaviour 
	{ 
		get 
		{
			return (MonoBehaviour) RunnerCharacterController.use;

		}
	}

	private static IRunnerCharacterController_JumpSlide _jumpSlide = null;

	public static IRunnerCharacterController_JumpSlide jumpSlide
	{
		
		get 
		{
			if ( _jumpSlide == null )
			{
				if( RunnerCharacterControllerJumpSlide.Exists() )
					_jumpSlide = RunnerCharacterControllerJumpSlide.use;
			}
			
			
			return _jumpSlide; 
		}
	}
	
	private static IRunnerCharacterController_FasterSlower _fasterSlower = null;
	
	public static IRunnerCharacterController_FasterSlower fasterSlower
	{
		
		get 
		{
			if ( _fasterSlower == null )
			{
				if( RunnerCharacterControllerFasterSlower.Exists() )
					_fasterSlower = RunnerCharacterControllerFasterSlower.use;
				else if( RunnerCharacterControllerClimbing.Exists() )
					_fasterSlower = RunnerCharacterControllerClimbing.use;
			}
			
			
			return _fasterSlower; 
		}
	}





}
