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
	protected ParticleSystem hitParticles = null;
	[HideInInspector]
	public FroggerCharacterAnimator characterAnimator = null;
	protected Vector3 originalScale = Vector3.one;
	protected BoneAnimation[] boneAnimations;
	protected Renderer[] renderers;
	protected BoxCollider boundsCollider;	// this is not a 2D collider, because bounds cannot be retrieved for those 
											// (this collider is primarily used for clamping characters to the screen)

	protected void Awake()
	{
		SetUpLocal();
	}

	protected void Start()
	{
	}

	public virtual void SetUpLocal()
	{
		boneAnimations = GetComponentsInChildren<BoneAnimation>();
		renderers = GetComponentsInChildren<Renderer>();
		originalScale = transform.localScale;

		if (boundsCollider == null)
		{
			boundsCollider = GetComponent<BoxCollider>();
		}
		if (boundsCollider == null)
		{
			Debug.LogError("FroggerPlayer: Missing bounds collider.");
		}

		if (characterAnimator == null)
		{
			characterAnimator = GetComponent<FroggerCharacterAnimator>();
		}
		if (characterAnimator == null)
		{
			Debug.LogError("FroggerPlayer: Missing character animator.");
		}

		if (hitParticles == null)
		{
			hitParticles = GetComponentInChildren<ParticleSystem>();
		}

		if (hitParticles == null)
		{
			Debug.LogError("FroggerPlayer: Missing hit particles.");
		}
	}

	protected void Update () 
	{
		CheckSurface();
		UpdatePosition();
	}

	public virtual void Reset()
	{
		ShowCharacter(true);

		movingToLane = false;
		if (laneMoveRoutine != null && laneMoveRoutine.Running)
			laneMoveRoutine.StopRoutine();

		currentLane = FroggerLaneManager.use.GetLane(0);
		transform.position = currentLane.transform.position + new Vector3(0, 0, -5);

		transform.localScale = Vector3.one * maxScale;

		characterAnimator.PlayAnimation(characterAnimator.idleDown);

		Debug.Log("Reset character: " + gameObject.name);
	}

	protected void ScaleByDistanceHorizontal()
	{
		transform.localScale = Vector3.one * Mathf.Lerp(maxScale, minScale, (transform.position.v2() - FroggerLaneManager.use.GetBottomLaneCenter()).y / FroggerLaneManager.use.GetLevelLengthLaneCenters());
	}

	protected void MoveSideways(bool right)
	{
		if (right)
		{
			characterAnimator.PlayAnimation(characterAnimator.walkSide);

			if (transform.localScale.x > 0)
				transform.localScale = transform.localScale.x(-1 * Mathf.Abs(transform.localScale.x));

			if (GetCollisionHorizontal(right))
				return;

			transform.Translate(transform.right.normalized * speed * Time.deltaTime, Space.World);
		}
		else
		{
			characterAnimator.PlayAnimation(characterAnimator.walkSide);

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

		if (targetLaneIndex > currentLaneIndex)
		{
			// don't move if there is a collision item on the target lane
			if (GetCollisionVertical(targetLane, true))
				return;

			characterAnimator.PlayAnimation(characterAnimator.walkUp);
			laneMoveRoutine = LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, true));
		}
		else if (targetLaneIndex < currentLaneIndex)
		{	
			// don't move if there is a collision item on the target lane
			if (GetCollisionVertical(targetLane, false))
				return;

			characterAnimator.PlayAnimation(characterAnimator.walkDown);
			laneMoveRoutine = LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, false));
		}
		else
		{
			//Debug.Log("Character is trying to move to the lane: " + targetLaneIndex + " and it is already on that lane! Stopping.");
			return;
		}
	}

	protected bool GetCollisionVertical(FroggerLane targetLane, bool movingUp)
	{
		// first check current lane to see if we can exit in the proposed direction
		Vector2 checkLocation = new Vector2(transform.position.x, currentLane.transform.position.y);
		RaycastHit2D[] hits = Physics2D.RaycastAll(checkLocation, this.transform.forward);
		foreach(RaycastHit2D hit in hits)
		{
			FroggerCollider froggerCollider = hit.transform.GetComponent<FroggerCollider>();
			
			if (hit.transform != null && froggerCollider != null)
			{
				if (froggerCollider.colliderType == FroggerCollider.FroggerColliderType.Bottom && !movingUp)
					return true;
				else if (froggerCollider.colliderType == FroggerCollider.FroggerColliderType.Top && movingUp)
					return true;
			}
		}

		// then check target lane to see if we can enter in the proposed direction
		checkLocation = new Vector2(transform.position.x, targetLane.transform.position.y);
		hits = Physics2D.RaycastAll(checkLocation, this.transform.forward);
		foreach(RaycastHit2D hit in hits)
		{
			FroggerCollider froggerCollider = hit.transform.GetComponent<FroggerCollider>();

			if (hit.transform != null && froggerCollider != null)
			{
				if (froggerCollider.colliderType == FroggerCollider.FroggerColliderType.All)
				    return true;
				else if (froggerCollider.colliderType == FroggerCollider.FroggerColliderType.Bottom && movingUp)
					return true;
				else if (froggerCollider.colliderType == FroggerCollider.FroggerColliderType.Top && !movingUp)
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
			checkLocation = new Vector2(transform.position.x + boundsCollider.bounds.extents.x, transform.position.y);
		}
		else
		{
			checkLocation = new Vector2(transform.position.x - boundsCollider.bounds.extents.x, transform.position.y);
		}

		RaycastHit2D[] hits = Physics2D.RaycastAll(checkLocation, this.transform.forward);
		foreach(RaycastHit2D hit in hits)
		{
			FroggerCollider froggerCollider = hit.transform.GetComponent<FroggerCollider>();

			if (hit.transform != null && froggerCollider != null)
			{
				if (froggerCollider.colliderType == FroggerCollider.FroggerColliderType.All)
					return true;
			}
		}

		return false;
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

		movingToLane = false;
	}

	protected void ClampToScreen()
	{
		if (!FroggerGameManager.use.gameRunning)
			return;

		// clamp character to the screen in all directions (in traditional Frogger, only X clamping would be relevant, but we are trying to make everything possible in every direction)
		Vector3 screenPos = LugusCamera.game.WorldToScreenPoint(transform.position);
		Bounds spriteBounds = boundsCollider.bounds;	// size of sprite is added to / subtracted from screen edges ! Not spriterenderer.bounds, because those are in world coordinates
		
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
		if (!FroggerGameManager.use.gameRunning)
			return;

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

	public void ShowCharacter(bool show)
	{
		foreach(Renderer r in renderers)
		{
			r.enabled = show;
		}
	}

	public void Blink(Color color, float duration, int count)
	{
		LugusCoroutines.use.StartRoutine(SmoothMovesUtil.Blink(boneAnimations, color, duration, count));
	}

	public void DoHitAnimation()
	{
		hitParticles.Play();
		hitParticles.transform.position = hitParticles.transform.position.z(0);
		characterAnimator.PlayAnimation(characterAnimator.hit);

		// make hit animation 'bend' in direction of hit
		if (currentLane != null)
		{
			if (currentLane.goRight)
			{
				if (transform.localScale.x > 0)
					transform.localScale = transform.localScale.x(-1 * Mathf.Abs(transform.localScale.x));
			}
			else
			{
				if (transform.localScale.x < 0)
					transform.localScale = transform.localScale.x(Mathf.Abs(transform.localScale.x));
			}
		}
	}
}
