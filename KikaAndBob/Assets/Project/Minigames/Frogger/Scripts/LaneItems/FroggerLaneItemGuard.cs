using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class FroggerLaneItemGuard : FroggerLaneItemLethal
{
	public string guardIdleAnimationName = "";
	public string guardChasingAnimationName = "";

	public float minimumTurnAroundTime = 1f;
	public float maximumTurnAroundTime = 5f;
	public float guardChaseSpeed = 10f;

	public BoneAnimation boneAnimation = null;

	protected bool lookingRight = false;

	public enum GuardState
	{
		NONE = -1,
		GUARDING = 1,
		CHASING = 2
	};

	protected GuardState state = GuardState.NONE;
	protected FroggerCharacter player = null;

	public override void SetUpLocal()
	{
		base.SetUpLocal();

		state = GuardState.GUARDING;

		// Randomly decide which side to face first
		if (Random.value < 0.5f)
		{
			lookingRight = true;

			if (transform.localScale.x > 0)
			{
				transform.localScale = transform.localScale.x(-Mathf.Abs(transform.localScale.x));
			}
			else
			{
				transform.localScale = transform.localScale.x(Mathf.Abs(transform.localScale.x));
			}
		}
		else
		{
			lookingRight = false;
		}
	}
	
	public override void SetupGlobal()
	{
		base.SetupGlobal();

		SearchPlayer();

		if (boneAnimation == null)
		{
			boneAnimation = transform.FindChild("Bone Animation").GetComponent<BoneAnimation>();
			if (boneAnimation == null)
			{
				Debug.Log("Cannot find the bone animation for the guard!");
			}
			else
			{
				boneAnimation.Stop();
				if (boneAnimation.AnimationClipExists(guardIdleAnimationName))
				{
					boneAnimation.Play(guardIdleAnimationName, PlayMode.StopAll);
				}
			}
		}

		StartCoroutine(ToggleDirectionRoutine());
	}
	
	protected void Awake()
	{
		SetUpLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
		if (player == null)
		{
			SearchPlayer();
		}

		if (!IsPlayerVisible())
		{
			state = GuardState.GUARDING;

			if (boneAnimation != null)
			{
				if (!boneAnimation.IsPlaying(guardIdleAnimationName))
				{
					boneAnimation.Play(guardIdleAnimationName);
				}
			}
		}
		else
		{
			state = GuardState.CHASING;

			float xMovement = guardChaseSpeed * Time.deltaTime;

			if (!lookingRight)
			{
				xMovement *= -1f;
			}

			transform.Translate(Vector3.right * xMovement, Space.World);

			// Start the chasing animation if isn't playing already
			if (boneAnimation != null)
			{
				if (!boneAnimation.IsPlaying(guardChasingAnimationName))
				{
					boneAnimation.Play(guardChasingAnimationName);
				}
			}
		}
	}

	protected void SearchPlayer()
	{
		if (player != null)
		{
			return;
		}

		GameObject playerObj = GameObject.Find("Player");
		if (playerObj != null)
		{
			player = playerObj.GetComponent<FroggerCharacter>();
			if (player == null)
			{
				Debug.LogError("Could not find the player!");
			}
		}
	}

	private IEnumerator ToggleDirectionRoutine()
	{
		while (Application.isPlaying)
		{
			if (state != GuardState.CHASING)
			{
				if (transform.localScale.x > 0)
				{
					transform.localScale = transform.localScale.x(- Mathf.Abs(transform.localScale.x));
				}
				else
				{
					transform.localScale = transform.localScale.x(Mathf.Abs(transform.localScale.x));
				}

				lookingRight = !lookingRight;
			}

			yield return new WaitForSeconds(Random.Range(minimumTurnAroundTime, maximumTurnAroundTime));
		}

		yield break;
	}

	private bool IsPlayerVisible()
	{
		// If either of the following is true, then the player is not in sight
		if ((player == null)	// No player
			|| (player.CurrentLane != ParentLane)	// Player on different lane
			|| (lookingRight && (player.transform.position.x < transform.position.x))	// Looking in wrong direction
			|| (!lookingRight && (player.transform.position.x > transform.position.x)))	// Looking in wrong direction
		{
			return false;
		}

		// Possible TODO: Look for any obstacles in the way (i.e. a rock) through which the guard cannot pass

		// Else, the player is in sight
		return true;
	}
}
