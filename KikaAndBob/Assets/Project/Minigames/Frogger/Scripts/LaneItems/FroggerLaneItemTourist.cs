using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class FroggerLaneItemTourist : FroggerLaneItemLethal 
{
	public string walkingAnimation = "";
	public string idleAnimation = "";
	public BoneAnimation animation = null;

	public TouristState State
	{
		get
		{
			return touristState;
		}
	}

	public bool HasIdled
	{
		get
		{
			return hasIdled;
		}
	}

	public float TryAgainTimer
	{
		get
		{
			return tryAgainTimer;
		}
		set
		{
			tryAgainTimer = value;
		}
	}

	public enum TouristState
	{
		NONE = -1,
		IDLE = 1,
		WALKING = 2
	};

	protected TouristState touristState = TouristState.NONE;
	protected bool hasIdled = false;
	protected float tryAgainTimer = -1f;
	
	public override  void SetupGlobal()
	{
		base.SetupGlobal();

		if (animation == null)
		{
			animation = transform.FindChild("Bone Animation").GetComponent<BoneAnimation>();
			if (animation == null)
			{
				Debug.LogError(name + ": could not find the bone animation.");
			}
			else
			{
				if (animation.AnimationClipExists(walkingAnimation))
				{
					touristState = TouristState.WALKING;
					animation.Play(walkingAnimation, PlayMode.StopAll);
				}
			}
		}
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	protected void Update()
	{
		tryAgainTimer -= Time.deltaTime;
	}

	public void StartIdle(float idleTime)
	{
		if (touristState == TouristState.IDLE)
		{
			return;
		}

		StartCoroutine(IdleRoutine(idleTime));
	}

	private IEnumerator IdleRoutine(float idleTime)
	{
		// This routine lets the character stop doing
		// the walking animation for a period of time

		touristState = TouristState.IDLE;

		if (animation != null)
		{
			if (animation.AnimationClipExists(idleAnimation)
				&& animation.AnimationClipExists(walkingAnimation))
			{
				animation.Play(idleAnimation, PlayMode.StopAll);

				yield return new WaitForSeconds(idleTime);

				animation.Play(walkingAnimation, PlayMode.StopAll);
			}
		}

		touristState = TouristState.WALKING;
		hasIdled = true;

		yield break;
	}
}
