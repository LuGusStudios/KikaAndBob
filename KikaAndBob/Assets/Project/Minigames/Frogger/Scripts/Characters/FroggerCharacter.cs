using UnityEngine;
using System.Collections;
using SmoothMoves;
using System.Collections.Generic;

namespace KikaAndBobFrogger
{
	public enum CharacterType
	{
		None,
		Kika,
		Bob,
		Enemy
	}
}

public class FroggerCharacter : MonoBehaviour {

	public float speed = 100;
	public float maxScale = 1f;
	public float minScale = 0.4f;
	public KikaAndBobFrogger.CharacterType characterType = KikaAndBobFrogger.CharacterType.None;

	protected FroggerLane currentLane = null;
	protected FroggerLaneItem currentLaneItem = null;
	protected bool movingToLane = false;
	protected ILugusCoroutineHandle laneMoveRoutine = null;
	protected Vector3 originalScale = Vector3.one;
	protected BoneAnimation[] boneAnimations;

	void Start()
	{
		boneAnimations = GetComponentsInChildren<BoneAnimation>();
		originalScale = transform.localScale;

		PlayAnimation("Idle");
	}

	void Update () 
	{
		CheckSurface();
		UpdatePosition();
	}

	public virtual void Reset()
	{
		movingToLane = false;
		if (laneMoveRoutine != null && laneMoveRoutine.Running)
			laneMoveRoutine.StopRoutine();

		currentLane = FroggerLaneManager.use.GetLane(0);
		transform.position = currentLane.transform.position + new Vector3(0, 0, -5);
		transform.localScale = Vector3.one * maxScale;
	}

	protected void ScaleByDistanceHorizontal()
	{
		transform.localScale = Vector3.one * Mathf.Lerp(maxScale, minScale, (transform.position.v2() - FroggerLaneManager.use.GetBottomLaneCenter()).y / FroggerLaneManager.use.GetLevelLengthLaneCenters());
	}

	protected void MoveSideways(bool right)
	{
		if (right)
		{
			PlayAnimation("Left");

			if (transform.localScale.x > 0)
				transform.localScale = transform.localScale.x(-1 * Mathf.Abs(transform.localScale.x));

			if (GetCollisionHorizontal(right))
				return;

			transform.Translate(transform.right.normalized * speed * Time.deltaTime, Space.World);
		}
		else
		{
			PlayAnimation("Left");

			if (transform.localScale.x < 0)
				transform.localScale = transform.localScale.x(Mathf.Abs(transform.localScale.x));

			if (GetCollisionHorizontal(right))
				return;

			transform.Translate(-1 * transform.right.normalized * speed * Time.deltaTime, Space.World);
		}
	}

	protected void MoveToLane(FroggerLane targetLane)
	{
		// block this if we already moving (set in the move routine)
		if (movingToLane)
			return;

		if (targetLane == null)
		{
			Debug.LogError("Target lane is null! Stopping.");
			return;
		}

		int currentLaneIndex = FroggerLaneManager.use.GetLaneIndex(currentLane);
		int targetLaneIndex = FroggerLaneManager.use.GetLaneIndex(targetLane);

		// don't move if there is a collision item on the target lane
		if (GetCollisionVertical(targetLane))
		    return;

		if (targetLaneIndex > currentLaneIndex)
		{
			PlayAnimation("Up");
			laneMoveRoutine = LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, true));

		}
		else if (targetLaneIndex < currentLaneIndex)
		{	
			PlayAnimation("Down");
			laneMoveRoutine = LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, false));
		}
		else
		{
			Debug.Log("Character is trying to move to the lane: " + targetLaneIndex + " and it is already on that lane! Stopping.");
			return;
		}
	}

	protected bool GetCollisionVertical(FroggerLane lane)
	{
		Vector2 checkLocation = new Vector2(transform.position.x, lane.transform.position.y);

		RaycastHit2D[] hits = Physics2D.RaycastAll(checkLocation, this.transform.forward);
		foreach(RaycastHit2D hit in hits)
		{
			if (hit.transform != null && hit.transform.GetComponent<FroggerCollider>() != null)
			{
				return true;
			}
		}
		return false;
	}

	protected bool GetCollisionHorizontal(bool right)
	{
		Vector2 checkLocation;
		if (right)
		{
			checkLocation = new Vector2(transform.position.x + GetComponent<SpriteRenderer>().bounds.extents.x, transform.position.y);
		}
		else
		{
			checkLocation = new Vector2(transform.position.x - GetComponent<SpriteRenderer>().bounds.extents.x, transform.position.y);
		}

		RaycastHit2D[] hits = Physics2D.RaycastAll(checkLocation, this.transform.forward);
		foreach(RaycastHit2D hit in hits)
		{
			if (hit.transform != null && hit.transform.GetComponent<FroggerCollider>() != null)
			{
				return true;
			}
		}

		return false;
	}

	private string currentAnimation;
	protected void PlayAnimation(string animName)
	{
		if (animName == currentAnimation)
			return;

		foreach(BoneAnimation ba in boneAnimations)
		{
			ba.gameObject.SetActive(false);

			if (ba.gameObject.name == animName)
			{
				currentAnimation = animName;
				ba.gameObject.SetActive(true);
			}
		}
	}

	protected IEnumerator LaneMoveRoutine(FroggerLane targetLane, bool toHigher)
	{
		movingToLane = true;

		// the character has to move a certain distance from the middle of one lane to the middle of the next
		// this means moving half of the current lane's height plus half of the target lane's height
		//float targetDistance = (currentLane.GetHeight() * 0.5f) +  (targetLane.GetHeight() * 0.5f);

		float targetDistance = Vector2.Distance(currentLane.GetCenterPoint(), targetLane.GetCenterPoint());

		float coveredDistance = 0;
		Vector3 lastPosition = transform.position;

		float startZ = currentLane.transform.position.z;
		float targetZ = targetLane.transform.position.z - 5;

		while(coveredDistance < targetDistance)
		{
			float nextMoveDistance = speed * Time.deltaTime;

			// Clamp the distance added this frame to maximum amount still needed. If not, the character might overshoot due to framerate.
			nextMoveDistance = Mathf.Clamp(nextMoveDistance, 0, targetDistance - coveredDistance);

			// break on very small move distances; if not, this loop might continue forever on rounding errors alone
			if (nextMoveDistance < 0.0001f)
				break;

			if (toHigher)
			{
				transform.Translate(transform.up.normalized * nextMoveDistance, Space.World);
			}
			else
			{
				transform.Translate(-1 * transform.up.normalized * nextMoveDistance, Space.World);
			}

			// lerp between lane z positions
			transform.position = transform.position.z(Mathf.Lerp(startZ, targetZ, targetDistance / coveredDistance));

			// measure distance since last frame - this way we can track progress in whichever direction instead of just one
			// !!!: Make sure to ignore z movement here (so set z distance for the two comparison points to be identical)
			coveredDistance += Vector3.Distance(lastPosition, transform.position.z(lastPosition.z));
			lastPosition = transform.position;

			ScaleByDistanceHorizontal();

			yield return new WaitForEndOfFrame();
		}

		PlayAnimation("Idle");

		movingToLane = false;
	}

	protected void ClampToScreen()
	{
		// clamp character to the screen in all directions (in traditional Frogger, only X clamping would be relevant, but we are trying to make everything possible in every direction)
		Vector3 screenPos = LugusCamera.game.WorldToScreenPoint(transform.position);
		Bounds spriteBounds = GetComponent<SpriteRenderer>().sprite.bounds;	// size of sprite is added to / subtracted from screen edges ! Not spriterenderer.bounds, because those are in world coordinates
		
		if (screenPos.x < spriteBounds.extents.x)
		{
			transform.position =
				transform.position.x(LugusCamera.game.ScreenToWorldPoint(screenPos.x(spriteBounds.extents.x)).x);
		}
		else if (screenPos.x > Screen.width - spriteBounds.extents.x)
		{
			transform.position =
				transform.position.x(LugusCamera.game.ScreenToWorldPoint(screenPos.x(Screen.width - spriteBounds.extents.x)).x);
		}
		
		if (screenPos.y < 0)
		{
			transform.position =
				transform.position.y(0);
		}
		else if (screenPos.y > Screen.height - spriteBounds.extents.y)
		{
			transform.position =
				transform.position.x(LugusCamera.game.ScreenToWorldPoint(screenPos.y(Screen.height - spriteBounds.extents.y)).y);
		}
	}

	protected virtual void UpdatePosition()
	{

	}

	protected virtual void CheckSurface()
	{
//		if (movingToLane)
//			return;

		FroggerLaneItem laneItemUnderMe = null;

		// check all raycast hits
		RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position + new Vector3(0, 0, -1000), this.transform.forward);
		bool laneItemFound = false;
		foreach(RaycastHit2D hit in hits)
		{
			if (hit.transform != null)
			{
				// first check if we're on any other kind of lane item (e.g. log) and only the detect the topmost of these (in case anything ever passes under something else)
				if (laneItemFound == false)
				{
					laneItemUnderMe = hit.transform.GetComponent<FroggerLaneItem>();
					if (laneItemUnderMe != null)
					{
						Debug.Log (laneItemUnderMe.name, laneItemUnderMe.gameObject);
						laneItemFound = true;
						continue;
					}
				}

				// then check if we're over a lane; if yes set current lane
				// MIND: ONLY apply EnterSurfaceEffect if we didn't yet detect a lane item that we're on (e.g. floating log)
				FroggerLane lane = hit.transform.GetComponent<FroggerLane>();

				if (lane != null && !movingToLane)	// only check this when the character has arrived on a lane; 
													// it's only really relevant then and helps prevent glitches with slightly overlapping colliders
				{
					if (currentLane != null)
					{
						if (currentLane != lane)	// if we stepped from one lane into another
						{
							if (laneItemFound)		// if we stepped from one lane onto a lane item in the next lane
							{
								currentLane.Leave(this);
							}
							else 					// if we stepped from one lane into a different lane (no lane item under character)
							{
								currentLane.Leave(this);
								lane.Enter(this);
							}
						}
						else 						// if we didn't step from one lane into another, but for example stepped sideways
						{
							if (!laneItemFound) 	// if we stepped sideways off a lane item onto the lane proper
							{
								lane.Enter(this);
							}
						}
						currentLane = lane;
					}
				}
		
			}
		}

		if (currentLaneItem != laneItemUnderMe)
		{
			if (currentLaneItem != null)
				currentLaneItem.Leave(this);

			if (laneItemUnderMe != null)
				laneItemUnderMe.Enter(this);
		}

		currentLaneItem = laneItemUnderMe;
	}
}
