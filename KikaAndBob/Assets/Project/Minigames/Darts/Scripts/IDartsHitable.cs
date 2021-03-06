﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class IDartsHitable : MonoBehaviour 
{
	public float lastShownTime = 0.0f;
	public float lastHideTime = 0.0f;
	
	public DartsFunctionalityGroup group = null;

//	public int score = 100;

	protected int _hitCount = 0;
	public int HitCount
	{
		get
		{
			return _hitCount;
		}
		set 
		{
			_hitCount = value;

			if( group != null )
				group.HitableHit(this);
		}
	}

	protected bool _shown = false;
	public bool Shown
	{
		get{ return _shown; }
		set
		{
			if( value == true )
			{
				lastShownTime = Time.time;

				if( group != null )
					group.HitableShown(this);
			}
			else
			{
				lastHideTime = Time.time;

				if( autoHideHandle != null )
				{
					autoHideHandle.StopRoutine();
					autoHideHandle = null;
				}
				
				if( group != null )
					group.HitableHidden(this);
			}

			_shown = value; 
		}
	}

	protected ILugusCoroutineHandle autoHideHandle = null;

	public void AutoHide(float delay)
	{
		autoHideHandle = LugusCoroutines.use.StartRoutine( AutoHideRoutine(delay) );
	}

	public IEnumerator AutoHideRoutine(float delay)
	{
		if (delay <= 0)
			yield break;

		yield return new WaitForSeconds(delay);

		if( Shown )
			Hide();
	}

	public abstract void Show();
	public abstract void Hide();

	// standard the same as Hide() - can be used to define a "fast" hide method, e.g. not animated 
	public virtual void Disable()
	{
		Hide();
	}

	// standard does nothing, but can be used to define custom behavior for start a level 
	// (e.g. make this object visible, but don't immediately set Shown (see DartsBarrel.cs)
	public virtual void Enable()
	{
	}
	
	public abstract void OnHit();

	public virtual int GetScore()
	{
		return group.score;
	}
}
