using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace KikaAndBob
{
	public enum LaneItemType
	{
		NONE = -1,

		SINGLE = 1,
		STREAK = 2
	}

	public enum LaneItemActionType
	{
		NONE = -1,
		
		BUTTON = 1, // ipad specific
		DOWN = 2, // down arrow
		RIGHT = 3, // right arrow
		UP = 4, // up arrow
		LEFT = 5 // left arrow
	}
}


public class DanceHeroLaneItem 
{

	public KikaAndBob.LaneItemType type = KikaAndBob.LaneItemType.NONE;
	public KikaAndBob.LaneItemActionType actionType = KikaAndBob.LaneItemActionType.NONE;

	public DanceHeroLane lane = null;

	public float delay = 0.0f;
	public float duration = 0.0f;

	public const float singleDuration = 0.2f;

	public float speed = 4.0f; // units per second
	public float actionDistance = 0.8f; // interactive distance from lane's actionpoint (if closer than this, interaction is accepted)

	public KeyCode KeyCode
	{
		get
		{
			if( actionType == KikaAndBob.LaneItemActionType.DOWN )
				return KeyCode.DownArrow;
			else if( actionType == KikaAndBob.LaneItemActionType.UP )
				return KeyCode.UpArrow;
			else if( actionType == KikaAndBob.LaneItemActionType.RIGHT )
				return KeyCode.RightArrow;
			else if( actionType == KikaAndBob.LaneItemActionType.LEFT )
				return KeyCode.LeftArrow;
			else
				return KeyCode.None;
		}
	}



	public DanceHeroLaneItem(DanceHeroLane lane, float delay, KikaAndBob.LaneItemActionType type,  float speed, float duration = DanceHeroLaneItem.singleDuration)
	{
		this.lane = lane;
		this.delay = delay;
		this.actionType = type;
		this.duration = duration;
		this.speed = speed;

		if( duration <= DanceHeroLaneItem.singleDuration )
		{
			this.type = KikaAndBob.LaneItemType.SINGLE;
		}
		else
		{
			this.type = KikaAndBob.LaneItemType.STREAK;
		}
	}
}
