using UnityEngine;
using System.Collections;

public class FroggerCharacter : MonoBehaviour {

	public float speed = 100;

	protected FroggerLane currentLane = null;
	protected bool movingToLane = false;

	void Start () 
	{
		currentLane = FroggerLaneManager.use.GetLane(0);
		transform.position = currentLane.transform.position.z(transform.position.z);
	}
	
	void Update () 
	{
		CheckSurface();
		UpdatePosition();
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
			LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, true));
		}
		else if (targetLaneIndex < currentLaneIndex)
		{
			LugusCoroutines.use.StartRoutine(LaneMoveRoutine(targetLane, false));
		}
		else
		{
			Debug.Log("Character is trying to move to the lane it is on already! Stopping.");
			return;
		}
	}

	protected IEnumerator LaneMoveRoutine(FroggerLane targetLane, bool toHigher)
	{
		movingToLane = true;

		// the character has to move a certain distance from the middle of one lane to the middle of the next
		// this means moving half of the current lane's height plus half of the target lane's height
		float targetDistance = (currentLane.height * 0.5f) +  (targetLane.height * 0.5f);
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


	protected virtual void UpdatePosition()
	{

	}

	protected virtual void CheckSurface()
	{
		// check all raycast hits
		RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, this.transform.forward);
		bool topMostSurfaceFound = false;
		foreach(RaycastHit2D hit in hits)
		{
			if (hit.transform != null)
			{
				// first check if we're over a layer; if yes set current lane
				FroggerLane lane = hit.transform.GetComponent<FroggerLane>();
				if (lane != null)
				{
					currentLane = lane;
				}
				// then, check if we're on any other kind of surface (e.g. log) and only the detect the topmost of these surfaces
				else if (topMostSurfaceFound == false)
				{
					FroggerSurface surface = hit.transform.GetComponent<FroggerSurface>();
					if (surface != null)
					{
						surface.EnterSurfaceEffect();
						topMostSurfaceFound = true;
					}
				}
			}
		}
	}
}
