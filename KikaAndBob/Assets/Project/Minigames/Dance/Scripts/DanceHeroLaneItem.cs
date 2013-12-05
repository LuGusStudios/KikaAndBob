using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace KikaAndBob
{
	public enum LaneItemType
	{
		NONE = -1,
		
		BUTTON, // ipad specific
		DOWN, // down arrow
		RIGHT, // right arrow
		UP, // up arrow
		LEFT // left arrow
	}
}


public class DanceHeroLaneItem 
{
	public float delay = 0.0f;
	public KikaAndBob.LaneItemType type = KikaAndBob.LaneItemType.NONE;
	public float duration = 0.0f;

	public DanceHeroLaneItem(float delay, KikaAndBob.LaneItemType type, float duration)
	{
		this.delay = delay;
		this.type = type;
		this.duration = duration;
	}
}
