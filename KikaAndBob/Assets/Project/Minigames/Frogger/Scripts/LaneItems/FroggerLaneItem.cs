using UnityEngine;
using System.Collections;

public class FroggerLaneItem : FroggerSurface
{
	public bool behindPlayer = false;			// if true, item is placed behind player (e.g. "logs", platforms etc.)
												// if false, item ends up before player (e.g. "cars", obstacles)
	public bool goRight = false;

	public FroggerLane ParentLane
	{
		get
		{
			if (parentLane == null)
			{
				FindParentLane();
			}
			
			return parentLane;
		}
	}

	protected float coveredDistance = 0;
	protected float laneLength = 0;
	protected int location = 0;			// 0: off screen first, 1: on screen
	protected float positioning  = -1; // if -1, will be placed randomly and spawned, otherwise is placed fixed
	protected FroggerLane parentLane = null;

	public virtual void SetupGlobal()
	{
		FindParentLane();
	}

	private void Start()
	{
		SetupGlobal(); 
	}
	
	public void UpdateLaneItem(float displacement)
	{
		if (goRight)
			transform.Translate(transform.right.normalized * displacement, Space.World);
		else
			transform.Translate(-1 * transform.right.normalized * displacement, Space.World);

		coveredDistance += displacement;

		AfterMovedEffect();
	}

	public bool CrossedLevel()
	{
		return coveredDistance > laneLength;
	}

	public void SetLaneDistance(float _laneLength)
	{
		laneLength = _laneLength;
	}

	public void SetPositioning(float _positioning)
	{
		positioning = _positioning;
	}

	public float GetPositioning()
	{
		return positioning;
	}

	protected virtual void AfterMovedEffect()
	{
	}

	protected void FindParentLane()
	{
		if (transform.parent != null)
		{
			FroggerLane lane = transform.parent.GetComponent<FroggerLane>();
			if (lane != null)
			{
				parentLane = lane;
			}
		}
	}
}
