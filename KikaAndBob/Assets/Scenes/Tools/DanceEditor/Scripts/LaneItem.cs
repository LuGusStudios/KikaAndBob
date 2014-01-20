using UnityEngine;
using System.Collections;

namespace DanceEditor
{
	public enum LaneItemType
	{
		NONE = -1,
		SINGLE = 1,
		STREAK = 2
	}
}

/**
 * Simple struct that contains the time and duration of the item.
 **/
public class LaneItem
{
	public const float singleDuration = 0.2f;
	public const float maxStreakDuration = 10.0f;

	public float Time
	{
		get
		{
			return _time;
		}
		set
		{
			if (value < 0.0f)
				_time = 0.0f;
			else if ((value + _duration) > AudioPlayer.use.Source.clip.length)
				_time = AudioPlayer.use.Source.clip.length - _duration;
			else
				_time = value;
		}
	}
	public float Duration
	{
		get
		{
			return _duration;
		}
		set
		{
			if (value < LaneItem.singleDuration)
				_duration = LaneItem.singleDuration;
			else if (value > LaneItem.maxStreakDuration)
				_duration = LaneItem.maxStreakDuration;
			else if ((_time + value) > AudioPlayer.use.Source.clip.length)
				_duration = AudioPlayer.use.Source.clip.length - _time;
			else
				_duration = value;
		}
	}
	public DanceEditor.LaneItemType Type
	{
		get
		{
			if (_duration > LaneItem.singleDuration)
				return DanceEditor.LaneItemType.STREAK;
			else
				return DanceEditor.LaneItemType.SINGLE;
		}
	}

	protected float _time = 0.0f;
	protected float _duration = 0.0f;

	public LaneItem(float time, float duration = LaneItem.singleDuration)
	{
		this._time = time;
		this._duration = duration;
	}
}
