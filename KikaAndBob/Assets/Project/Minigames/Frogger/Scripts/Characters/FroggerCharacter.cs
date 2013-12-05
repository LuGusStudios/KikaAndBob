using UnityEngine;
using System.Collections;

public class FroggerCharacter : MonoBehaviour {

	public float speed = 100;
	public Vector3 maxScale = new Vector3(1f, 1f, 1f);
	public Vector3 minScale = new Vector3(0.4f, 0.4f, 0.4f);

	protected FroggerLane currentLane = null;
	protected FroggerLaneItem currentLaneItem = null;
	protected bool movingToLane = false;
	protected ILugusCoroutineHandle laneMoveRoutine = null;
	protected Vector3 originalScale = Vector3.one;

	void Start()
	{
		originalScale = transform.localScale;
	}

	void Update () 
	{
		CheckSurface();
		UpdatePosition();
		ScaleByDistanceHorizontal();
	}

	public virtual void Reset()
	{
		movingToLane = false;
		if (laneMoveRoutine != null && laneMoveRoutine.Running)
			laneMoveRoutine.StopRoutine();

		currentLane = FroggerLaneManager.use.GetLane(0);
		transform.position = currentLane.transform.position.z(transform.position.z);
	}

	protected void ScaleByDistanceHorizontal()
	{
		transform.localScale = Vector3.Lerp(maxScale, minScale, (transform.position.v2() - FroggerLaneManager.use.GetBottomLaneCenter()).y / FroggerLaneManager.use.GetLevelLengthLaneCenters());
	}

	protected void MoveSideways(bool right)
	{
		if (right)
			transform.Translate(transform.right.normalized * speed * Time.deltaTime, Space.World);
		else
			transform.Translate(-1 * transform.right.normalized * speed * Time.deltaTime, Space.World);
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
			laneMoveRoutine = LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, true));
		}
		else if (targetLaneIndex < currentLaneIndex)
		{
			laneMoveRoutine = LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, false));
		}
		else
		{
			Debug.Log("Character is trying to move to the lane: " + targetLaneIndex + " and it is already on that lane! Stopping.");
			return;
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

			coveredDistance += Vector3.Distance(lastPosition, transform.position);
			lastPosition = transform.position;

			yield return new WaitForEndOfFrame();
		}

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
		FroggerLaneItem laneItemUnderMe = null;

		// check all raycast hits
		RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, this.transform.forward);
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
}
