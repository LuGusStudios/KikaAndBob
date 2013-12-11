using UnityEngine;
using System.Collections;

public class FroggerLaneItem : FroggerSurface
{
	public bool behindPlayer = false;			// if true, item is placed behind player (e.g. "logs", platforms etc.)
												// if false, item ends up before player (e.g. "cars", obstacles)
	public bool goRight = false;

	protected float coveredDistance = 0;
	protected float laneLength = 0;
	protected int location = 0; // 0: off screen first, 1: on screen
	
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

	protected virtual void AfterMovedEffect()
	{
	}
}
